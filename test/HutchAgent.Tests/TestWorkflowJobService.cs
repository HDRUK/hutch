using AutoMapper;
using HutchAgent.Data;
using HutchAgent.MappingProfiles;
using HutchAgent.Models;
using HutchAgent.Services;
using Microsoft.EntityFrameworkCore;

namespace HutchAgent.Tests;

public class TestWorkflowJobService : IDisposable
{
  private readonly HutchAgentContext _db;
  private readonly IMapper _mapper;
  
  private readonly string _id1 = Guid.NewGuid().ToString();
  private readonly string _id2 = Guid.NewGuid().ToString();

  private readonly string _baseWorkingDirectory = "path/to";

  public TestWorkflowJobService()
  {
    _db = new HutchAgentContext(
      new DbContextOptionsBuilder<HutchAgentContext>()
        .UseInMemoryDatabase("TestWorkflowJobService")
        .Options);
    
    var config = new MapperConfiguration(cfg =>
    {
      cfg.AddProfile<WorkflowJobProfile>();
    });
    _mapper = config.CreateMapper();
  }

  [Fact]
  public async void Create_Creates_NewJob()
  {
    // Arrange
    var jobWorkDir = Path.Combine(_baseWorkingDirectory, _id1);
    var service = new WorkflowJobService(_db, _mapper);

    // Act
    var createdId = await service.Create(new() { Id = _id1, WorkingDirectory = jobWorkDir });

    // Assert

    // Verify the return value
    Assert.Equal(_id1, createdId);

    // Verify side effects (db state)
    var storedEntity = _db.Find<Data.Entities.WorkflowJob>(_id1);
    Assert.NotNull(storedEntity);
    Assert.Equal(jobWorkDir, storedEntity?.WorkingDirectory);
    Assert.Equal(_id1, storedEntity?.Id);
  }

  [Fact]
  public async void List_Returns_AllJobs()
  {
    // Arrange
    var storedValues = new List<Data.Entities.WorkflowJob>()
    {
      new() { Id = _id1, WorkingDirectory = "first/path" },
      new() { Id = _id2, WorkingDirectory = "second/path" }
    };
    _db.AddRange(storedValues);
    _db.SaveChanges();

    var expectedValues = new List<WorkflowJob>()
    {
      new() { Id = _id1, WorkingDirectory = "first/path" },
      new() { Id = _id2, WorkingDirectory = "second/path" }
    };

    var service = new WorkflowJobService(_db, _mapper);

    // Act
    var actualValues = await service.List();

    // Assert
    Assert.Equal(expectedValues.Count, actualValues.Count);

    for (var i = 0; i < expectedValues.Count; i++)
    {
      Assert.Equal(expectedValues[i].Id, actualValues[i].Id);
      Assert.Equal(expectedValues[i].WorkingDirectory, actualValues[i].WorkingDirectory);
    }
  }

  [Fact]
  public async void Get_Returns_JobWithSpecifiedId()
  {
    // Arrange
    var workDir = Path.Combine(_baseWorkingDirectory, _id1);

    var storedEntity = new Data.Entities.WorkflowJob
    {
      Id = _id1,
      WorkingDirectory = workDir,
      ExecutorRunId = _id2
    };
    _db.Add(storedEntity);
    _db.SaveChanges();

    var expectedObject = new WorkflowJob()
    {
      Id = _id1,
      WorkingDirectory = workDir,
      ExecutorRunId = _id2
    };

    var service = new WorkflowJobService(_db, _mapper);

    // Act
    var actualObject = await service.Get(storedEntity.Id);

    // Assert
    Assert.Equal(expectedObject.Id, actualObject.Id);
    Assert.Equal(expectedObject.WorkingDirectory, actualObject.WorkingDirectory);
    Assert.Equal(expectedObject.ExecutorRunId, actualObject.ExecutorRunId);
  }

  [Fact]
  public async void Get_Throws_KeyNotFoundExceptionWhenKeyNotFound()
  {
    // Arrange
    var keyThatShouldNotExist = _id1;

    var service = new WorkflowJobService(_db, _mapper);

    // Act
    var action = async () => await service.Get(keyThatShouldNotExist);

    // Assert
    await Assert.ThrowsAsync<KeyNotFoundException>(action);
  }

  [Fact]
  public async void Set_Updates_PropertiesAsSpecified()
  {
    // Arrange
    var workDir = Path.Combine(_baseWorkingDirectory, _id1);
    var originalEntity = new Data.Entities.WorkflowJob()
    {
      Id = _id1,
      WorkingDirectory = workDir,
    };
    _db.Add(originalEntity);
    _db.SaveChanges();

    var inputModel = new WorkflowJob()
    {
      Id = _id1,
      WorkingDirectory = workDir,
      ExecutorRunId = _id2,
    };

    var service = new WorkflowJobService(_db, _mapper);

    // Act
    var outputModel = await service.Set(inputModel);

    // Assert
    var updatedEntity = _db.Find<Data.Entities.WorkflowJob>(_id1);

    Assert.Equal(updatedEntity!.Id, outputModel.Id);
    Assert.Equal(updatedEntity!.WorkingDirectory, outputModel.WorkingDirectory);
    Assert.Equal(updatedEntity!.ExecutorRunId, outputModel.ExecutorRunId);
  }

  [Fact]
  public async void Set_Throws_KeyNotFoundExceptionWhenKeyNotFound()
  {
    // Arrange
    var newObject = new WorkflowJob
    {
      Id = _id1,
    };

    var service = new WorkflowJobService(_db, _mapper);

    // Act
    var action = async () => await service.Set(newObject);

    // Assert
    await Assert.ThrowsAsync<KeyNotFoundException>(action);
  }

  [Fact]
  public async void Delete_Removes_JobWithSpecifiedId()
  {
    // Arrange
    var originalEntity = new Data.Entities.WorkflowJob()
    {
      Id = _id1,
    };
    _db.Add(originalEntity);
    _db.SaveChanges();

    var service = new WorkflowJobService(_db, _mapper);

    // Act
    await service.Delete(originalEntity.Id);

    // Assert
    var shouldBeNull = _db.Find<Data.Entities.WorkflowJob>(_id1);
    Assert.Null(shouldBeNull);
  }

  [Fact]
  public async void Delete_Succeeds_WhenJobNotFound()
  {
    // Arrange
    var service = new WorkflowJobService(_db, _mapper);

    // Act
    var action = async () => await service.Delete("42");
    var exception = await Record.ExceptionAsync(action);

    // Assert
    Assert.Null(exception);
  }

  public void Dispose()
  {
    _db.Database.EnsureDeleted();
  }
}
