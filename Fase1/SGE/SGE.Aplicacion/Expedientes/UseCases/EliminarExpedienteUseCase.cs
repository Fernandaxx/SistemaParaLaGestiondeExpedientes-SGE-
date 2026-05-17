using SGE.Dominio.Comun;
using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Expedientes;
public class EliminarExpedienteUseCase (IExpedienteRepository repositorio) {
    public EliminarExpedienteResponse Ejecutar(EliminarExpedienteRequest request)
    {
        Expediente expediente = repositorio.ObtenerPorId(request.Id);

        if (expediente is null)
            throw new DominioException("Entidad no encontrada. No se puede eliminar.");


        repositorio.Eliminar(expediente);

        return new EliminarExpedienteResponse();
    }
}