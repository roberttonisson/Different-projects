using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DAL.App.EF;
using Domain.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace WebApp
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
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("MsSqlConnection")));

            
            services.AddIdentity<AppUser, AppRole>()//options => options.SignIn.RequireConfirmedAccount = true
                .AddDefaultUI()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
    
            services.AddControllersWithViews();
            services.AddRazorPages();
            
            services.AddCors(options =>
            {
                options.AddPolicy("CorsAllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
            });
            
            // =============== JWT support ===============
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication()
                .AddCookie(options => { options.SlidingExpiration = true; })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["JWT:Issuer"],
                        ValidAudience = Configuration["JWT:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:SigningKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });
            
            // services.Configure<RequestLocalizationOptions>(options =>{
            //     var supportedCultures = new[]{
            //         new CultureInfo(name: "en"),
            //         new CultureInfo(name: "et")
            //     };
            //
            //     // State what the default culture for your application is. 
            //     options.DefaultRequestCulture = new RequestCulture(culture: "en-GB", uiCulture: "en-GB");
            //
            //     // You must explicitly state which cultures your application supports.
            //     options.SupportedCultures = supportedCultures;
            //
            //     // These are the cultures the app supports for UI strings
            //     options.SupportedUICultures = supportedCultures;
            // });
            //
            // services.AddApiVersioning(options =>
            //             {
            //                 options.ReportApiVersions = true;
            //                 // options.DefaultApiVersion = new ApiVersion(1,0);
            //                 // options.AssumeDefaultVersionWhenUnspecified = false;
            //             });
            //             
            // services.AddVersionedApiExplorer( options => options.GroupNameFormat = "'v'VVV" );
            // services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            // services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env /*, IApiVersionDescriptionProvider provider*/)
        {
            UpdateDatabase(app, env, Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRequestLocalization(
                options: app.ApplicationServices
                    .GetService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("CorsAllowAll");

            app.UseRouting();
            
            
            // app.UseSwagger();
            // app.UseSwaggerUI(
            //     options =>
            //     {
            //         foreach ( var description in provider.ApiVersionDescriptions )
            //         {
            //             options.SwaggerEndpoint(
            //                 $"/swagger/{description.GroupName}/swagger.json",
            //                 description.GroupName.ToUpperInvariant() );
            //         }
            //     } );

            
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        private static void UpdateDatabase(IApplicationBuilder app, IWebHostEnvironment env,
            IConfiguration Configuration)
        {
            // give me the scoped services (everyhting created by it will be closed at the end of service scope life).
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            using var ctx = serviceScope.ServiceProvider.GetService<AppDbContext>();
            using var userManager = serviceScope.ServiceProvider.GetService<UserManager<AppUser>>();
            using var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<AppRole>>();

            if (Configuration["AppDataInitialization:DropDatabase"] == "True")
            {
                Console.WriteLine("DropDatabase");
                DAL.App.EF.Helpers.DataInitializers.DeleteDatabase(ctx);
            }

            if (Configuration["AppDataInitialization:MigrateDatabase"] == "True")
            {
                Console.WriteLine("MigrateDatabase");
                DAL.App.EF.Helpers.DataInitializers.MigrateDatabase(ctx);
            }

            if (Configuration["AppDataInitialization:SeedIdentity"] == "True")
            {
                Console.WriteLine("SeedIdentity");
                DAL.App.EF.Helpers.DataInitializers.SeedIdentity(userManager, roleManager);
            }

            if (Configuration.GetValue<bool>("AppDataInitialization:SeedData"))
            {
                Console.WriteLine("SeedData");
                DAL.App.EF.Helpers.DataInitializers.SeedData(ctx);
            }
        }
    }
}