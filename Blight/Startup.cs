using Blight.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Blight.Interfaces;
using Blight.Entieties;
using Blight.Middlewares;
using Blight.Repository;
using FluentValidation;
using Blight.Models;
using Blight.Models.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Blight.Authentication;
using Blight.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.EntityFrameworkCore.Proxies;
using Blight.Interfaces.MethodsProvider;
using Blight.Services.MethodProvider;
using Blight.Services.MethodProvider.SearchingDb.Shared;
using Microsoft.AspNetCore.Http;
using Blight.Services.MethodProvider.SearchingDb;

namespace Blight
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authenticationSettings = new AuthenticationSettings();
            Configuration.GetSection("Authentication").Bind(authenticationSettings);

            services.AddSingleton(authenticationSettings);
            services.AddSingleton<IConfiguration>(Configuration);
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = authenticationSettings.JwtIssuer,
                        ValidAudience = authenticationSettings.JwtIssuer,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
                    };
                });
            services.AddControllers()
                    .AddNewtonsoftJson(opt=>
                    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
                    .AddFluentValidation();

            services.AddCors(options => options.AddDefaultPolicy(
                policy => policy.WithOrigins(Configuration["AllowedCORSOrigin"])
                                           .AllowAnyHeader()
                                           .AllowAnyMethod()));
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Blight", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }

                });
            });
            services.AddDbContext<BlightDbContext>(cfg =>
                cfg.LogTo(Console.WriteLine,LogLevel.Information)
                   .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<PaginationPhoneQuery>, PaginationQueryValidator>();
            services.AddScoped<IValidator<PaginationUserQuery>, PaginationQueryValidator>();
            services.AddScoped<IValidator<PaginationQuery>, PaginationQueryValidator>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IUserRepository, UserRepos>();
            services.AddScoped<IPhoneRepository, PhoneRepos>();
            services.AddScoped<ISchemeGenerator, SchemesGenerator>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IAdminPasswordService, AdminPasswordService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IPaginating, PaginatingMethod>();
            services.AddScoped<ISorting<PhoneNumber>, SortingIPhoneDto>();
            services.AddScoped<ISorting<User>, SortingIUserDto>();
            services.AddScoped<ISearchingUserDbSet, SearchingUserDb>();
            services.AddScoped<ISearchingPhoneDbSet>(provider =>
            {
                var userContextService = provider.GetService<IUserContextService>();
                var userRole = userContextService.GetUserRole;

                if (userRole == "User")
                    return new UserSearchingPhoneDb();
                else if (userRole == "Admin" || userRole == "Master")
                    return new AdminSearchingPhoneDb();
                else
                    return null;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseResponseCaching();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blight v1"));
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
