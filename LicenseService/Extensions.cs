using LicenseService.Models;

namespace LicenseService
{
    public static class Extensions
    {
        public static bool HasActiveRental(this License license)
        {
            return license.RentalExpirationDate > DateTime.UtcNow;
        }

    }
}
