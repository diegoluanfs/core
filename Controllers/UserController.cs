using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoreAPI.Models;
using CoreAPI.Services;
using Serilog;
using System.Security.Claims;

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

    [Authorize]
    [HttpGet("check-logged-in")]
    public IActionResult CheckLoggedIn()
    {
        Log.Information("Verificando se o usu�rio est� autenticado.");

        // Obter as claims do usu�rio autenticado
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        Log.Information("Claims dispon�veis: {@Claims}", claims);

        // Verificar se as claims necess�rias est�o presentes
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            Log.Warning("A claim 'sub' (userId) est� ausente.");
            return Unauthorized(new ApiResponse<object>(
                StatusCodes.Status401Unauthorized,
                "Usu�rio n�o autenticado: 'userId' n�o encontrado."
            ));
        }

        if (string.IsNullOrEmpty(email))
        {
            Log.Warning("A claim 'email' est� ausente.");
            return Unauthorized(new ApiResponse<object>(
                StatusCodes.Status401Unauthorized,
                "Usu�rio n�o autenticado: 'email' n�o encontrado."
            ));
        }

        // Retornar mensagem de sucesso
        Log.Information("Usu�rio autenticado com sucesso. UserId: {UserId}, Email: {Email}", userId, email);
        return Ok(new ApiResponse<object>(
            StatusCodes.Status200OK,
            "Usu�rio autenticado com sucesso.",
            new { UserId = userId, Email = email }
        ));
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        Log.Information("Recebida requisi��o para registrar um novo usu�rio.");

        if (user == null)
        {
            return BadRequest(new ApiResponse<object>(
                StatusCodes.Status400BadRequest,
                "Os dados do usu�rio n�o podem estar vazios."
            ));
        }

        try
        {
            await _userService.CreateUserAsync(user);
            return Ok(new ApiResponse<object>(
                StatusCodes.Status200OK,
                "Usu�rio registrado com sucesso."
            ));
        }
        catch (ArgumentException ex)
        {
            Log.Warning(ex, "Erro de valida��o ao registrar o usu�rio.");
            return BadRequest(new ApiResponse<object>(
                StatusCodes.Status400BadRequest,
                ex.Message
            ));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao registrar usu�rio.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>(
                StatusCodes.Status500InternalServerError,
                "Erro interno no servidor."
            ));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
    {
        Log.Information("Recebida requisi��o para autenticar um usu�rio.");

        try
        {
            var user = await _userService.Authenticate(loginRequest.Email, loginRequest.Password);

            if (user == null)
            {
                return Unauthorized(new ApiResponse<object>(
                    StatusCodes.Status401Unauthorized,
                    "Credenciais inv�lidas."
                ));
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new ApiResponse<object>(
                StatusCodes.Status200OK,
                "Usu�rio autenticado com sucesso.",
                new { token }
            ));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao autenticar usu�rio.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>(
                StatusCodes.Status500InternalServerError,
                "Erro interno no servidor."
            ));
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
                return NotFound(new ApiResponse<object>(
                    StatusCodes.Status404NotFound,
                    "Usu�rio n�o encontrado."
                ));
            }

            return Ok(new ApiResponse<User>(
                StatusCodes.Status200OK,
                "Usu�rio encontrado com sucesso.",
                user
            ));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao obter usu�rio. ID: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>(
                StatusCodes.Status500InternalServerError,
                "Erro interno no servidor."
            ));
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
    {
        Log.Information("Recebida requisi��o para atualizar o usu�rio. ID: {Id}", id);

        if (user == null)
        {
            return BadRequest(new ApiResponse<object>(
                StatusCodes.Status400BadRequest,
                "Dados inv�lidos para atualiza��o do usu�rio."
            ));
        }

        try
        {
            var updatedUser = await _userService.UpdateUserAsync(id, user);

            if (updatedUser == null)
            {
                return NotFound(new ApiResponse<object>(
                    StatusCodes.Status404NotFound,
                    "Usu�rio n�o encontrado para atualiza��o."
                ));
            }

            return Ok(new ApiResponse<User>(
                StatusCodes.Status200OK,
                "Usu�rio atualizado com sucesso.",
                updatedUser
            ));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao atualizar usu�rio. ID: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>(
                StatusCodes.Status500InternalServerError,
                "Erro interno no servidor."
            ));
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
                return NotFound(new ApiResponse<object>(
                    StatusCodes.Status404NotFound,
                    "Usu�rio n�o encontrado para exclus�o."
                ));
            }

            return Ok(new ApiResponse<object>(
                StatusCodes.Status200OK,
                "Usu�rio exclu�do com sucesso."
            ));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao excluir usu�rio. ID: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>(
                StatusCodes.Status500InternalServerError,
                "Erro interno no servidor."
            ));
        }
    }
}
