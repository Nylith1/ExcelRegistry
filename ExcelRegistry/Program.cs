using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ExcelRegistry.Core;
using ExcelRegistry.Shared.Helpers;
using ExcelRegistry.Shared.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
               builder
               .Configuration
               .GetConnectionString("Default");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://localhost:7025")
           .AllowAnyHeader()
           .AllowCredentials()
           .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMediatR(cfg =>
{
    var assemblies = new[]
    {
        Assembly.GetExecutingAssembly(),
        Assembly.Load("ExcelRegistry.Domain.Handlers"),
    };
    cfg.RegisterServicesFromAssemblies(assemblies);
});

builder.ConfigureOptions();
builder.Services.ConfigureExcelRegistryServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});

app.Run();