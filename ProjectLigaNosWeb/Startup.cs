using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectLigaNosWeb.Data.Entities;
using ProjectLigaNosWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectLigaNosWeb.Helpers;
using Syncfusion.Licensing;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectLigaNosWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                cfg.SignIn.RequireConfirmedEmail = true;
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<DataContext>() 
                .AddDefaultTokenProviders();

            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddAuthentication()
               .AddCookie()
               .AddJwtBearer(cfg =>
               {
                   cfg.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidIssuer = this.Configuration["Tokens:Issuer"],
                       ValidAudience = this.Configuration["Tokens:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(this.Configuration["Tokens:Key"]))
                   };
               });

            services.AddTransient<SeedDb>();
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<IBlobHelper, BlobHelper>();
            services.AddScoped<IConverterHelper, ConverterHelper>();
            services.AddTransient<IMailHelper, MailHelper>();

            services.AddScoped<IClubsRepository, ClubsRepository>();
            services.AddScoped<IPlayersRepository, PlayersRepository>();
            services.AddScoped<IGamesRepository, GamesRepository>();
            services.AddScoped<IStatisticsRepository, StatisticsRepository>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/NotAuthorized";
                options.AccessDeniedPath = "/Account/NotAuthorized";
            });

            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; 
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SeedDb seeder)
        {
            SyncfusionLicenseProvider.RegisterLicense("MzQ5NjQ4OEAzMjM3MmUzMDJlMzBrVERwaXUvOGM5Y09tRzdtMlgwcjFoUDd0MlBaVjFWeWlMdHJnSk93SFZjPQ=="); 

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

           
            app.UseAuthentication();
            app.UseAuthorization();

            seeder.SeedAsync().Wait();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
