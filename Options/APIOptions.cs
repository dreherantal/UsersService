using System;
using System.ComponentModel.DataAnnotations;

namespace UsersService.Options;

public class APIOptions
{
    [StringLength(32)]
    public required string UsersServiceAPIKey { get; set; }
}

public class JWTOptions
{   
    [StringLength(32)]
    public required string AccessTokenSecretKey { get; set; }
    public required int AccessTokenValidityMins { get; set; }
    public required int RefreshTokenValidityHours { get; set; }
    
}
