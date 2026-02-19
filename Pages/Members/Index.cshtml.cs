using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GolfClubDB.Data;
using GolfClubDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GolfClubDB.Pages.Members
{
    public class IndexModel : PageModel
    {
        private readonly GolfClubContext _context;

        public IndexModel(GolfClubContext context)
        {
            _context = context;
        }

        public IList<Member> Member { get; set; } = new List<Member>();

        // Filters / Sorting (kept in query string)
        [BindProperty(SupportsGet = true)]
        public string? Gender { get; set; }

        // "below10" | "between11and20" | "above20" | null/empty
        [BindProperty(SupportsGet = true)]
        public string? HandicapRange { get; set; }

        // "name_asc" | "name_desc" | "hcap_asc" | "hcap_desc" | null
        [BindProperty(SupportsGet = true)]
        public string? SortOrder { get; set; }

        // For dropdown options
        public List<string> GenderOptions { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            // Build gender options (distinct values from DB)
            GenderOptions = await _context.Members
                .AsNoTracking()
                .Select(m => m.Gender)
                .Where(g => g != null && g != "")
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            IQueryable<Member> query = _context.Members.AsNoTracking();

            // Filter: Gender
            if (!string.IsNullOrWhiteSpace(Gender))
            {
                query = query.Where(m => m.Gender == Gender);
            }

            // Filter: Handicap ranges (assignment-specific)
            if (!string.IsNullOrWhiteSpace(HandicapRange))
            {
                switch (HandicapRange)
                {
                    case "below10":
                        query = query.Where(m => m.Handicap < 10);
                        break;

                    case "between11and20":
                        query = query.Where(m => m.Handicap >= 11 && m.Handicap <= 20);
                        break;

                    case "above20":
                        query = query.Where(m => m.Handicap > 20);
                        break;
                }
            }

            // Sorting
            query = SortOrder switch
            {
                "name_desc" => query.OrderByDescending(m => m.Name),
                "hcap_asc" => query.OrderBy(m => m.Handicap).ThenBy(m => m.Name),
                "hcap_desc" => query.OrderByDescending(m => m.Handicap).ThenBy(m => m.Name),
                _ => query.OrderBy(m => m.Name) // default: name_asc
            };

            Member = await query.ToListAsync();
        }
    }
}
