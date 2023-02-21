using System.Text.Json;
using System.Text.Json.Nodes;

namespace ROCrates.Models;

/// <summary>
/// This class represents the base Entity used in an ROCrate.
/// </summary>
public class Entity
{
  public ROCrate RoCrate { get; set; }
  public string Identifier { get; set; } = Guid.NewGuid().ToString();

  public JsonObject Properties { get; set; } = new();
  
  public Entity(ROCrate crate, string? identifier = null, JsonObject? properties = null)
  {
    RoCrate = crate;
    if (identifier != null) Identifier = identifier;
    if (properties != null) Properties = properties;
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
    T? deserialisedProperty;
    Properties.TryGetPropertyValue(propertyName, out var property);
    deserialisedProperty = property.Deserialize<T>();
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
      // If a property already exists, remove first to avoid an exception, then add the new propterty
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
  /// Remove a property from the <c>Properties</c> field.
  /// </summary>
  /// <param name="propertyName"></param>
  public void DeleteProperty(string propertyName)
  {
    Properties.Remove(propertyName);
  }
}
