using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace CarRental.Models;
using System;
using System.Text.Json.Serialization;

public partial class MaintenanceLog
{
    public int LogId { get; set; }

    public int VehicleId { get; set; }

    public int UserId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime ServiceDate { get; set; }

    public decimal Cost { get; set; }

    public DateTime? Created { get; set; }

    public DateTime Modified { get; set; }
    [JsonIgnore]
    [BindNever]
    public virtual User? User { get; set; } = null!;
    [JsonIgnore]
    [BindNever]
    public virtual Vehicle? Vehicle { get; set; } = null!;
}
