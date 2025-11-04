using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UsersService.Models;
using UsersService.Options;

namespace UsersService.Services;

public class TokenService(UsersDbContext db, IAccessTokenService accessTokenService, IRefreshTokenService refreshTokenService, IOptions<JWTOptions> _JWTOptions, ILogger<TokenService> _logger) : ITokenService
{
    private readonly int refreshTokenValidityHours = _JWTOptions.Value.RefreshTokenValidityHours;
    public async Task<(int uid, int rtid, bool success)> VerifyRefreshTokenAsync(string Refresh_Token)
    {
        var (uid, rt, success) = refreshTokenService.Deserialize(Refresh_Token);

        if ((success = true) && (uid > 0) && (rt.Length > 0))
        {
            var RTId = await db.RefreshTokens
            .Where(x => x.UserId == uid)
            .Where(x => x.RefreshToken == rt)
            .Where(x => x.RTokenCreatedAtUTC > DateTime.UtcNow.AddHours(-refreshTokenValidityHours))
            .Select(x => new { Id = x.Id })
            .FirstOrDefaultAsync();

            var UserId = await db.Users.Where(x => x.Id == uid)
            .Where(x => x.IsDisabled == false)
            .Select(x => new { Id = x.Id })
            .FirstOrDefaultAsync();

            if ((RTId is not null) && (UserId is not null))
            {
                return (UserId.Id, RTId.Id, true);

            }
        }

        await CleanupAsync(uid);

        return (0, 0, false);

    }

    public async Task<object?> CreateTokenAsync(int uid, int rtid)
    {
        var (RT, RTBase64) = refreshTokenService.Create(uid);


        var refreshTokenObject = new RefreshTokens
        {
            UserId = uid,
            RefreshToken = RT,
            RTokenCreatedAtUTC = DateTime.UtcNow
        };


        if (rtid != 0)
        {
            var result = await db.RefreshTokens.FindAsync(rtid);

            if (result is not null)
            {
                result.RefreshToken = RT;
                result.RTokenCreatedAtUTC = DateTime.UtcNow;

            }
            else
            {
                // In a rare theory it is possible that we VERIFIED the Refresh Token in the calling function with id {rtid} 
                // and between calling this task it has been expired and deleted by the CleanupAsync task.
                // In this case we still want a new Refresh Token so we will create one. 
                // (Otherwise we would need to send back failure and call this task again without {rtid}.)

                _logger.LogWarning("Refresh Token with {rtid} could not be found for userId: {uid}\n Maybe it was deleted by Cleanup? We will create a new one now.", rtid, uid);

                await db.RefreshTokens.AddAsync(refreshTokenObject);
            }
        }
        else
        {
            await db.RefreshTokens.AddAsync(refreshTokenObject);
        }

        await db.SaveChangesAsync();

        object token = new
        {
            token_type = "bearer",
            access_token = accessTokenService.Create(uid),
            expires_in = accessTokenService.GetValidityMins(),
            refresh_token = RTBase64
        };

        return token;

    }

    private async Task CleanupAsync(int uid)
    {
        _logger.LogInformation("Cleaning up expired Refresh Tokens for userId: {uid}", uid);

        await db.RefreshTokens
            .Where(x => x.UserId == uid)
            .Where(x => x.RTokenCreatedAtUTC < DateTime.UtcNow.AddHours(-refreshTokenValidityHours))
            .ExecuteDeleteAsync();
    }
}
