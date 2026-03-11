using System.ComponentModel.DataAnnotations;

namespace LogisticAssistantMinimalAPI.Models
{
    public class TruckViewModel
    {
        public int Id { get; set; }
        [Required]
        public string LicensePlate { get; set; }
        [Required]
        public int Vmax { get; set; }
        [Required]
        public int DriverBreak { get; set; } // in minutes
    }
}
