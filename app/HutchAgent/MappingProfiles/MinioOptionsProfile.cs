using AutoMapper;
using HutchAgent.Config;
using HutchAgent.Models;

namespace HutchAgent.MappingProfiles;

public class MinioOptionsProfile : Profile 
{
  public MinioOptionsProfile()
  {
    CreateMap<FileStorageDetails?, MinioOptions>();
  }
}
