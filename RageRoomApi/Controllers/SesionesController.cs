using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RageRoomApi.Models;

namespace RageRoomApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SesionesController : ControllerBase
{
    private readonly RageRoomDbContext _context;

    public SesionesController(RageRoomDbContext context)
    {
        _context = context;
    }

    // 1. GET: api/Sesiones (Con filtros por query string)
    // Códigos posibles: 200 OK
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sesion>>> GetSesiones(
        [FromQuery] string? estado,
        [FromQuery] int? salaId,
        [FromQuery] string? clienteEmail,
        [FromQuery] DateTime? fecha)
    {
        var query = _context.Sesiones
            .AsNoTracking()
            .Include(s => s.Sala)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(estado))
        {
            var est = estado.Trim().ToLower();
            query = query.Where(s => s.Estado.ToLower() == est);
        }

        if (salaId.HasValue)
        {
            query = query.Where(s => s.SalaId == salaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(clienteEmail))
        {
            var email = clienteEmail.Trim().ToLower();
            query = query.Where(s => s.ClienteEmail.ToLower().Contains(email));
        }

        if (fecha.HasValue)
        {
            var fechaBusqueda = fecha.Value.Date;
            query = query.Where(s => s.FechaHora.Date == fechaBusqueda);
        }

        var sesiones = await query.ToListAsync();
        return Ok(sesiones);
    }

    // 2. GET: api/Sesiones/{id}
    // Códigos posibles: 200 OK, 404 NotFound
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Sesion>> GetSesion(int id)
    {
        var sesion = await _context.Sesiones
            .AsNoTracking()
            .Include(s => s.Sala)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sesion == null)
        {
            return NotFound(new { message = $"No se encontró la sesión con ID {id}." });
        }

        return Ok(sesion);
    }

    // 3. POST: api/Sesiones
    // Códigos posibles: 201 CreatedAtAction, 400 BadRequest (Sala inexistente o capacidad excedida), 409 Conflict (Cruce de horarios)
    [HttpPost]
    public async Task<ActionResult<Sesion>> CreateSesion(Sesion sesion)
    {
        // Validación 400: La sala foránea debe existir
        var sala = await _context.Salas.FindAsync(sesion.SalaId);
        if (sala == null)
        {
            return BadRequest(new { message = $"La sala con ID {sesion.SalaId} especificada no existe." });
        }

        // Validación 400: La sala debe estar activa
        if (!sala.Activa)
        {
            return BadRequest(new { message = $"La sala '{sala.Nombre}' se encuentra temporalmente inactiva o en mantenimiento." });
        }

        // Validación 400: La cantidad de personas no puede superar la capacidad máxima de la sala
        if (sesion.CantidadPersonas > sala.CapacidadMaxima)
        {
            return BadRequest(new { message = $"La cantidad de personas ({sesion.CantidadPersonas}) excede la capacidad máxima de la sala '{sala.Nombre}' ({sala.CapacidadMaxima} personas)." });
        }

        // Regla de Negocio (409 Conflict): Cruce de horarios en la misma sala
        var inicioNueva = sesion.FechaHora;
        var finNueva = sesion.FechaHora.AddMinutes(sesion.DuracionMinutos);

        var sesionesMismaSala = await _context.Sesiones
            .Where(s => s.SalaId == sesion.SalaId && s.Estado != "Cancelada")
            .ToListAsync();

        var existeCruce = sesionesMismaSala.Any(s =>
        {
            var inicioExistente = s.FechaHora;
            var finExistente = s.FechaHora.AddMinutes(s.DuracionMinutos);
            return inicioNueva < finExistente && finNueva > inicioExistente;
        });

        if (existeCruce)
        {
            return Conflict(new { message = $"La sala '{sala.Nombre}' ya cuenta con una sesión reservada en el rango horario solicitado ({inicioNueva:g} a {finNueva:t})." });
        }

        // Limpiar referencia de navegación si fue enviada para que EF no intente insertarla
        sesion.Sala = null;

        _context.Sesiones.Add(sesion);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSesion), new { id = sesion.Id }, sesion);
    }

    // 4. PUT: api/Sesiones/{id}
    // Códigos posibles: 204 NoContent, 400 BadRequest, 404 NotFound, 409 Conflict
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSesion(int id, Sesion sesion)
    {
        if (id != sesion.Id)
        {
            return BadRequest(new { message = "El ID del parámetro de ruta no coincide con el ID del cuerpo de la petición." });
        }

        var sesionDb = await _context.Sesiones.FindAsync(id);
        if (sesionDb == null)
        {
            return NotFound(new { message = $"No se encontró la sesión con ID {id} para actualizar." });
        }

        // Regla de Negocio (409 Conflict): No se puede editar una sesión Completada o Cancelada
        if (sesionDb.Estado == "Completada" || sesionDb.Estado == "Cancelada")
        {
            return Conflict(new { message = $"No se pueden modificar los datos de una sesión que ya ha sido '{sesionDb.Estado}'." });
        }

        // Validar sala foránea si cambió
        var sala = await _context.Salas.FindAsync(sesion.SalaId);
        if (sala == null)
        {
            return BadRequest(new { message = $"La sala con ID {sesion.SalaId} especificada no existe." });
        }

        if (sesion.CantidadPersonas > sala.CapacidadMaxima)
        {
            return BadRequest(new { message = $"La cantidad de personas ({sesion.CantidadPersonas}) supera la capacidad máxima de la sala '{sala.Nombre}' ({sala.CapacidadMaxima})." });
        }

        sesionDb.SalaId = sesion.SalaId;
        sesionDb.ClienteNombre = sesion.ClienteNombre.Trim();
        sesionDb.ClienteEmail = sesion.ClienteEmail.Trim();
        sesionDb.ClienteTelefono = sesion.ClienteTelefono;
        sesionDb.FechaHora = sesion.FechaHora;
        sesionDb.DuracionMinutos = sesion.DuracionMinutos;
        sesionDb.CantidadPersonas = sesion.CantidadPersonas;
        sesionDb.PrecioTotal = sesion.PrecioTotal;
        sesionDb.Estado = sesion.Estado;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // 5. DELETE: api/Sesiones/{id}
    // Códigos posibles: 204 NoContent, 404 NotFound, 409 Conflict
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSesion(int id)
    {
        var sesion = await _context.Sesiones.FindAsync(id);
        if (sesion == null)
        {
            return NotFound(new { message = $"No se encontró la sesión con ID {id} para eliminar." });
        }

        // Regla de Negocio (409 Conflict): No se puede eliminar una sesión 'Completada' para mantener auditoría
        if (sesion.Estado == "Completada")
        {
            return Conflict(new { message = "No es posible eliminar una sesión con estado 'Completada' por motivos de auditoría contable. Puede cambiar su estado a 'Cancelada' si corresponde." });
        }

        _context.Sesiones.Remove(sesion);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

