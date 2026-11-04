using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RageRoomApi.Models;

namespace RageRoomApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalasController : ControllerBase
{
    private readonly RageRoomDbContext _context;

    public SalasController(RageRoomDbContext context)
    {
        _context = context;
    }

    // 1. GET: api/Salas (Con filtros por query string)
    // Códigos posibles: 200 OK
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sala>>> GetSalas(
        [FromQuery] string? buscar,
        [FromQuery] bool? activa,
        [FromQuery] int? capacidadMinima)
    {
        var query = _context.Salas.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim().ToLower();
            query = query.Where(s => s.Nombre.ToLower().Contains(texto) ||
                                     (s.Descripcion != null && s.Descripcion.ToLower().Contains(texto)));
        }

        if (activa.HasValue)
        {
            query = query.Where(s => s.Activa == activa.Value);
        }

        if (capacidadMinima.HasValue)
        {
            query = query.Where(s => s.CapacidadMaxima >= capacidadMinima.Value);
        }

        var salas = await query.ToListAsync();
        return Ok(salas);
    }

    // 2. GET: api/Salas/{id}
    // Códigos posibles: 200 OK, 404 NotFound
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Sala>> GetSala(int id)
    {
        var sala = await _context.Salas
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sala == null)
        {
            return NotFound(new { message = $"No se encontró la sala con ID {id}." });
        }

        return Ok(sala);
    }

    // 3. GET: api/Salas/{id}/sesiones (Ruta anidada requerida)
    // Códigos posibles: 200 OK, 404 NotFound
    [HttpGet("{id:int}/sesiones")]
    public async Task<ActionResult<IEnumerable<Sesion>>> GetSesionesPorSala(int id)
    {
        var existeSala = await _context.Salas.AnyAsync(s => s.Id == id);
        if (!existeSala)
        {
            return NotFound(new { message = $"No existe la sala con ID {id}." });
        }

        var sesiones = await _context.Sesiones
            .AsNoTracking()
            .Where(s => s.SalaId == id)
            .ToListAsync();

        return Ok(sesiones);
    }

    // 4. POST: api/Salas
    // Códigos posibles: 201 CreatedAtAction, 400 BadRequest (DataAnnotations), 409 Conflict (Regla de negocio 1: nombre duplicado)
    [HttpPost]
    public async Task<ActionResult<Sala>> CreateSala(Sala sala)
    {
        // Regla de Negocio 1 (409 Conflict): El nombre de la sala debe ser único
        var existeNombre = await _context.Salas
            .AnyAsync(s => s.Nombre.ToLower() == sala.Nombre.Trim().ToLower());

        if (existeNombre)
        {
            return Conflict(new { message = $"Ya existe una sala registrada con el nombre '{sala.Nombre}'." });
        }

        sala.Nombre = sala.Nombre.Trim();
        _context.Salas.Add(sala);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSala), new { id = sala.Id }, sala);
    }

    // 5. PUT: api/Salas/{id}
    // Códigos posibles: 204 NoContent, 400 BadRequest, 404 NotFound, 409 Conflict (nombre duplicado)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSala(int id, Sala sala)
    {
        if (id != sala.Id)
        {
            return BadRequest(new { message = "El ID del parámetro de ruta no coincide con el ID del cuerpo de la petición." });
        }

        var salaDb = await _context.Salas.FindAsync(id);
        if (salaDb == null)
        {
            return NotFound(new { message = $"No se encontró la sala con ID {id} para actualizar." });
        }

        // Regla de Negocio (409 Conflict): No duplicar nombre con otra sala
        var nombreEnUso = await _context.Salas
            .AnyAsync(s => s.Id != id && s.Nombre.ToLower() == sala.Nombre.Trim().ToLower());

        if (nombreEnUso)
        {
            return Conflict(new { message = $"El nombre '{sala.Nombre}' ya está en uso por otra sala." });
        }

        salaDb.Nombre = sala.Nombre.Trim();
        salaDb.Descripcion = sala.Descripcion;
        salaDb.CapacidadMaxima = sala.CapacidadMaxima;
        salaDb.NivelProteccion = sala.NivelProteccion;
        salaDb.PrecioBase = sala.PrecioBase;
        salaDb.Activa = sala.Activa;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // 6. DELETE: api/Salas/{id}
    // Códigos posibles: 204 NoContent, 404 NotFound, 409 Conflict (Regla de negocio 2: eliminar con dependencias)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSala(int id)
    {
        var sala = await _context.Salas.FindAsync(id);
        if (sala == null)
        {
            return NotFound(new { message = $"No se encontró la sala con ID {id} para eliminar." });
        }

        // Regla de Negocio 2 (409 Conflict): No se puede eliminar una sala que tiene sesiones asociadas
        var tieneSesiones = await _context.Sesiones.AnyAsync(s => s.SalaId == id);
        if (tieneSesiones)
        {
            return Conflict(new { message = $"No se puede eliminar la sala '{sala.Nombre}' (ID: {id}) porque tiene sesiones registradas. Considere desactivarla cambiando Activa a falso." });
        }

        _context.Salas.Remove(sala);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

