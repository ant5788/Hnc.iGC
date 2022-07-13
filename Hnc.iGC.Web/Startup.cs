using Hnc.iGC.Models;
using Hnc.iGC.Web.Areas.Identity;
using Hnc.iGC.Web.Hubs;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

using System.Globalization;
using System.Reflection;

namespace Hnc.iGC.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddiGCOptions(Configuration);

            switch (Configuration["Database"])
            {
                case "Sqlite":
                    services.AddDbContextFactory<ApplicationDbContext>(options =>
                    {
                        options.UseSqlite(Configuration.GetConnectionString("Sqlite"));
                    });
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseSqlite(Configuration.GetConnectionString("Sqlite"));
                    });
                    break;

                case "SqlServer":
                    services.AddDbContextFactory<ApplicationDbContext>(options =>
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
                    });
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
                    });
                    break;

                default:
                    services.AddDbContextFactory<ApplicationDbContext>(options =>
                    {
                        options.UseSqlite(Configuration.GetConnectionString("Sqlite"));
                    });
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseSqlite(Configuration.GetConnectionString("Sqlite"));
                    });
                    break;
            }

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("zh-Hans"),
                new CultureInfo("en-US"),
            };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("zh-Hans");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddControllers().AddControllersAsServices().AddJsonOptions(c =>
            {
                c.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
                c.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                c.JsonSerializerOptions.PropertyNamingPolicy = null;
                c.JsonSerializerOptions.WriteIndented = false;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "iGS API",
                    Description = "iGS HTTP API",
                    TermsOfService = new Uri("http://www.hszn.link/"),
                    Contact = new OpenApiContact
                    {
                        Name = "山东华数智能科技有限公司",
                        Email = "shandonghuashuznkj@163.com",
                        Url = new Uri("http://www.hszn.link/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License",
                        Url = new Uri("http://www.hszn.link/"),
                    }
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddRazorPages().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddRazorRuntimeCompilation();
            services.AddServerSideBlazor();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.AddAntDesign();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            services.AddDatabaseDeveloperPageExceptionFilter();

            var physicalProvider = new PhysicalFileProvider(Configuration.GetValue<string>("StoredFilesPath"));
            services.AddSingleton<IFileProvider>(physicalProvider);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                 {
                     builder.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                 });
            });

            services.AddSingleton(new NcLink.NcLinkService2(Configuration["MQTT:IP"] ?? "127.0.0.1", Configuration.GetValue<int?>("MQTT:Port")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRequestLocalization();

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapHub<ControlHub>("/controlhub");
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
