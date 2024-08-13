var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.Sources.Clear();
        config.AddJsonFile("Resources/appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.Configure<ConfigWorker>(hostContext.Configuration.GetSection(ConfigWorker.SectionName));
    })
    .Build();

await host.RunAsync();
