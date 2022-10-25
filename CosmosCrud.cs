using CosmosCrud.Base;
using Microsoft.Azure.Cosmos;

namespace Host.Cosmos.Crud;

/// <summary>
/// Use this static class to startup your CosmosDbService, and use the return value to use any crud-call
/// </summary>
public static class CosmosCrud
{
    /// <summary>
    /// CosmosCrud.SpinUp to start up the Cosmos DB Service to use for any future Crud Calls
    /// </summary>
    /// <param name="databaseName"></param>
    /// <param name="containerName"></param>
    /// <param name="account"></param>
    /// <param name="key"></param>
    /// <param name="partitionKey"></param>
    /// <returns>CosmosDbService</returns>s
    public static async Task<CosmosDbService> SpinUp(
        string databaseName, 
        string containerName, 
        string account, 
        string key,
        string partitionKey = "/id")
    {
        CosmosClient client = new(account, key);
        CosmosDbService service = new(client, databaseName, containerName);
        DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        await database.Database.CreateContainerIfNotExistsAsync(containerName, partitionKey);

        return service;
    }
}