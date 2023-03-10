using ROCrates.Models;

namespace ROCrates.Tests;

public class TestComputerLanguage
{
  [Fact]
  public void TestDefaultType_Is_ComputerLanguage()
  {
    // Arrange
    var roCrate = new ROCrate();
    var computerLanguage = new ComputerLanguage(roCrate);
    const string expectedType = "ComputerLanguage";

    // Act
    var actualType = computerLanguage.GetProperty<string>("@type");

    // Assert
    Assert.NotNull(actualType);
    Assert.Equal(expectedType, actualType);
  }
}
