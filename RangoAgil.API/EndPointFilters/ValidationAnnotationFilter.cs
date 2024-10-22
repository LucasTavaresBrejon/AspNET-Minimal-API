
using MiniValidation;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndPointFilters;

public class ValidationAnnotationFilter : IEndpointFilter
{

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var rangoForCreationDTO = context.GetArgument<RangoForCreationDTO>(2);
        if (!MiniValidator.TryValidate(rangoForCreationDTO, out var validationErrors)) //aqui ele verifica se o miniValidator vai ter sucesso ao validar nosso argumento
                                                                                       //do contexto, caso de certo ele vai para o proximo nó da corrente, caso contrario
                                                                                       //ira devolver a mensagem de erro.
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        return await next(context);
    }
}
