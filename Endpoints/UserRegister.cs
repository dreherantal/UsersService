using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UsersService.Encryption;
using UsersService.Models;

namespace UsersService.Endpoints;

public class UserRegister
{
    public record Request(string FirstName, string LastName, string Email, string Password);
    public static async Task<Results<Created, Conflict<string>>> Handler(UsersDbContext db, Request request)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLower(),
            PasswordHash = SecretHasher.Hash(request.Password)
        };

        await db.Users.AddAsync(user);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException x) when (x.InnerException is SqlException inner && (inner.Number == 2627 || inner.Number == 2601))
        {
            var result = await db.Users
            .Where(x => x.Email == user.Email)
            .Select(x => new { x.Id })
            .FirstOrDefaultAsync();

            if (result != null)
            {
                return TypedResults.Conflict($"Email '{user.Email}' already registered. Please use another one to register.");
            }
        }

        return TypedResults.Created();

    }
}
