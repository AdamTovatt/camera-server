using CameraServer.Controllers;
using Microsoft.AspNet.SignalR.WebSockets;

namespace CameraServer
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

            builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
            {
                builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseWebSockets();
            //app.UseMiddleware<StreamInputHandler>();
            app.Map("/video-input-stream", (app) => { app.UseMiddleware<StreamInputHandler>(); });
            app.UseCors("corsapp");
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}