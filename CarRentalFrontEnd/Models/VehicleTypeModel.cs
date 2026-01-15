using System.ComponentModel.DataAnnotations;

namespace CarRentalFrontEnd.Models
{
    public class VehicleTypeModel
    {
        public int VehicleTypeId { get; set; }

        [Required(ErrorMessage = "Type Name is required.")]
        [StringLength(100, ErrorMessage = "Type Name cannot exceed 100 characters.")]
        public string TypeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        public DateTime? Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
