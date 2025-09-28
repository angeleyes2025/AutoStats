using AutoStats.Data;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoStats.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required]
        public string Make { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        public string RegistrationNumber { get; set; } = string.Empty;

        public int Year { get; set; }

        [Required(ErrorMessage = "Broj šasije je obavezan.")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "Broj šasije mora imati tačno 17 karaktera.")]
        [RegularExpression("^[A-Z0-9]{17}$", ErrorMessage = "Broj šasije smije sadržavati samo velika slova i brojeve.")]
        public string ChassisNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unesite zapreminu motora u cm³ (od 1 do 10.000).")]
        [Range(1, 10000, ErrorMessage = "Zapremina motora mora biti između 1 i 10.000 cm³.")]
        public int EngineDisplacement { get; set; }

        [Required(ErrorMessage = "Unesite snagu motora u kW (od 1 do 1.000).")]
        [Range(1, 1000, ErrorMessage = "Snaga motora mora biti između 1 i 1.000 kW.")]
        public int PowerKW { get; set; }

        public string UserId { get; set; } = string.Empty;

        public AutoStatsUser? User { get; set; }

        public ICollection<ServiceRecord>? ServiceRecords { get; set; }
    }
}
