using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;

public class ListarTramitesPorExpedienteUseCase(ITramiteRepository _repository)
{
    public ListarTramitesPorExpedienteResponse Ejecutar(ListarTramitesPorExpedienteRequest request)
    {
        IEnumerable<Tramite> tramites = _repository.ListarPorExpediente(request.IdExpediente);

        var dtos = new List<TramiteDTO>();
        foreach (var t in tramites)
        {
            var dto = new TramiteDTO(t.Id, t.ExpedienteId, t.Etiqueta, t.Contenido.Valor);
            dtos.Add(dto);
        }

        return new ListarTramitesPorExpedienteResponse(dtos);
    }
}