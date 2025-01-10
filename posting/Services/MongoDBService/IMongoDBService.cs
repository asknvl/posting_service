using MongoDB.Driver;
using posting.Models.Users;

namespace posting.Services.MongoDBService
{
    public interface IMongoDBService
    {
        Task AddOrUpdateUser(UserModel user);
        Task<List<UserModel>> GetUsers(int direction_id);
        Task<UserModel> GetUser(int direction_id, long telegram_id);
        Task<List<UserModel>> RemoveUser(int direction_id, string id);
    }
}
