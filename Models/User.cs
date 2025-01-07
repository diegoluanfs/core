using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("password")]
        public string Password { get; set; } = null!;

        [BsonElement("phone_number")]
        public string PhoneNumber { get; set; } = null!;
    }
}
