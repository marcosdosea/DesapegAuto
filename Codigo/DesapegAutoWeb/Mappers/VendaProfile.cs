using AutoMapper;
using Core;
using DesapegAutoWeb.Models;

namespace DesapegAutoWeb.Mappers
{
    public class VendaProfile : Profile
    {
        public VendaProfile()
        {
            CreateMap<VendaViewModel, Venda>().ReverseMap();
        }
    }
}