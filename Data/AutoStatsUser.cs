using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AutoStats.Data
{
    public class AutoStatsUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public required string LastName { get; set; }

        // Po potrebi dodaj: datum rođenja, adresa, itd.
    }
}
