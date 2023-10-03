using FootballAPI.Security;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Endpoints;
using WebAPI.Extensions;
using WebAPI.Security;
using WebAPI.Services.Abstract;
using WebAPI.Services.Concrete;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITokenHelper, JwtHelper>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterSwagger();

TokenOptions? tokenOptions = builder.Configuration
    .GetSection("TokenOptions").Get<TokenOptions>();
builder.Services.RegisterAuth(tokenOptions);
builder.Services.AddAuthorization();

builder.Services.AddDbContext<ApplicationDBContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLServer")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.RegisterAuthEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.Run();