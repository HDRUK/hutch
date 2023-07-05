using System.Runtime.Serialization;

namespace ROCrates.Exceptions;

/// <summary>
/// <para>An error that should be thrown when something is wrong with an RO-Crate's metadata.</para>
/// <para>It should be used when the deserialised metadata JSON is invalid,
/// not when there is an issue reading the <c>ro-crate-metadata.json</c> file.
/// </para>
/// </summary>
public class MetadataException : Exception
{
  public MetadataException()
  {
  }

  protected MetadataException(SerializationInfo info, StreamingContext context) : base(info, context)
  {
  }

  public MetadataException(string? message) : base(message)
  {
  }

  public MetadataException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
