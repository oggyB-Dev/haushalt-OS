using System.Text;

using HaushaltOS.Api.Common.Auth;
using HaushaltOS.Api.Common.Persistence;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((options => 
    options.ReadFrom.Configuration(builder.Configuration)
));

builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Postgres")!);

builder.Services.AddDbContextPool<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
);

builder.Services
    .AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();

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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseStatusCodePages();

app.UseSerilogRequestLogging();

app.MapGet("/", () => "Hello World!");
app.MapHealthChecks("/health");

app.Run();
