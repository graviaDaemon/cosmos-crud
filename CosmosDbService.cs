using System.Net;
using Microsoft.Azure.Cosmos;

namespace Cosmos.Crud;

public class CosmosDbService : ICosmosDbService
{
    private readonly Container _container;

    public CosmosDbService(
        CosmosClient dbClient,
        string databaseName,
        string containerName)
    {
        _container = dbClient.GetContainer(databaseName, containerName);
    }

    public async Task<IEnumerable<T?>?> GetItemsAsync<T>(string queryString) where T : BaseModel
    {
        // Start a query for which items to return
        FeedIterator<T> query = _container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
        IEnumerable<T>? results = null;
        while (query.HasMoreResults)
        {
            // get next query item
            FeedResponse<T> response = await query.ReadNextAsync();
            // set all queried items into Enumerable
            results = response.AsEnumerable();
        }

        return results;
    }

    public async Task<T?> GetItemAsync<T>(string id) where T : BaseModel
    {
        try
        {
            ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    public async Task<T?> AddItemAsync<T>(T item) where T : BaseModel
    {
        return await _container.CreateItemAsync(item, new(item.Id));
    }

    public async IAsyncEnumerable<ItemResponse<T>?> AddItemsAsync<T>(IEnumerable<T> items) where T : BaseModel
    {
        foreach (T item in items)
        {
           yield return await _container.CreateItemAsync(item, new(item.Id)) ?? null;
        }
    }

    public async Task<T?> UpdateItemAsync<T>(string id, T item) where T : BaseModel
    {
        return await _container.UpsertItemAsync(item, new(item.Id));
    }

    public async IAsyncEnumerable<ItemResponse<T>> UpdateItemsAsync<T>(string query, IEnumerable<T> items)
    where T : BaseModel
    {
        FeedIterator<T> request = _container.GetItemQueryIterator<T>(new QueryDefinition(query));
        foreach (T item in items)
        {
            if (request.HasMoreResults)
            {
                yield return await _container.UpsertItemAsync(item, new(item.Id));
            }
        }
    }

    public async Task<bool> DeleteItemAsync<T>(string id) where T : BaseModel
    {
        ItemResponse<T>? result = await _container.DeleteItemAsync<T>(id, new(id));
        return result.StatusCode is HttpStatusCode.Accepted or HttpStatusCode.OK;
    }

    public async Task<bool> DeleteItemsAsync<T>(string query) where T : BaseModel
    {
        FeedIterator<T> request = _container.GetItemQueryIterator<T>(new QueryDefinition(query));
        IEnumerable<T> toDelete = Array.Empty<T>().AsEnumerable();
        bool result = false;
        while (request.HasMoreResults)
        {
            FeedResponse<T> response = await request.ReadNextAsync();
            toDelete = response.AsEnumerable();
        }

        foreach (T item in toDelete)
        {
            ItemResponse<T>? res = await _container.DeleteItemAsync<T>(item.Id, new(item.Id));
            if (!result) return false;
            result = res.StatusCode is HttpStatusCode.Accepted or HttpStatusCode.OK;
        }

        return result;
    }
}