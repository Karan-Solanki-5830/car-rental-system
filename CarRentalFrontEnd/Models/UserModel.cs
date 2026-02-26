namespace CarRentalFrontEnd.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";
        
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public IFormFile? ProfileImage { get; set; }
        public string? ProfileImagePath { get; set; }
        public DateTime? Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
