using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarRental.Models;

public partial class Agreement
{
    public int AgreementId { get; set; }

    public int BookingId { get; set; }

    public int CustomerId { get; set; }

    public bool? TermsAccepted { get; set; }

    public DateTime AgreementDate { get; set; }

    public string AgreementPdfpath { get; set; } = null!;

    public DateTime? Created { get; set; }

    public DateTime Modified { get; set; }

    [JsonIgnore]
    [BindNever]
    public virtual Booking? Booking { get; set; }

    [JsonIgnore]
    [BindNever]
    public virtual Customer? Customer { get; set; }
}
