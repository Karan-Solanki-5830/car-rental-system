namespace CarRentalFrontEnd.Models
{
    public class AgreementModel
    {
        public int AgreementId { get; set; }
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public bool TermsAccepted { get; set; } = false;
        public DateTime AgreementDate { get; set; }
        public string AgreementPdfpath { get; set; } = string.Empty;
        public DateTime? Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
