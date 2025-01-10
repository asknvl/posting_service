using posting.Models.LoaderBot;
using posting.Services.MongoDBService;
using posting.Utils.MessageConstructor;

namespace posting.Services.LoaderBotService
{
    public class LoaderBotFactory : ILoaderBotFactory
    {
        public ILoaderBot Create(LoaderBotModel model, IMongoDBService mongoDBService, IMessageConstructor messageConstructor, ILogger logger)
        {
            return new LoaderBot(model, mongoDBService, messageConstructor, logger);
        }
    }
}
