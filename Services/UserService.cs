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

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                Log.Information("Buscando todos os usuários na camada de serviço.");
                return await _userRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar todos os usuários na camada de serviço.");
                throw;
            }
        }

        public async Task CreateUserAsync(User user)
        {
            try
            {
                Log.Information("Validando dados para criação do usuário. Nome: {Name}, Email: {Email}, Phone: {PhoneNumber}",
                    user.Name, user.Email, user.PhoneNumber);

                // Validação de email
                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    Log.Warning("Dados inválidos: email está vazio. Nome: {Name}", user.Name);
                    throw new ArgumentException("O email do usuário não pode estar vazio.");
                }

                // Validação de senha
                if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 6)
                {
                    Log.Warning("Dados inválidos: senha muito curta. Nome: {Name}", user.Name);
                    throw new ArgumentException("A senha deve ter pelo menos 6 caracteres.");
                }

                // Validação de número de telefone
                if (string.IsNullOrWhiteSpace(user.PhoneNumber) || !System.Text.RegularExpressions.Regex.IsMatch(user.PhoneNumber, @"^\+?[1-9]\d{1,14}$"))
                {
                    Log.Warning("Dados inválidos: número de telefone incorreto. Nome: {Name}", user.Name);
                    throw new ArgumentException("O número de telefone é inválido.");
                }

                // Verificar duplicatas
                var existingUser = await _userRepository.GetByEmailOrPhoneAsync(user.Email, user.PhoneNumber);
                if (existingUser != null)
                {
                    Log.Warning("Tentativa de cadastro falhou: usuário com o mesmo email ou número de telefone já existe. Email: {Email}, Phone: {PhoneNumber}",
                        user.Email, user.PhoneNumber);
                    throw new ArgumentException("Já existe um usuário com este email ou número de telefone.");
                }

                await _userRepository.CreateAsync(user);
                Log.Information("Usuário criado com sucesso na camada de serviço. Nome: {Name}, Email: {Email}, Phone: {PhoneNumber}",
                    user.Name, user.Email, user.PhoneNumber);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao criar usuário na camada de serviço. Nome: {Name}, Email: {Email}, Phone: {PhoneNumber}",
                    user.Name, user.Email, user.PhoneNumber);
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

                // Validação de email
                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    Log.Warning("Dados inválidos: email está vazio para atualização. ID: {Id}", id);
                    throw new ArgumentException("O email do usuário não pode estar vazio.");
                }

                // Validação de senha
                if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 6)
                {
                    Log.Warning("Dados inválidos: senha muito curta para atualização. ID: {Id}", id);
                    throw new ArgumentException("A senha deve ter pelo menos 6 caracteres.");
                }

                // Validação de número de telefone
                if (string.IsNullOrWhiteSpace(user.PhoneNumber) || !System.Text.RegularExpressions.Regex.IsMatch(user.PhoneNumber, @"^\+?[1-9]\d{1,14}$"))
                {
                    Log.Warning("Dados inválidos: número de telefone incorreto para atualização. ID: {Id}", id);
                    throw new ArgumentException("O número de telefone é inválido.");
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
