using System.Text.Json;
using System.Text.Json.Nodes;

namespace ROCrates.Models;

/// <summary>
/// This class represents the base Entity used in an ROCrate.
/// </summary>
public class Entity
{
  private protected string DefaultType = "Thing";
  public ROCrate RoCrate { get; set; }
  public string Identifier { get; set; } = Guid.NewGuid().ToString();

  public JsonObject Properties { get; set; }

  public Entity(ROCrate crate, string? identifier = null, JsonObject? properties = null)
  {
    RoCrate = crate;
    if (identifier is not null) Identifier = identifier;
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
  }

  /// <summary>
  /// Get the canonical ID of the crate that the entity is in.
  /// </summary>
  /// <returns></returns>
  public string GetCanonicalId()
  {
    return RoCrate.ResolveId(Identifier);
  }

  /// <summary>
  /// Retrieve a property from <c>Properties</c> deserialsed as type <c>T</c>.
  /// </summary>
  /// <param name="propertyName">The name of the property to retrieve.</param>
  /// <typeparam name="T">The type to deserialise the property into.</typeparam>
  /// <returns>
  /// <para>
  /// The property as type <c>T</c> if the property exists, or <c>null</c> if the property does no exist on the entity.
  /// </para>T
  /// </returns>
  public T? GetProperty<T>(string propertyName)
  {
    Properties.TryGetPropertyValue(propertyName, out var property);
    var deserialisedProperty = property.Deserialize<T>();
    return deserialisedProperty;
  }

  /// <summary>
  /// Sersialise a property into the <c>Properties</c> field. This will update a property if it already exists.
  /// </summary>
  /// <param name="propertyName"></param>
  /// <param name="property"></param>
  /// <typeparam name="T"></typeparam>
  public void SetProperty<T>(string propertyName, T property)
  {
    var serialisedProperty = JsonSerializer.SerializeToNode(property);
    if (serialisedProperty != null)
    {
      // If a property already exists, remove first to avoid an exception, then add the new property
      if (Properties.Remove(propertyName))
      {
        Properties.Add(propertyName, serialisedProperty);
      }
      else
      {
        Properties.Add(propertyName, serialisedProperty);
      }
    }
  }

  /// <summary>
  /// Append a value to the entity's property.
  /// </summary>
  /// <param name="key">The element to append the value to.</param>
  /// <param name="value">The value to be appended.</param>
  /// <typeparam name="T">The type of <c>Entity</c> to be appended.</typeparam>
  /// <exception cref="Exception">
  /// Thrown when attempting to append to reserved key (those starting with '@').
  /// </exception>
  /// <exception cref="NullReferenceException">Thrown when <c>value</c> is <c>null</c>.</exception>
  /// <example>
  /// <code>
  /// var roCrate = new ROCrate();
  /// var rootDataset = new RootDataset(roCrate);
  /// var person = new Person(roCrate, identifier: "Alice");
  /// rootDataset.AppendTo("author", person);
  /// </code>
  /// </example>
  public void AppendTo<T>(string key, T value) where T : Entity
  {
    if (key.StartsWith('@')) throw new Exception($"Cannot append to {key}");
    if (value is null) throw new NullReferenceException("value cannot be null.");
    
    var newItem = new Part { Identifier = value.GetCanonicalId() };
    var itemList = new List<Part>{ newItem };
    
    if (Properties.TryGetPropertyValue(key, out var propsJson))
    {
      var currentItems = propsJson.Deserialize<List<Part>>() ?? new List<Part>();
      if (currentItems.Count > 0) itemList.InsertRange(0, currentItems);
    }
    
    SetProperty(key, itemList);
  }

  /// <summary>
  /// Remove a property from the <c>Properties</c> field.
  /// </summary>
  /// <param name="propertyName"></param>
  public void DeleteProperty(string propertyName)
  {
    Properties.Remove(propertyName);
  }

  private protected JsonObject _empty()
  {
    var emptyJsonString = new Dictionary<string, string>
    {
      { "@id", Identifier },
      { "@type", DefaultType }
    };
    var emptyObject = JsonSerializer.SerializeToNode(emptyJsonString).AsObject();
    return emptyObject;
  }

  private protected virtual string _formatIdentifier(string identifier)
  {
    return identifier;
  }

  private protected void _unpackProperties(JsonObject props)
  {
    using var propsEnumerator = props.GetEnumerator();
    while (propsEnumerator.MoveNext())
    {
      var (key, value) = propsEnumerator.Current;
      if (value != null) SetProperty(key, value);
    }
  }
}
