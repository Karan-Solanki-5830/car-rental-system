using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace CarRental.Models;

public partial class VehicleType
{
    public int VehicleTypeId { get; set; }

    public string TypeName { get; set; } = null!;

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
