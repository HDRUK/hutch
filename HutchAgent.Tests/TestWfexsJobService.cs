using HutchAgent.Data;
using HutchAgent.Data.Entities;
using HutchAgent.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HutchAgent.Tests;

public class TestWfexsJobService
{
  [Fact]
  public async void WfexsJobService_Creates_NewJob()
  {
    // Arrange
    var expectedUnpackedPath = Path.Combine("path", "to");
    var expectedRunId = Guid.NewGuid().ToString();
    var newObject = new WfexsJob()
    {
      UnpackedPath = expectedUnpackedPath, WfexsRunId = expectedRunId
    };

    var mockSet = new Mock<DbSet<WfexsJob>>();
    var mockContext = new Mock<HutchAgentContext>();
    mockContext.Setup(m => m.WfexsJobs).Returns(mockSet.Object);
    var service = new WfexsJobService(mockContext.Object);

    // Act
    var createdObject = await service.Create(newObject);

    // Assert
    Assert.Equal(expectedUnpackedPath, createdObject.UnpackedPath);
    Assert.Equal(expectedRunId, createdObject.WfexsRunId);

    mockContext.Verify(m => m.AddAsync(It.IsAny<WfexsJob>(), CancellationToken.None), Times.Once);
    mockContext.Verify(m => m.SaveChangesAsync(CancellationToken.None), Times.Once);
  }
}
