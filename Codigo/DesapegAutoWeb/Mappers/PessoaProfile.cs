using AutoMapper;
using Core;
using Core.DTO;
using DesapegAutoWeb.Models;

namespace DesapegAutoWeb.Mappers
{
    /// <summary>
    /// Configura o mapeamento entre os modelos da entidade Pessoa.
    /// </summary>
    public class PessoaProfile : Profile
    {
        public PessoaProfile()
        {
            // Mapeia da entidade para o DTO
            CreateMap<Pessoa, PessoaDTO>().ReverseMap();

            // Mapeia do ViewModel para a entidade
            CreateMap<PessoaViewModel, Pessoa>().ReverseMap();
        }
    }
}