public static class CosmosBDClientProvider
{
    private static readonly CosmosClient Client;

    private static readonly Database bd;

    private static readonly Container container;

    private static string endpointUri = Environment.GetEnvironmentVariable("COSMOSURL");

    private static string primaryKey = Environment.GetEnvironmentVariable("COSMOSKEY");

    private static string containerName = Environment.GetEnvironmentVariable("COSMOSCONTAINER");

    private static string bdName = Environment.GetEnvironmentVariable("COSMOSBDNAME");

    static CosmosBDClientProvider()
    {
        Client = new CosmosClient(endpointUri, primaryKey, new CosmosClientOptions()
        {
             ApplicationName = "data-agent-service",
             AllowBulkExecution = true,
             RequestTimeout = new TimeSpan(0,0,10),
        });
        bd = Client.GetDatabase(bdName);
        container = bd.GetContainer(containerName);
    }

    public static CosmosClient GetClient()
    {
        return Client;
    }

    public static Container GetContainer()
    {
       return container;
    }

   public static Database GetDatabase()
   {
     return bd;
   }

}