using System.ComponentModel.DataAnnotations;

namespace SkiServiceAPI.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^\+?[0-9\s-]{7,15}$", ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Service type is required.")]
        public string Service { get; set; }

        [Required(ErrorMessage = "Priority is required.")]
        [RegularExpression("^(Tief|Standard|Express)$", ErrorMessage = "Priority must be 'Tief', 'Standard', or 'Express'.")]
        public string Priority { get; set; }

        [Required(ErrorMessage = "Pickup Date is required.")]
        public string PickupDate { get; set; }

        public string? Comment { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "Offen"; // Default status
    }
}
