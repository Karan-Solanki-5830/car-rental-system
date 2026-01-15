using System.ComponentModel.DataAnnotations;

namespace CarRentalFrontEnd.Models
{
    public class BookingModel
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Customer is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Vehicle is required.")]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Start date and time is required.")]
        [DataType(DataType.DateTime)]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "End date and time is required.")]
        [DataType(DataType.DateTime)]
        public DateTime EndDateTime { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = null!;

        [Display(Name = "Created Date")]
        public DateTime? Created { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; }

        // Display-only fields populated in controller for list view
        public string? VehicleBrand { get; set; }
        public string? VehicleModel { get; set; }
    }
}
