using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RageRoomApi.Models;

[Table("Sesiones")]
public class Sesion
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El identificador de la sala (SalaId) es obligatorio.")]
    public int SalaId { get; set; }

    [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre del cliente debe tener entre 2 y 100 caracteres.")]
    public string ClienteNombre { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico del cliente es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres.")]
    public string ClienteEmail { get; set; } = null!;

    [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres.")]
    public string? ClienteTelefono { get; set; }

    [Required(ErrorMessage = "La fecha y hora de la sesión es obligatoria.")]
    public DateTime FechaHora { get; set; }

    [Required(ErrorMessage = "La duración en minutos es obligatoria.")]
    [Range(15, 120, ErrorMessage = "La duración de la sesión debe ser entre 15 y 120 minutos.")]
    public int DuracionMinutos { get; set; }

    [Required(ErrorMessage = "La cantidad de personas es obligatoria.")]
    [Range(1, 10, ErrorMessage = "La cantidad de personas debe ser entre 1 y 10.")]
    public int CantidadPersonas { get; set; }

    [Required(ErrorMessage = "El precio total es obligatorio.")]
    [Range(0.0, 50000.0, ErrorMessage = "El precio total no puede ser negativo.")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioTotal { get; set; }

    [Required(ErrorMessage = "El estado de la sesión es obligatorio.")]
    [StringLength(20, ErrorMessage = "El estado no puede superar los 20 caracteres.")]
    public string Estado { get; set; } = "Pendiente";

    // Admite nulo para que en las peticiones POST solo se requiera enviar la clave foránea SalaId
    public virtual Sala? Sala { get; set; }
}

