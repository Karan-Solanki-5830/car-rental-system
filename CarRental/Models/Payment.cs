using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarRental.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string Remarks { get; set; } = null!;

    public DateTime? Created { get; set; }

    public DateTime Modified { get; set; }

    [JsonIgnore]
    [BindNever]
    public virtual Booking? Booking { get; set; } = null!;

    [JsonIgnore]
    [BindNever]
    public virtual User? User { get; set; } = null!;
}
