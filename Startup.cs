using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleAPI.Models;
using ExampleAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

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
                    //options.SerializerSettings.ContractResolver = new UserContractResolver();
                    options.SerializerSettings.Converters.Add(new UserConverter());
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

    class UserConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            // we can only convert users and lists of users
            if (objectType == typeof(UserModel)|| objectType == typeof(List<UserModel>))
            {
                return true;
            }
            return false;
        }

        // cannot deserialize json
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is UserModel)
            {
                // convert and set password to null
                var user = (UserModel)value;
                
                user.Password = null;

                serializer.Serialize(writer, user);
            }

            if (value is List<UserModel>)
            {

                List<UserModel> users = (List<UserModel>)value;
                
                // start writing array
                writer.WriteStartArray();

                foreach(var user in users)
                {
                    user.Password = null;   // set password to null
                    serializer.Serialize(writer, user); //serialize into json
                }

                writer.WriteEndArray();

                
            }
            
            
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
