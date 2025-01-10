using Microsoft.Extensions.Options;
using posting.Dtos.LoaderBot;
using posting.Models.LoaderBot;
using posting.Services.MongoDBService;
using posting.Utils.MessageConstructor;
using System.Reflection;

namespace posting.Services.LoaderBotService
{
    public class LoaderBotService : ILoaderBotService, IHostedService
    {
        #region vars
        readonly List<ILoaderBot> loaderBots = new();
        readonly ILoaderBotFactory loaderBotFactory;
        readonly ILogger logger;
        readonly List<LoaderBotModel> loaderBotModels;
        readonly IMessageConstructor messageConstructor;
        readonly IMongoDBService mongoDBService;           
        #endregion

        public LoaderBotService(IOptions<List<LoaderBotModel>> loaderBotModels,
                                ILoaderBotFactory loaderBotFactory,
                                IMongoDBService mongoDBService,  
                                IMessageConstructor messageConstructor,
                                ILogger<LoaderBotService> logger)
        {            
            this.loaderBotModels = loaderBotModels.Value;
            this.loaderBotFactory = loaderBotFactory;
            this.mongoDBService = mongoDBService;            
            this.messageConstructor = messageConstructor;
            this.logger = logger;
        }

        public LoaderBotDto GetLoaderBot(int direction_id)
        {
            var found = loaderBots.SingleOrDefault(b => b.Model.direction_id == direction_id);

            if (found != null)
            {
                return new LoaderBotDto()
                {
                    direction_id = found.Model.direction_id,
                    token = found.Model.token,
                    username = found.Username,
                    is_active = found.IsActive
                };
            }
            else
                throw new KeyNotFoundException($"LoaderBot (direction_id={direction_id}) not found");
        }

        public async Task ToggleLoaderBot(int direction_id, bool state)
        {
            var found = loaderBots.SingleOrDefault(b => b.Model.direction_id ==  direction_id);
            if (found == null)
                throw new KeyNotFoundException($"LoaderBot (direction_id={direction_id}) not found");

            if (state)
                await found.Start();
            else
                await found.Stop(); 
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Starting LoaderBots...");

            foreach (var model in loaderBotModels)
            {
                try
                {
                    var found = loaderBots.FirstOrDefault(b => b.Model.direction_id == model.direction_id);
                    if (found != null)
                    {
                        if (!found.IsActive)
                            await found.Start();
                    }
                    else
                    {                        
                        var loaderBot = loaderBotFactory.Create(model, mongoDBService, messageConstructor, logger);
                        loaderBots.Add(loaderBot);
                        await loaderBot.Start();
                    }

                }
                catch (Exception ex)
                {
                    logger.LogError($"Unable to start LoggerBot (direction_id={model.direction_id})");
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Stopping LoaderBots...");
            foreach (var loaderBot in loaderBots)
            {
                try
                {
                    await loaderBot.Stop();
                } catch (Exception ex)
                {                    
                }
            }
        }
    }
}
