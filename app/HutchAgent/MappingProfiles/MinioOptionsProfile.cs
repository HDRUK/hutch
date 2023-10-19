using AutoMapper;
using HutchAgent.Models;

namespace HutchAgent.MappingProfiles;

public class MinioOptionsProfile : Profile 
{
  public MinioOptionsProfile()
  {
    CreateMap<FileStorageDetails, MinioOptionsProfile>();
  }
}
