using System.ComponentModel.DataAnnotations;

namespace LicenseService.Models
{
    public class RentLicenseRequestModel
    {
        [Required(AllowEmptyStrings = false)]
        public string ClientId { get; set; } = string.Empty;
    }
}
