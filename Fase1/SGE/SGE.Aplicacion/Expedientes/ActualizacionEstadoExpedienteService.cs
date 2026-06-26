using SGE.Aplicacion.Tramites;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Expedientes;

public class ActualizacionEstadoExpedienteService(IExpedienteRepository _repository, ITramiteRepository _tramiteRepository)
{
    public void ActualizarEstadoExpediente(Guid idExpediente, Guid idUsuario)
    {
        var expediente = _repository.ObtenerPorId(idExpediente);
        if (expediente == null)
            throw new EntidadNoEncontradaException("El expediente especificado no existe.");

        var tramites = _tramiteRepository.ListarPorExpediente(idExpediente);

        Tramite? ultimoTramite = null;
        foreach (var t in tramites) {
            if (ultimoTramite == null || t.FechaCreacion > ultimoTramite.FechaCreacion) {
                ultimoTramite = t;
            }
        }

        var ultimaEtiqueta = ultimoTramite?.Etiqueta;

        bool cambio = expediente.ActualizarEstado(ultimaEtiqueta, idUsuario);

        if (cambio)
            _repository.Modificar(expediente);
    }
}
