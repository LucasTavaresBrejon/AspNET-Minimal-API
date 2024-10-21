using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndPointHandlers;

public static class IngredientesHandlers
{
    //ATENÇÃO, O ASYNC JUNTO COM AWAIT TRANSFORMAS NOSSOS ENDPOINTS EM ASSINCRONOS
    public static async Task<Results<NoContent, Ok<IEnumerable<IngredienteDTO>>>> GetIngredientesAsync(
    RangoDbContext rangoDbContext //Injetando o context
    , IMapper mapper //Injetando o mapeamento para mapear as entidades para DTOs
    , int rangoId)
    {
        var retornoIngredienteDTO = mapper.Map<IEnumerable<IngredienteDTO>>((
            await rangoDbContext.Rangos //Acessando a entidade ou tabela Rangos
            .Include(rango => rango.Ingredientes)// Include inclui no retorno os ingredientes  que pertencem a coleção(declarada na classe Rango) daquele rango.
            .FirstOrDefaultAsync(x => x.Id == rangoId))?.Ingredientes);//Busca o primeiro Rango(prato) cujo Id seja igual ao rangoId. 
                                                                       //caso encorntre ele retornara os ingredientes que ele incluiu na coleção anterior com o include
                                                                       //Se não encontrar, retornará null
        if (retornoIngredienteDTO == null || retornoIngredienteDTO.Count() <= 0)
        {
            return TypedResults.NoContent();
        }
        return TypedResults.Ok(retornoIngredienteDTO);
    }
}

