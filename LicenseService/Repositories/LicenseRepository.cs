using LicenseService.Models;
using LicenseService.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace LicenseService.Repositories
{
    public class LicenseRepository : ILicenseRepository
    {
        private readonly IMongoCollection<License> _licenses;
        public LicenseRepository(ILicenseDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.Database);
            _licenses = database.GetCollection<License>(settings.LicenseCollection);
        }

        public async Task CreateLicense(License license)
        {
            await _licenses.InsertOneAsync(license);
        }

        public async Task<List<License>> GetLicenses(Expression<Func<License, bool>> filter)
        {
            return await _licenses.FindAsync(filter).Result.ToListAsync();
        }

        public async Task<License> GetLicense(Expression<Func<License, bool>> filter)
        {
            return await _licenses.FindAsync(filter).Result.FirstOrDefaultAsync();
        }

        public async Task RentLicense(License license, string clientId)
        {
            var filter = Builders<License>.Filter.Eq("Id", license.Id);
            var update = Builders<License>.Update.Set("ClientId", clientId)
                .Set("RentalExpirationDate", DateTime.UtcNow.AddSeconds(15));

            await _licenses.UpdateOneAsync(filter, update);
        }

    }
}
