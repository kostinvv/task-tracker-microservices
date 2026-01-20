Env.TraversePath().Load("./docker-compose/.env.emailsender");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(configuration: builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Ok.");

app.Run();