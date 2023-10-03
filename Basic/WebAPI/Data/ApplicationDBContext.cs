using Microsoft.EntityFrameworkCore;
using System;
using WebAPI.Security;

namespace WebAPI.Data;

public class ApplicationDBContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }
    public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public ApplicationDBContext(DbContextOptions options) : base(options) { }
}
