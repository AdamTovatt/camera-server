using CameraServer.Controllers;
using CameraServer.Helpers;
using CameraServer.Repositories;

namespace CameraServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Migrator.PerformDatabaseMigrations(); // run database migrations

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
            {
                builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }));

            builder.Services.AddScoped<ICameraRepository, CameraRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseWebSockets();
            app.Map("/video-input-stream", (app) => { app.UseMiddleware<StreamInputHandler>(); });
            app.Map("/video-output-stream", (app) => { app.UseMiddleware<StreamOutputHandler>(); });
            app.UseCors("corsapp");
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}