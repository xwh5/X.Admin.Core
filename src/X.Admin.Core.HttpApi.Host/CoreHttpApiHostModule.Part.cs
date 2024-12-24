using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using Microsoft.AspNetCore.Hosting;
using StackExchange.Redis;
using Volo.Abp.Caching;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Auditing;
using Volo.Abp.Auditing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.MultiTenancy;
using X.Admin.Core.MultiTenancy;
using X.Admin.Core.Permissions;

namespace X.Admin.Core
{
    public partial class CoreHttpApiHostModule
    {
        private void ConfigureCache(ServiceConfigurationContext context, IConfiguration configuration)
        {
            Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "x.amdin.core:"; });
            var redis = ConnectionMultiplexer.Connect(configuration.GetValue<string>("Redis:Configuration"));
            context.Services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "XAdmin-Protection-Keys"); ;
        }

        private void ConfigureHangfire(ServiceConfigurationContext context)
        {
            //var redisStorageOptions = new RedisStorageOptions()
            //{
            //    Db = context.Services.GetConfiguration().GetValue<int>("Hangfire:Redis:DB")
            //};

            //Configure<AbpBackgroundJobOptions>(options => { options.IsJobExecutionEnabled = true; });

            //context.Services.AddHangfire(config =>
            //{
            //    config.UseRedisStorage(
            //            context.Services.GetConfiguration().GetValue<string>("Hangfire:Redis:Host"), redisStorageOptions)
            //        .WithJobExpirationTimeout(TimeSpan.FromDays(7));
            //    var delaysInSeconds = new[] { 10, 60, 60 * 3 }; // 重试时间间隔
            //    const int Attempts = 3; // 重试次数
            //    config.UseFilter(new AutomaticRetryAttribute() { Attempts = Attempts, DelaysInSeconds = delaysInSeconds });
            //    //config.UseFilter(new AutoDeleteAfterSuccessAttribute(TimeSpan.FromDays(7)));
            //    config.UseFilter(new JobRetryLastFilter(Attempts));
            //});
        }
        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,// 开启密钥进行验证
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true, // 验证 JWT 是否过期
                        ClockSkew = TimeSpan.Zero, // 设置允许的时间偏差（通常是 5 分钟，避免时钟差异）
                        ValidIssuer = configuration["Jwt:Issuer"], // 发行者（Issuer）
                        ValidAudience = configuration["Jwt:Audience"], // 受众（Audience）
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt:SecurityKey"]))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var path = context.HttpContext.Request.Path;
                            if (path.StartsWithSegments("/login"))
                            {
                                return Task.CompletedTask;
                            }

                            var accessToken = string.Empty;
                            if (context.HttpContext.Request.Headers.ContainsKey("Authorization"))
                            {
                                accessToken = context.HttpContext.Request.Headers["Authorization"];
                                if (!string.IsNullOrWhiteSpace(accessToken))
                                {
                                    accessToken = accessToken.Split(" ").LastOrDefault();
                                }
                            }

                            if (accessToken.IsNullOrWhiteSpace())
                            {
                                accessToken = context.Request.Query["access_token"].FirstOrDefault();
                            }

                            if (accessToken.IsNullOrWhiteSpace())
                            {
                                accessToken = context.Request.Cookies[HttpApiConst.DefaultCookieName];
                            }

                            context.Token = accessToken;
                            context.Request.Headers.Remove("Authorization");
                            context.Request.Headers.Add("Authorization", $"Bearer {accessToken}");

                            return Task.CompletedTask;
                        }
                    };
                });
        }

        private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<CoreDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}X.Admin.Core.Domain.Shared"));
                    options.FileSets.ReplaceEmbeddedByPhysical<CoreDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}X.Admin.Core.Domain"));
                    options.FileSets.ReplaceEmbeddedByPhysical<CoreApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}X.Admin.Core.Application.Contracts"));
                    options.FileSets.ReplaceEmbeddedByPhysical<CoreApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}X.Admin.Core.Application"));
                });
            }
        }

        private void ConfigureConventionalControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(CoreApplicationModule).Assembly);
            });
        }

        private static void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
        {
            // 添加 Swagger 服务
            context.Services.AddSwaggerGen(options =>
            {
                // 设置 Swagger 文档的基础信息
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Your API",
                    Version = "v1",
                    Description = "A comprehensive API for testing and interacting with the backend.",
                    Contact = new OpenApiContact
                    {
                        Name = "weihao.xia",
                        Email = "xwh7351@163.com"
                    }
                });

                // 配置认证：例如使用 JWT Bearer Token
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your Bearer token here"
                });

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
                    new string[] { }
                }
            });

                // 添加注释文件，以便生成更清晰的 API 文档
                var xmlFile = Path.Combine(AppContext.BaseDirectory, "X.Admin.Core.xml");
                options.IncludeXmlComments(xmlFile);
            });
        }

        private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins(
                            configuration["App:CorsOrigins"]?
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.Trim().RemovePostFix("/"))
                                .ToArray() ?? Array.Empty<string>()
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        private void ConfigureAuditLog(ServiceConfigurationContext context)
        {
            Configure<AbpAuditingOptions>
            (
                options =>
                {
                    options.IsEnabled = true;
                    options.EntityHistorySelectors.AddAllEntities();
                    options.ApplicationName = "X.Admin.Core";
                }
            );

            Configure<AbpAspNetCoreAuditingOptions>(
                options =>
                {
                    options.IgnoredUrls.Add("/AuditLogs/page");
                    options.IgnoredUrls.Add("/hangfire/stats");
                    options.IgnoredUrls.Add("/hangfire/recurring/trigger");
                    options.IgnoredUrls.Add("/cap");
                    options.IgnoredUrls.Add("/");
                });
        }

        private void ConfigureSignalR(ServiceConfigurationContext context, IConfiguration configuration)
        {
            var redisConfiguration = configuration["Redis:Configuration"];
            if (!redisConfiguration.IsNullOrWhiteSpace())
            {
                context.Services.PreConfigure<ISignalRServerBuilder>(r => r.AddStackExchangeRedis(redisConfiguration, redisOptions =>
                {
                    redisOptions.Configuration.ChannelPrefix = "X.Admin";// 可选: 配置 Redis 通道名称，默认为 "SignalR"
                }));
            }
        }

        private void ConfigureIdentity(ServiceConfigurationContext context)
        {
            context.Services.Configure<IdentityOptions>(options => { options.Lockout = new LockoutOptions() { AllowedForNewUsers = false }; });
        }

        private void ConfigurationMultiTenancy()
        {
            Configure<AbpMultiTenancyOptions>(options => { options.IsEnabled = MultiTenancyConsts.IsEnabled; });
        }

        private void ConfigureCap(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var enabled = configuration.GetValue("Cap:Enabled", false);
            if (enabled)
            {
                context.Services.AddCap(capOptions =>
                {
                    capOptions.UseMySql(configuration["ConnectionStrings:Default"]);
                    capOptions.UseRabbitMQ(option =>
                    {
                        option.HostName = configuration.GetValue<string>("Cap:RabbitMq:HostName");
                        option.UserName = configuration.GetValue<string>("Cap:RabbitMq:UserName");
                        option.Password = configuration.GetValue<string>("Cap:RabbitMq:Password");
                        option.Port = configuration.GetValue<int>("Cap:RabbitMq:Port");
                    });

                    var hostingEnvironment = context.Services.GetHostingEnvironment();
                    capOptions.UseDashboard(options =>
                    {
                        options.AuthorizationPolicy = CorePermissions.CapManagement.Query;
                    });
                });
            }
            else
            {
                context.Services.AddCap(capOptions =>
                {
                    capOptions.UseInMemoryStorage();
                    var hostingEnvironment = context.Services.GetHostingEnvironment();
                    var auth = !hostingEnvironment.IsDevelopment();
                    capOptions.UseDashboard();
                });
            }
        }
    }
}
