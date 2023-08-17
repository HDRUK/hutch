using System.Security.Cryptography;
using System.Text;

namespace HutchAgent.Services;

/// <summary>
/// Service for computing SHA512 checksums of <see cref="Stream"/> objects.
/// </summary>
public class Sha512ChecksumService
{
  private readonly SHA512 _sha512 = SHA512.Create();

  /// <summary>
  /// Compute and return a SHA512 checksum of a <see cref="Stream"/>.
  /// </summary>
  /// <param name="stream">The input <see cref="Stream"/>.</param>
  /// <returns>The checksum for the input <see cref="Stream"/></returns>
  public string ComputeSha512(Stream stream)
  {
    // Set the stream back to the start
    stream.Position = 0;
    var hash = _sha512.ComputeHash(stream);

    // Convert byte array to a string
    var builder = new StringBuilder();
    foreach (var h in hash)
    {
      builder.Append(h.ToString("x2"));
    }

    return builder.ToString();
  }
}
