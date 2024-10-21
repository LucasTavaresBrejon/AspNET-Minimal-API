﻿namespace RangoAgil.API.Models;

public class RangoForUpdateDTO // O que será exposto pela nossa api no metodo put, a diferença entre RangoForUpdateDTO e RangoDTO
                               // é que quando vamos enviar algo ao nosso banco não enviamos id por exemplo, então nosso dto foi criado para passar somente o que foi enviado
{
    public required string Nome { get; set; }
}

