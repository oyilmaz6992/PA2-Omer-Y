
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace PA2.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

            //Invitation entity
            modelBuilder.Entity("Invitation", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<string>("GuestEmail")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<string>("GuestName")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<int>("EventId")
                    .HasColumnType("INTEGER");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.HasIndex("EventId");

                b.ToTable("Invitations");
            });

            //event details
            modelBuilder.Entity("EventDetails", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<DateTime>("EventDate")
                    .HasColumnType("TEXT");

                b.Property<string>("Venue")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.ToTable("Events");
            });

            // foreign key between Invitation and EventDetails
            modelBuilder.Entity("Invitation", b =>
            {
                b.HasOne("EventDetails", "Event")
                    .WithMany("Invitations")
                    .HasForeignKey("EventId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Event");
            });

            modelBuilder.Entity("EventDetails", b =>
            {
                b.Navigation("Invitations");
            });
#pragma warning restore 612, 618
        }
    }
}
