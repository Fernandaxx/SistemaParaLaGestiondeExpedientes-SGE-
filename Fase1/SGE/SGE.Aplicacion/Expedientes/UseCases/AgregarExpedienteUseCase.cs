using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Expedientes;
public class AgregarExpedienteUseCase (IExpedienteRepository repositorio) {
    public AgregarExpedienteResponse Ejecutar(AgregarExpedienteRequest request)
    {
        // Los Value Objects se encargan de las validaciones de formato/rango
        var caratula = new Caratula(request.Caratula);

        // La Entidad nace con su identidad (Guid) de forma autónoma
        var expediente = new Expediente(caratula, request.IdUsuario);

        // Persistencia
        repositorio.Agregar(expediente);

        // Salida
        return new AgregarExpedienteResponse(expediente.Id);
    }
}