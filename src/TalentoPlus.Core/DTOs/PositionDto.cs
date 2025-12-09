using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Core.DTOs
{
    public class PositionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PositionCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }

    public class PositionUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}