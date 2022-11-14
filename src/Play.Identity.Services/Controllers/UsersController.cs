using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Play.Identity.Contracts;
using Play.Identity.Services.Entities;
using static IdentityServer4.IdentityServerConstants;

namespace Play.Identity.Services.Controllers;

[ApiController]
[Route("users")]
[Authorize(Policy = LocalApi.PolicyName, Roles = Roles.Admin)]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPublishEndpoint publishEndpoint;

    public UsersController(UserManager<ApplicationUser> userManager, IPublishEndpoint publishEndpoint)
    {
        this._userManager = userManager;
        this.publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> Get()
    {
        var users = this._userManager.Users
            .ToList()
            .Select(user => user.AsDto());

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await this._userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return NotFound();

        return Ok(user.AsDto());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> PutAsync(Guid id, UpdateUserDto userDto)
    {
        var user = await this._userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return NotFound();

        user.UserName = userDto.Email;
        user.Gil = userDto.Gil;

        var result = await this._userManager.UpdateAsync(user);

        await publishEndpoint.Publish(new UserUpdated(user.Id, user.Email, user.Gil));

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        var user = await this._userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return NotFound();

        var result = await this._userManager.DeleteAsync(user);

        await publishEndpoint.Publish(new UserUpdated(user.Id, user.Email, 0));

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return NoContent();
    }
}