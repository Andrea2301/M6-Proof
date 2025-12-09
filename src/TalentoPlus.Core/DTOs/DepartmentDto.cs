namespace TalentoPlus.Core.DTOs
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }

    public class DepartmentCreateDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }

    public class DepartmentUpdateDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}