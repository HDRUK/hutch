using AutoMapper;

namespace HutchAgent.MappingProfiles;

public class WorkflowJobProfile : Profile
{
  public WorkflowJobProfile()
  {
    CreateMap<Data.Entities.WorkflowJob, Models.WorkflowJob>();
  }
}
