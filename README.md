# cosmos-crud

This project aims to create a simple CRUD interaction with the basic Cosmos SDK. I've had to set this simple thing up a couple of times for different models now,
and I figured I could make this into a generic library and use it that way. At least that's what I hope.

# Usage
Call the static class `CosmosCrud`'s `SpinUp()` method, and provide the neccecary parameters.
Parameters:
- databaseName (`string`) this is the name of your Azure CosmosDb resource's database
- containerName (`string`) this is the container within the datbaase
- account (`string`) this is the connectionstring provided by the Azure Resource
- key (`string`) this is the key provided in the same screen as your connectionstring
- partitionKey Default: "/id" (`string`) the partition key name. By default this will be the same as your document's ID.

After initializing the CosmosCrud the SpinUp method will return an instance of CosmosDbService, which contains all the interactions.
Extend the `BaseModel` class and either leave the "id" property default, which is a stringified GUID, or override the "id" property with your own identifier.

# Methods
- GetItemAsync > This method is to retrieve a single item from your CosmosDb and will return the item as the Model you extended from BaseModel
- GetItemsAsync > This method is to retrieve an IEnumerable of items from your CosmosDb at once, using a query string to filter which to look for. Returns an IEnumerable of the Model you extended from BaseModel
- AddItemAsync > This method is to create a single item in your CosmosDb. Returns the created item as the Model you extended from BaseModel
- AddItemsAsync > This method iterates over an IEnumerable of your Model and creates multiple items at once. Returns an IEnumerable of items as the Model you extended from BaseModel
- UpdateItemAsync > This method updates a single item by "id" in the container. Returns the updated item as the Model you extended from BaseModel.
- UpdateItemsAsync > This method updates multiple items in the database at once, using a querystring to filter which item to update. Returns an IEnumerable of items if the Model you extended from BaseModel
- DeleteItemAsync > This method deletes a single item from the container, looking for an "id" property. Returns a boolean wether or not it was successful
- DeleteItemsAsync > This method deletes multiple items from the container, using a queryString to filter the items that needed deleting. Returns a single boolean value for success.

# Example
```cs
public class MyModel : BaseModel
{
  // BaseModel has the following properties already prepared:
  // [JsonProperty("id")] public virtual string Id {get;set;}
  // [JsonProperty("created_at")] public DateTime CreatedAt {get;set;}
  // [JsonProperty("updated_at")] public DateTime UpdatedAt {get;set;}
  [JsonProperty("my_property")]public string MyProperty {get;set;}
}
public static class Program
{
  public static void Main(string[] args)
  {
    var service = CosmosCrud.SpinUp("myDatabase", "myContainer", "super.iffy.connection/string", "someHashKeyProvidedByCosmos");

    MyModel createdItem = await service.AddItemAsync(new MyModel() { Id = "some-identifier-string" /*MUST be GUID type by default*/, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, MyProperty = "some data in here" });
    MyModel retrievedItem = await service.GetItemAsync("some-identifier-string");
    MyModel updatedItem = await service.UpdateItemAsync("some-identifier-string", retrievedItem);
    bool deletedItem = await service.DeleteItemAsync("some-identifier-string");
  }
}
```
