using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? Created { get; set; }

    public DateTime Modified { get; set; }

    public string? ProfileImagePath { get; set; }

    [NotMapped]
    [JsonIgnore]
    [BindNever]

    public virtual ICollection<FuelType> FuelTypes { get; set; } = new List<FuelType>();
    [JsonIgnore]
    [BindNever]
    public virtual ICollection<MaintenanceLog> MaintenanceLogs { get; set; } = new List<MaintenanceLog>();
    [JsonIgnore]
    [BindNever]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    [JsonIgnore]
    [BindNever]
    public virtual ICollection<VehicleType> VehicleTypes { get; set; } = new List<VehicleType>();
    [JsonIgnore]
    [BindNever]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
