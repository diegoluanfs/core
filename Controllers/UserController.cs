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
        Log.Information("Verificando se o usuário está autenticado.");

        // Obter as claims do usuário autenticado
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        Log.Information("Claims disponíveis: {@Claims}", claims);

        // Verificar se as claims necessárias estão presentes
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            Log.Warning("A claim 'sub' (userId) está ausente.");
            return Unauthorized(new ApiResponse<object>(
                StatusCodes.Status401Unauthorized,
                "Usuário não autenticado: 'userId' não encontrado."
            ));
        }

        if (string.IsNullOrEmpty(email))
        {
            Log.Warning("A claim 'email' está ausente.");
            return Unauthorized(new ApiResponse<object>(
                StatusCodes.Status401Unauthorized,
                "Usuário não autenticado: 'email' não encontrado."
            ));
        }

        // Retornar mensagem de sucesso
        Log.Information("Usuário autenticado com sucesso. UserId: {UserId}, Email: {Email}", userId, email);
        return Ok(new ApiResponse<object>(
            StatusCodes.Status200OK,
            "Usuário autenticado com sucesso.",
            new { UserId = userId, Email = email }
        ));
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        Log.Information("Recebida requisição para registrar um novo usuário.");

        if (user == null)
        {
            return BadRequest(new ApiResponse<object>(
                StatusCodes.Status400BadRequest,
                "Os dados do usuário não podem estar vazios."
            ));
        }

        try
        {
            await _userService.CreateUserAsync(user);
            return Ok(new ApiResponse<object>(
                StatusCodes.Status200OK,
                "Usuário registrado com sucesso."
            ));
        }
        catch (ArgumentException ex)
        {
            Log.Warning(ex, "Erro de validação ao registrar o usuário.");
            return BadRequest(new ApiResponse<object>(
                StatusCodes.Status400BadRequest,
                ex.Message
            ));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao registrar usuário.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>(
                StatusCodes.Status500InternalServerError,
                "Erro interno no servidor."
            ));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
    {
        Log.Information("Recebida requisição para autenticar um usuário.");

        try
        {
            var user = await _userService.Authenticate(loginRequest.Email, loginRequest.Password);

            if (user == null)
            {
                return Unauthorized(new ApiResponse<object>(
                    StatusCodes.Status401Unauthorized,
                    "Credenciais inválidas."
                ));
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new ApiResponse<object>(
                StatusCodes.Status200OK,
                "Usuário autenticado com sucesso.",
                new { token }
            ));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao autenticar usuário.");
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
        Log.Information("Recebida requisição para obter o usuário. ID: {Id}", id);

        try
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new ApiResponse<object>(
                    StatusCodes.Status404NotFound,
                    "Usuário não encontrado."
                ));
            }

            return Ok(new ApiResponse<User>(
                StatusCodes.Status200OK,
                "Usuário encontrado com sucesso.",
                user
            ));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao obter usuário. ID: {Id}", id);
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
        Log.Information("Recebida requisição para atualizar o usuário. ID: {Id}", id);

        if (user == null)
        {
            return BadRequest(new ApiResponse<object>(
                StatusCodes.Status400BadRequest,
                "Dados inválidos para atualização do usuário."
            ));
        }

        try
        {
            var updatedUser = await _userService.UpdateUserAsync(id, user);

            if (updatedUser == null)
            {
                return NotFound(new ApiResponse<object>(
                    StatusCodes.Status404NotFound,
                    "Usuário não encontrado para atualização."
                ));
            }

            return Ok(new ApiResponse<User>(
                StatusCodes.Status200OK,
                "Usuário atualizado com sucesso.",
                updatedUser
            ));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao atualizar usuário. ID: {Id}", id);
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
        Log.Information("Recebida requisição para excluir o usuário. ID: {Id}", id);

        try
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>(
                    StatusCodes.Status404NotFound,
                    "Usuário não encontrado para exclusão."
                ));
            }

            return Ok(new ApiResponse<object>(
                StatusCodes.Status200OK,
                "Usuário excluído com sucesso."
            ));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao excluir usuário. ID: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>(
                StatusCodes.Status500InternalServerError,
                "Erro interno no servidor."
            ));
        }
    }
}
