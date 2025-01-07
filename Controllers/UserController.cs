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
        Log.Information("Recebida requisição para listar todos os usuários.");

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
            Log.Error(ex, "Erro ao listar todos os usuários.");
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        Log.Information("Recebida requisição para criar um novo usuário. Nome: {Name}, Email: {Email}", user?.Name, user?.Email);

        if (user == null)
        {
            Log.Warning("Tentativa de criação falhou: dados do usuário estão nulos.");
            return BadRequest(new { message = "Dados inválidos." });
        }

        try
        {
            await _userService.CreateUserAsync(user);
            return Ok(new { message = "Usuário criado com sucesso." });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao criar usuário. Nome: {Name}, Email: {Email}", user.Name, user.Email);
            return StatusCode(500, new { message = "Erro interno no servidor." });
        }
    }

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
}
