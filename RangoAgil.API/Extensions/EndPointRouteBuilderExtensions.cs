using RangoAgil.API.EndPointFilters;
using RangoAgil.API.EndPointHandlers;
using System.Runtime.CompilerServices;

namespace RangoAgil.API.Extensions;

public static class EndPointRouteBuilderExtensions
{
    public static void RegisterRangosEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        #region GET

        endpointRouteBuilder.MapGet("/", () => "Hello World!");

        //ATENÇÃO, O ASYNC JUNTO COM AWAIT TRANSFORMAS NOSSOS ENDPOINTS EM ASSINCRONOS
        var rangosEndPoint = endpointRouteBuilder.MapGroup("/rangos");
        var rangosComIdEndPoint = rangosEndPoint.MapGroup("/{rangoId:int}");

        var rangosComIdEFilter = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}").AddEndpointFilter(new RangosFilters(1)) //Passando a nossa classe que contem o filtro para rangos com o parametro que sera usado
                                                                                 .AddEndpointFilter(new RangosFilters(2)); // no construtor para dizer qual id é imutável; 
        //Agrupamento de endpoints
        rangosEndPoint.MapGet("", RangosHandlers.GetRangoAsync);// O metodo estatico  GetRangoAsync esta sendo chamado sem () pq ele não vai ser executando na classe
                                                                // e assim me devolvendo um retorno, mas sim vai me voltar ali todo o conteudo do metodo.
                                                                // é como seu estivesse dando um copia e cola no metodo da classe para o nosso delegate.



        rangosComIdEndPoint.MapGet("", RangosHandlers.GetRangoIdAsync).WithName("GetRangos");//Aplicando um nome ao nosso end point;


        //************** Os endPoints abaixo só estão no código por motivo de estudo ********************************

        //[Como o parametro será passado via url(dando um nome ao parametro)] aqui estamos mostrando outra maneira de passar os parâmetros

        ////Rota
        //app.MapGet("/rangos", async (RangoDbContext rangoDbContext) =>// aqui estou delegando para o segundo parametro a responsabilidade do que vai ser feito quando a rota der match
        //                                                              // esse segundo parâmetro é um delegate
        //                                                              // como RangoDbContext foi passado para o builder,
        //                                                              // temos que pessalo novamente como parametro para poder acessar o banco de dados
        //{
        //    return await rangoDbContext.Rangos.ToListAsync();//O que a rota Retorna
        //});
        #endregion
        #region POST
        rangosEndPoint.MapPost("", RangosHandlers.PostRangoAsync).AddEndpointFilter<ValidationAnnotationFilter>();
        #endregion
        #region Put
        rangosComIdEFilter.MapPut("", RangosHandlers.PutRangoAsync);
        #endregion
        #region Delete
        rangosComIdEndPoint.MapDelete("", RangosHandlers.DeleteRangosAsync)
            .AddEndpointFilter(new RangosFilters(1))//Passando a nossa classe que contem o filtro para rangos com o parametro que sera usado
            .AddEndpointFilter(new RangosFilters(2)); //no construtor para dizer qual id é imutável;
        #endregion
    }
    public static void RegisterIngredientesEndPoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var rangosIngredientesEndPoint = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}/ingredientes");
        rangosIngredientesEndPoint.MapGet("", IngredientesHandlers.GetIngredientesAsync).WithName("GetIngredientes");//Aplicando um nome ao nosso end point;
    }
}

