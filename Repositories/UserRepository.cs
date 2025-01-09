using CoreAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;

namespace CoreAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("CoreDatabase"); // Nome do banco
            _users = database.GetCollection<User>("Users"); // Nome da coleção
        }

        public async Task CreateAsync(User user)
        {
            try
            {
                Log.Information("Inserindo novo usuário no banco de dados. Nome: {Name}, Email: {Email}", user.Name, user.Email);
                await _users.InsertOneAsync(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao inserir usuário no banco de dados. Nome: {Name}, Email: {Email}", user.Name, user.Email);
                throw;
            }
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            try
            {
                var objectId = new ObjectId(id);
                Log.Information("Buscando usuário pelo ID no banco de dados. ID: {Id}", id);
                return await _users.Find(user => user.Id == objectId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar usuário pelo ID no banco de dados. ID: {Id}", id);
                throw;
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                Log.Information("Buscando usuário pelo email no banco de dados. Email: {Email}", email);
                return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar usuário pelo email no banco de dados. Email: {Email}", email);
                throw;
            }
        }

        public async Task<User?> GetByEmailOrPhoneAsync(string email, string phoneNumber)
        {
            try
            {
                Log.Information("Buscando usuário por email ou número de telefone. Email: {Email}, Phone: {PhoneNumber}", email, phoneNumber);
                return await _users.Find(user => user.Email == email || user.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar usuário por email ou telefone. Email: {Email}, Phone: {PhoneNumber}", email, phoneNumber);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                Log.Information("Buscando todos os usuários no banco de dados.");
                return await _users.Find(FilterDefinition<User>.Empty).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar todos os usuários no banco de dados.");
                throw;
            }
        }

        public async Task<User?> UpdateAsync(string id, User user)
        {
            try
            {
                var objectId = new ObjectId(id);
                Log.Information("Atualizando usuário no banco de dados. ID: {Id}", id);
                var result = await _users.ReplaceOneAsync(u => u.Id == objectId, user);

                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    return user;
                }
                else
                {
                    Log.Warning("Usuário não encontrado para atualização. ID: {Id}", id);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao atualizar usuário no banco de dados. ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var objectId = new ObjectId(id);
                Log.Information("Excluindo usuário no banco de dados. ID: {Id}", id);
                var result = await _users.DeleteOneAsync(user => user.Id == objectId);

                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao excluir usuário no banco de dados. ID: {Id}", id);
                throw;
            }
        }
    }
}
