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
        public List<MediaItem>? media { get; set; }
    }

    public class MediaItem
    {
        public string s3_file_id { get; set; }
        public string s3_url { get; set; }  
        public string caption { get; set; }
        public List<MessageEntity> entities { get; set; }
    }
}
