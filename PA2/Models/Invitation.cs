using System.ComponentModel.DataAnnotations;


public enum InvitationStatus
{
    NotSent,   //invitations that havent been sent
    Sent,      //inivitations thats been sent
    Accepted,  
    Declined   
}


public class Invitation //invitation record stored in db
{
    
    public int Id { get; set; }

    [Required, EmailAddress]
    [Display(Name = "Guest Email")]
    public string GuestEmail { get; set; }

    [Required]
    [Display(Name = "Guest Name")]
    public string GuestName { get; set; }

  
    public InvitationStatus Status { get; set; } = InvitationStatus.NotSent; //tracks for rvsp purpose

    [Required]
    public EventDetails Event { get; set; }

    [Required]
    public int EventId { get; set; }
 
}
