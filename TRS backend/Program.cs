using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TRS_backend.DBModel;

// Debug code to generate hashed password and salt
/*
byte[] salt = Crypto.GenerateRandomBytes(32);
byte[] hashedPassword = Crypto.HashPassword("MySecretPassword123", salt);

Debug.WriteLine($"Hashed password: {BitConverter.ToString(hashedPassword)}");
Debug.WriteLine($"Salt: {BitConverter.ToString(salt)}");
*/

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*
// Set up HTTPS LetsEncrypt certificate
builder.Host.ConfigureWebHostDefaults(options => {
    options.ConfigureKestrel(serverOptions => {
        serverOptions.Listen(IPAddress.Any, 7043);
        serverOptions.ConfigureHttpsDefaults(httpsOptions =>
        {
            httpsOptions.ServerCertificate = new X509Certificate2("localhost.pfx", "password");
        });
    });
});
*/

builder.Services.AddDbContext<TRSDbContext>(options => {
    options.UseMySQL(builder.Configuration.GetConnectionString("MySQLDB")!);
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(bearerOpt => {
    
    var jwtConfig = builder.Configuration.GetSection("JWT");
    var issuers = jwtConfig.GetSection("Issuers").Get<string[]>();
    var audiences = jwtConfig.GetSection("Audiences").Get<string[]>();
    var signingKey = jwtConfig["SigningKey"];

    bearerOpt.TokenValidationParameters = new TokenValidationParameters
    {
        // Set proper values for JWT validation
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:JWTSigningKey"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        // Make sure token expires exactly at token expiration time
        ClockSkew = TimeSpan.Zero,
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
