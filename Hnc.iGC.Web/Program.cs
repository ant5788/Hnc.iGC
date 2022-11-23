using Hnc.iGC.Models;
using Hnc.iGC.Web.Worker;

using Microsoft.AspNetCore.Identity;

using Fleck;
using Quartz;
using Quartz.Impl;

using Serilog;

namespace Hnc.iGC.Web
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true, true)
                .Build();

            var allSockets = new List<IWebSocketConnection>();
            var server = new WebSocketServer("ws://127.0.0.1:7181");
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("与客户端已经连接上 Open!");
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine("与客户端已经断开连接 Close!");
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    Console.WriteLine(message);
                    allSockets.ToList().ForEach(s => s.Send("客户端 Echo: " + message));
                };
            });


            IJobDetail job = JobBuilder.Create<WorkJob>()
                .WithIdentity("workJob", "work")
                .Build();

            job.JobDataMap.Put("allSockets", allSockets);

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("workTrigger", "work")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
                .Build();

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();
            await scheduler.ScheduleJob(job, trigger);

            Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();

            try
            {
                var storedFilesPath = configuration["StoredFilesPath"] ?? @"C:\ProgramData\Hnc\iGC\CNC_Files";
                Directory.CreateDirectory(storedFilesPath);

                Log.Information("{namespace}程序启动", typeof(Program).Namespace);
                var host = CreateHostBuilder(args).Build();
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        SeedDataAsync(services).Wait();
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred creating the DB.");
                    }
                }
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "{namespace}程序中断", typeof(Program).Namespace);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureServices(services =>
                {
                    //services.AddHostedService<AirDashboardWorker>();

                    //services.AddHostedService<MockDataWorker>();

                    //services.AddHostedService<CNCNeighborWorker>();
                    //services.AddHostedService<CNCStatisticWorker>();
                    //services.AddHostedService<CNCProgWorker>();

                    //services.AddHostedService<RRWorker>();
                    //services.AddHostedService<SerialPortWorker>();
                });

        public const string RoleNameAdministrator = "Administrator";

        public const string UserNameAdmin = "admin";
        public const string UserPasswordAdmin = "Hs123456";

        public const string UserNameGuest = "guest";
        public const string UserPasswordGuest = "Hs123456";

        private static async Task SeedDataAsync(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
            if (!dbContext.Roles.Any())
            {
                var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
                var role = new ApplicationRole { Name = RoleNameAdministrator };
                if (!(await roleManager.CreateAsync(role)).Succeeded)
                {
                    throw new ArgumentException($"初始化角色[{RoleNameAdministrator}]失败！");
                }
            }
            if (!dbContext.Users.Any())
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var userAdmin = new ApplicationUser { UserName = UserNameAdmin };
                if (!(await userManager.CreateAsync(userAdmin, UserPasswordAdmin)).Succeeded)
                {
                    throw new ArgumentException($"初始化用户[{UserNameAdmin}]失败！");
                }
                else if (!(await userManager.AddToRoleAsync(userAdmin, RoleNameAdministrator)).Succeeded)
                {
                    throw new ArgumentException($"添加角色[{RoleNameAdministrator}]到用户[{UserNameAdmin}]失败！");
                }
                var userGuest = new ApplicationUser { UserName = UserNameGuest };
                if (!(await userManager.CreateAsync(userGuest, UserPasswordGuest)).Succeeded)
                {
                    throw new ArgumentException($"初始化用户[{UserNameGuest}]失败！");
                }
            }
        }

    }
}
