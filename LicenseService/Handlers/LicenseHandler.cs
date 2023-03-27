using LicenseService.Exceptions;
using LicenseService.Models;
using LicenseService.MongoDB;
using LicenseService.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Net;

namespace LicenseService.Handlers
{
    public class LicenseHandler : ILicenseHandler
    {
        private readonly ILogger _logger;
        private readonly ILicenseRepository _licenseRepository;

        public LicenseHandler(ILogger<LicenseHandler> logger, ILicenseRepository licenseRepository)
        {
            _logger = logger;
            _licenseRepository = licenseRepository;
        }
        public async Task<License> AddLicense(AddLicenseRequestModel addLicenseRequestModel)
        {
            var license = addLicenseRequestModel.ToLicense();
            await _licenseRepository.CreateLicense(license);
            return license;
        }

        public async Task<List<LicenseResponseModel>> GetLicenses()
        {
            var licenses = await _licenseRepository.GetLicenses(license => true);
            return licenses.Select(license => new LicenseResponseModel(license)).ToList();
        }

        public async Task<RentLicenseResponseModel> RentLicense(RentLicenseRequestModel rentLicenseRequestModel)
        {
            var firstAvailableLicense = await _licenseRepository.GetLicense(license => 
            license.RentalExpirationDate < DateTime.UtcNow);

            if (firstAvailableLicense == null)
            {
                throw new HttpResponseException((int)HttpStatusCode.NotFound, "No Licenses available for rental");
            }
            _logger.LogInformation($"License: {firstAvailableLicense.Name} expired at {firstAvailableLicense.RentalExpirationDate}");

            if (await ClientHasActiveRental(rentLicenseRequestModel.ClientId))
            {
                throw new HttpResponseException((int)HttpStatusCode.BadRequest,
                    $"Client: {rentLicenseRequestModel.ClientId} already have an active license rental");
            }

            await _licenseRepository.RentLicense(firstAvailableLicense, rentLicenseRequestModel.ClientId);
            return new RentLicenseResponseModel() { Id = firstAvailableLicense.Id, ClientId = firstAvailableLicense.ClientId, Name = firstAvailableLicense.Name };
        }
        public async Task<bool> HasLicenseRentalExpired(string licenseId)
        {
            var license = await _licenseRepository.GetLicense(x => x.Id == licenseId);
            if (license == null)
            {
                throw new HttpResponseException((int)HttpStatusCode.NotFound, $"License with Id = {licenseId} not found");
            }
            return !license.HasActiveRental();
        }

        public async Task<LicenseResponseModel> GetLicenseById(string licenseId)
        {
            var license = await _licenseRepository.GetLicense(x => x.Id == licenseId);
            return new LicenseResponseModel(license);
        }

        private async Task<bool> ClientHasActiveRental(string clientId)
        {
            var clientLicenses = await _licenseRepository.GetLicenses(licenses => licenses.ClientId == clientId);
            return clientLicenses.Any(license => license.HasActiveRental());
        }
    }
}
