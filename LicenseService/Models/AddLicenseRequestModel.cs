using System.ComponentModel.DataAnnotations;

namespace LicenseService.Models
{
    public class AddLicenseRequestModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = string.Empty;

        internal License ToLicense()
        {
            return new License { Name = Name };
        }
    }
}
