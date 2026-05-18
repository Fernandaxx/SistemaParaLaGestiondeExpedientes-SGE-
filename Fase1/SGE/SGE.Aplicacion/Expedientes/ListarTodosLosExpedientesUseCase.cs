using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class ListarTodosLosExpedientesUseCase(IExpedienteRepository _repository)
{
    public ListarTodosLosExpedientesResponse Ejecutar(ListarTodosLosExpedientesRequest request)
    {
        IEnumerable<Expediente> expedientes = _repository.ListarTodos();

        var dtos = new List<ExpedienteDTO>();
        foreach (var e in expedientes)
        {
            var dto = new ExpedienteDTO(e.Id, e.Caratula.Valor, e.Estado);
            dtos.Add(dto);
        }

        return new ListarTodosLosExpedientesResponse(dtos);
    }
}