using System;
using System.ComponentModel.DataAnnotations;

namespace AutoStats.Models
{
    public class ServiceRecord
    {
        public int Id { get; set; }

        [Required]
        public DateTime ServiceDate { get; set; }

        [Required]
        public int Mileage { get; set; }

        [Required]
        public string ServiceType { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public decimal Cost { get; set; }

        public string? InvoiceNumber { get; set; }

        public string? ServiceCenter { get; set; }

        public string? Warranty { get; set; }

        [Required]
        public int VehicleId { get; set; }

        public Vehicle? Vehicle { get; set; }
    }
}
