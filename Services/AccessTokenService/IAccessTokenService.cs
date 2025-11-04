using System;

namespace UsersService.Services;

public interface IAccessTokenService
{
    string Create(int UserId);
    int GetValidityMins();
}
