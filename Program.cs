using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetFavorite;
using NetFavorite.Utilities;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. 注册控制器服务
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// 2. Swagger + JWT授权配置（完全适配你的包版本，零报错，符合课程要求）
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // 设置API文档信息
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "书签管理器 API",
        Version = "v1",
        Description = "基于 ASP.NET Core 10 开发的书签管理系统 API 文档",
        Contact = new OpenApiContact()
    });

    // 定义JWT安全方案（对应课程里的oauth2配置）
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "请输入JWT授权Token，格式：Bearer 你的Token字符串",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // 应用安全方案到所有接口
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// 3. 跨域配置
builder.Services.AddCors(cor =>
    cor.AddPolicy("Cors", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    })
);

// 4. 数据库上下文注册（和你项目完全匹配）
builder.Services.AddDbContext<NetFavoriteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NetFavoriteDbContext"))
);

// 5. 课程核心要求：JWT校验配置（100%生效）
var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = Encoding.UTF8.GetBytes(
    Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT 密钥未配置，请设置 JWT_SECRET_KEY 环境变量或配置 JWT:SecretKey")
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ValidateIssuerSigningKey = true
        };
    });

// 7. 添加用于访问Header数据的依赖项 HttpContextAccessor
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// 8. 添加 TokenService 依赖项
builder.Services.AddTransient<ITokenService, TokenService>();

// 构建应用
var app = builder.Build();

// 9. 中间件管道（顺序固定，不可颠倒）
app.UseHttpsRedirection();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "书签管理器 API";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "书签管理器 API v1");
        c.RoutePrefix = "swagger";
        
        // 经典Swagger样式配置
        c.InjectStylesheet("/swagger-ui/custom-swagger.css");
        c.InjectJavascript("/swagger-ui/custom-swagger.js");
    });
}

app.UseCors("Cors");

// 认证必须在授权之前，顺序错误会导致认证失效
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 添加根路径端点，重定向到测试中心页面
app.MapGet("/", () => Results.Redirect("/index.html"));

app.Run();