using CoreAPI.Config;
using CoreAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

namespace CoreAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoClient mongoClient, IOptions<DatabaseSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _users = database.GetCollection<User>("Users");
        }

        public async Task CreateAsync(User user)
        {
            try
            {
                Log.Debug("Inserindo usuário no banco de dados. Nome: {Name}, Email: {Email}, Phone: {PhoneNumber}",
                    user.Name, user.Email, user.PhoneNumber);
                await _users.InsertOneAsync(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao inserir usuário no banco de dados. Nome: {Name}, Email: {Email}, Phone: {PhoneNumber}",
                    user.Name, user.Email, user.PhoneNumber);
                throw;
            }
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            try
            {
                Log.Debug("Buscando usuário no banco de dados. ID: {Id}", id);
                return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar usuário no banco de dados. ID: {Id}", id);
                throw;
            }
        }

        public async Task<User?> UpdateAsync(string id, User user)
        {
            try
            {
                Log.Debug("Atualizando usuário no banco de dados. ID: {Id}", id);
                var result = await _users.ReplaceOneAsync(u => u.Id == id, user);

                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    Log.Debug("Usuário atualizado com sucesso no banco de dados. ID: {Id}, Nome: {Name}, Email: {Email}, Phone: {PhoneNumber}",
                        id, user.Name, user.Email, user.PhoneNumber);
                    return user;
                }

                Log.Warning("Nenhum usuário encontrado para atualização no banco de dados. ID: {Id}", id);
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao atualizar usuário no banco de dados. ID: {Id}, Nome: {Name}, Email: {Email}, Phone: {PhoneNumber}",
                    id, user.Name, user.Email, user.PhoneNumber);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                Log.Debug("Excluindo usuário no banco de dados. ID: {Id}", id);
                var result = await _users.DeleteOneAsync(user => user.Id == id);

                if (result.DeletedCount > 0)
                {
                    Log.Debug("Usuário excluído com sucesso no banco de dados. ID: {Id}", id);
                    return true;
                }

                Log.Warning("Nenhum usuário encontrado para exclusão no banco de dados. ID: {Id}", id);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao excluir usuário no banco de dados. ID: {Id}", id);
                throw;
            }
        }
    }
}
