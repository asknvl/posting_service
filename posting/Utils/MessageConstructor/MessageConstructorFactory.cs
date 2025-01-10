using posting.Services.MongoDBService;
using posting.Services.S3Service;

namespace posting.Utils.MessageConstructor
{
    public class MessageConstructorFactory : IMessageConstructorFactory
    {
        IMongoDBService mongoDBService;
        IS3Service s3Service;

        public MessageConstructorFactory(IMongoDBService mongoDBService, IS3Service s3Service) {
            this.mongoDBService = mongoDBService;
            this.s3Service = s3Service; 
        }

        public IMessageConstructor Create()
        {
            return new MessageConstructor(mongoDBService, s3Service);
        }
    }
}
