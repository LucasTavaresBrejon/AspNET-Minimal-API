﻿namespace RangoAgil.API.Models;

public class IngredienteDTO // O que será exposto pela nossa api
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public int RangoId { get; set; }

}

