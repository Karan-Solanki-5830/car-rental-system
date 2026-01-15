namespace CarRentalFrontEnd.Models
{
    public class MaintenanceModel
    {
        public int LogId { get; set; }

        public int VehicleId { get; set; }

        public int UserId { get; set; }

        public string Description { get; set; } = null!;

        public DateTime ServiceDate { get; set; }

        public decimal Cost { get; set; }

        public DateTime? Created { get; set; }

        public DateTime Modified { get; set; }
    }
}
