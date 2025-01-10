using posting.Models.Messages;
using posting.Services.MongoDBService;
using posting.Services.S3Service;
using Telegram.Bot.Types;

namespace posting.Utils.MessageConstructor
{
    public class MessageConstructor : IMessageConstructor
    {
        #region vars
        IMongoDBService mongoDBService;
        IS3Service s3Service;        
        #endregion
        public MessageConstructor(IMongoDBService mongoDBService, IS3Service s3Service) {
            this.mongoDBService = mongoDBService;
            this.s3Service = s3Service;
        }

        public Task<MessageBase> Create(Message message)
        {
            throw new NotImplementedException();
        }
        #region public
        #endregion
    }
}
