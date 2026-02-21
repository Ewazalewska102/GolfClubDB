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
    public class CreateModel : PageModel
    {
        private readonly GolfClubContext _context;

        public CreateModel(GolfClubContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Booking Booking { get; set; } = new Booking();

        // Used to build the extra player dropdowns (Player2/3/4)
        public SelectList MemberSelectList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // Default values so form doesn't show 01/01/0001
            var now = DateTime.Now;
            int roundedMinutes = ((now.Minute + 14) / 15) * 15;

            DateTime roundedTime;
            if (roundedMinutes == 60)
                roundedTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(1);
            else
                roundedTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, roundedMinutes, 0);

            Booking = new Booking
            {
                TeeTime = roundedTime,
                BookingDate = roundedTime.Date,
                PlayerCount = 1
            };

            await ReloadMembersDropdownAsync();
            return Page();
        }

      
        public async Task<IActionResult> OnGetMemberInfoAsync(int id)
        {
            var m = await _context.Members
                .AsNoTracking()
                .Where(x => x.MembershipNumber == id)
                .Select(x => new { x.MembershipNumber, x.Name, x.Handicap })
                .FirstOrDefaultAsync();

            if (m == null) return NotFound();
            return new JsonResult(m);
        }

        public async Task<IActionResult> OnPostAsync()
        {
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
                ModelState.AddModelError(string.Empty,
                    "Could not save booking. This member may already have a booking for that date.");
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private async Task ReloadMembersDropdownAsync()
        {
            var members = await _context.Members
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .ToListAsync();

            // Main member dropdown (Value: MembershipNumber, Text: Name)
            ViewData["MemberId"] = new SelectList(
                members,
                nameof(Member.MembershipNumber),
                nameof(Member.Name),
                Booking.MemberId
            );

            // Extra player dropdowns reuse the same list
            MemberSelectList = new SelectList(
                members,
                nameof(Member.MembershipNumber),
                nameof(Member.Name)
            );
        }
    }
}