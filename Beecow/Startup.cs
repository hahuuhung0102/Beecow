using Beecow.Entities;
using Beecow.Handlers;
using Beecow.Helpers;
using Beecow.Interfaces;
using Beecow.Requirements;
using Beecow.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beecow
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
            services.AddDbContext<CustomersDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CustomersDbConnectionString")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = TokenHelper.Issuer,
                            ValidAudience = TokenHelper.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(TokenHelper.Secret))
                        };

                    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("OnlyNonBlockedCustomer", policy => {
                    policy.Requirements.Add(new CustomerBlockedStatusRequirement(false));

                });
            });

            services.AddSingleton<IAuthorizationHandler, CustomerBlockedStatusHandler>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddAutoMapper(typeof(Startup));
            services.AddControllers();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation  
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "JWT Token Authentication API",
                    Description = "ASP.NET Core 3.1 Web API"
                });
                // To Enable authorization using Swagger (JWT)  
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });



            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
