using CoreAPI.Models;
using CoreAPI.Repositories;
using Serilog;

namespace CoreAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task CreateUserAsync(User user)
        {
            try
            {
                Log.Information("Validando dados para criação do usuário. Nome: {Name}, Email: {Email}", user.Name, user.Email);

                // Validação básica
                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    Log.Warning("Dados inválidos: email está vazio. Nome: {Name}", user.Name);
                    throw new ArgumentException("O email do usuário não pode estar vazio.");
                }

                await _userRepository.CreateAsync(user);
                Log.Information("Usuário criado com sucesso na camada de serviço. Nome: {Name}, Email: {Email}", user.Name, user.Email);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao criar usuário na camada de serviço. Nome: {Name}, Email: {Email}", user.Name, user.Email);
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            try
            {
                Log.Information("Buscando usuário pelo ID na camada de serviço. ID: {Id}", id);
                return await _userRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar usuário na camada de serviço. ID: {Id}", id);
                throw;
            }
        }

        public async Task<User?> UpdateUserAsync(string id, User user)
        {
            try
            {
                Log.Information("Validando dados para atualização do usuário. ID: {Id}", id);

                // Validação básica
                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    Log.Warning("Dados inválidos: email está vazio para atualização. ID: {Id}", id);
                    throw new ArgumentException("O email do usuário não pode estar vazio.");
                }

                var updatedUser = await _userRepository.UpdateAsync(id, user);

                if (updatedUser != null)
                {
                    Log.Information("Usuário atualizado com sucesso na camada de serviço. ID: {Id}, Nome: {Name}, Email: {Email}",
                        id, updatedUser.Name, updatedUser.Email);
                }

                return updatedUser;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao atualizar usuário na camada de serviço. ID: {Id}, Nome: {Name}, Email: {Email}",
                    id, user.Name, user.Email);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                Log.Information("Iniciando exclusão de usuário na camada de serviço. ID: {Id}", id);
                var result = await _userRepository.DeleteAsync(id);

                if (result)
                {
                    Log.Information("Usuário excluído com sucesso na camada de serviço. ID: {Id}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao excluir usuário na camada de serviço. ID: {Id}", id);
                throw;
            }
        }
    }
}
