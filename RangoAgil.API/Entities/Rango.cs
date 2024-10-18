using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RangoAgil.API.Entities;

public class Rango
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public required string Nome { get; set; } = null!;//Dizendo que mesmo que no meu projeto esteja configurado para aceitar null o nome é obrigatorio
    public ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();//ICollection mostra que Rango pode ter varios objetos
                                                                            //Ingredientes ligado a ela e  com o new list monta uma lista delas
    public Rango()
    {

    }

    [SetsRequiredMembers]//Diz que os intens abaixo são obrigatórios;
    public Rango(int id, string nome)
    {
        Id = id;
        Nome = nome;
    }
}

