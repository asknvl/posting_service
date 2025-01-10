using posting.Models.LoaderBot;
using posting.Services.MongoDBService;

namespace posting.Services.LoaderBotService
{
    public class LoaderBotFactory : ILoaderBotFactory
    {
        public ILoaderBot Create(LoaderBotModel model, IMongoDBService mongoDBService, ILogger logger)
        {
            return new LoaderBot(model, mongoDBService, logger);
        }
    }
}
