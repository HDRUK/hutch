using HutchAgent.Data;
using HutchAgent.Data.Entities;
using HutchAgent.Services;
using HutchAgent.Tests.AsyncHandlers;
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

  [Fact]
  public async void List_Returns_AllJobs()
  {
    // Arrange
    var expectedValues = new List<WfexsJob>()
    {
      new() { UnpackedPath = "first/path" },
      new() { UnpackedPath = "second/path" },
      new() { UnpackedPath = "third/path" }
    }.AsQueryable();

    var mockSet = new Mock<DbSet<WfexsJob>>();
    mockSet.As<IAsyncEnumerable<WfexsJob>>()
      .Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
      .Returns(new AsyncEnumerator<WfexsJob>(expectedValues.GetEnumerator()));

    mockSet.As<IQueryable<WfexsJob>>()
      .Setup(m => m.Provider)
      .Returns(new AsyncQueryProvider<WfexsJob>(expectedValues.Provider));

    mockSet.As<IQueryable<WfexsJob>>().Setup(m => m.Expression).Returns(expectedValues.Expression);
    mockSet.As<IQueryable<WfexsJob>>().Setup(m => m.ElementType).Returns(expectedValues.ElementType);
    mockSet.As<IQueryable<WfexsJob>>().Setup(m => m.GetEnumerator()).Returns(expectedValues.GetEnumerator());

    var mockContext = new Mock<HutchAgentContext>();
    mockContext.Setup(m => m.WfexsJobs).Returns(mockSet.Object);

    var service = new WfexsJobService(mockContext.Object);

    // Act
    var actualValues = await service.List();

    // Assert
    Assert.Equal("first/path", actualValues[0].UnpackedPath);
    Assert.Equal("second/path", actualValues[1].UnpackedPath);
    Assert.Equal("third/path", actualValues[2].UnpackedPath);
  }

  [Fact]
  public async void Get_Returns_JobWithSpecifiedId()
  {
    // Arrange
    var expectedObject = new WfexsJob()
    {
      UnpackedPath = "path/to",
      WfexsRunId = Guid.NewGuid().ToString(),
      RunFinished = true
    };

    var mockSet = new Mock<DbSet<WfexsJob>>();
    mockSet.Setup(m => m.FindAsync(expectedObject.Id)).Returns(ValueTask.FromResult(expectedObject));
    var mockContext = new Mock<HutchAgentContext>();
    mockContext.Setup(m => m.WfexsJobs).Returns(mockSet.Object);

    var service = new WfexsJobService(mockContext.Object);

    // Act
    var actualObject = await service.Get(expectedObject.Id);

    // Assert
    Assert.Equal(expectedObject.Id, actualObject.Id);
  }

  [Fact]
  public async void Get_Throws_KeyNotFoundExceptionWhenKeyNotFound()
  {
    // Arrange
    var objectThatShouldExist = new WfexsJob()
    {
      Id = 1,
      UnpackedPath = "path/to",
      WfexsRunId = Guid.NewGuid().ToString(),
      RunFinished = true
    };
    var keyThatShouldNotExist = 2;

    var mockSet = new Mock<DbSet<WfexsJob>>();
    mockSet.Setup(m => m.FindAsync(objectThatShouldExist.Id)).Returns(ValueTask.FromResult(objectThatShouldExist));
    var mockContext = new Mock<HutchAgentContext>();
    mockContext.Setup(m => m.WfexsJobs).Returns(mockSet.Object);

    var service = new WfexsJobService(mockContext.Object);

    // Act
    var action = async () => await service.Get(keyThatShouldNotExist);

    // Assert
    await Assert.ThrowsAsync<KeyNotFoundException>(action);
  }

  [Fact]
  public async void Set_Updates_PropertiesAsSpecified()
  {
    // Arrange
    var originalObject = new WfexsJob()
    {
      Id = 1,
      UnpackedPath = "path/to",
      WfexsRunId = Guid.NewGuid().ToString(),
      RunFinished = true
    };

    var newObject = new WfexsJob()
    {
      Id = originalObject.Id,
      UnpackedPath = "path/to/elsewhere",
      WfexsRunId = Guid.NewGuid().ToString(),
      RunFinished = false
    };

    var mockSet = new Mock<DbSet<WfexsJob>>();
    mockSet.Setup(m => m.FindAsync(originalObject.Id)).Returns(ValueTask.FromResult(originalObject));
    var mockContext = new Mock<HutchAgentContext>();
    mockContext.Setup(m => m.WfexsJobs).Returns(mockSet.Object);

    var service = new WfexsJobService(mockContext.Object);

    // Act
    var updatedObject = await service.Set(newObject);

    // Assert
    Assert.Equal(newObject.Id, updatedObject.Id);
    Assert.Equal(newObject.UnpackedPath, updatedObject.UnpackedPath);
    Assert.Equal(newObject.WfexsRunId, updatedObject.WfexsRunId);
    Assert.Equal(newObject.RunFinished, updatedObject.RunFinished);
  }

  [Fact]
  public async void Set_Throws_KeyNotFoundExceptionWhenKeyNotFound()
  {
    // Arrange
    var originalObject = new WfexsJob()
    {
      Id = 1,
      UnpackedPath = "path/to",
      WfexsRunId = Guid.NewGuid().ToString(),
      RunFinished = true
    };

    var newObject = new WfexsJob()
    {
      Id = 2,
      UnpackedPath = "path/to/elsewhere",
      WfexsRunId = Guid.NewGuid().ToString(),
      RunFinished = false
    };

    var mockSet = new Mock<DbSet<WfexsJob>>();
    mockSet.Setup(m => m.FindAsync(originalObject.Id)).Returns(ValueTask.FromResult(originalObject));
    var mockContext = new Mock<HutchAgentContext>();
    mockContext.Setup(m => m.WfexsJobs).Returns(mockSet.Object);

    var service = new WfexsJobService(mockContext.Object);

    // Act
    var action = async () => await service.Set(newObject);

    // Assert
    await Assert.ThrowsAsync<KeyNotFoundException>(action);
  }
}
