using CoreAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserRepository _repository;

    public UserController(UserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repository.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        await _repository.CreateAsync(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, User user)
    {
        var existingUser = await _repository.GetByIdAsync(id);
        if (existingUser == null) return NotFound();

        user.Id = id;
        await _repository.UpdateAsync(id, user);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null) return NotFound();

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
