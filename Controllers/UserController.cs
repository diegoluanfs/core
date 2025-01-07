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
        Log.Information("Recebida requisi��o para registrar um novo usu�rio. Nome: {Name}, Email: {Email}", user?.Name, user?.Email);

        if (user == null)
        {
            Log.Warning("Tentativa de registro falhou: dados do usu�rio est�o nulos.");
            return BadRequest(new { message = "Dados inv�lidos." });
        }

        try
        {
            await _userService.CreateUserAsync(user);
            return Ok(new { message = "Usu�rio registrado com sucesso." });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao registrar usu�rio. Nome: {Name}, Email: {Email}", user.Name, user.Email);
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
    {
        Log.Information("Recebida requisi��o para autenticar um usu�rio. Email: {Email}", loginRequest.Email);

        try
        {
            var user = await _userService.Authenticate(loginRequest.Email, loginRequest.Password);

            if (user == null)
            {
                Log.Warning("Falha na autentica��o: credenciais inv�lidas. Email: {Email}", loginRequest.Email);
                return Unauthorized(new { message = "Credenciais inv�lidas." });
            }

            var token = _tokenService.GenerateToken(user);
            Log.Information("Usu�rio autenticado com sucesso. Email: {Email}", loginRequest.Email);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao autenticar usu�rio. Email: {Email}", loginRequest.Email);
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        Log.Information("Recebida requisi��o para obter o usu�rio. ID: {Id}", id);

        try
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                Log.Warning("Usu�rio n�o encontrado. ID: {Id}", id);
                return NotFound(new { message = "Usu�rio n�o encontrado." });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao obter usu�rio. ID: {Id}", id);
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
    {
        Log.Information("Recebida requisi��o para atualizar o usu�rio. ID: {Id}", id);

        if (user == null)
        {
            Log.Warning("Tentativa de atualiza��o falhou: dados do usu�rio est�o nulos.");
            return BadRequest(new { message = "Dados inv�lidos." });
        }

        try
        {
            var updatedUser = await _userService.UpdateUserAsync(id, user);

            if (updatedUser == null)
            {
                Log.Warning("Usu�rio n�o encontrado para atualiza��o. ID: {Id}", id);
                return NotFound(new { message = "Usu�rio n�o encontrado." });
            }

            return Ok(new { message = "Usu�rio atualizado com sucesso.", updatedUser });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao atualizar usu�rio. ID: {Id}", id);
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        Log.Information("Recebida requisi��o para excluir o usu�rio. ID: {Id}", id);

        try
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                Log.Warning("Usu�rio n�o encontrado para exclus�o. ID: {Id}", id);
                return NotFound(new { message = "Usu�rio n�o encontrado." });
            }

            return Ok(new { message = "Usu�rio exclu�do com sucesso." });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao excluir usu�rio. ID: {Id}", id);
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

    [Authorize]
    [HttpGet("protected")]
    public IActionResult ProtectedEndpoint()
    {
        Log.Information("Endpoint protegido acessado.");
        return Ok(new { message = "Voc� acessou um endpoint protegido!" });
    }
}
