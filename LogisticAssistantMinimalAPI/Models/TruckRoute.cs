using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticAssistantMinimalAPI.Models
{
    public class TruckRoute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int TruckId { get; set; }
        [ForeignKey("TruckId")]
        public Truck Truck { get; set; }
        [Required]
        public int Distance { get; set; } // in kilometers
        [Required]
        public int BreakFrequency { get; set; } // in minutes
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int TruckVmax { get; set; }
        [Required]
        public int TruckDriverBreak { get; set; }
        [Required]
        public double RemainingToBreak { get; set; }
    }
}
