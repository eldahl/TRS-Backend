using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TRS_backend.DBModel;
using TRS_backend.Operational;
using TRS_backend.Services;

// Debug code to generate hashed password and salt
/*
byte[] salt = Crypto.GenerateRandomBytes(32);
byte[] hashedPassword = Crypto.HashPassword("MySecretPassword123", salt);

Debug.WriteLine($"Hashed password: {BitConverter.ToString(hashedPassword)}");
Debug.WriteLine($"Salt: {BitConverter.ToString(salt)}");
*/
//TimeSlotService timeSlotService = new TimeSlotService();
//timeSlotService.GenerateTimeSlots(DateOnly.FromDateTime(DateTime.Today), new TimeOnly(16, 00), new TimeOnly(21, 00), TimeSpan.FromMinutes(120), 2, new TimeOnly(00, 15));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Set up HTTPS LetsEncrypt certificate for direct HTTPS access
/*
builder.WebHost.UseKestrel(serverOptions => {
    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ServerCertificate = new X509Certificate2("/cert.pfx", builder.Configuration["CertificateKey"]);
    });
});
*/

// Add HTTP traffic logging
builder.Services.AddHttpLogging(options => {
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    options.MediaTypeOptions.AddText("application/javascript");
    options.RequestBodyLogLimit = 4096;
    options.ResponseBodyLogLimit = 4096;
});

// Settings file context for saving settings and loading between restarts
builder.Services.AddSingleton<SettingsFileContext>();
builder.Services.AddScoped<TimeSlotService>();

// Database context
builder.Services.AddDbContext<TRSDbContext>(options => {
    options.UseMySQL(builder.Configuration.GetConnectionString("MySQLDB")!);
});

// JWT authentication
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

// Build configuration into web application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add traffic logging
app.UseHttpLogging();

// HTTP is insecure 
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();
