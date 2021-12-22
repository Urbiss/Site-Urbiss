using AspNetCoreHero.Results;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Urbiss.API.Filters;
using Urbiss.API.Middlewares;
using Urbiss.Domain.Constants;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;
using Urbiss.Repository;
using Urbiss.Services;

namespace Urbiss.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private bool HangFireEnabled => _configuration.GetSection("Hangfire").GetValue<bool>("Enabled");
        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        private void ConfigureHangfire(IServiceCollection services)
        {
            if (HangFireEnabled)
            {
                services.AddHangfire(opt => opt
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseRecommendedSerializerSettings()                
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UsePostgreSqlStorage(_configuration.GetConnectionString("DefaultConnection"),
                        new PostgreSqlStorageOptions
                        {
                            QueuePollInterval = TimeSpan.FromSeconds(_configuration.GetSection("Hangfire").GetValue<int>("QueuePollInterval")),
                            PrepareSchemaIfNecessary = true
                        }));
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //https://github.com/efcore/EFCore.NamingConventions
            services.AddDbContext<UrbissDbContext>(
                x => x.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"),
                                 o => o.UseNetTopologySuite())
                      .UseLowerCaseNamingConvention()
                      //.UseSnakeCaseNamingConvention()
                    //.LogTo(Console.WriteLine)
                    );

            services.AddIdentity<User, Role>(options =>
             {
                 options.SignIn.RequireConfirmedAccount = true;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequireDigit = false;
                 options.Password.RequireLowercase = false;
                 options.Password.RequireUppercase = false;
                 options.Password.RequiredLength = 6;
             }).AddEntityFrameworkStores<UrbissDbContext>()
               .AddRoleValidator<RoleValidator<Role>>()
               .AddRoleManager<RoleManager<Role>>()
               .AddSignInManager<SignInManager<User>>()
               .AddDefaultTokenProviders();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = _configuration["JWTSettings:Issuer"],
                    ValidAudience = _configuration["JWTSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:Key"])),
                };
                x.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        if (c.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            c.Response.Headers.Add("Token-Expired", "true");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(Result.Fail("Você não está autorizado"));
                        return context.Response.WriteAsync(result);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(Result.Fail("Você não tem autorização para acessar este recurso"));
                        return context.Response.WriteAsync(result);
                    },
                };
            });

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));

            }).AddNewtonsoftJson(x => 
            { 
                x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; 
                x.SerializerSettings.Converters.Add(new StringEnumConverter()); 
            });

            services.Configure<MailSettingsDto>(_configuration.GetSection("MailSettings"));
            services.Configure<JWTSettingsDto>(_configuration.GetSection("JWTSettings"));
            services.Configure<AppSettingsDto>(_configuration.GetSection("AppSettings"));

            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<ISendMailService, SendMailBackgroundService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ISurveyService, SurveyService>();
            services.AddTransient<ICityService, CityService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IOrderRequestProductBackgroundService, OrderRequestProductBackgroundService>();
            services.AddTransient<IVoucherService, VoucherService>();
            services.AddTransient<INtsService, NtsService>();
            services.AddTransient<ISurveyFileService, SurveyFileService>();

            services.AddTransient(typeof(ICityRepository), typeof(CityRepository));
            services.AddTransient(typeof(ISurveyRepository), typeof(SurveyRepository));
            services.AddTransient(typeof(IDatabaseUtilsRepository), typeof(DatabaseUtilsRepository));
            services.AddTransient(typeof(IUserSurveyRepository), typeof(UserSurveyRepository));
            services.AddTransient(typeof(IOrderRepository), typeof(OrderRepository));
            services.AddTransient(typeof(IVoucherRepository), typeof(VoucherRepository));
            services.AddTransient(typeof(IAerialPhotoRepository), typeof(AerialPhotoRepository));
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //services.AddAutoMapper(typeof(AutoMapperProfiles));

            services.AddCors();
            services.AddControllers();

            ConfigureHangfire(services);

            services.AddMemoryCache(options =>
            {
                options.SizeLimit = _configuration.GetSection("MemoryCache").GetValue<int>("Size");
            });
        }

        private static async Task CreateRoles(RoleManager<Role> roleManager)
        {
            string[] rolesNames = { "Admin", "User", "Surveyor" };
            IdentityResult result;
            foreach (var namesRole in rolesNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(namesRole);
                if (!roleExist)
                {
                    result = await roleManager.CreateAsync(new Role(namesRole));
                }
            }
        }

        private void CreateFolders()
        {
            var folders = new string[]
            {
                "DataFolder",
                "OutputFolder",
                "LogFolder",
                "TempFolder"
            };
            foreach (var folder in folders)
                Directory.CreateDirectory(AppSettingsDto.GetAppFolder(_configuration.GetSection("AppSettings").GetValue<string>(folder)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RoleManager<Role> roleManager, ILoggerFactory loggerFactory)
        {
            CreateFolders();

            //loggerFactory.AddFile("../Logs/urbiss-api-{Date}.txt", isJson: true);
            loggerFactory.AddFile(AppSettingsDto.GetAppFolder(_configuration.GetSection("AppSettings").GetValue<string>("LogFolder"), "urbiss-api-{Date}.txt"), isJson: true);

            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            CreateRoles(roleManager).Wait();

            if (HangFireEnabled)
            {
                var backgroundServerOptions = new BackgroundJobServerOptions
                {
                    Queues = new[] { HangfireConsts.SEND_MAIL_QUEUE, HangfireConsts.ORDER_REQUEST_PRODUCT_QUEUE },
                    WorkerCount = _configuration.GetSection("Hangfire").GetValue<int>("WorkerCount"),
                };
                app.UseHangfireServer(backgroundServerOptions);

                List<HangfireDashboardAuthorizationFilter> filters = new List<HangfireDashboardAuthorizationFilter>();
                if (!env.IsDevelopment())
                    filters.Add(new HangfireDashboardAuthorizationFilter()); 

                app.UseHangfireDashboard("/jobs", new DashboardOptions
                {
                    Authorization = filters.ToArray(),
                    StatsPollingInterval = 30000,
                    // no back link
                    AppPath = null
                });
            }
        }
    }
}
