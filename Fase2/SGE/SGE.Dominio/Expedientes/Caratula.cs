using SGE.Dominio.Comun;

namespace SGE.Dominio.Expedientes;

public record class Caratula
{
    public string Valor { get; init; }

    protected Caratula()
    {
        Valor = string.Empty;
    }

    public Caratula(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new DominioException("La carátula no puede estar vacía ni ser nula.");
        }

        Valor = valor;
    }
}
