using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GolfClubDB.Data;
using GolfClubDB.Models;

namespace GolfClubDB.Pages.Members
{
    public class DetailsModel : PageModel
    {
        private readonly GolfClubDB.Data.GolfClubContext _context;

        public DetailsModel(GolfClubDB.Data.GolfClubContext context)
        {
            _context = context;
        }

        public Member Member { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FirstOrDefaultAsync(m => m.MembershipNumber == id);

            if (member is not null)
            {
                Member = member;

                return Page();
            }

            return NotFound();
        }
    }
}
