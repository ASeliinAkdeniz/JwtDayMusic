using AutoMapper;
using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Entites;

namespace JwtDayMusic.WebApi.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping() 
        {
            CreateMap<Artist, ResultArtistDto>().ReverseMap() ;
            CreateMap<Artist, CreateArtistDto>().ReverseMap() ;
        }
    }
}
