using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Telegram.Bot.Types;

namespace posting.Models.Messages
{
    public class MessageBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }          
        public TelegramData telegram_data { get; set; }
    }

    public class TelegramData
    {   
        public string? text { get; set; }        
        public List<MessageEntity>? entities { get; set; }        
        public string? mediagroup_id { get; set; }
        public List<MediaItem>? medias { get; set; }
    }

    public class MediaItem
    {
        public string storage_id { get; set; }
        public string url { get; set; }  
        public string? caption { get; set; }
        public List<MessageEntity>? entities { get; set; }
        public int? file_size { get; set; }
        public int? height { get; set; }
        public int? width { get; set; }
    }
}
