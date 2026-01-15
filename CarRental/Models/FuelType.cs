using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarRental.Models;

public partial class FuelType
{

    public int FuelTypeId { get; set; }

    public string FuelName { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime? Created { get; set; }

    public DateTime Modified { get; set; }
    [JsonIgnore]
    [BindNever]
    public virtual User? User { get; set; } = null!;
    [JsonIgnore]
    [BindNever]
    public virtual ICollection<Vehicle>? Vehicles { get; set; } = new List<Vehicle>();
}
