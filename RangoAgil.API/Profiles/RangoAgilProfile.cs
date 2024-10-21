using AutoMapper;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.Profiles;

public class RangoAgilProfile : Profile //Necessario fazer a herança para o automapper saber onde deve olhar o mapeamento que vai ser feito
{
    public RangoAgilProfile() // Construtor necessario para fazer o mapeamento do meu dominio Rango para seu DTO
    {
        //CreateMap(de onde, para onde).e faça o contrario também
        CreateMap<Rango,RangoDTO>().ReverseMap();
        CreateMap<Rango, RangoForCreationDTO>().ReverseMap();
        CreateMap<Rango, RangoForUpdateDTO>().ReverseMap();
        CreateMap<Ingrediente, IngredienteDTO>()
            .ForMember( //Utilizando o forMember eu digo que meu RangoId do ingredienteDTO receberá o primeiro Id que retornar da lista de Rango
                        //que esta dentro da classe Ingrediente, com isso o EF já mapeia e distribui o restante das id's com os itens da lista de forma sequencial
            d => d.RangoId
            , o => o.MapFrom(s => s.Rangos.First().Id));
    }
}

