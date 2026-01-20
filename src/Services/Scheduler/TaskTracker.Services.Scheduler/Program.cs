using TaskTracker.Services.Scheduler.Services;

Env.TraversePath().Load("./docker-compose/.env.scheduler");

var builder = Host.CreateApplicationBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")!;
builder.Services.AddDbContext<SchedulerDbContext>(options => options.UseNpgsql(connectionString));

const string kafkaSectionName = $"{KafkaOptions.SectionName}:{KafkaOptions.TasksSectionName}";
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection(kafkaSectionName));
builder.Services.AddSingleton<IKafkaProducer<EmailNotificationEvent>, KafkaProducer<EmailNotificationEvent>>();

builder.Services.AddSingleton<IRazorEngineService>(_ =>
{
    var templateServiceConfiguration = new TemplateServiceConfiguration();
    return RazorEngineService.Create(templateServiceConfiguration);
});

builder.Services.AddSingleton<IEmailTemplateService, RazorEmailTemplateService>();
builder.Services.AddScoped<IUserReportService, UserReportService>();

builder.Services.AddTransient<Worker>();
builder.Services.Configure<SchedulerOptions>(builder.Configuration.GetSection(SchedulerOptions.SectionName));

builder.Services.AddOptions<QuartzOptions>()
    .Configure<IOptions<SchedulerOptions>>((options, dep) =>
    {
        const string jobName = nameof(Worker);
        
        options.AddJob<Worker>(job => job.WithIdentity(name: jobName));
        
        options.AddTrigger(trigger => trigger
            .WithIdentity(name: jobName)
            .ForJob(jobName)
            .WithCronSchedule(dep.Value.CronSchedule)
        );
    });

builder.Services.AddQuartz();

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

var host = builder.Build();
host.Run();