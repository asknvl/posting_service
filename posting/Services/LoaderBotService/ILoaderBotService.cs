using posting.Dtos.LoaderBot;
using posting.Models.LoaderBot;

namespace posting.Services.LoaderBotService
{
    public interface ILoaderBotService
    {        
        LoaderBotDto GetLoaderBot(int direction_id);
        Task ToggleLoaderBot(int direction_id, bool state);
    }
}
