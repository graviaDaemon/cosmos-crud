using System.Net;
using Host.Cosmos.Crud;
using Microsoft.Azure.Cosmos;

namespace CosmosCrud.Base;

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

    public async Task<IEnumerable<BaseModel?>?> GetItemsAsync(string queryString)
    {
        FeedIterator<BaseModel> query = _container.GetItemQueryIterator<BaseModel>(new QueryDefinition(queryString));
        IEnumerable<BaseModel>? results = null;
        while (query.HasMoreResults)
        {
            FeedResponse<BaseModel> response = await query.ReadNextAsync();
            results = response.AsEnumerable();
        }

        return results;
    }

    public async Task<BaseModel?> GetItemAsync(string id)
    {
        try
        {
            ItemResponse<BaseModel> response = await _container.ReadItemAsync<BaseModel>(id, new(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    public async Task<BaseModel> AddItemAsync(BaseModel item)
    {
        return await _container.CreateItemAsync(item, new(item.Id));
    }

    public async IAsyncEnumerable<ItemResponse<BaseModel>?> AddItemsAsync(IEnumerable<BaseModel> items)
    {
        foreach (var item in items)
        {
           yield return await _container.CreateItemAsync(item, new(item.Id)) ?? null;
        }
    }

    public async Task<BaseModel?> UpdateItemAsync(string id, BaseModel item)
    {
        return await _container.UpsertItemAsync(item, new(item.Id));
    }

    public async IAsyncEnumerable<ItemResponse<BaseModel>> UpdateItemsAsync(string query, IEnumerable<BaseModel> items)
    {
        FeedIterator<BaseModel> request = _container.GetItemQueryIterator<BaseModel>(new QueryDefinition(query));
        foreach (BaseModel item in items)
        {
            if (request.HasMoreResults)
            {
                yield return await _container.UpsertItemAsync(item, new(item.Id));
            }
        }
    }

    public async Task<bool> DeleteItemAsync(string id)
    {
        ItemResponse<BaseModel>? result = await _container.DeleteItemAsync<BaseModel>(id, new(id));
        return result.StatusCode == HttpStatusCode.Accepted || result.StatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> DeleteItemsAsync(string query)
    {
        FeedIterator<BaseModel> request = _container.GetItemQueryIterator<BaseModel>(new QueryDefinition(query));
        IEnumerable<BaseModel> toDelete = Array.Empty<BaseModel>().AsEnumerable();
        bool result = false;
        while (request.HasMoreResults)
        {
            FeedResponse<BaseModel> response = await request.ReadNextAsync();
            toDelete = response.AsEnumerable();
        }

        foreach (BaseModel item in toDelete)
        {
            ItemResponse<BaseModel>? res = await _container.DeleteItemAsync<BaseModel>(item.Id, new(item.Id));
            if (!result) return false;
            result = res.StatusCode is HttpStatusCode.Accepted or HttpStatusCode.OK;
        }

        return result;
    }
}