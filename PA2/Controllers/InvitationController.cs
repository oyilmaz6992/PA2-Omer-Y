using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class InvitationController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly MailService _emailService;

    // Constructor for dependency injection of database and email services
    public InvitationController(ApplicationDbContext dbContext, MailService emailService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
    }

    // Displays the invitation management dashboard for a specific event
    public async Task<IActionResult> Manage(int eventId)
    {
        // Retrieve event details along with associated invitations
        var eventDetails = await _dbContext.Events
                                           .Include(evt => evt.Invitations)
                                           .FirstOrDefaultAsync(evt => evt.Id == eventId);

        // If the event does not exist, return a 404 response
        if (eventDetails == null) return NotFound();

        // Calculate RSVP response statistics
        ViewBag.TotalInvitations = eventDetails.Invitations.Count;
        ViewBag.Accepted = eventDetails.Invitations.Count(invite => invite.Status == InvitationStatus.Accepted);
        ViewBag.Declined = eventDetails.Invitations.Count(invite => invite.Status == InvitationStatus.Declined);
        ViewBag.Pending = eventDetails.Invitations.Count(invite => invite.Status == InvitationStatus.Sent);

        return View(eventDetails);
    }

    // Adds a new guest invitation to the event
    [HttpPost]
    public async Task<IActionResult> AddInvitation(int eventId, string guestName, string guestEmail)
    {
        // Verify if the event exists before proceeding
        var eventDetails = await _dbContext.Events.FindAsync(eventId);
        if (eventDetails == null) return NotFound();

        // Create a new invitation entry with default status
        var newInvitation = new Invitation
        {
            GuestName = guestName,
            GuestEmail = guestEmail,
            EventId = eventId,
            Status = InvitationStatus.NotSent
        };

        // Add the new invitation to the database and save changes
        _dbContext.Invitations.Add(newInvitation);
        await _dbContext.SaveChangesAsync();

        // Redirect back to the invitation management page
        return RedirectToAction(nameof(Manage), new { eventId });
    }

    // Sends an invitation email to a guest
    [HttpPost]
    public async Task<IActionResult> SendInvitation(int invitationId)
    {
        // Retrieve invitation details including related event data
        var invitationDetails = await _dbContext.Invitations
                                                .Include(invite => invite.Event)
                                                .FirstOrDefaultAsync(invite => invite.Id == invitationId);
        if (invitationDetails == null) return NotFound();

        // Construct email subject line
        string emailSubject = $"You're Invited to {invitationDetails.Event.Title}!";

        // Compose email content with event details and RSVP link
        string emailBody = $@"
            <p>Hello {invitationDetails.GuestName},</p>
            <p>You are invited to <strong>{invitationDetails.Event.Title}</strong> 
            on <strong>{invitationDetails.Event.EventDate.ToShortDateString()}</strong> 
            at <strong>{invitationDetails.Event.Venue}</strong>.</p>
            <p>Click below to confirm your attendance:</p>
            <p><a href='https://localhost:5001/Invitation/Respond/{invitationDetails.Id}' 
            style='display:inline-block; padding:10px 20px; font-size:16px; color:#fff; background:#007bff; text-decoration:none; border-radius:5px;'>RSVP Here</a></p>
            <p>Looking forward to seeing you!<br/>Event Organizer</p>";

        // Send the invitation email using the email service
        await _emailService.SendEmailAsync(invitationDetails.GuestEmail, emailSubject, emailBody);

        // Update the invitation status to reflect that it has been sent
        invitationDetails.Status = InvitationStatus.Sent;
        await _dbContext.SaveChangesAsync();

        // Redirect back to Manage page to reflect the updated status
        return RedirectToAction(nameof(Manage), new { eventId = invitationDetails.EventId });
    }

    // Displays the RSVP response page for the invited guest
    [HttpGet("/Invitation/Respond/{invitationId}")]
    public async Task<IActionResult> Respond(int invitationId)
    {
        // Retrieve invitation details and related event information
        var invitationDetails = await _dbContext.Invitations
                                                .Include(invite => invite.Event)
                                                .FirstOrDefaultAsync(invite => invite.Id == invitationId);
        if (invitationDetails == null) return NotFound();

        // Render RSVP response form with guest details
        return View(invitationDetails);
    }

    // Processes the RSVP response from the guest
    [HttpPost]
    public async Task<IActionResult> SubmitResponse(int invitationId, string response)
    {
        // Locate the corresponding invitation record in the database
        var invitationDetails = await _dbContext.Invitations.FindAsync(invitationId);
        if (invitationDetails == null) return NotFound();

        // Update invitation status based on guest response
        invitationDetails.Status = response.ToLower() == "yes" ? InvitationStatus.Accepted
                                      : response.ToLower() == "no" ? InvitationStatus.Declined
                                      : invitationDetails.Status;

        // Save the updated response status
        await _dbContext.SaveChangesAsync();

        // Redirect to a confirmation page
        return RedirectToAction(nameof(ThankYou));
    }

    // Displays a "Thank You" message after RSVP submission
    public IActionResult ThankYou()
    {
        return View();
    }
}
