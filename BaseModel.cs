using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Cosmos.Crud;

/// <summary>
/// The base model to extend all other models to work with the database. This model is using Newtonsoft.Json to set
/// the json properties for Http request bodies.
/// The ID property should be a string from a GUID, and it will check for the GUID pattern. If you wish to remove this check,
/// just override the id property with your own validation.
/// </summary>
public class BaseModel
{
    private string _id = string.Empty;
    private readonly Regex _rg = new(@"([\d\w]{8})-([\d\w]{4})-([\d\w]{4})-([\d\w]{4})-([\d\w]{12})");
    
    public override bool Equals(object? obj)
    {
        if (obj is not BaseModel x) return false;
        if (ReferenceEquals(x, this)) return true;
        if (ReferenceEquals(x, null)) return false;

        if (x.Id != Id) return false;
        if (x.CreatedAt != CreatedAt) return false;
        if (x.UpdatedAt != UpdatedAt) return false;
        return true;
    }

    public override int GetHashCode()
    {
        BaseModel obj = this;
        unchecked
        {
            int hashCode = obj.Id.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.CreatedAt.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.UpdatedAt.GetHashCode();
            return hashCode;
        }
    }

    [JsonProperty("id")]
    public virtual string Id
    {
        get => _id;
        set
        {
            if (!_rg.IsMatch(value)) throw new($"'{value}' is not a valid GUID pattern");
            _id = value;
        }
    }

    [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }
    [JsonProperty("updated_at")] public DateTime UpdatedAt { get; set; }
}