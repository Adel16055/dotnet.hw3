using Homework.Contracts;
using Homework.Services;
using Microsoft.AspNetCore.Mvc;

namespace Homework.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(UserService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] UserFilter filter) => Ok(await service.GetAllAsync(filter));

    [HttpPost]
    public async Task<IActionResult> Create(SaveUserRequest request)
    {
        try { return StatusCode(201, await service.CreateAsync(request)); }
        catch (DuplicateLoginException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPost("authorize")]
    public async Task<IActionResult> Authorize(LoginRequest request)
    {
        var user = await service.AuthorizeAsync(request);
        return user is null ? Unauthorized(new { message = "Неверный логин или пароль." }) : Ok(user);
    }

    [HttpPut("{login}")]
    public async Task<IActionResult> Edit(string login, SaveUserRequest request)
    {
        try
        {
            var user = await service.EditAsync(login, request);
            return user is null ? NotFound() : Ok(user);
        }
        catch (DuplicateLoginException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpDelete("{login}")]
    public async Task<IActionResult> Delete(string login) =>
        await service.DeleteAsync(login) ? NoContent() : NotFound();
}
