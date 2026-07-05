using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((options => 
    options.ReadFrom.Configuration(builder.Configuration)
));

builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Postgres")!);

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseSerilogRequestLogging();

app.MapGet("/", () => "Hello World!");
app.MapHealthChecks("/health");

app.Run();
