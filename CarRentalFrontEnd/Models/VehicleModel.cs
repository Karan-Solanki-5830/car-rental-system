using System.Text.Json.Serialization;

namespace CarRentalFrontEnd.Models
{
    public partial class VehicleModel
    {
        public int VehicleId { get; set; }
        public int UserId { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public string PlateNumber { get; set; } = null!;
        public int FuelTypeId { get; set; }
        public int VehicleTypeId { get; set; }
        public string Status { get; set; } = null!;
        public double Mileage { get; set; }


        public string ConditionNote { get; set; } = null!;
        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }
        public DateTime? Created { get; set; }
        public DateTime Modified { get; set; }
    }
}