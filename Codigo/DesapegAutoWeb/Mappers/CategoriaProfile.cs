using AutoMapper;
using Core;
using DesapegAutoWeb.Models;

namespace DesapegAutoWeb.Mappers
{
    public class CategoriaProfile : Profile
    {
        public CategoriaProfile()
        {
            CreateMap<CategoriaViewModel, Categoria>().ReverseMap();
        }
    }
}
