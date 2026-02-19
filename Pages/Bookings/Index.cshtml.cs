using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GolfClubDB.Data;
using GolfClubDB.Models;

namespace GolfClubDB.Pages.Bookings
{
    public class IndexModel : PageModel
    {
        private readonly GolfClubDB.Data.GolfClubContext _context;

        public IndexModel(GolfClubDB.Data.GolfClubContext context)
        {
            _context = context;
        }

        public IList<Booking> Booking { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Booking = await _context.Bookings
                .Include(b => b.Member).ToListAsync();
        }
    }
}
