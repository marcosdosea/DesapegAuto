using AutoMapper;
using Core;
using DesapegAutoWeb.Models;

namespace DesapegAutoWeb.Mappers
{
    public class AnuncioProfile : Profile
    {
        public AnuncioProfile()
        {
            CreateMap<AnuncioViewModel, Anuncio>().ReverseMap();
        }
    }
}
