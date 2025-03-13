using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class EventController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    // Constructor for dependency injection of database context
    public EventController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Displays a list of all scheduled events along with guest invitations
    public async Task<IActionResult> Index()
    {
        var allEvents = await _dbContext.Events
                                        .Include(evt => evt.Invitations)
                                        .ToListAsync();
        return View(allEvents);
    }

    // Renders the form for creating a new event
    public IActionResult Create() => View();

    // Handles event creation and stores it in the database
    [HttpPost]
    public async Task<IActionResult> Create(EventDetails newEvent)
    {
        // Ensure input data is valid before proceeding
        if (ModelState.IsValid)
        {
            _dbContext.Events.Add(newEvent);
            await _dbContext.SaveChangesAsync();

            // Redirect to the event list upon successful creation
            return RedirectToAction(nameof(Index));
        }

        return View(newEvent);
    }

    // Retrieves event details for editing based on its unique identifier
    public async Task<IActionResult> Edit(int eventId)
    {
        var eventDetails = await _dbContext.Events.FindAsync(eventId);

        // If the event is not found, return a 404 error
        if (eventDetails == null) return NotFound();

        return View(eventDetails);
    }

    // Processes an update request for an existing event
    [HttpPost]
    public async Task<IActionResult> Edit(int eventId, EventDetails updatedEvent)
    {
        // Validate that the provided ID matches the model's event ID
        if (eventId != updatedEvent.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _dbContext.Update(updatedEvent);
            await _dbContext.SaveChangesAsync();

            // Redirect to the event list after the update
            return RedirectToAction(nameof(Index));
        }

        return View(updatedEvent);
    }

    // Retrieves event details for deletion confirmation
    public async Task<IActionResult> Delete(int eventId)
    {
        var eventDetails = await _dbContext.Events.FindAsync(eventId);

        // If the event does not exist, return a 404 error
        if (eventDetails == null) return NotFound();

        return View(eventDetails);
    }

    // Confirms and processes event deletion
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int eventId)
    {
        // Locate the event and include associated invitations
        var eventDetails = await _dbContext.Events
                                           .Include(evt => evt.Invitations)
                                           .FirstOrDefaultAsync(evt => evt.Id == eventId);

        // Ensure the event exists before proceeding with deletion
        if (eventDetails == null) return NotFound();

        // Remove all invitations linked to the event before deleting it
        _dbContext.Invitations.RemoveRange(eventDetails.Invitations);
        _dbContext.Events.Remove(eventDetails);
        await _dbContext.SaveChangesAsync();

        // Store a success message for display after redirection
        TempData["SuccessMessage"] = "Event has been successfully removed.";

        return RedirectToAction(nameof(Index));
    }
}
