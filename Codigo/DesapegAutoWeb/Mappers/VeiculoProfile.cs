using AutoMapper;
using Core;
using DesapegAutoWeb.Models;
namespace DesapegAutoWeb.Mappers

{
    public class VeiculoProfile : Profile
    {
        public VeiculoProfile()
        {
            CreateMap<VeiculoViewModel, Veiculo>().ReverseMap();
        }

    }
}
