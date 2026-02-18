using System.ComponentModel.DataAnnotations;

namespace GolfClubDB.Models
{
    public class Booking
    {
        public int Id { get; set; }

        // Which member made the booking
        [Required]
        public int MemberId { get; set; }
        public Member? Member { get; set; }

        // Date only (used for "one booking per day" rule)
        [Required]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        // Tee time (we will enforce 15-min rule later)
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TeeTime { get; set; }

        // Up to 4 players
        [Range(1, 4)]
        public int PlayerCount { get; set; } = 1;

        // Player 1 is the member (stored via MemberId + Member)
        // Players 2-4 are extra optional players:
        [StringLength(80)]
        public string? Player2Name { get; set; }
        [Range(0, 54)]
        public int? Player2Handicap { get; set; }

        [StringLength(80)]
        public string? Player3Name { get; set; }
        [Range(0, 54)]
        public int? Player3Handicap { get; set; }

        [StringLength(80)]
        public string? Player4Name { get; set; }
        [Range(0, 54)]
        public int? Player4Handicap { get; set; }
    }
}
