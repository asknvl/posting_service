using MongoDB.Driver;
using posting.Models.Messages;
using posting.Models.Users;

namespace posting.Services.MongoDBService
{
    public class MongoDBService : IMongoDBService
    {
        #region vars
        IMongoCollection<UserModel> users;
        IMongoCollection<MessageBase> messages;
        #endregion
        public MongoDBService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);

            users = database.GetCollection<UserModel>("users");
            messages = database.GetCollection<MessageBase>("messages");
        }
        #region public
        #region users
        public async Task AddOrUpdateUser(UserModel user)
        {
            var filter = Builders<UserModel>.Filter.Eq("telegram_id", user.telegram_id);

            var update = Builders<UserModel>.Update
                .Set(u => u.firstname, user.firstname)
                .Set(u => u.lastname, user.lastname)
                .Set(u => u.username, user.username)                            
                .SetOnInsert(u => u.telegram_id, user.telegram_id)
                .SetOnInsert(u => u.direction_id, user.direction_id)
                .SetOnInsert(u => u.created_at, DateTime.UtcNow);
            
            var options = new UpdateOptions { IsUpsert = true };

            var result = await users.UpdateOneAsync(filter, update, options);

            if (result.MatchedCount > 0 && result.ModifiedCount > 0)
            {
                var updateUpdatedAt = Builders<UserModel>.Update.Set(u => u.updated_at, DateTime.UtcNow);
                await users.UpdateOneAsync(filter, updateUpdatedAt);
            }
        }

        public async Task<List<UserModel>> GetUsers(int direction_id)
        {
            var filter = Builders<UserModel>.Filter.Eq("direction_id", direction_id);
            var res = await users.Find(filter).ToListAsync();
            return res;
        }

        public async Task<UserModel> GetUser(int direction_id, long telegram_id)
        {
            var filter = Builders<UserModel>.Filter.And(
                         Builders<UserModel>.Filter.Eq(u => u.direction_id, direction_id),
                         Builders<UserModel>.Filter.Eq(u => u.telegram_id, telegram_id));

            return await users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<UserModel>> RemoveUser(int direction_id, string id)
        {
            var filter = Builders<UserModel>.Filter.And(
                         Builders<UserModel>.Filter.Eq(u => u.direction_id, direction_id),
                         Builders<UserModel>.Filter.Eq(u => u.id, id));

            await users.DeleteOneAsync(filter);

            return await GetUsers(direction_id);
        }
        #endregion
        #endregion
    }
}
