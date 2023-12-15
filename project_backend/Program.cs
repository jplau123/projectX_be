using DbUp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using project_backend.Extensions;
using project_backend.Interfaces;
using project_backend.Repositories;
using project_backend.Services;
using System.Data;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["MySecrets:PostgreConnection"] ?? throw new ArgumentNullException("Connection string was not found."); ;

string JWTconfigurationKey = builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT key was not found.");

// JWT configuration
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddCookie(opt =>
{
    opt.Cookie.Name = "X-Token";
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("JWT issuer was not found."),
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? throw new ArgumentNullException("JWT audience was not found."),
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(JWTconfigurationKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = new TimeSpan(0, 0, 5)
    };
    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context => 
        {
            context.Token = context.Request.Cookies["X-Token"];
            return Task.CompletedTask;
        }
    };
});

// Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IAdminService, AdminService>();
builder.Services.AddTransient<IAdminRepository, AdminRepository>();

EnsureDatabase.For.PostgresqlDatabase(connectionString);

var upgrader =
    DeployChanges.To
        .PostgresqlDatabase(connectionString)
        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
        .LogToConsole()
        .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
#if DEBUG
    Console.ReadLine();
#endif
}

builder.Services.AddTransient<IItemRepository, ItemRepository>();
builder.Services.AddTransient<IItemService, ItemService>();
builder.Services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(connectionString));

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserAuthRepository, UserAuthRepository>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// My Error handler middleware
app.UseErrorMiddleware();

app.MapControllers();

app.Run();
