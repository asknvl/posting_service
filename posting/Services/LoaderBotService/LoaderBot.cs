using Microsoft.Extensions.Logging;
using posting.Models.LoaderBot;
using posting.Models.Users;
using posting.Services.MongoDBService;
using posting.Utils.MessageConstructor;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace posting.Services.LoaderBotService
{
    public class LoaderBot : ILoaderBot
    {
        public LoaderBotModel Model { get; }
        public string? Username { get; set; }    
        public bool IsActive { get; protected set; }

        #region vars
        ITelegramBotClient bot;
        CancellationTokenSource cts;
        IMongoDBService mongoDBService;        
        ILogger logger;
        IMessageConstructor messageConstructor;
        #endregion

        public LoaderBot(LoaderBotModel model, IMongoDBService mongoDBService, IMessageConstructor messageConstructor, ILogger logger)
        {
            Model = model;
            this.mongoDBService = mongoDBService;
            this.messageConstructor = messageConstructor;   
            this.logger = logger;
        }

        #region helpers
        bool checkPassword(string text)
        {
            if (!string.IsNullOrEmpty(text) && text.StartsWith("PASSWORD:"))
            {
                var splt = text.Split(":");
                string shash = "";

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(splt[1]);
                    byte[] hash = sha256.ComputeHash(bytes);

                    // Конвертируем байты в строку HEX
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hash)
                    {
                        sb.Append(b.ToString("x2")); // "x2" — форматирование в HEX
                    }

                    shash = sb.ToString();
                }

                return shash.Equals(Model.access_hash);
            }

            return false;
        }

        async Task<bool> checkOperator(long telegram_id)
        {
            var found = await mongoDBService.GetUser(Model.direction_id, telegram_id);
            return found != null;
        }
        #endregion

        #region private
        async Task HandleUpdateAsync(ITelegramBotClient bot, Update update,  CancellationToken cancellationToken)
        {
            if (update.Message == null)
                return;

            try
            {
                var operators = await mongoDBService.GetUsers(Model.direction_id);

                var text = update.Message.Text;
                var from = update.Message.From;

                if (checkPassword(text))
                {
                    if (string.IsNullOrEmpty(from.Username))
                    {
                        await bot.SendMessage(from.Id, "Требуется установить юзернейм у вашего аккаунта и ввести пароль еще раз");
                        return;
                    }

                    await mongoDBService.AddOrUpdateUser(new UserModel() {
                        telegram_id = from.Id,
                        username = from.Username,
                        firstname = from.FirstName,
                        lastname = from.LastName
                    });

                } else
                {
                    var isOperator = await checkOperator(from.Id);
                    if (isOperator)
                    {

                        logger.LogInformation($"{Username}: {update.Message}");

                        var m = messageConstructor.Create(bot, update.Message);

                        //await
                        
                        //foreach (var op in operators)
                        //    await bot.ForwardMessage(op.telegram_id, op.telegram_id, update.Message.Id);
                    }
                    else
                        await bot.SendMessage(from.Id, "Вы не авторизованы");

                }

            } catch (Exception ex)
            {

            }

            await Task.CompletedTask;
        }
        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"{Username} Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };            
            return Task.CompletedTask;
        }
        #endregion

        #region public
        public async Task Start()
        {
            if (IsActive)
                return;
                //throw new LoaderBotException($"Bot direction_id={Model.direction_id} aleready running");

#if DEBUG
            bot = new TelegramBotClient(Model.token);
#elif RELEASE
            bot = new TelegramBotClient(new TelegramBotClientOptions(Model.token, "http://localhost:8081/bot/"));
#endif

            var u = await bot.GetMe();
            Username = u.Username;

            cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[]
                {
                    UpdateType.Message
                }
            };

            bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);

            IsActive = true;
            await Task.CompletedTask;
        }

        public async Task Stop()
        {            
            cts?.Cancel();
            IsActive = false;
            await Task.CompletedTask;
        }
#endregion
    }
}
