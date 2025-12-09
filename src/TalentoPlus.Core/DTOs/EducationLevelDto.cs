using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Core.DTOs
{
    public class EducationLevelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class EducationLevelCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }

    public class EducationLevelUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}