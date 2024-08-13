namespace data_agent_service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;
    private readonly ConfigWorker config;
    private readonly string organization = Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORGANIZATION") ?? "AddExistingOrg";
    private readonly string pat = Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORGANIZATION_PAT") ?? "AddValidToken";

    public Worker(ILogger<Worker> logger, IOptions<ConfigWorker> config)
    {
        this.logger = logger;
        this.config = config?.Value;
        logger.LogInformation("Worker init time: {time}", DateTimeOffset.Now);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string url = string.Format(config.UrlAnalitycs, organization);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Configurar el tiempo de espera
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    // Configurar la autenticación
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", $"{pat}");
                    await GetAzureData(httpClient, url, 1);
                }
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(config.RequestInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Global Error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Función recursiva para obtener los eventos de Analitycs y manera el paginado
    /// </summary>
    /// <param name="client">Cliente para la petición</param>
    /// <param name="url">Url del request</param>
    /// <param name="pages">Número de página, que deberá de saltar en la siguiente iteración</param>
    /// <returns>Retorna la tarea asyncrona</returns>
    private async Task GetAzureData(HttpClient client, string url, int pages)
    {
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error response: {response.ReasonPhrase} In Analytics Azure");
            return;
        }

        var jsonResponse = await response.Content.ReadFromJsonAsync<ResponseMetricData>();
        if (jsonResponse == null)
        {
            return;
        }

        Console.WriteLine("Recovered-Objects From Analytics: {0}", jsonResponse.Value?.Count);
        await SendToCloud(client, pages, jsonResponse);
    }

    /// <summary>
    /// Envía la data formateada al provedor de datos proporcionado
    /// </summary>
    /// <param name="client">Cliente original de la petición para la llamada recursiva</param>
    /// <param name="pages">Paginado actual</param>
    /// <param name="jsonResponse">Lista de datos formateados</param>
    /// <returns>Tarea asyncrona</returns>
    private async Task SendToCloud(HttpClient client, int pages, ResponseMetricData jsonResponse)
    {
        if (!(jsonResponse != null && jsonResponse.Value != null && jsonResponse.Value.Any()))
        {
            Console.WriteLine($"Sin elementos que indexar");
            return;
        }
        try
        {
            await manager.ProcessToCloud(FormatData(jsonResponse), (CloudDatabaseType)Enum.Parse(typeof(CloudDatabaseType), this.config.CloudProvider));
            if (jsonResponse.OdataNextLink != null)
            {
                logger.LogInformation("Procesando paginación: {0}", pages);
                var skiptValue = string.Format("&$skip={0}", pages * config.Offset);
                await GetAzureData(client,  string.Format(config.UrlAnalitycs, organization) + skiptValue, pages + 1);
            }
            logger.LogInformation("Fin Paginación");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error {ex.Message}: In Cloud Index Service");
            return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonResponse"></param>
    /// <returns></returns>
    private static IEnumerable<ItemProccess> FormatData(ResponseMetricData jsonResponse)
        => from data in jsonResponse.Value
           select new ItemProccess
           {
               CompletedDate = data.CompletedDate,
               DateValue = data.DateValue,
               StateCategory = data.StateCategory,
               State = data.State,
               WorkItemType = data.WorkItemType,
               WorkItemId = data.WorkItemId,
               StoryPoints = data.StoryPoints,
               Count = data.Count,
               IterationEndDate = data.Iteration.EndDate,
               IterationName = data.Iteration.IterationName,
               IterationStartDate = data.Iteration.StartDate,
               IterationPath = data.Iteration.IterationPath,
               TeamName = data.Team.TeamName,
               ProjectName = data.Project.ProjectName,
               AreaPath = data.Area.AreaPath
           };
}