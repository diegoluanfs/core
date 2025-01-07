using CoreAPI.Config;
using CoreAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                Log.Debug("Buscando todos os usuários no banco de dados.");
                return await _users.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar todos os usuários no banco de dados.");
                throw;
            }
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

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                Log.Debug("Buscando usuário por email no banco de dados. Email: {Email}", email);
                return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar usuário por email no banco de dados. Email: {Email}", email);
                throw;
            }
        }

        public async Task<User?> GetByEmailOrPhoneAsync(string email, string phoneNumber)
        {
            try
            {
                Log.Debug("Verificando duplicatas no banco de dados. Email: {Email}, Phone: {PhoneNumber}", email, phoneNumber);
                return await _users.Find(user => user.Email == email || user.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao verificar duplicatas no banco de dados. Email: {Email}, Phone: {PhoneNumber}", email, phoneNumber);
                throw;
            }
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            try
            {
                var objectId = ObjectId.Parse(id); // Converte a string para ObjectId
                Log.Debug("Buscando usuário no banco de dados. ID: {Id}", id);
                return await _users.Find(user => user.Id == objectId).FirstOrDefaultAsync();
            }
            catch (FormatException ex)
            {
                Log.Warning("Formato inválido para o ID: {Id}", id);
                return null; // Retorna null caso o ID seja inválido
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
                var objectId = ObjectId.Parse(id); // Converte a string para ObjectId
                Log.Debug("Atualizando usuário no banco de dados. ID: {Id}", id);
                var result = await _users.ReplaceOneAsync(u => u.Id == objectId, user);

                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    Log.Debug("Usuário atualizado com sucesso no banco de dados. ID: {Id}, Nome: {Name}, Email: {Email}, Phone: {PhoneNumber}",
                        id, user.Name, user.Email, user.PhoneNumber);
                    return user;
                }

                Log.Warning("Nenhum usuário encontrado para atualização no banco de dados. ID: {Id}", id);
                return null;
            }
            catch (FormatException ex)
            {
                Log.Warning("Formato inválido para o ID: {Id}", id);
                return null; // Retorna null caso o ID seja inválido
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
                var objectId = ObjectId.Parse(id); // Converte a string para ObjectId
                Log.Debug("Excluindo usuário no banco de dados. ID: {Id}", id);
                var result = await _users.DeleteOneAsync(user => user.Id == objectId);

                if (result.DeletedCount > 0)
                {
                    Log.Debug("Usuário excluído com sucesso no banco de dados. ID: {Id}", id);
                    return true;
                }

                Log.Warning("Nenhum usuário encontrado para exclusão no banco de dados. ID: {Id}", id);
                return false;
            }
            catch (FormatException ex)
            {
                Log.Warning("Formato inválido para o ID: {Id}", id);
                return false; // Retorna false caso o ID seja inválido
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao excluir usuário no banco de dados. ID: {Id}", id);
                throw;
            }
        }
    }
}
