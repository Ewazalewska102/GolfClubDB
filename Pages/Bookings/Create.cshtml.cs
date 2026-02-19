using System.Threading.Tasks;
using GolfClubDB.Data;
using GolfClubDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GolfClubDB.Pages.Bookings
{
    public class CreateModel : PageModel
    {
        private readonly GolfClubContext _context;

        public CreateModel(GolfClubContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Booking Booking { get; set; } = new Booking();

        public async Task<IActionResult> OnGetAsync()
        {
            await ReloadMembersDropdownAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Always reload dropdown when returning Page()
            // so the dropdown doesn't break on validation errors.
            await ReloadMembersDropdownAsync();

            if (!ModelState.IsValid)
                return Page();

            // Force BookingDate to match the TeeTime date
            Booking.BookingDate = Booking.TeeTime.Date;

            // Rule 1: 15-minute intervals only
            if (Booking.TeeTime.Minute % 15 != 0 || Booking.TeeTime.Second != 0)
            {
                ModelState.AddModelError("Booking.TeeTime",
                    "Tee time must be in 15-minute intervals (00, 15, 30, 45).");
                return Page();
            }

            // Rule 2: member can’t book more than one game per day
            bool alreadyBooked = await _context.Bookings.AnyAsync(b =>
                b.MemberId == Booking.MemberId &&
                b.BookingDate == Booking.BookingDate);

            if (alreadyBooked)
            {
                ModelState.AddModelError(string.Empty,
                    "This member already has a booking for that date.");
                return Page();
            }

            _context.Bookings.Add(Booking);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // If the unique index triggers, show a friendly message
                ModelState.AddModelError(string.Empty,
                    "Could not save booking. This member may already have a booking for that date.");
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private async Task ReloadMembersDropdownAsync()
        {
            var members = await _context.Members.AsNoTracking().ToListAsync();

            // Value: MembershipNumber (PK), Text: Email (you can change to Name later)
            ViewData["MemberId"] = new SelectList(
                members,
                nameof(Member.MembershipNumber),
                nameof(Member.Email),
                Booking.MemberId
            );
        }
    }
}
