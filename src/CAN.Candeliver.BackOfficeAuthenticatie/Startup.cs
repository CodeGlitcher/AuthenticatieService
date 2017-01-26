using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CAN.Candeliver.BackOfficeAuthenticatie.Data;
using CAN.Candeliver.BackOfficeAuthenticatie.Models;
using Swashbuckle.Swagger.Model;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CAN.Candeliver.BackOfficeAuthenticatie.Security;
using Newtonsoft.Json.Serialization;
using CAN.Candeliver.BackOfficeAuthenticatie.Services;
using CAN.Candeliver.BackOfficeAuthenticatie.Swagger;
using System.IdentityModel.Tokens.Jwt;
using CAN.Candeliver.BackOfficeAuthenticatie.Data.Repository;
using Serilog;

namespace CAN.Candeliver.BackOfficeAuthenticatie
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
             .ReadFrom
             .ConfigurationSection(Configuration.GetSection("Serilog"))
             .CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Environment.GetEnvironmentVariable("dbconnectionstring")));

            services.AddIdentity<ApplicationUser, IdentityRole>(
                  o => {
                      o.Password.RequireDigit = false;
                      o.Password.RequireUppercase = false;
                      o.Password.RequireNonAlphanumeric = false;
                      o.Password.RequireLowercase = false;
                      o.Password.RequiredLength = 6;

                      
                  
                  })
                  .AddEntityFrameworkStores<ApplicationDbContext>()
                  .AddDefaultTokenProviders();

            services.Configure<TokenProviderOptions>(CreateTokenOptions);
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddMvc();
            services.AddAuthorization();

            services.AddScoped<IAccountService, AccountService>();

            

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Backoffice authenicatation service",
                    Description = "Backoffice authenicatation service",
                    TermsOfService = "None"
                });

                options.OperationFilter<SwaggerAuthorization>();
                
            });


        }

        private void CreateTokenOptions(TokenProviderOptions obj)
        {

            var secretKey = Configuration.GetValue<string>("SecretKey");
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            obj.Audience = "http://cancandeliverbackofficeauthenticatie_can.candeliver.backofficeauthenticatie_1";
            obj.Issuer = "http://cancandeliverbackofficeauthenticatie_can.candeliver.backofficeauthenticatie_1";
            obj.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            obj.Expiration = TimeSpan.FromMinutes(30);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Serilog"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();


            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();
           


            var secretKey = Configuration.GetValue<string>("SecretKey");
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = "http://cancandeliverbackofficeauthenticatie_can.candeliver.backofficeauthenticatie_1",
                ValidateAudience = true,
                ValidAudience = "http://cancandeliverbackofficeauthenticatie_can.candeliver.backofficeauthenticatie_1",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });
            app.UseSwagger();
            app.UseSwaggerUi();
            app.UseMvc();
            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            ApplicationDbContext.SeedDb(app);


        }



        
    }
}
