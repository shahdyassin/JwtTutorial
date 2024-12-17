
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Configuration;
using System.Text;
using TestApiJWT.Helpers;
using TestApiJWT.Models;
using TestApiJWT.Services;

namespace TestApiJWT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                           .AddJwtBearer(o =>
                           {
                               o.RequireHttpsMetadata = false;
                               o.SaveToken = false;
                               o.TokenValidationParameters = new TokenValidationParameters
                               {
                                   ValidateIssuerSigningKey = true,
                                   ValidateIssuer = true,
                                   ValidateAudience = true,
                                   ValidateLifetime = true,
                                   ValidIssuer = builder.Configuration["JWT:Issuer"],
                                   ValidAudience = builder.Configuration["JWT:Audience"],
                                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                                   ClockSkew = TimeSpan.Zero

                               };
                           });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapScalarApiReference();
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
