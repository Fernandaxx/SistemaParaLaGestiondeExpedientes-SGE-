using SGE.Dominio.Comun;

namespace SGE.Dominio.Tramites;

public record ContenidoTramite
{
    public string Valor { get; init; }

    public ContenidoTramite(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new DominioException("El contenido del trámite es obligatorio y no puede estar vacío.");
        }

        Valor = valor;
    }


}