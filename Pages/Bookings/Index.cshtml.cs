using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GolfClubDB.Data;
using GolfClubDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GolfClubDB.Pages.Bookings
{
    public class IndexModel : PageModel
    {
        private readonly GolfClubContext _context;

        public IndexModel(GolfClubContext context)
        {
            _context = context;
        }

        public IList<Booking> Booking { get; set; } = new List<Booking>();

        [BindProperty(SupportsGet = true)]
        public int? MemberId { get; set; }

        public Member? SelectedMember { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<Booking> query = _context.Bookings
                .AsNoTracking()
                .Include(b => b.Member);

            if (MemberId.HasValue)
            {
                query = query.Where(b => b.MemberId == MemberId.Value);

                SelectedMember = await _context.Members
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.MembershipNumber == MemberId.Value);
            }

            // Nice default ordering
            query = query.OrderByDescending(b => b.BookingDate).ThenBy(b => b.TeeTime);

            Booking = await query.ToListAsync();
        }
    }
}
