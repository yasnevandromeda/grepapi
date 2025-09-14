using grepapi.Models;
using Microsoft.EntityFrameworkCore;

namespace grepapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMemoryCache();
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            string completeConnectionString = "Password=yayqaCIEw20?9B?CTO5PEj;Host=51.250.102.249;Port = 5432;Database=grep;Username=postgres";

           builder.Services.AddDbContext<GrepContext>(
                        optionsBuilder => optionsBuilder.UseNpgsql(
                                completeConnectionString,
                                options => options.EnableRetryOnFailure(
                                    maxRetryCount: 3,
                                    maxRetryDelay: TimeSpan.FromMilliseconds(100),
                                    errorCodesToAdd: null))
                            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking),
                        ServiceLifetime.Transient);


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
