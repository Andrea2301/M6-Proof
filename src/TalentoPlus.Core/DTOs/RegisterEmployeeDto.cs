using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Core.DTOs
{
    public class RegisterEmployeeDto
    {
        [Required(ErrorMessage = "El documento es requerido")]
        [StringLength(20, ErrorMessage = "El documento no puede exceder 20 caracteres")]
        public string Document { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "El departamento es requerido")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "El cargo es requerido")]
        public int PositionId { get; set; }

        [Required(ErrorMessage = "El nivel educativo es requerido")]
        public int EducationLevelId { get; set; }

        [StringLength(500, ErrorMessage = "El perfil profesional no puede exceder 500 caracteres")]
        public string? ProfessionalProfile { get; set; }
    }
}