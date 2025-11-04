using System;

namespace UsersService.Services;

public interface IRefreshTokenService
{
    (string RT, string RTBase64) Create(int UserId);
    (int rtid, string rt, bool success) Deserialize(string RTString);
}
