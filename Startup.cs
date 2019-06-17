using auth.Extension;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityUser = Microsoft.AspNetCore.Identity.MongoDB.IdentityUser;
using IdentityRole = Microsoft.AspNetCore.Identity.MongoDB.IdentityRole;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.HttpOverrides;

namespace auth
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
            services.Configure<ConfigurationOptions>(Configuration);
            services.AddSingleton<IProfileService, ProfileService>();

            // var connectionString = $"{Configuration.GetValue<string>("MongoConnection")}/{Configuration.GetValue<string>("MongoDatabaseName")}";
            // services.AddIdentityWithMongoStoresUsingCustomTypes<IdentityUser, IdentityRole>(connectionString)
            //     .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 6;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            services.AddCors();

            services.AddIdentityServer(
                    // Enable IdentityServer events for logging capture - Events are not turned on by default
                    options =>
                    {
                        options.Events.RaiseSuccessEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseErrorEvents = true;
                    }
                )
                .AddDeveloperSigningCredential()
                .AddMongoRepository()
                .AddMongoDbForAspIdentity<IdentityUser, IdentityRole>(Configuration)
                .AddClients()
                .AddIdentityApiResources()
                .AddPersistedGrants()
                .AddAspNetIdentity<IdentityUser>()
                .AddProfileService<ProfileService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder =>
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials()
            );

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseIdentityServer();
            app.UseMongoDbForIdentityServer();
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
