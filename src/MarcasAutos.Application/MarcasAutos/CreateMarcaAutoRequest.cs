using System.ComponentModel.DataAnnotations;

namespace MarcasAutos.Application.MarcasAutos;

public record CreateMarcaAutoRequest(
    [Required(ErrorMessage = "El nombre de la marca es requerido.")]
    [MaxLength(100, ErrorMessage = "El nombre de la marca no puede exceder 100 caracteres.")]
    string Nombre
);
