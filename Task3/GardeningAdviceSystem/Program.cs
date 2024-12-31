
using Core.Models;
using DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GardeningAdviceSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseLazyLoadingProxies()
                    .UseSqlServer(builder.Configuration.GetConnectionString("DBConnectionString"));
            });

            string repositoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            builder.Services.AddScoped<FileRepository>(provider =>
                new FileRepository(provider.GetRequiredService<ILogger<FileRepository>>(), repositoryPath));

            builder.Services.AddScoped(typeof(Repository<>));
            builder.Services.AddScoped(typeof(UnitOfWork));

            builder.Services.AddIdentity<Account, IdentityRole>().
                AddDefaultTokenProviders().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>().AddSignInManager<SignInManager<Account>>();
            builder.Services.AddScoped<UserManager<Account>>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
