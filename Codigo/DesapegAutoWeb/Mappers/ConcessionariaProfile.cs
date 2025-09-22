using AutoMapper;
using Core;
using DesapegAutoWeb.Models;

namespace DesapegAutoWeb.Mappers
{
    public class ConcessionariaProfile : Profile
    {
        public ConcessionariaProfile()
        {
            CreateMap<Concessionaria, ConcessionariaViewModel>()
                .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco))
                .ReverseMap()
                .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco));
        }
    }
}
