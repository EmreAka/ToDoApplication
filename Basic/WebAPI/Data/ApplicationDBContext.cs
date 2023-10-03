using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.Security;

namespace WebAPI.Data;

public class ApplicationDBContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }
    public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<ToDo> ToDos { get; set; }
    public ApplicationDBContext(DbContextOptions options) : base(options) { }
}
