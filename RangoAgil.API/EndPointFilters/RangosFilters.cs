using Microsoft.AspNetCore.Mvc;
using RangoAgil.API.DbContexts;

namespace RangoAgil.API.EndPointFilters;

public class RangosFilters : IEndpointFilter //Implementando a interface IEndpointFilter que permite uma implementação
                                             //customizada de um filtro que pode ser aplicado a um endpoint.
{
    public readonly int _rangoInalteravel;

    public RangosFilters(int rangoInalteravel) //construtor para receber  o id que não pode ser alterado
    {
            _rangoInalteravel = rangoInalteravel; //Alimentando nossa propriedade com ele
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)//O método InvokeAsync é obrigatório é através dele
                                                                                                                     //que recebemos os paremetros do contexto e o próximo
                                                                                                                     //item no pipeline de execução.
    {
        int rangoId = 0;

        if (context.HttpContext.Request.Method == "PUT") rangoId = context.GetArgument<int>(2);//Aqui estamos dizendo que queremos os valor do item 2 que é um item da nossa
                                                                                               //classe RangoHandlers do metodo PutRangoAsync,sendo que o item 0 seria 
                                                                                               //rangoDbContext,1 mapper e 2 rangoId.
        else if (context.HttpContext.Request.Method == "DELETE") rangoId = context.GetArgument<int>(1);//Aqui estamos dizendo que queremos os valor do item 2 que é um item
                                                                                                       //da nossa classe RangoHandlers do metodo DeleteRangoAsync,sendo que o item
                                                                                                       //0 seria rangoDbContext 2 rangoId.
        else throw new NotSupportedException("Esse é um filtro somente para os verbos Put e Delete!");

        if (rangoId == _rangoInalteravel)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Status = 400,
                Title = "Modificação não permitida.",
                Detail = "Essa receita não pode ser modificada ou deletada 2"
            });
        }

        return await next(context);
    }
}


