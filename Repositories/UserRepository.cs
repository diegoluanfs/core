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
            _users = database.GetCollection<User>("Users"); // Nome da cole��o
        }

        public async Task CreateAsync(User user)
        {
            try
            {
                Log.Information("Inserindo novo usu�rio no banco de dados. Nome: {Name}, Email: {Email}", user.Name, user.Email);
                await _users.InsertOneAsync(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao inserir usu�rio no banco de dados. Nome: {Name}, Email: {Email}", user.Name, user.Email);
                throw;
            }
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            try
            {
                var objectId = new ObjectId(id);
                Log.Information("Buscando usu�rio pelo ID no banco de dados. ID: {Id}", id);
                return await _users.Find(user => user.Id == objectId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar usu�rio pelo ID no banco de dados. ID: {Id}", id);
                throw;
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                Log.Information("Buscando usu�rio pelo email no banco de dados. Email: {Email}", email);
                return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar usu�rio pelo email no banco de dados. Email: {Email}", email);
                throw;
            }
        }

        public async Task<User?> GetByEmailOrPhoneAsync(string email, string phoneNumber)
        {
            try
            {
                Log.Information("Buscando usu�rio por email ou n�mero de telefone. Email: {Email}, Phone: {PhoneNumber}", email, phoneNumber);
                return await _users.Find(user => user.Email == email || user.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar usu�rio por email ou telefone. Email: {Email}, Phone: {PhoneNumber}", email, phoneNumber);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                Log.Information("Buscando todos os usu�rios no banco de dados.");
                return await _users.Find(FilterDefinition<User>.Empty).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar todos os usu�rios no banco de dados.");
                throw;
            }
        }

        public async Task<User?> UpdateAsync(string id, User user)
        {
            try
            {
                var objectId = new ObjectId(id);
                Log.Information("Atualizando usu�rio no banco de dados. ID: {Id}", id);
                var result = await _users.ReplaceOneAsync(u => u.Id == objectId, user);

                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    return user;
                }
                else
                {
                    Log.Warning("Usu�rio n�o encontrado para atualiza��o. ID: {Id}", id);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao atualizar usu�rio no banco de dados. ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var objectId = new ObjectId(id);
                Log.Information("Excluindo usu�rio no banco de dados. ID: {Id}", id);
                var result = await _users.DeleteOneAsync(user => user.Id == objectId);

                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao excluir usu�rio no banco de dados. ID: {Id}", id);
                throw;
            }
        }
    }
}
