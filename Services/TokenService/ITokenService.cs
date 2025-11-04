namespace UsersService.Services;

public interface ITokenService
{
    Task<(int uid, int rtid, bool success)> VerifyRefreshTokenAsync(string Refresh_Token);
    Task <object?> CreateTokenAsync(int uid, int rtid = 0 );
}
