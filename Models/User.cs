using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SkiServiceAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username can only contain letters and numbers.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression("^(Admin|Employee|User)$", ErrorMessage = "Role must be 'Admin', 'Employee', or 'User'.")]
        public string Role { get; set; } = "User"; // Default role is "User"
    }

}
