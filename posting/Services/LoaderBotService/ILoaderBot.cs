using posting.Models.LoaderBot;

namespace posting.Services.LoaderBotService
{
    public interface ILoaderBot
    {
        LoaderBotModel Model { get; }
        string? Username { get; }
        bool IsActive { get; }  
        Task Start();
        Task Stop();
    }

    public class LoaderBotException : Exception
    {
        public LoaderBotException(string msg) : base(msg) { }
    }
}
