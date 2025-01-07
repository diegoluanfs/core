using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoreAPI.Models;
using CoreAPI.Services;
using Serilog;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public UserController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        Log.Information("Recebida requisição para registrar um novo usuário. Nome: {Name}, Email: {Email}", user?.Name, user?.Email);

        if (user == null)
        {
            Log.Warning("Tentativa de registro falhou: dados do usuário estão nulos.");
            return BadRequest(new { message = "Dados inválidos." });
        }

        try
        {
            await _userService.CreateUserAsync(user);
            return Ok(new { message = "Usuário registrado com sucesso." });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao registrar usuário. Nome: {Name}, Email: {Email}", user.Name, user.Email);
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
    {
        Log.Information("Recebida requisição para autenticar um usuário. Email: {Email}", loginRequest.Email);

        try
        {
            var user = await _userService.Authenticate(loginRequest.Email, loginRequest.Password);

            if (user == null)
            {
                Log.Warning("Falha na autenticação: credenciais inválidas. Email: {Email}", loginRequest.Email);
                return Unauthorized(new { message = "Credenciais inválidas." });
            }

            var token = _tokenService.GenerateToken(user);
            Log.Information("Usuário autenticado com sucesso. Email: {Email}", loginRequest.Email);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao autenticar usuário. Email: {Email}", loginRequest.Email);
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        Log.Information("Recebida requisição para obter o usuário. ID: {Id}", id);

        try
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                Log.Warning("Usuário não encontrado. ID: {Id}", id);
                return NotFound(new { message = "Usuário não encontrado." });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao obter usuário. ID: {Id}", id);
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
    {
        Log.Information("Recebida requisição para atualizar o usuário. ID: {Id}", id);

        if (user == null)
        {
            Log.Warning("Tentativa de atualização falhou: dados do usuário estão nulos.");
            return BadRequest(new { message = "Dados inválidos." });
        }

        try
        {
            var updatedUser = await _userService.UpdateUserAsync(id, user);

            if (updatedUser == null)
            {
                Log.Warning("Usuário não encontrado para atualização. ID: {Id}", id);
                return NotFound(new { message = "Usuário não encontrado." });
            }

            return Ok(new { message = "Usuário atualizado com sucesso.", updatedUser });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao atualizar usuário. ID: {Id}", id);
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        Log.Information("Recebida requisição para excluir o usuário. ID: {Id}", id);

        try
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                Log.Warning("Usuário não encontrado para exclusão. ID: {Id}", id);
                return NotFound(new { message = "Usuário não encontrado." });
            }

            return Ok(new { message = "Usuário excluído com sucesso." });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao excluir usuário. ID: {Id}", id);
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

    [Authorize]
    [HttpGet("protected")]
    public IActionResult ProtectedEndpoint()
    {
        Log.Information("Endpoint protegido acessado.");
        return Ok(new { message = "Você acessou um endpoint protegido!" });
    }
}
