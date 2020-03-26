using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleAPI.Models;
using ExampleAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ExampleAPI
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
            
           
            services.AddControllers().AddNewtonsoftJson(
                options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                }
                );

            services.AddSingleton(Configuration);
            services.AddDbContext<ApplicationDbContext>();

            // Inject Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEventService, EventService>();

            var secretBytes = Encoding.UTF8.GetBytes(Configuration.GetSection("key").Value);
            var key = new SymmetricSecurityKey(secretBytes);

            
            
            services.AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", config => {
                    config.SaveToken = true;
                    config.TokenValidationParameters = new TokenValidationParameters() {

                        ValidIssuer = "localhost",
                        ValidAudience = "localhost",
                        IssuerSigningKey = key,

                    };
            });

            services.AddCors(
                    options =>
                    {
                        options.AddPolicy("AllowAll", builder => {
                            builder.AllowAnyOrigin()
                                   .AllowAnyMethod()
                                   .AllowAnyHeader();
                            });
                    }
                );
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseCors("AllowAll");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
        }
    }
}
