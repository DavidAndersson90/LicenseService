using LicenseService.Models;
using System.Linq.Expressions;

namespace LicenseService.Repositories
{
    public interface ILicenseRepository
    {

        Task CreateLicense(License license);
        Task<List<License>> GetLicenses(Expression<Func<License, bool>> filter);
        Task<License> GetLicense(Expression<Func<License, bool>> filter);
        Task RentLicense(License license, string clientId);

    }
}
