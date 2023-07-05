using System.Runtime.Serialization;

namespace ROCrates.Exceptions;

/// <summary>
/// An exception that can be thrown whenever an RO-crate cannot be read.
/// </summary>
public class CrateReadException : Exception
{
  public CrateReadException()
  {
  }

  public CrateReadException(string? message, Exception? innerException) : base(message, innerException)
  {
  }

  public CrateReadException(string? message) : base(message)
  {
  }

  protected CrateReadException(SerializationInfo info, StreamingContext context) : base(info, context)
  {
  }
}
