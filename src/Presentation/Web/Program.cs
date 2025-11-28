using Application;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Web.Configurations;
using Web.Configurations.Common;

namespace Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureRoutes();
        
        builder.Services.AddInfrastructure();
        builder.Services.AddApplication();

        builder.Services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(
                new KebabCaseParameterTransformer()));
        });

        builder.Services.AddSwaggerGen();

        var corsOrigins = Environment.GetEnvironmentVariable("CORS_ORIGINS")
            ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? ["http://localhost:5173", "http://localhost:3000"];

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(corsOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors();

        app.MapControllers();

        app.Run();
    }
}