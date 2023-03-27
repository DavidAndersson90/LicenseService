namespace LicenseService.Models
{
    public class LicenseResponseModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RentalExpirationDate { get; set; }
        public string ClientId { get; set; } = string.Empty;

        public LicenseResponseModel(License license)
        {
            Id = license.Id;
            Name = license.Name;
            Status = license.HasActiveRental() ? $"{license.ClientId}, {(license.RentalExpirationDate - DateTime.UtcNow).Seconds} second(s) left" : "Not Rented";
            RentalExpirationDate = license.RentalExpirationDate;
            ClientId = license.ClientId;

        }
    }
}
