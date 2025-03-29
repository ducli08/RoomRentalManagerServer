using Microsoft.EntityFrameworkCore;
using RoomRentalManagerServer.Infrastructure.Data;
using Microsoft.OpenApi.Models;
using System.Reflection;
using RoomRentalManagerServer.Domain.Interfaces.UserInterfaces;
using RoomRentalManagerServer.Infrastructure.Repositories.UserRepository;
using Npgsql.EntityFrameworkCore.PostgreSQL;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RoomRentalManagerServerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var applicationAssembly = Assembly.Load("RoomRentalManagerServer.Application");
var types = applicationAssembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && x.Name.EndsWith("Service"));
foreach(var type in types)
{
    var interfaceType = type.GetInterfaces().FirstOrDefault();
    if(interfaceType != null)
    {
        builder.Services.AddScoped(interfaceType, type);
    }
}
var domainAssembly = typeof(IUserRepository).Assembly;
var infrastructureAssembly = typeof(UserRepository).Assembly;

var repositoryInterface = domainAssembly.GetTypes().Where(x => x.IsInterface && x.Name.EndsWith("Repository")).ToList();
foreach(var interfaceType in repositoryInterface)
{
    var implementationType = infrastructureAssembly.GetTypes().FirstOrDefault(x => interfaceType.IsAssignableFrom(x) && x.IsClass);
    if(implementationType != null)
    {
        builder.Services.AddScoped(interfaceType, implementationType);
    }
}
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.AddServer(new OpenApiServer { Url = "https://localhost:7246" });
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
