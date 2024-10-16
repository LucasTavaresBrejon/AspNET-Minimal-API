using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace RangoAgil.API.Entities;

public class Ingrediente
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public required string Nome { get; set; }
    public ICollection<Rango> Rangos { get; set; } = new List<Rango>();

    public Ingrediente()
    {

    }

    [SetsRequiredMembers]
    public Ingrediente(int id, string nome)
    {
        Id = id;
        Nome = nome;
    }
}

