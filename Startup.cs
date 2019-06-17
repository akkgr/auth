using auth.Extension;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityUser = Microsoft.AspNetCore.Identity.MongoDB.IdentityUser;
using IdentityRole = Microsoft.AspNetCore.Identity.MongoDB.IdentityRole;

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
            //services.AddIdentityWithMongoStores($"{Configuration.GetValue<string>("MongoConnection")}/{Configuration.GetValue<string>("MongoDatabaseName")}");
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
                .AddProfileService<ProfileService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseIdentityServer();
            app.UseMongoDbForIdentityServer();
            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
