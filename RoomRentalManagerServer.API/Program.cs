using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RoomRentalManagerServer.Application.Model.UsersModel.UserProfileMapper;
using RoomRentalManagerServer.Domain.Interfaces.RedisCache;
using RoomRentalManagerServer.Domain.Interfaces.UserInterfaces;
using RoomRentalManagerServer.Infrastructure.Data;
using RoomRentalManagerServer.Infrastructure.RedisCache;
using RoomRentalManagerServer.Infrastructure.Repositories.UserRepository;
using StackExchange.Redis;
using System.Reflection;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
//cấu hình redis
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
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
builder.Services.AddHttpContextAccessor();
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

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var options = new ConfigurationOptions
    {
        EndPoints = { configuration["Redis:ConnectionString"] },
        ConnectTimeout = int.Parse(configuration["Redis:ConnectTimeout"] ?? "5000"),
        SyncTimeout = int.Parse(configuration["Redis:SyncTimeout"] ?? "10000"),
        AbortOnConnectFail = false
    };
    return ConnectionMultiplexer.Connect(options);
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"])
            )
        };
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
