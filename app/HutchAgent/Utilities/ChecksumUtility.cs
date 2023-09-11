using System.Security.Cryptography;
using System.Text;

namespace HutchAgent.Utilities;

/// <summary>
/// Utility Class for working with checksums
/// </summary>
public static class ChecksumUtility
{
  /// <summary>
  /// Compute and return a SHA512 checksum of a <see cref="Stream"/>.
  /// </summary>
  /// <param name="stream">The input <see cref="Stream"/>.</param>
  /// <returns>The checksum for the input <see cref="Stream"/></returns>
  public static string ComputeSha512(Stream stream)
  {
    var sha512 = SHA512.Create();
    // Set the stream back to the start
    stream.Position = 0;
    var hash = sha512.ComputeHash(stream);

    // Convert byte array to a string
    var builder = new StringBuilder();
    foreach (var h in hash)
    {
      builder.Append(h.ToString("x2"));
    }

    return builder.ToString();
  }
}
