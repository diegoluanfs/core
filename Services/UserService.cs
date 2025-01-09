using CoreAPI.Models;
using CoreAPI.Repositories;
using Serilog;
using System.Text.RegularExpressions;

namespace CoreAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
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
            // Validações específicas de cada campo
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                throw new ArgumentException("O campo 'Name' não pode estar vazio.");
            }

            if (string.IsNullOrWhiteSpace(user.Email) || !IsValidEmail(user.Email))
            {
                throw new ArgumentException("O campo 'Email' é inválido ou está vazio.");
            }

            if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 6)
            {
                throw new ArgumentException("O campo 'Password' deve ter pelo menos 6 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(user.PhoneNumber) || !IsValidPhoneNumber(user.PhoneNumber))
            {
                throw new ArgumentException("O campo 'PhoneNumber' é inválido ou está vazio.");
            }

            // Verificar duplicidade de email ou telefone
            var existingUser = await _userRepository.GetByEmailOrPhoneAsync(user.Email, user.PhoneNumber);
            if (existingUser != null)
            {
                throw new ArgumentException("Já existe um usuário com este email ou número de telefone.");
            }

            try
            {
                Log.Information("Registrando novo usuário. Nome: {Name}, Email: {Email}, Phone: {PhoneNumber}",
                    user.Name, user.Email, user.PhoneNumber);
                await _userRepository.CreateAsync(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao registrar novo usuário. Nome: {Name}, Email: {Email}", user.Name, user.Email);
                throw new Exception("Erro interno ao registrar o usuário.");
            }
        }

        public async Task<User?> Authenticate(string email, string password)
        {
            Log.Information("Iniciando autenticação para o email: {Email}", email);

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null || user.Password != password)
            {
                Log.Warning("Falha na autenticação: credenciais inválidas. Email: {Email}", email);
                return null;
            }

            Log.Information("Usuário autenticado com sucesso. Email: {Email}", email);
            return user;
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
                Log.Error(ex, "Erro ao atualizar usuário na camada de serviço. ID: {Id}", id);
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

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^\+?[1-9]\d{1,14}$");
        }
    }
}
