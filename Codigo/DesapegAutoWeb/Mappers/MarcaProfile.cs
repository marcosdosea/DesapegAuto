using AutoMapper;
using Core;
using DesapegAutoWeb.Models;

namespace DesapegAutoWeb.Mappers
{
    public class MarcaProfile : Profile
    {
       public MarcaProfile()
       {
           CreateMap<MarcaViewModel, Marca>().ReverseMap();
        }
    }
}
