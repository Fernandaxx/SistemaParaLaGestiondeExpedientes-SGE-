using SGE.Dominio.Comun;
using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Expedientes;
public class ListarTodosLosExpedientesUseCase(IExpedienteRepository repositorio)
{
    public ListarTodosLosExpedientesResponse Ejecutar()
    {
        IEnumerable<Expediente> expedientes = repositorio.ListarTodos();

        var dtos = new List<ExpedienteDTO>();
        foreach (var e in expedientes)
        {
            var dto = new ExpedienteDTO(e.Id, e.Caratula.Valor, e.Estado);
            dtos.Add(dto);
        }
        
        return new ListarTodosLosExpedientesResponse(dtos);
    }
}