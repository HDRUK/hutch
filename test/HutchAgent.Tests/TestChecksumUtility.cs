using System.Text;
using HutchAgent.Utilities;

namespace HutchAgent.Tests;

public class TestChecksumUtility
{
  [Fact]
  public void ComputeSha512_Returns_CorrectChecksum()
  {
    // Arrange
    const string input = "hello world";
    const string expected =
      "309ecc489c12d6eb4cc40f50c902f2b4d0ed77ee511a7c7a9bcd3ca86d4cd86f989dd35bc5ff499670da34255b45b0cfd830e81f605dcf7dc5542e93ae9cd76f";
    var stream = new MemoryStream();
    var writer = new BinaryWriter(stream, Encoding.UTF8);
    writer.Write(input.ToArray());

    // Act
    var result = ChecksumUtility.ComputeSha512(stream);

    // Assert
    Assert.Equal(expected, result);
  }
}
