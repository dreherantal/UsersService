using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace UsersService.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private const int size = 32;
    public (string RT, string RTBase64) Create(int UserId)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        string RT = RandomNumberGenerator.GetString(chars, size);

        object RTO = new { Id = UserId, RT = RT };

        string RTBase64 = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(RTO)));

        return (RT, RTBase64);
    }

    public (int rtid, string rt, bool success) Deserialize(string RTString)
    {
        try
        {
            var result = JsonSerializer.Deserialize<RTObject>(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(RTString)));

            if (result is not null)
            {
                return (result.Id, result.RT, true);

            } return (0, "", false);

            
        }
        catch
        {
            return (0, "", false);
        }
    }
       
}

public class RTObject
{
    public required int Id { get; set; }
    public required string RT { get; set; }

}

