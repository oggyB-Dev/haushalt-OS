using HaushaltOS.Api.Common.Auth;
using HaushaltOS.Api.Common.Persistence;

using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseSerilogRequestLogging();

app.MapGet("/", () => "Hello World!");
app.MapHealthChecks("/health");

app.Run();
