using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext //for event invitatiton 
{
    //initialize db
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    
    public DbSet<EventDetails> Events { get; set; }  // the event details
    public DbSet<Invitation> Invitations { get; set; }  //invitations for events

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Invitation>()
            .Property(invite => invite.Status)
            .HasConversion<string>();
        modelBuilder.Entity<Invitation>()
            .HasOne(invite => invite.Event)
            .WithMany(evt => evt.Invitations)
            .HasForeignKey(invite => invite.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder); 
    }
}
