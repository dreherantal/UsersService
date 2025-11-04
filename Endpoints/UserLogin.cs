using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UsersService.Encryption;
using UsersService.Models;
using UsersService.Services;

namespace UsersService.Endpoints;

public class UserLogin
{
    public record Request(string Email, string Password);

    public static async Task<Results<UnauthorizedHttpResult, JsonHttpResult<object>>> Handler(UsersDbContext db, Request request, ITokenService tokenService)
    {
        var User = await db.Users
        .Where(x => x.Email == request.Email.ToLower())
        .Select(x => new { x.Email, x.PasswordHash, x.Id, x.IsDisabled })
        .FirstOrDefaultAsync();

        if ((User is not null) && (User.IsDisabled is false) && SecretHasher.Verify(request.Password, User.PasswordHash))
        {
            var token = await tokenService.CreateTokenAsync(User.Id);

            return TypedResults.Json(token);

        }
        else
        {
            return TypedResults.Unauthorized();
        }
    }
}
