using System.Text;
using forest_report_api.Context;
using forest_report_api.Entities;
using forest_report_api.Facade;
using forest_report_api.Options;
using forest_report_api.Repositories;
using forest_report_api.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AutoMapper;
using forest_report_api.Helper;
using forest_report_api.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace forest_report_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(GetSwaggerOptions());
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            #region Mapper

            var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });

            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            #endregion

            #region Repository

            services.AddScoped<ITypeActivityRepository, TypeActivityRepository>();
            services.AddScoped<IReportTypeRepository, ReportTypeRepository>();
            services.AddScoped<IPeriodRepository, PeriodRepository>();
            services.AddScoped<IUserCheckinIntervalRepository, UserCheckinIntervalRepository>();
            services.AddScoped<ILogReportRepository, LogReportRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReportTabRepository, ReportTabRepository>();
            services.AddScoped<IMailer, Mailer>();

            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IUserIntervalService, UserIntervalService>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<DirectoriesService>();

            services.AddScoped<IEntityService, EntityService>();
            services.AddScoped<IEntityRepository, EntityRepository>();

            #endregion

            #region Fasade

            services.AddScoped<AccountServiceFacade>();
            services.AddScoped<ReportServiceFacade>();
            services.AddScoped<DirectoriesFacade>();
            services.AddScoped<UserIntervalFacade>();
            services.AddScoped<EntityFacade>();

            #endregion

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWT:ValidAudience"],
                    ValidIssuer = Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };
            });
            services.AddCors();

            services.AddMvc(option => option.EnableEndpointRouting = false)
                .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    }
                )
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNamingPolicy = null)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddControllersAsServices();

            if (GetSwaggerOptions().IsDefault())
                Configuration.GetSection(nameof(SwaggerOptions)).Bind(_swaggerOptions);

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "forest-report-api",
                    Version = "v1"
                });
#if DEBUG
                x.AddServer(new OpenApiServer {Url = "https://localhost:5001"});
#endif
                foreach (var server in _swaggerOptions.Servers)
                {
                    x.AddServer(new OpenApiServer {Url = server});
                }

                x.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use((context, next) =>
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "content-disposition");
                context.Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Results");
                return next.Invoke();
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHsts();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            if (GetSwaggerOptions().IsDefault())
                Configuration.GetSection(nameof(SwaggerOptions)).Bind(_swaggerOptions);

            app.UseSwagger(option => { option.RouteTemplate = _swaggerOptions.JsonRoute; });
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(_swaggerOptions.UiEndpoint, _swaggerOptions.Description);
            });

            app.UseCors(builder => builder.AllowAnyOrigin()
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private SwaggerOptions GetSwaggerOptions()
        {
            return _swaggerOptions ??= new SwaggerOptions();
        }

        private SwaggerOptions _swaggerOptions;
    }
}