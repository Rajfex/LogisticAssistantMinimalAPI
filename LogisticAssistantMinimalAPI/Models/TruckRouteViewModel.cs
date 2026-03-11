using System.ComponentModel.DataAnnotations;

namespace LogisticAssistantMinimalAPI.Models
{
    public class TruckRouteViewModel
    {
        [Required]
        public int TruckId { get; set; }
        [Required]
        public int Distance { get; set; } // in kilometers
        [Required]
        public int BreakFrequency { get; set; } // in minutes
        [Required]
        public DateTime Date { get; set; }
    }
}
