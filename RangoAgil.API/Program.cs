using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"]));//Configurando Program para utilizar a nossa classe 
// RangoDbContext para acesso ao banco de dados, � necessario acrescentar a configura��o em appsettings.json

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());//Aqui ele j� vai pegar a classe dominio atual da nossa api e vai usar as dll(Assemblies) utilizadas
                                                                        //e com base nisso vai achar a classe que herda profile(RangoAgilProfile) e vai utilizala no builder.

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

//ATEN��O, O ASYNC JUNTO COM AWAIT TRANSFORMAS NOSSOS ENDPOINTS EM ASSINCRONOS

app.MapGet("/rangos", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> //Dentro do Task colocamos nossos retornos, no caso do OK estou dizendo que
                                                                      //dentro dele haver� um parametro de lista da entidade Rango
    (RangoDbContext rangoDbContext,
     IMapper mapper,
    [FromQuery(Name = "name")] string? rangoNome
    ) =>
{
    var rangosEntity = mapper.Map<IEnumerable<RangoDTO>>( await rangoDbContext.Rangos
                .Where(x => rangoNome == null || x.Nome.ToLower().Contains(rangoNome.ToLower()))
                .ToListAsync()); //Retornando somente o registro com o mesmo id

    if (rangosEntity.Count() > 0 || rangosEntity != null)
    {
        return TypedResults.Ok(rangosEntity);
    }
    else
    {
        return TypedResults.NoContent();
    }

});

app.MapGet("/rango/{rangoId:int}/ingredientes", async Task<Results<NoContent, Ok<IEnumerable<IngredienteDTO>>>> (
    RangoDbContext rangoDbContext //Injetando o context
    , IMapper mapper //Injetando o mapeamento para mapear as entidades para DTOs
    , int rangoId) =>
{
    var retornoIngredienteDTO = mapper.Map<IEnumerable<IngredienteDTO>>((
        await rangoDbContext.Rangos //Acessando a entidade ou tabela Rangos
        .Include(rango => rango.Ingredientes)// Include inclui no retorno os ingredientes  que pertencem a cole��o(declarada na classe Rango) daquele rango.
        .FirstOrDefaultAsync(x => x.Id == rangoId))?.Ingredientes);//Busca o primeiro Rango(prato) cujo Id seja igual ao rangoId. 
                                                                   //caso encorntre ele retornara os ingredientes que ele incluiu na cole��o anterior com o include
                                                                   //Se n�o encontrar, retornar� null
    if (retornoIngredienteDTO == null || retornoIngredienteDTO.Count() <= 0)
    {
        return TypedResults.NoContent();
    }
    return TypedResults.Ok(retornoIngredienteDTO);

});

app.MapGet("/rango", async Task<Results<NoContent,Ok<RangoDTO>>> (RangoDbContext rangoDbContext
    ,IMapper mapper
    ,[FromQuery(Name = "RangoId")] int id) =>  // para recuperar o parametro na url, existem varias formas e podermos mudar o nome.
                                              
{
    var retornoNomeRango = mapper.Map<RangoDTO>(await rangoDbContext.Rangos. // Como n�o � uma lista, somente 1 retorno,
                      FirstOrDefaultAsync(x => x.Id == id));                 // n�o usamos o Ienumerable,somente nosso DTO

    if (retornoNomeRango == null)
    {
        return TypedResults.NoContent();
    }
        return TypedResults.Ok(retornoNomeRango);
});


//************** Os endPoints abaixo s� est�o no c�digo por motivo de estudo ********************************

//[Como o parametro ser� passado via url(dando um nome ao parametro)] aqui estamos mostrando outra maneira de passar os par�metros

////Rota
//app.MapGet("/rangos", async (RangoDbContext rangoDbContext) =>// aqui estou delegando para o segundo parametro a responsabilidade do que vai ser feito quando a rota der match
//                                                              // esse segundo par�metro � um delegate
//                                                              // como RangoDbContext foi passado para o builder,
//                                                              // temos que pessalo novamente como parametro para poder acessar o banco de dados
//{
//    return await rangoDbContext.Rangos.ToListAsync();//O que a rota Retorna
//});

app.Run();
