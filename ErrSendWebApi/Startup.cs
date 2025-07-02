using ErrSendApplication;
using ErrSendApplication.Common.Configs;
using ErrSendApplication.Interfaces;
using ErrSendApplication.Mappings;
using ErrSendPersistensTelegram;
using ErrSendWebApi.ExceptionMidlevare;
using ErrSendWebApi.Middleware;

using ErrSendWebApi.Serviсe;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace ErrSendWebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // JWT configuration from environment variables
            var tokenConfig = new TokenConfig
            {
                TokenKey = Environment.GetEnvironmentVariable("JWT_TOKEN_KEY") ?? 
                    (_environment.IsDevelopment() ? "development-key-that-is-at-least-32-characters-long" : 
                        throw new InvalidOperationException("JWT_TOKEN_KEY environment variable is not set in production")),
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "ErrorSenderApi",
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "https://localhost:5001",
                ExpiryInMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES") ?? "60")
            };

            services.Configure<TokenConfig>(config =>
            {
                config.TokenKey = tokenConfig.TokenKey;
                config.Issuer = tokenConfig.Issuer;
                config.Audience = tokenConfig.Audience;
                config.ExpiryInMinutes = tokenConfig.ExpiryInMinutes;
            });

            services.AddAutoMapper(config =>
            {
                config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
                config.AddProfile(new AssemblyMappingProfile(typeof(IHttpClientWr).Assembly));
            });

            services.AddApplication(Configuration);
            services.AddPersistenceTelegram(Configuration);
            services.AddControllers();
            
            // JWT Authentication configuration
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenConfig.Issuer,
                    ValidAudience = tokenConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig.TokenKey))
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });

            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ErrorSender API",
                    Version = "v1",
                    Description = "API для відправки помилок в Telegram групу"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Авторизація",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Введіть JWT Token у форматі: Bearer {token}"
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
                              },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                          },
                         new string[] {}
                    }
                });
            });

            services.AddSingleton<ICurrentService, CurrentService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IJwtService, JwtService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            env.WebRootPath = string.Empty;

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.RoutePrefix = string.Empty;
                config.SwaggerEndpoint("swagger/v0.3/swagger.json", "ErrorSender API v0.3");
            });

            app.UseTelegramErrorReporting();
            app.UseCustomExceptionHandler();
            app.UseRouting();
            
            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
