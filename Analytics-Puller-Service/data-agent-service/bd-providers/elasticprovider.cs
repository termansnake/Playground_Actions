public static class ElasticsearchClientProvider
{
    private static readonly ElasticClient Client;

    static ElasticsearchClientProvider()
    {
        var defaultIndex = "devops.oxxo.cfd";
        var connectionSettings = new ConnectionSettings(cloudId: Environment.GetEnvironmentVariable("ELASTICSEARCH_CLOUDID"), credentials: new ApiKeyAuthenticationCredentials(Environment.GetEnvironmentVariable("ELASTICSEARCH_APIKEY")))
            .DefaultIndex(defaultIndex)
            .EnableApiVersioningHeader()
            .ServerCertificateValidationCallback((o, certificate, chain, errors) => true);

        Client = new ElasticClient(connectionSettings);
    }

    public static ElasticClient GetClient()
    {
        return Client;
    }
}
