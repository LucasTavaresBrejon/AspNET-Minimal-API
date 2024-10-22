using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Extensions;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(//Injeção de dependencia
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"]));//Configurando Program para utilizar a nossa classe 
// RangoDbContext para acesso ao banco de dados, é necessario acrescentar a configuração em appsettings.json

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());//Aqui ele já vai pegar a classe dominio atual da nossa api e vai usar as dll(Assemblies) utilizadas
                                                                        //e com base nisso vai achar a classe que herda profile(RangoAgilProfile) e vai utilizala no builder.

builder.Services.AddProblemDetails();// Automaticamente retornara um json com o tratamento do erro
var app = builder.Build();

//Tratamento de erro
if (!app.Environment.IsDevelopment()) // Verifica se estamos em desenvolvimento ou produção
{
    app.UseExceptionHandler();
}

app.RegisterRangosEndpoints();
app.RegisterIngredientesEndPoints();

app.Run();
