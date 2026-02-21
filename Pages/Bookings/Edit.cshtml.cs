using System;
using System.Linq;
using System.Threading.Tasks;
using GolfClubDB.Data;
using GolfClubDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GolfClubDB.Pages.Bookings
{
    public class EditModel : PageModel
    {
        private readonly GolfClubContext _context;

        public EditModel(GolfClubContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            Booking = booking;

            await ReloadMembersDropdownAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Always reload dropdown when returning Page()
            await ReloadMembersDropdownAsync();

            if (!ModelState.IsValid)
                return Page();

            // Force BookingDate to match TeeTime date (same as Create)
            Booking.BookingDate = Booking.TeeTime.Date;

            // Rule 1: 15-minute intervals only
            if (Booking.TeeTime.Minute % 15 != 0 || Booking.TeeTime.Second != 0)
            {
                ModelState.AddModelError("Booking.TeeTime",
                    "Tee time must be in 15-minute intervals (00, 15, 30, 45).");
                return Page();
            }

            // Rule 2: member can’t book more than one game per day
            // IMPORTANT: exclude the current booking by Id
            bool alreadyBooked = await _context.Bookings.AnyAsync(b =>
                b.Id != Booking.Id &&
                b.MemberId == Booking.MemberId &&
                b.BookingDate == Booking.BookingDate);

            if (alreadyBooked)
            {
                ModelState.AddModelError(string.Empty,
                    "This member already has a booking for that date.");
                return Page();
            }

            // Attach + update
            _context.Attach(Booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                
                ModelState.AddModelError(string.Empty,
                    "Could not save booking. This member may already have a booking for that date.");
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private async Task ReloadMembersDropdownAsync()
        {
            var members = await _context.Members.AsNoTracking().ToListAsync();

            // Value: MembershipNumber, Text: Name (better UX)
            ViewData["MemberId"] = new SelectList(
                members,
                nameof(Member.MembershipNumber),
                nameof(Member.Name),
                Booking.MemberId
            );
        }
    }
}