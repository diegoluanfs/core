using Microsoft.AspNetCore.Mvc;
using CoreAPI.Models;
using CoreAPI.Services;
using Serilog;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        Log.Information("Recebida requisi��o para listar todos os usu�rios.");

        try
        {
            var users = await _userService.GetAllUsersAsync();

            // Converte ObjectId para string antes de retornar
            var response = users.Select(u => new
            {
                Id = u.Id.ToString(),
                u.Name,
                u.Email,
                u.PhoneNumber
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao listar todos os usu�rios.");
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        Log.Information("Recebida requisi��o para criar um novo usu�rio. Nome: {Name}, Email: {Email}", user?.Name, user?.Email);

        if (user == null)
        {
            Log.Warning("Tentativa de cria��o falhou: dados do usu�rio est�o nulos.");
            return BadRequest(new { message = "Dados inv�lidos." });
        }

        try
        {
            await _userService.CreateUserAsync(user);
            return Ok(new { message = "Usu�rio criado com sucesso." });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao criar usu�rio. Nome: {Name}, Email: {Email}", user.Name, user.Email);
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

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
}
