using Microsoft.OpenApi.Validations;
using posting.Models.Messages;
using posting.Services.MongoDBService;
using posting.Services.S3Service;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace posting.Utils.MessageConstructor
{
    public class MessageConstructor : IMessageConstructor
    {
        #region vars
        IMongoDBService mongoDBService;
        IS3Service s3Service;
        #endregion
        public MessageConstructor(IMongoDBService mongoDBService, IS3Service s3Service)
        {
            this.mongoDBService = mongoDBService;
            this.s3Service = s3Service;
        }

        #region helpers
        async Task downoadFile(string fileId)
        {
            await Task.CompletedTask;
        }
        #endregion

        #region public
        public async Task<MessageBase> Create(ITelegramBotClient bot, Message input)
        {
            MessageBase message;
            string? fileId = null;
            
            if (string.IsNullOrEmpty(input.MediaGroupId))
                message = new MessageBase();
            else
            {
                message = await mongoDBService.GetMessageByGroupId(input.MediaGroupId);

                if (message == null)
                    message = new MessageBase();
            }

            switch (input.Type)
            {
                case MessageType.Text:
                    break;

                case MessageType.Photo:
                    fileId = input.Photo.Last().FileId;
                    break;

                case MessageType.Video:
                    fileId = input.Video.FileId;
                    break;

                case MessageType.Document:
                    fileId = input.Document.FileId;
                    break;

                case MessageType.VideoNote:
                    fileId = input.VideoNote.FileId;
                    break;

                case MessageType.Voice:
                    fileId = input.Voice.FileId;
                    break;

                case MessageType.Poll:
                    break;
            }

            if (!string.IsNullOrEmpty(fileId))
            {
                if (message.telegram_data.medias == null)
                    message.telegram_data.medias = new List<MediaItem>();

                using (var stream = new MemoryStream())
                {
                    var file = await bot.GetInfoAndDownloadFile(fileId, stream);
                }
            }

            return message;
        }
        #endregion
    }
}
