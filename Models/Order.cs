using System.ComponentModel.DataAnnotations;

namespace SkiServiceAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Service { get; set; }
        public string Priority { get; set; }
        public string PickupDate { get; set; }
        public string? Comment { get; set; }

        [Required]
        public string Status { get; set; } = "Offen"; // Default value
    }

}
