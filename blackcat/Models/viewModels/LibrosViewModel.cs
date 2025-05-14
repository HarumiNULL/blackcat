using System.ComponentModel.DataAnnotations;

namespace blackcat.Models.viewModels;

public class LibrosViewModel
{
    public int IdL { get; set; }
    
    [Required(ErrorMessage = "El nombre del libro es obligatorio.")]
    public string? NombreL { get; set; }

    [Required(ErrorMessage = "El autor es obligatorio.")]
    public string? Autor { get; set; }
    public string? Archivo { get; set; }
    [Required(ErrorMessage = "El archivo del libro es obligatorio.")]
    [DataType(DataType.Upload)]
    [AllowedExtensions(new[] { ".pdf", ".epub" }, ErrorMessage = "Solo se permiten archivos PDF o EPUB.")]
    public IFormFile? ArchivoForm { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string? Descripcion { get; set; }

    public string? Imagen { get; set; }
    
    [Required(ErrorMessage = "La imagen de portada es obligatoria.")]
    [DataType(DataType.Upload)]
    [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" }, ErrorMessage = "Solo se permiten imágenes (JPG, JPEG, PNG).")]
    public IFormFile? ImagenForm { get; set; }
    public virtual ICollection<Busquedum> Busqueda { get; set; } = new List<Busquedum>();

    public virtual ICollection<ListaU> ListaUs { get; set; } = new List<ListaU>();
    
    public byte[] ? Foto { get; set; }
    
    // Atributo personalizado para validar extensiones
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions) => _extensions = extensions;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_extensions.Contains(extension))
                    return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}