using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Zenzinger_Eshop_Web.Models.Database;
using Zenzinger_Eshop_Web.Models.Entity.Identity;

namespace Zenzinger_Eshop_Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                if (scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<EshopDbContext>();
                    DatabaseInit dbInit = new DatabaseInit();
                    dbInit.Initialization(dbContext);
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>(); ;
                    await dbInit.EnsureRoleCreated(roleManager);
                    await dbInit.EnsureAdminCreated(userManager);
                    await dbInit.EnsureManagerCreated(userManager);
                }
            }
            
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((loggingBuilder) =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();
                    loggingBuilder.AddFile("Logs/eshop-log-{Date}.txt");
                });
    }
}
