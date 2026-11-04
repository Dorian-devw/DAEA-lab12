using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RageRoomApi.Models;

[Table("Salas")]
public class Sala
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la sala es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    [StringLength(250, ErrorMessage = "La descripción no puede exceder los 250 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "La capacidad máxima es obligatoria.")]
    [Range(1, 10, ErrorMessage = "La capacidad máxima debe estar entre 1 y 10 personas.")]
    public int CapacidadMaxima { get; set; }

    [Required(ErrorMessage = "El nivel de protección es obligatorio.")]
    [StringLength(20, ErrorMessage = "El nivel de protección no puede superar los 20 caracteres.")]
    public string NivelProteccion { get; set; } = null!;

    [Required(ErrorMessage = "El precio base es obligatorio.")]
    [Range(0.0, 10000.0, ErrorMessage = "El precio base debe ser mayor o igual a 0.")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioBase { get; set; }

    public bool Activa { get; set; } = true;

    // Se ignora en serialización JSON para evitar ciclos sin necesidad de DTOs
    [JsonIgnore]
    public virtual ICollection<Sesion> Sesiones { get; set; } = new List<Sesion>();
}

