using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Tramites;

namespace SGE.Aplicacion.Expedientes;

public class ObtenerExpedientePorIdUseCase(IExpedienteRepository _repository, ITramiteRepository _tramiteRepository)
{
    public ObtenerExpedientePorIdResponse Ejecutar(ObtenerExpedientePorIdRequest request)
    {
        var expediente = _repository.ObtenerPorId(request.Id);
        if (expediente is null)
            throw new EntidadNoEncontradaException("No se encontró el expediente solicitado.");

        var tramites = _tramiteRepository.ListarPorExpediente(request.Id)
            .Select(t => new TramiteDetalleDTO(
                t.Id,
                t.ExpedienteId,
                t.Etiqueta,
                t.Contenido.Valor,
                t.FechaCreacion,
                t.FechaUltimaModificacion,
                t.UsuarioUltimoCambio))
            .ToList();

        var dto = new ExpedienteDetalleDTO(
            expediente.Id,
            expediente.Caratula.Valor,
            expediente.Estado,
            expediente.FechaCreacion,
            expediente.FechaModificacion,
            expediente.UsuarioUltimoCambio,
            tramites);

        return new ObtenerExpedientePorIdResponse(dto);
    }
}
