using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TaskFlowAPI.Data;
using TaskFlowAPI.Models;
using Microsoft.AspNetCore.Identity;
using TaskFlowAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Omogućava kontrolere
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dodavanje usluge za autentifikaciju
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Registracija AppDbContext u Dependency Injection kontejneru
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dodavanje PasswordHasher za heširanje lozinki
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Dodavanje CORS podrške
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

//Registracija Email Servisa
builder.Services.AddScoped<EmailService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Dodavanje CORS middleware
app.UseCors("AllowAllOrigins");

// Dodavanje middleware za autentifikaciju
app.UseAuthentication();
app.UseAuthorization();

// Mapiranje kontrolera
app.MapControllers();

app.Run();
