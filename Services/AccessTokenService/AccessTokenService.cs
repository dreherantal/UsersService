using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using UsersService.Options;

namespace UsersService.Services;

public class AccessTokenService (IOptions<JWTOptions> _JWTOptions) : IAccessTokenService
{
    private readonly string accessTokenSecretKey = _JWTOptions.Value.AccessTokenSecretKey;
    private readonly int accessTokenValidityMins = _JWTOptions.Value.AccessTokenValidityMins;
    private const char segmentDelimiter = '.';
    private const string _typ = "JWT";
    private const string _alg = "HS256";


    private class Header
    {
        public string? typ { get; set; }
        public string? alg { get; set; }

    }

    private class Payload
    {
        public int? uid { get; set; }
        public long? iat { get; set; }
        public long? exp { get; set; }
    }

    private string CreateMessage(int UserId)
    {
        var header = new { typ = _typ, alg = _alg };

        var payload = new Payload
        {
            uid = UserId,
            iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            exp = DateTimeOffset.UtcNow.AddMinutes(accessTokenValidityMins).ToUnixTimeSeconds()

        };

        return string.Join(segmentDelimiter, EncodeObjectToBase64(header), EncodeObjectToBase64(payload));

    }

    private static string EncodeObjectToBase64(object obj)
    {
        string base64String = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj)));

        return base64String;
    }


    private byte[] CreateSignature(string message)
    {
        var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(accessTokenSecretKey));
        byte[] hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(message));
        return hash;

    }

    public string Create(int UserId)
    {
        string message = CreateMessage(UserId);

        string signature = WebEncoders.Base64UrlEncode(CreateSignature(message));

        string JWT = string.Join(segmentDelimiter, message, signature);

        return JWT;

    }

    public int GetValidityMins()
    {
        return accessTokenValidityMins;
    }
}
