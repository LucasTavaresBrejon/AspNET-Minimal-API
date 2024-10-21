using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(//Inje��o de dependencia
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"]));//Configurando Program para utilizar a nossa classe 
// RangoDbContext para acesso ao banco de dados, � necessario acrescentar a configura��o em appsettings.json

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());//Aqui ele j� vai pegar a classe dominio atual da nossa api e vai usar as dll(Assemblies) utilizadas
                                                                        //e com base nisso vai achar a classe que herda profile(RangoAgilProfile) e vai utilizala no builder.
var app = builder.Build();

#region GET

app.MapGet("/", () => "Hello World!");

//Agrupamento de endpoints
var rangosEndPoint = app.MapGroup("/rangos");
var rangosComIdEndPoint = rangosEndPoint.MapGroup("/{rangoId:int}");
var rangosIngredientesEndPoint = rangosComIdEndPoint.MapGroup("/ingredientes");

//ATEN��O, O ASYNC JUNTO COM AWAIT TRANSFORMAS NOSSOS ENDPOINTS EM ASSINCRONOS

rangosEndPoint.MapGet("", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> //Dentro do Task colocamos nossos retornos, no caso do OK estou dizendo que
                                                                                //dentro dele haver� um parametro de lista da entidade Rango
    (RangoDbContext rangoDbContext,
     IMapper mapper,
    [FromQuery(Name = "name")] string? rangoNome
    ) =>
{
    var rangosEntity = mapper.Map<IEnumerable<RangoDTO>>(await rangoDbContext.Rangos
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

rangosIngredientesEndPoint.MapGet("", async Task<Results<NoContent, Ok<IEnumerable<IngredienteDTO>>>> (
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

}).WithName("GetIngredientes");//Aplicando um nome ao nosso end point;

rangosComIdEndPoint.MapGet("", async Task<Results<NoContent, Ok<RangoDTO>>> (RangoDbContext rangoDbContext
    , IMapper mapper
    ,int rangoId) =>  // para recuperar o parametro na url, existem varias formas e podermos mudar o nome.

{
    var retornoNomeRango = mapper.Map<RangoDTO>(await rangoDbContext.Rangos. // Como n�o � uma lista, somente 1 retorno,
                      FirstOrDefaultAsync(x => x.Id == rangoId));                 // n�o usamos o Ienumerable,somente nosso DTO

    if (retornoNomeRango == null)
    {
        return TypedResults.NoContent();
    }
    return TypedResults.Ok(retornoNomeRango);

}).WithName("GetRangos");//Aplicando um nome ao nosso end point;


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
#endregion

#region POST
rangosEndPoint.MapPost("", async Task<CreatedAtRoute<RangoDTO>> (// o endpoint vai retornar um objeto da classe RangoDTO, junto com um status HTTP 201 (Created)
                                                            // e a rota onde o recurso rec�m-criado pode ser acessado (no caso, a rota GetRango).
    RangoDbContext rangoDbContext,//� injetado para que o c�digo possa interagir com o banco de dados.
    IMapper mapper,// Inje��o do auto mapeamento
    [FromBody] RangoForCreationDTO rangoForCreationDTO //Por padr�o o post j� recebe sua informa��o do body, aqui estou colocando s�mente para exemplificar,
                                                       //como parametro estou dizendo que a informa��o vai ser recebida � um RangoForCreationDTO.

    //Essas duas inje��es abaixo s� s�o necessarias se usarmos o maneira alternativa de retornar o caminho.
    //LinkGenerator linkGenerator, //Permite capturar a uri pelo nome que foi dado para endpoint
    // HttpContext httpContext// usamos ele para passar como parametro no metodo GetUriByName() da classe LinkGenerator
    ) =>
        {
            var rangoEntity = mapper.Map<Rango>(rangoForCreationDTO); //O AutoMapper vai pegar todas as propriedades que existem tanto no RangoForCreationDTO quanto na
                                                                      //entidade Rango e preencher automaticamente as propriedades correspondentes. Ele faz isso com base
                                                                      //no nome das propriedades. Se houver propriedades adicionais na entidade que n�o existem no DTO (como o Id),
                                                                      //essas propriedades simplesmente n�o ser�o preenchidas pelo mapeamento e continuar�o com seus valores padr�es.
            rangoDbContext.Add(rangoEntity);//adicionando ao contexto nosso mapeamento
            await rangoDbContext.SaveChangesAsync();//persistindo  as mudan�as no banco de Dados

            var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);//Ap�s salvar no banco, a entidade Rango � mapeada de volta para um DTO do tipo RangoDTO

            return TypedResults.CreatedAtRoute(rangoToReturn, "GetRangos", new { rangoId = rangoToReturn.Id });//Retorna a URI onde o novo recurso pode ser acessado, seguindo a rota "GetRango"

            //Referencia para estudos, maneira ALTERNATIVA pois muitos programadores fazem o que foi feito acima da maneira que estar� aqui embaixo

            //var linkToReturn = linkGenerator.GetUriByName(
            //    httpContext,// 1� parametro de Contexto que estamos
            //    "GetRango",  //2� parametro o nome da url do nosso endPoint
            //    new { id = rangoToReturn.Id }); //3� parametro � o objeto que vai montar a url,
            //                                    //como id � o nosso parametro em GetRango estamos alimentando ele com o id que foi mapeado de RangoDTO 
            //return TypedResults.Created(linkToReturn, rangoToReturn);
        }

);
#endregion
#region Put
rangosComIdEndPoint.MapPut("", async Task<Results<NotFound, Ok<RangoForUpdateDTO>>> (RangoDbContext rangoDbContext
    , IMapper mapper
    , int rangoId
    , [FromBody] RangoForUpdateDTO rangoForUpdateDTO) =>  
{
    var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);  //Verifica se o que eu quero atualizar,
                                                                                          //existe com base no parametro a ser recebido (Id) 
    if (rangosEntity == null)// Caso n�o exista retorna notFound
        return TypedResults.NotFound();

    mapper.Map(rangoForUpdateDTO, rangosEntity);//para o put caso o objeto j� exista no nosso context, eu posso fazer o mapeamento novamente para atualizar as informa��es
    await rangoDbContext.SaveChangesAsync(); //Atualizando o banco de dados com os dados atualizados
    return TypedResults.Ok(rangoForUpdateDTO);

});
#endregion
#region Delete
rangosComIdEndPoint.MapDelete("", async Task<Results<NotFound, Ok>> (RangoDbContext rangoDbContext, int rangoId) =>//na exclus�o n�o precisamos do mapeamento
{
    var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);  //Verifica se o que eu quero excluir,
                                                                                          //existe com base no parametro a ser recebido (Id) 
    if (rangosEntity == null)// Caso n�o exista retorna notFound
        return TypedResults.NotFound();

    rangoDbContext.Rangos.Remove(rangosEntity);//Removendo o item
    
    await rangoDbContext.SaveChangesAsync(); //Atualizando o banco de dados com a dele��o
    return TypedResults.Ok();

});
#endregion
app.Run();
