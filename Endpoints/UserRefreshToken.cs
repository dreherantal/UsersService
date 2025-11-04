using Microsoft.AspNetCore.Http.HttpResults;
using UsersService.Services;

namespace UsersService.Endpoints;

public class UserRefreshToken
{
    public record Request(string Refresh_Token);
    public static async Task<Results<UnauthorizedHttpResult, JsonHttpResult<object>>> Handler(Request request, ITokenService tokenService)
    {
        var (uid, rtid, success) = await tokenService.VerifyRefreshTokenAsync(request.Refresh_Token);

        if (success is true)
        {
            var token = await tokenService.CreateTokenAsync(uid, rtid);

            return TypedResults.Json(token);
        }

        return TypedResults.Unauthorized();


    }
}
