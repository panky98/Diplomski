 using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using UserMicroservice.Configuration;
using UserMicroservice.Repositories;
using UserMicroservice.Services;

namespace UserMicroservice
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserMicroservice", Version = "v1" });
            });

            string conn = Configuration.GetConnectionString("Konekcija");
            services.AddDbContext<UserContext>(options =>
            {
                options.UseSqlServer(conn);
            });

            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }).AddXmlSerializerFormatters();

            services.AddCors(options => {
                options.AddPolicy("Corse", builder => {
                    builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins("http://localhost:4200", "http://eventmicroservice:80", "http://streamingmicroservice:80", "http://usermicroservice:80", "http://apigateway:80")
                    .AllowCredentials();
                });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    //provera issuer-a u tokenu
                    ValidateIssuer = true,
                    //provera audience u tokenu
                    ValidateAudience = true,
                    //provera LifeTime, da nije isteklo vreme tokenu
                    ValidateLifetime = true,
                    //provera key-a
                    ValidateIssuerSigningKey = true,
                    ValidAudience = Configuration["Jwt:Issuer"],
                    ValidIssuer = Configuration["Jwt:Audience"],
                    //niz bajtova koji se ocekuje da se primi preko token-a, taj niz bajtova u sustini predstavlja secretkey
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddHostedService<EventService>();

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,UserContext context)
        {
            context.Database.Migrate();
            Console.WriteLine("Migrations applied");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserMicroservice v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("Corse");

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationsHub>("/notifications");
            });
        }
    }
}
