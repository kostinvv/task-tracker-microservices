Env.TraversePath().Load("./.env.emailsender");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConsumer<UserRegisteredEvent, UserRegisteredEventHandler>(
    builder.Configuration.GetSection(KafkaOptions.SectionName));

var app = builder.Build();

app.MapGet("/", () => "Ok.");

app.Run();