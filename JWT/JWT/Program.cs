using JWT.Data;
using JWT.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "Configure Jwt:Key with a secret of at least 32 bytes using user-secrets or an environment variable.");
}

var connectionString = builder.Configuration.GetConnectionString("DbConn");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Configure the ConnectionStrings:DbConn setting using user-secrets or an environment variable.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWT Demo API",
        Version = "v1",
        Description = "A demonstration API for JWT authentication and role-based authorization."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter a JWT using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await SeedBootstrapAdministratorAsync(app);

app.Run();

static async Task SeedBootstrapAdministratorAsync(WebApplication app)
{
    var configuration = app.Configuration;
    var username = configuration["BootstrapAdmin:Username"];
    var email = configuration["BootstrapAdmin:Email"];
    var password = configuration["BootstrapAdmin:Password"];

    // Bootstrap is optional. Configure all three values only when provisioning
    // the first administrator; remove them after the account has been created.
    if (string.IsNullOrWhiteSpace(username) ||
        string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(password))
    {
        return;
    }

    if (password.Length < 12)
    {
        throw new InvalidOperationException(
            "BootstrapAdmin:Password must contain at least 12 characters.");
    }

    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (await dbContext.Users.AnyAsync())
    {
        return;
    }

    dbContext.Users.Add(new User
    {
        Id = Guid.NewGuid(),
        Username = username.Trim(),
        Email = email.Trim(),
        Password = BCrypt.Net.BCrypt.HashPassword(password),
        Role = "Admin"
    });

    await dbContext.SaveChangesAsync();
    app.Logger.LogWarning(
        "A bootstrap administrator was created. Remove BootstrapAdmin settings after first use.");
}
