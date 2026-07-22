using System.Text;

using HaushaltsOS.Api.Common.Auth;
using HaushaltsOS.Api.Common.Persistence;
using HaushaltsOS.Api.Features.Auth.Login;
using HaushaltsOS.Api.Features.Auth.Refresh;
using HaushaltsOS.Api.Features.Auth.Register;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((options => 
    options.ReadFrom.Configuration(builder.Configuration)
));

builder.Services.AddCors(options => 
    options.AddPolicy("Frontend", policy => 
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
    )
);

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
        options.MapInboundClaims = false;
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

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUser>();

builder.Services.AddAuthorization();

var app = builder.Build();

// Ausstehende Migrationen beim Start anwenden
using (var scope = app.Services.CreateScope())
{
    AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseExceptionHandler();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseStatusCodePages();

app.UseSerilogRequestLogging();

app.MapRegister();
app.MapLogin();
app.MapRefresh();
app.MapHealthChecks("/health");

app.Run();
