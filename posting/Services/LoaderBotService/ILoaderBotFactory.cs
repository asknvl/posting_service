using posting.Models.LoaderBot;
using posting.Services.MongoDBService;

namespace posting.Services.LoaderBotService
{
    public interface ILoaderBotFactory
    {
        ILoaderBot Create(LoaderBotModel model, IMongoDBService mongoDBService, ILogger logger);
    }
}
