using Auxiliary.Elves.Api.ApiService;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Domain;
using Auxiliary.Elves.Infrastructure.Config;
using Auxiliary.Elves.Server.Exceptions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString(SystemConstant.DefaultConnection);

// 获取基础目录
var baseDic = AppDomain.CurrentDomain.BaseDirectory;
var videoDirectory = Path.Combine(baseDic, "videos");
if (!Directory.Exists(videoDirectory))
{
    Directory.CreateDirectory(videoDirectory);
}
builder.Services.AddScoped<IJWTApiService, JWTApiService>();
builder.Services.AddScoped<ILoginApiService, LoginApiService>();
builder.Services.AddScoped<IPointsApiService, PointsApiService>();
builder.Services.AddScoped<IAnnouncementApiService, AnnouncementApiService>();
builder.Services.AddScoped<ISystemSettingApiService, SystemSettingApiService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = SystemConstant.JwtAudience,
        ValidIssuer = SystemConstant.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SystemConstant.JwtSecurityKey)),
        LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
        {
            return expires >= DateTime.Now;
        }
    };
});
builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy(nameof(RoleEnum.), policyBuilder => policyBuilder.Requirements.Add(new AdminRequirement()));
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()   // 允许所有来源
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 314572800; // 300 MB
});

// Add services to the container.
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AuxiliaryDbContext>((options) =>
{
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // 获取项目的 xml 注释文件路径
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // 加载注释
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });

    // 添加 JWT 认证
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "输入格式: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

});
builder.Services.AddHttpClient();

builder.Services.AddMemoryCache();
builder.Host.UseNLog();

builder.WebHost.UseUrls($"http://0.0.0.0:{GetPort()}");
var app = builder.Build();

// 启用静态文件服务
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(videoDirectory),
    RequestPath = "",
    ServeUnknownFileTypes = true,
    OnPrepareResponse = ctx =>
    {
        var fileExtension = Path.GetExtension(ctx.File.Name).ToLower();
        ctx.Context.Response.Headers["Content-Type"] = GetMimeType(fileExtension);
        ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
        ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=3600";
    }
});

// 可选：根目录访问返回提示
app.MapGet("/", () => "Video server is running...");

app.UseCors("AllowAll");  // 开启 CORS

app.UseSwagger();
app.UseSwaggerUI();

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
app.UseAuthentication();
app.UseAuthorization();

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


 string GetMimeType(string fileExtension)
{
    return fileExtension.ToLower() switch
    {
        ".mp4" => "video/mp4",
        ".webm" => "video/webm",
        ".ogg" => "video/ogg",
        ".avi" => "video/x-msvideo",
        ".mov" => "video/quicktime",
        ".wmv" => "video/x-ms-wmv",
        ".mkv" => "video/x-matroska",
        _ => "application/octet-stream"
    };
}