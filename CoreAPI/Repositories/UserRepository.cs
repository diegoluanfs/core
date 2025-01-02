using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CoreAPI.Config;

namespace CoreAPI.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoClient mongoClient, IOptions<DatabaseSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _users = database.GetCollection<User>("Users");
        }

        public async Task<List<User>> GetAllAsync() => await _users.Find(user => true).ToListAsync();

        public async Task<User> GetByIdAsync(string id) => await _users.Find(user => user.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(User user) => await _users.InsertOneAsync(user);

        public async Task UpdateAsync(string id, User user) => 
            await _users.ReplaceOneAsync(u => u.Id == id, user);

        public async Task DeleteAsync(string id) => 
        await _users.DeleteOneAsync(user => user.Id == id);
    }
}

