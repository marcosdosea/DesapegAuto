using AutoMapper;
using Core;
using DesapegAutoWeb.Models;

namespace DesapegAutoWeb.Mappers
{
    public class VersaoProfile : Profile
    {
        public VersaoProfile()
        {
            CreateMap<VersaoViewModel, Versao>().ReverseMap();
        }
    }
}