using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndPointHandlers;

public static class RangosHandlers
{
    //ATENÇÃO, O ASYNC JUNTO COM AWAIT TRANSFORMAS NOSSOS ENDPOINTS EM ASSINCRONOS
    public static async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> GetRangoAsync //GetRangoAsync é o nome do nosso metodo,
                                                                                          //Dentro do Task colocamos nossos retornos, no caso do OK estou dizendo que
                                                                                          //dentro dele haverá um parametro de lista da entidade Rango
    (RangoDbContext rangoDbContext,
    IMapper mapper,
    [FromQuery(Name = "name")] string? rangoNome
    )
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
    }

    public static async Task<Results<NoContent, Ok<RangoDTO>>> GetRangoIdAsync
     (RangoDbContext rangoDbContext
    , IMapper mapper
    , int rangoId)  // para recuperar o parametro na url, existem varias formas e podermos mudar o nome.

    {
        var retornoNomeRango = mapper.Map<RangoDTO>(await rangoDbContext.Rangos. // Como não é uma lista, somente 1 retorno,
                          FirstOrDefaultAsync(x => x.Id == rangoId));                 // não usamos o Ienumerable,somente nosso DTO

        if (retornoNomeRango == null)
        {
            return TypedResults.NoContent();
        }
        return TypedResults.Ok(retornoNomeRango);
    }

    public static async Task<CreatedAtRoute<RangoDTO>> PostRangoAsync(// o endpoint vai retornar um objeto da classe RangoDTO, junto com um status HTTP 201 (Created)
                                                                      // e a rota onde o recurso recém-criado pode ser acessado (no caso, a rota GetRango).
    RangoDbContext rangoDbContext,//É injetado para que o código possa interagir com o banco de dados.
    IMapper mapper,// Injeção do auto mapeamento
    [FromBody] RangoForCreationDTO rangoForCreationDTO //Por padrão o post já recebe sua informação do body, aqui estou colocando sómente para exemplificar,
                                                       //como parametro estou dizendo que a informação vai ser recebida é um RangoForCreationDTO.

    //Essas duas injeções abaixo só são necessarias se usarmos o maneira alternativa de retornar o caminho.
    //LinkGenerator linkGenerator, //Permite capturar a uri pelo nome que foi dado para endpoint
    // HttpContext httpContext// usamos ele para passar como parametro no metodo GetUriByName() da classe LinkGenerator
    )
    {
        var rangoEntity = mapper.Map<Rango>(rangoForCreationDTO); //O AutoMapper vai pegar todas as propriedades que existem tanto no RangoForCreationDTO quanto na
                                                                  //entidade Rango e preencher automaticamente as propriedades correspondentes. Ele faz isso com base
                                                                  //no nome das propriedades. Se houver propriedades adicionais na entidade que não existem no DTO (como o Id),
                                                                  //essas propriedades simplesmente não serão preenchidas pelo mapeamento e continuarão com seus valores padrões.
        rangoDbContext.Add(rangoEntity);//adicionando ao contexto nosso mapeamento
        await rangoDbContext.SaveChangesAsync();//persistindo  as mudanças no banco de Dados

        var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);//Após salvar no banco, a entidade Rango é mapeada de volta para um DTO do tipo RangoDTO

        return TypedResults.CreatedAtRoute(rangoToReturn, "GetRangos", new
        {
            rangoId = rangoToReturn.Id
        });//Retorna a URI onde o novo recurso pode ser acessado, seguindo a rota "GetRango"

        //Referencia para estudos, maneira ALTERNATIVA pois muitos programadores fazem o que foi feito acima da maneira que estará aqui embaixo

        //var linkToReturn = linkGenerator.GetUriByName(
        //    httpContext,// 1° parametro de Contexto que estamos
        //    "GetRango",  //2° parametro o nome da url do nosso endPoint
        //    new { id = rangoToReturn.Id }); //3° parametro é o objeto que vai montar a url,
        //                                    //como id é o nosso parametro em GetRango estamos alimentando ele com o id que foi mapeado de RangoDTO 
        //return TypedResults.Created(linkToReturn, rangoToReturn);
    }

    public static async Task<Results<NotFound, Ok<RangoForUpdateDTO>>> PutRangoAsync
        (RangoDbContext rangoDbContext
    , IMapper mapper
    , int rangoId
    , [FromBody] RangoForUpdateDTO rangoForUpdateDTO)
    {
        var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);  //Verifica se o que eu quero atualizar,
                                                                                                   //existe com base no parametro a ser recebido (Id) 
        if (rangosEntity == null)// Caso não exista retorna notFound
            return TypedResults.NotFound();

        mapper.Map(rangoForUpdateDTO, rangosEntity);//para o put caso o objeto já exista no nosso context, eu posso fazer o mapeamento novamente para atualizar as informações
        await rangoDbContext.SaveChangesAsync(); //Atualizando o banco de dados com os dados atualizados
        return TypedResults.Ok(rangoForUpdateDTO);

    }

    public static async Task<Results<NotFound, Ok>> DeleteRangosAsync
        (RangoDbContext rangoDbContext, int rangoId)//na exclusão não precisamos do mapeamento
    {
        var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);  //Verifica se o que eu quero excluir,
                                                                                                   //existe com base no parametro a ser recebido (Id) 
        if (rangosEntity == null)// Caso não exista retorna notFound
            return TypedResults.NotFound();

        rangoDbContext.Rangos.Remove(rangosEntity);//Removendo o item

        await rangoDbContext.SaveChangesAsync(); //Atualizando o banco de dados com a deleção
        return TypedResults.Ok();

    }
}

