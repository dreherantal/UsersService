using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace UsersService.Models;

public class User
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    [MaxLength(200)]
    public required string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAtUTC { get; init; } = DateTime.UtcNow;
    public bool IsDisabled { get; set; } = false;


}

public class RefreshTokens
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RTokenCreatedAtUTC { get; set; }

}

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshTokens> RefreshTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}