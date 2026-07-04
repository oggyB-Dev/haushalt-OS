using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((options => 
    options.ReadFrom.Configuration(builder.Configuration)
));

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseSerilogRequestLogging();

app.MapGet("/", () => "Hello World!");

app.Run();
