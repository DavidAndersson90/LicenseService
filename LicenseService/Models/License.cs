using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LicenseService.Models
{
    public class License
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime RentalExpirationDate { get; set; }
        public string ClientId { get; set; } = string.Empty;

    }
}
