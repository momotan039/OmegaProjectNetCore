using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OmegaProject.services;

namespace OmegaProject
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
            services.AddControllers();
          
            //Create Connection to DataBase By String Connection
            services.AddDbContext<MyDbContext>(
                o => o.UseSqlServer(Configuration.GetConnectionString("OmegaDbConnectionString")));

            //Configure cors in service
            services.AddCors(option => option.AddPolicy("myPolicy", builder =>
            {
                 builder.WithOrigins("http://localhost:4200", "https://omegaangular.firebaseapp.com")
            .AllowAnyHeader()
            .AllowAnyMethod();
                //builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            //to fix json lenth depth
            services.AddControllersWithViews().AddNewtonsoftJson(
                options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddHttpContextAccessor();
            //To complete the injection process properly
            services.AddScoped<JwtService>();
            //to declare jtw in this configuration
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
                            {
                                x.RequireHttpsMetadata = false;
                                x.SaveToken = true;
                                x.TokenValidationParameters = JwtService.TokenValidationParameters;
                            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            //services.Configure<IISServerOptions>(options =>
            //{
            //    options.MaxRequestBodySize = int.MaxValue;
            //});
            //services.Configure<KestrelServerOptions>(options =>
            //{
            //    options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
            //});
            services.Configure<FormOptions>(o =>  // currently all set to max, configure it to your needs!
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = long.MaxValue; // <-- !!! long.MaxValue
                
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            MyTools.ImagesRoot = System.IO.Path.Combine(env.WebRootPath, "Images");
            MyTools.Root = env.WebRootPath;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();
            //cors must be between routing and authentication
            app.UseCors("myPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            //Configure cors in http


            app.UseEndpoints(endpoints =>
            {
               

                endpoints.MapControllers();
            });


        }

    }
}
