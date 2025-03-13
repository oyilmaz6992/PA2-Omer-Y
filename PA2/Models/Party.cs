using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


public class EventDetails
{
 
    //identifier for event
    public int Id { get; set; }

    public List<Invitation> Invitations { get; set; } = new(); //invitations for the event

    [Required]
    [Display(Name = "Scheduled Date")] //for event date
    public DateTime EventDate { get; set; }

    [Required]
    [Display(Name = "Event Description")]
    public string Title { get; set; }

    [Required]
    public string Venue { get; set; } //this is the location

  
  
}
