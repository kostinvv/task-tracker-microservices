using Microsoft.EntityFrameworkCore;
using TaskTracker.Services.Shared.Events.Users;
using TaskTracker.Services.Tasks.Api.ApiExtensions;
using TaskTracker.Services.Tasks.ApplicationCore;
using TaskTracker.Services.Tasks.ApplicationCore.Models;
using TaskTracker.Services.Tasks.ApplicationCore.Options;
using TaskTracker.Services.Tasks.Infrastructure;
using TaskTracker.Services.Tasks.Infrastructure.Kafka;

namespace TaskTracker.Services.Tasks.Api;

public static class DependencyInjection
{
    private const string ConnectionStringSectionName = "DefaultConnection";
    private const string ClientUrlSectionName = "JsClient";
    private const string PolicyName = "JsClient";
    
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddProblemDetails();
        
        var connectionString = Environment.GetEnvironmentVariable(ConnectionStringSectionName)!;
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        
        builder.Services.AddProducer<UserRegisteredEvent>(builder.Configuration
            .GetSection(key: $"{KafkaOptions.SectionName}:{KafkaOptions.TasksSectionName}"));
        
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
        
        builder.Services.AddSwaggerConfiguration();
        
        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        
        var clientUrl = Environment.GetEnvironmentVariable(ClientUrlSectionName)!;
        builder.Services.AddCors(options 
            => options.AddPolicy(name: PolicyName, configurePolicy: policyBuilder 
                => policyBuilder
                    .WithOrigins(clientUrl)
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            )
        );
        
        builder.Services.AddAuthConfiguration(builder.Configuration);
        builder.Services.AddApplicationCore(builder.Configuration);
        builder.Services.AddInfrastructure();
        
        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskTracker Service API v1");
            });
        }
        
        app.UseHttpsRedirection();
        
        app.UseCors(policyName: PolicyName);
        
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        
        return app;
    }
}