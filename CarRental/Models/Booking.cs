using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarRental.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int CustomerId { get; set; }

    public int VehicleId { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? Created { get; set; }

    public DateTime Modified { get; set; }

    [JsonIgnore]
    [BindNever]
    public virtual ICollection<Agreement>? Agreements { get; set; } = new List<Agreement>();

    [JsonIgnore]
    [BindNever]
    public virtual Customer? Customer { get; set; } = null!;

    [JsonIgnore]
    [BindNever]
    public virtual ICollection<Payment>? Payments { get; set; } = new List<Payment>();

    [JsonIgnore]
    [BindNever]
    public virtual Vehicle? Vehicle { get; set; } = null!;
}
