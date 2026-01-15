using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarRental.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateTime? Created { get; set; }

    public DateTime Modified { get; set; }
    [JsonIgnore]
    [BindNever]
    public virtual ICollection<Booking>? Bookings { get; set; } = new List<Booking>();

    [JsonIgnore]
    [BindNever]
    public virtual ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();

}
