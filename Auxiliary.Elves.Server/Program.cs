using Auxiliary.Elves.Infrastructure.Config;
using Auxiliary.Elves.Server.Exceptions;
using NLog.Web;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddMemoryCache();
builder.Host.UseNLog();

builder.WebHost.UseUrls($"http://0.0.0.0:{GetPort()}");
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseException();
app.MapControllers();

app.Run();


string GetPort()
{
    var configBuilder = new ConfigurationBuilder()
   .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
   .AddJsonFile(SystemConstant.HostFileName)
   .Build();
    var port = configBuilder.GetSection(SystemConstant.HostPort).Value;
    return port;
}