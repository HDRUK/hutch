using System.Security.Cryptography;
using System.Text;

namespace HutchAgent.Services;

public class Sha512ChecksumService
{
  private readonly SHA512 _sha512 = SHA512.Create();

  public string ComputeSha512(Stream stream)
  {
    stream.Position = 0;
    var hash = _sha512.ComputeHash(stream);
    var builder = new StringBuilder();
    foreach (var h in hash)
    {
      builder.Append(h.ToString("x2"));
    }

    return builder.ToString();
  }
}
