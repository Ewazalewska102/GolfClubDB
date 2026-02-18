using GolfClubDB.Models;
using Microsoft.EntityFrameworkCore;

namespace GolfClubDB.Data
{
    public class GolfClubContext : DbContext
    {
        public GolfClubContext(DbContextOptions<GolfClubContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Members => Set<Member>();
        public DbSet<Booking> Bookings => Set<Booking>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Link Booking.MemberId -> Member.MembershipNumber
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Member)
                .WithMany(m => m.Bookings)
                .HasForeignKey(b => b.MemberId)
                .HasPrincipalKey(m => m.MembershipNumber);

            // Enforce: a member cannot book more than one game per day
            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.MemberId, b.BookingDate })
                .IsUnique();
        }
    }
}
