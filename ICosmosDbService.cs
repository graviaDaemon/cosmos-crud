using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Cosmos.Crud;

/// <summary>
/// The interface determining all CRUD interactions with a Cosmos DB. This is currently only available to a NoSQL database
/// This Package assumes your "id" property is the same as your partition key.
/// Be sure to extend the BaseModel so that the return types will be as expected.
/// </summary>
public interface ICosmosDbService
{
    /// <summary>
    /// The method to retrieve a complete list of items based on a query string.
    /// </summary>
    /// <param name="queryString">This is a regular string representing the query in the current container</param>
    /// <returns>IEnumerable of BaseModel extension, if no items were found in the query, null will be the return value.</returns>
    Task<IEnumerable<T?>?> GetItemsAsync<T>(string queryString) where T : BaseModel;
    /// <summary>
    /// The method to retrieve a single item from the database based on the document's ID
    /// </summary>
    /// <param name="id">The ID of the document you're trying to fetch.</param>
    /// <returns>The Model populated, if the item was not found, the return value is null</returns>
    Task<T?> GetItemAsync<T>(string id) where T : BaseModel;
    /// <summary>
    /// The method to add an item to the container's records.
    /// </summary>
    /// <param name="item">The item you wish to add to the documents. Extending the BaseModel</param>
    /// <returns>The item you added to the database, or if not successful, returns a null</returns>
    Task<T?> AddItemAsync<T>(T item) where T : BaseModel;

    /// <summary>
    /// A method to add multiple items at once. If an IEnumerable of said items is provided, it will add them one by one to the database.
    /// </summary>
    /// <param name="items">The IEnumerable of Items based on BaseModel</param>
    /// <returns>An IEnumerable of all items that were successfully added. If any item is not successfully added, it will return a null value in this list.</returns>
    IAsyncEnumerable<ItemResponse<T>?> AddItemsAsync<T>(IEnumerable<T> items) where T : BaseModel;
    /// <summary>
    /// Updating a single item by ID in the database
    /// </summary>
    /// <param name="id">The ID of the item you wish to update</param>
    /// <param name="item">the updated version of that item</param>
    /// <returns>The updated item, or if the item was not successfully updated, returns null</returns>
    Task<T?> UpdateItemAsync<T>(string id, T item) where T : BaseModel;
    /// <summary>
    /// Updating multiple items at once, by query to filter which items you wish to update.
    /// </summary>
    /// <param name="query">The string query to filter which items to update</param>
    /// <param name="items">The actual items to update</param>
    /// <returns>An IEnumerable of the updated items, for any unsuccessful ones a null value is returned</returns>
    IAsyncEnumerable<ItemResponse<T>> UpdateItemsAsync<T>(string query, IEnumerable<T> items) where T : BaseModel;
    /// <summary>
    /// The deletion of a single item by ID
    /// </summary>
    /// <param name="id">The "id" property of the item you wish to delete</param>
    /// <returns>a boolean value whether or not the deletion was successful</returns>
    Task<bool> DeleteItemAsync<T>(string id) where T : BaseModel;
    /// <summary>
    /// The deletion of multiple items by query
    /// </summary>
    /// <param name="query">the query string to search and filter which items to delete</param>
    /// <returns>A single bool value whether or not all items were deleted successfully</returns>
    Task<bool> DeleteItemsAsync<T>(string query) where T : BaseModel;
}