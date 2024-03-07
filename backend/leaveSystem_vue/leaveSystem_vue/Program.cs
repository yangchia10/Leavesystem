using leaveSystem_vue.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models; // 引入 Swagger 的命名空間
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting; // 包含IWebHostEnvironment的擴展方法IsDevelopment


namespace leaveSystem_vue
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 動態生成密鑰並打印到控制台（僅建議在開發環境中這樣做）
            //var secretKey = GenerateSecretKey(32);
            //Console.WriteLine("Generated Secret Key: " + secretKey);

            // 從appsettings.json讀取密鑰
            var secretKey = builder.Configuration["Jwt:SecretKey"];
            var key = Encoding.ASCII.GetBytes(secretKey);


            // 添加資料庫服務
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // 配置 CORS
            // builder.Services.AddCors(options =>
            // {
            //     options.AddPolicy("CorsPolicy", builder =>
            //         builder.WithOrigins("*") // 嘗試使用萬用字符指定允許的來源
            //                .AllowAnyMethod()
            //                .AllowAnyHeader()
            //                .AllowCredentials());
            // });
            builder.Services.AddCors(options =>
            {
               options.AddPolicy("CorsPolicy", builder =>
                   builder.SetIsOriginAllowed(origin => true) // 允許任何來源
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
            });//"http://localhost:8080,http://172.20.10.2:8080,http://172.20.10.5:8080"

            builder.Services.AddControllers();

            // 添加 Swagger 服務並配置 JWT 認證
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // 添加 JWT 認證支持
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // 配置JWT認證
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero // 立即過期
                    };

                    // 從Cookie中提取JWT的邏輯
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.ContainsKey("jwt"))
                            {
                                context.Token = context.Request.Cookies["jwt"];
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            var app = builder.Build();

            // 配置 HTTP 請求管道
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error"); //錯誤處理路徑
                // 其他生產環境配置...
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // 使用 CORS
            app.UseCors("CorsPolicy");

            app.UseAuthentication(); // 啟用認證
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}