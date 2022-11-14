using Play.Identity.Services.Entities;

namespace Play.Identity.Services;

public static class Extensions
{
    public static UserDto AsDto(this ApplicationUser user)
    {
        return new UserDto(
            user.Id,
            user.UserName,
            user.Email,
            user.Gil,
            user.CreatedOn
        );
    }
}