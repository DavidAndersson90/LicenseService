using LicenseService.Models;

namespace LicenseService.Handlers
{
    public interface ILicenseHandler
    {
        Task<List<LicenseResponseModel>> GetLicenses();
        Task<LicenseResponseModel> GetLicenseById(string licenseId);
        Task<License> AddLicense(AddLicenseRequestModel addLicenseRequestModel);
        Task<RentLicenseResponseModel> RentLicense(RentLicenseRequestModel rentLicenseRequestModel);
        Task<bool> HasLicenseRentalExpired(string licenseId);
    }
}
