using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace posting.Models.Users
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public int direction_id { get; set; }
        public long telegram_id { get; set; }
        public string? firstname {  get; set; }
        public string? lastname { get; set; }   
        public string username { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}