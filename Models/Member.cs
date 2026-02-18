using System.ComponentModel.DataAnnotations;

namespace GolfClubDB.Models
{
    public class Member
    {
        [Key]
        [Display(Name = "Membership Number")]
        public int MembershipNumber { get; set; }

        [Required]
        [StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Range(0, 54)]
        public int Handicap { get; set; }

        // Navigation property
        public List<Booking>? Bookings { get; set; }
    }
}
