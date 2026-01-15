namespace CarRentalFrontEnd.Models
{
    public class FuelTypeModel
    {
        public int FuelTypeId { get; set; }
        public string FuelName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
