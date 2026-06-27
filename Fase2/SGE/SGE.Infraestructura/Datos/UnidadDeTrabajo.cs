using SGE.Aplicacion.Comun;

namespace SGE.Infraestructura.Datos;

public class UnidadDeTrabajo(SgeContext _context) : IUnidadDeTrabajo
{
    public void Guardar()
    {
        _context.SaveChanges();
    }
}
