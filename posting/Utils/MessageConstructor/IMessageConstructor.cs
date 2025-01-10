using posting.Models.Messages;
using Telegram.Bot.Types;

namespace posting.Utils.MessageConstructor
{
    public interface IMessageConstructor
    {
        Task<MessageBase> Create(Message message); 
    }
}
