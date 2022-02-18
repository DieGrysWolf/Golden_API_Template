using API.Entities.Models.DTOs.Requests;
using AutoMapper;

namespace API.MapperProfiles
{
    public class UserMapProfile : Profile
    {
        public UserMapProfile()
        {
            // CreateMap<TSource, TDestination>();
            CreateMap<UserInfoRequestDTO, UserInfoModel>()
                .ForMember(
                destination => destination.FirstName,
                from => from.MapFrom(x => $"{x.FirstName}"))
                .ForMember(
                destination => destination.LastName,
                from => from.MapFrom(x => $"{x.LastName}"))
                .ForMember(
                destination => destination.EmailAddress,
                from => from.MapFrom(x => $"{x.EmailAddress}"))
                .ForMember(
                destination => destination.PhoneNumber,
                from => from.MapFrom(x => $"{x.PhoneNumber}"))
                .ForMember(
                destination => destination.Address,
                from => from.MapFrom(x => $"{x.Address}"))
                .ForMember(
                destination => destination.DateOfBirth,
                from => from.MapFrom(x => x.DateOfBirth));
        }
    }
}
