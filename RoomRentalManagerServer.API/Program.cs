using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RoomRentalManagerServer.Domain.Interfaces.UserInterfaces;
using RoomRentalManagerServer.Infrastructure.Data;
using RoomRentalManagerServer.Infrastructure.Repositories.UserRepository;
using RoomRentalManagerServer.Application.Model.UsersModel.UserProfileMapper;
using System.Reflection;
using RoomRentalManagerServer.Domain.Interfaces.RedisCache;
using RoomRentalManagerServer.Infrastructure.RedisCache;
var builder = WebApplication.CreateBuilder(args);
//cấu hình redis
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Configuration["Redis:ConnectionString"] = "localhost:6379";
// khởi tạo kết nối postgredb
builder.Services.AddDbContext<RoomRentalManagerServerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// khởi tạo các controller
builder.Services.AddControllers();
// khởi tạo objectmaper
builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// inject các service từ các project khác
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
// cho phép gọi api từ client
var AllowSpecificOrigins = "allowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(AllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
