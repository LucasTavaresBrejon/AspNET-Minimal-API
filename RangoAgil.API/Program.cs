using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(//Inje��o de dependencia
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"]));//Configurando Program para utilizar a nossa classe 
// RangoDbContext para acesso ao banco de dados, � necessario acrescentar a configura��o em appsettings.json

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());//Aqui ele j� vai pegar a classe dominio atual da nossa api e vai usar as dll(Assemblies) utilizadas
                                                                        //e com base nisso vai achar a classe que herda profile(RangoAgilProfile) e vai utilizala no builder.
var app = builder.Build();

app.RegisterRangosEndpoints();
app.RegisterIngredientesEndPoints();

app.Run();
