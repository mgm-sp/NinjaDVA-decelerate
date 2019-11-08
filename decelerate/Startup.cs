using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using decelerate.Models;
using decelerate.Hubs;
using decelerate.Utils;

namespace decelerate
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
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddSingleton(Configuration);
            services.AddDbContext<DecelerateDbContext>(options => options.UseSqlite(Configuration["database:connection"]));
            services.AddSingleton(new UserAuthManager(Configuration));
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = "/PresenterArea/Login";
            });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DecelerateDbContext dbContext,
            ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<PresenterHub>("/PresenterArea/Hub");
            });

            /* Make sure all migrations are applied: */
            dbContext.Database.Migrate();

            /* Create initial presenter if none exist already: */
            if (!dbContext.Presenters.Any())
            {
                var presenterName = Configuration.GetValue<string>("InitialPresenter:Name");
                var presenterPassword = Configuration.GetValue<string>("InitialPresenter:Password");
                if (presenterName != null && presenterName.Length != 0 && presenterPassword != null && presenterPassword.Length != 0)
                {
                    var presenter = new Presenter
                    {
                        Name = presenterName.ToLower(),
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(presenterPassword)
                    };
                    dbContext.Presenters.Add(presenter);
                    dbContext.SaveChanges();
                    logger.LogInformation("Initial presenter created.");

                    /* Create initial room if none exist already: */
                    if (!dbContext.Rooms.Any())
                    {
                        var roomName = Configuration.GetValue<string>("InitialRoom:Name");
                        var roomPublic = Configuration.GetValue<bool>("InitialRoom:Public");
                        var roomCode = Configuration.GetValue<string>("InitialRoom:AdmissionCode");
                        if (roomName != null && roomName.Length != 0 && roomCode != null && roomCode.Length != 0)
                        {
                            dbContext.Rooms.Add(new Room
                            {
                                Name = roomName,
                                Public = roomPublic,
                                AdmissionCode = roomCode,
                                PresenterName = presenter.Name
                            });
                            dbContext.SaveChanges();
                            logger.LogInformation("Initial room created.");
                        }
                        else
                        {
                            logger.LogWarning("Invalid initial room config, no initial room created.");
                        }
                    }
                    else
                    {
                        logger.LogInformation("There are already rooms, no initial room created.");
                    }
                }
                else
                {
                    logger.LogWarning("Invalid initial presenter config, no initial presenter created.");
                }
            }
            else
            {
                logger.LogInformation("There are already presenters, no initial presenter created.");
            }
        }
    }
}
