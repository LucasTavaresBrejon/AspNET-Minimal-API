using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"]));//Configurando Program para utilizar a nossa classe 
// RangoDbContext para acesso ao banco de dados, � necessario acrescentar a configura��o em appsettings.json
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
