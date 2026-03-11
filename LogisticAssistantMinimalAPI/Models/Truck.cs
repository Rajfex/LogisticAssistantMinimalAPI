using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticAssistantMinimalAPI.Models
{
    public class Truck
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string LicensePlate { get; set; }
        [Required]
        public int Vmax { get; set; }
        [Required]
        public int DriverBreak { get; set; } // in minutes
        [Required]
        public int UserId { get; set; }
    }
}
