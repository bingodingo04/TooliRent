using System.Text;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Application;
using Domain;
using Infrastructure.Data;
using Application.Services.Interfaces;
using Domain.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register EF Core DbContext and configure SQL Server using the "Default" connection string
            builder.Services.AddDbContext<TooliRentDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            // ASP.NET Core Identity
            builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(opt =>
            {
                // Require unique email addresses for accounts
                opt.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<TooliRentDbContext>() // use our DbContext as the Identity store
            .AddDefaultTokenProviders();                    // adds token providers (email confirm, reset password, etc.)

            // JWT Bearer authentication configuration
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Validate token origin and integrity
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        // Expected issuer/audience read from configuration
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],

                        // Symmetric signing key (keep secret out of source control in real projects)
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            // Authorization policies: gates for Member and Admin roles
            builder.Services.AddAuthorization(opt =>
            {
                opt.AddPolicy("MemberOnly", p => p.RequireRole("Member", "Admin"));
                opt.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
            });

            // AutoMapper: register mapping profile(s)
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            // FluentValidation: register validators from this assembly
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            // Dependency Injection for repositories and services (N-tier wiring)
            builder.Services.AddScoped<IToolRepository, ToolRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IToolService, ToolService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            // MVC controllers
            builder.Services.AddControllers();

            // API explorer + Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                // Basic Swagger document metadata
                c.SwaggerDoc("v1", new()
                {
                    Title = "TooliRent API",
                    Version = "v1",
                    Description = "A RESTful API for managing tool rentals in a makerspace.\r\nSupports authentication, tool browsing, booking, check-in/out, and admin features"
                });

                // Swagger security scheme for JWT Bearer
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **only** your JWT (no 'Bearer ' prefix). " +
                                  "Swagger UI will add the 'Bearer ' prefix automatically.",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                // Register the scheme and require it by default
                c.AddSecurityDefinition("Bearer", jwtSecurityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });

                // Include XML comments if the XML doc file is generated (improves Swagger summaries)
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            // Build the app pipeline
            var app = builder.Build();

            // Apply migrations + seed on startup
            using (var scope = app.Services.CreateScope())
            {
                Infrastructure.Data.Seed.RunAsync(app.Services).GetAwaiter().GetResult();
            }


            // Enable Swagger UI in Development environment
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Standard middleware
            app.UseHttpsRedirection();
            app.UseAuthentication(); // must be before UseAuthorization
            app.UseAuthorization();

            // Map controller routes (attribute routing)
            app.MapControllers();

            // Start the web application
            app.Run();
        }
    }
}
