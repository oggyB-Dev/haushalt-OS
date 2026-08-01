using System.Text;

using HaushaltsOS.Api.Common.Auth;
using HaushaltsOS.Api.Common.Persistence;
using HaushaltsOS.Api.Common.Realtime;
using HaushaltsOS.Api.Features.Auth.Login;
using HaushaltsOS.Api.Features.Auth.Refresh;
using HaushaltsOS.Api.Features.Auth.Register;
using HaushaltsOS.Api.Features.ShoppingList.CreateItem;
using HaushaltsOS.Api.Features.ShoppingList.DeleteItem;
using HaushaltsOS.Api.Features.ShoppingList.GetItems;
using HaushaltsOS.Api.Features.ShoppingList.ToggleItem;

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
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // WebSockets können keine Header senden, daher wird das Token als Query Parameter übergeben
                var accessToken = context.Request.Query["access_token"];

                if(!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUser>();

builder.Services.AddAuthorization();
builder.Services.AddSignalR();

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
app.MapCreateItem();
app.MapGetItems();
app.MapToggleItem();
app.MapDeleteItem();
app.MapHealthChecks("/health");

app.MapHub<ShoppingListHub>("/hubs/shopping-list");

app.Run();
