/// <summary>
/// Clase manejador para la comunicación con los servicios de nube.
/// </summary>
public static class manager
{
    /// <summary>
    /// Constructor estático, agregar la configuración necesaria para los proveedores de nube.
    /// </summary>
    static manager()
    {
        //Configurar algún provider si es requerido.
    }

    /// <summary>
    /// Procesa los registros hacia la nube del proveedor proporcionado
    /// </summary>
    /// <param name="items">Lista de elementos a procesar</param>
    /// <param name="type">Tipo de proveedor de nube proporcionado</param>
    /// <returns>Tarea de la ejecución</returns>
    public static async Task ProcessToCloud(IEnumerable<ItemProccess> items, CloudDatabaseType type)
    {
        switch (type)
        {
            case CloudDatabaseType.CosmosAzure:
            var cosmosContainer = CosmosBDClientProvider.GetContainer();
            await PersistItemsAsync(items, cosmosContainer);
            break;
            case CloudDatabaseType.ElasticCloud:
            var elasticClient = ElasticsearchClientProvider.GetClient();
            var indexResponse = await elasticClient.IndexManyAsync(items, elasticClient.ConnectionSettings.DefaultIndex);
            if (!indexResponse.IsValid)
            {
                Console.WriteLine($"Error al indexar el grupo de documentos : {indexResponse.OriginalException}");
                return;
            }
            break;
        }
    }

    /// <summary>
    /// Envía la información hacia el provedoor de azure
    /// </summary>
    /// <param name="itemList">Lista de elementos</param>
    /// <param name="container">Contenedor de azure</param>
    /// <returns>Tarea de la ejecución</returns>
    /// <exception cref="Exception">Excepción enviada</exception>
    private static async Task PersistItemsAsync(IEnumerable<ItemProccess> itemList, Container container)
    {
      var tasks = itemList.Select(async singleItem => 
       {
          var response = await container.UpsertItemAsync<ItemProccess>(singleItem, new PartitionKey(singleItem.PartitionKey));
          if (response.StatusCode == System.Net.HttpStatusCode.Created)
          {
            // Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", response.Resource.Id, response.RequestCharge);
          }
          else if (response.StatusCode == System.Net.HttpStatusCode.OK)
          {
           // Console.WriteLine("Updated item in database with id: {0} Operation consumed {1} RUs.\n", response.Resource.Id, response.RequestCharge);
          }
          else
         {
            throw new Exception($"Error al procesar el elemento con ID {singleItem.Id}. StatusCode: {response.StatusCode}");
         }
       });
        await Task.WhenAll(tasks);
    }
}