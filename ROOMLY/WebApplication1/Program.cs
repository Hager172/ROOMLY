using System.Text;
using Castle.Core.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ROOMLY.DTOs.email;
using ROOMLY.Identity;
using ROOMLY.Mapconfig;
using ROOMLY.UnitOfwork;
using WebApplication1.models;

namespace WebApplication1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
   


            builder.Services.AddDbContext<RoomlyContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("CS")));
            
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<RoomlyContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAutoMapper(typeof(MapConfig));
            builder.Services.AddScoped<UnitOfWork>();
           
            builder.Services.AddTransient<ROOMLY.DTOs.email.IEmailSender,EmailSender>();

         // builder.Services.AddTransient<ROOMLY.DTOs.email.IEmailSender, NullEmailSendercs>();


            builder.Services.AddAuthentication(options=> {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtsetting = builder.Configuration.GetSection("JwtSettings");
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtsetting["Issuer"],
                    ValidAudience = jwtsetting["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsetting["SecretKey"]))

                };


            })
                ;

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Roomly API V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            // Initialize Database (Seeding)
            await InitializeDatabaseAsync(app);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        private static async Task InitializeDatabaseAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await AppIdentityDBcontextSeed.SeedUserAsync(userManager, roleManager);
        }
    }
}
