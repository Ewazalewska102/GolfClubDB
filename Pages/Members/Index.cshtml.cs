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
    public class IndexModel : PageModel
    {
        private readonly GolfClubDB.Data.GolfClubContext _context;

        public IndexModel(GolfClubDB.Data.GolfClubContext context)
        {
            _context = context;
        }

        public IList<Member> Member { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Member = await _context.Members.ToListAsync();
        }
    }
}
