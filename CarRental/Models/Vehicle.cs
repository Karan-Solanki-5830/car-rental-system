using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarRental.Models
{
    public partial class Vehicle
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


        // Navigation properties
        [JsonIgnore]
        [BindNever]
        public virtual ICollection<Booking>? Bookings { get; set; } = new List<Booking>();

        [JsonIgnore]
        [BindNever]
        public virtual FuelType? FuelType { get; set; } = null!;

        [JsonIgnore]
        [BindNever]
        public virtual ICollection<MaintenanceLog>? MaintenanceLogs { get; set; } = new List<MaintenanceLog>();

        [JsonIgnore]
        [BindNever]
        public virtual User? User { get; set; } = null!;

        [JsonIgnore]
        [BindNever]
        public virtual VehicleType? VehicleType { get; set; } = null!;
    }
}
