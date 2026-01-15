using System.ComponentModel.DataAnnotations;

namespace CarRentalFrontEnd.Models
{
    public class CustomerModel
    {
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        
        public string? Role { get; set; }
    }
}
