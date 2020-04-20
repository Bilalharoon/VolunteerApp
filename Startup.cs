using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExampleAPI.Helpers;
using ExampleAPI.Models;
using ExampleAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            
           
            services.AddControllers(
                options => { options.Filters.Add(new HttpResponseExceptionFilter()); }
                ).AddNewtonsoftJson(
                options =>
                { 
                    // Ignore loops
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                    // add the converter to make the passwords null
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
            return typeof(UserModel).IsAssignableFrom(objectType) || objectType == typeof(List<UserModel>);
        }

        // cannot deserialize json
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case UserModel user:
                {
                    // Create a Json token from the object
                    var t = JToken.FromObject(user);


                    // Convert the Json token to a Json object
                        var o = (JObject)t;

                    // remove the password property
                    o.Remove("Password");

                    // write
                    o.WriteTo(writer);
                    break;
                }
                case List<UserModel> users:
                {
                    // start writing array
                    writer.WriteStartArray();

                    foreach(var u in users)
                    {
                        u.Password = null;   // set password to null
                        serializer.Serialize(writer, u); //serialize into json
                    }

                    writer.WriteEndArray();
                    break;
                }
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is HttpResponseException exception)
            {
                context.Result = new ObjectResult(exception.Value)
                {
                    StatusCode = exception.Status,
                    
                };
                context.ExceptionHandled = true;
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }

        public int Order { get; } = 1000;
    }
    
}
