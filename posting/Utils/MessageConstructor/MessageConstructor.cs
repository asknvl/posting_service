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
        string getExtensionFromFilePath(string filepath)
        {
            var splt = filepath.Split('.'); 
            return splt[1];
        }        
        #endregion

        #region public
        public async Task<MessageBase> Create(ITelegramBotClient bot, Message input)
        {
            MessageBase message;
            string? fileId = null;
            string? caption = null;
            MessageEntity[]? entities = null;
            int? file_size = null;
            int? height = null;
            int? width = null;

            if (string.IsNullOrEmpty(input.MediaGroupId))
                message = new MessageBase();
            else
            {
                message = await mongoDBService.GetMessageByGroupId(input.MediaGroupId);

                if (message == null)
                {
                    Debug.WriteLine($"grouped new");
                    message = new MessageBase();
                } else
                {
                    Debug.WriteLine($"grouped add");
                }
            }

            message.telegram_data.text = input.Text;
            message.telegram_data.entities = input.Entities;
            message.telegram_data.mediagroup_id = input.MediaGroupId;

            switch (input.Type)
            {
                case MessageType.Text:
                    break;

                case MessageType.Photo:
                    fileId = input.Photo.Last().FileId;
                    caption = input.Caption;
                    entities = input.CaptionEntities;                                        
                    break;

                case MessageType.Video:
                    fileId = input.Video.FileId;
                    caption = input.Caption;
                    entities = input.CaptionEntities;
                    height = input.Video.Height;
                    width = input.Video.Width;
                    break;

                case MessageType.Document:
                    fileId = input.Document.FileId;
                    caption = input.Caption;
                    entities = input.CaptionEntities;
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
                {
                    Debug.WriteLine($"init medias");
                    message.telegram_data.medias = new List<MediaItem>();
                }

                using (var stream = new MemoryStream())
                {
                    var file = await bot.GetInfoAndDownloadFile(fileId, stream);
                    var bytes = stream.ToArray();
                    var s3info = await s3Service.Upload(bytes, getExtensionFromFilePath(file.FilePath));

                    Debug.WriteLine($"add media");

                    message.telegram_data.medias.Add(new MediaItem()
                    {
                        storage_id = s3info.storage_id,
                        url = s3info.url,
                        caption = caption,
                        entities = entities,
                        file_size = file.FileSize,                        
                        
                    }); 
                }
            }

            await mongoDBService.SaveMessage(message);  

            return message;
        }
        #endregion
    }
}
