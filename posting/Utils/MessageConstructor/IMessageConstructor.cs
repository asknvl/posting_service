using posting.Models.Messages;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace posting.Utils.MessageConstructor
{
    public interface IMessageConstructor
    {
        Task<MessageBase> Create(ITelegramBotClient bot, Message message); 
    }
}
