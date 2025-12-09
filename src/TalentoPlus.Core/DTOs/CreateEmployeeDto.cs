namespace TalentoPlus.Core.DTOs;

public class CreateEmployeeDto
{
    public string Document { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int PositionId { get; set; }
    public int DepartmentId { get; set; }
    public int EmployeeStatusId { get; set; }
    public int EducationLevelId { get; set; }
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    public string ProfessionalProfile { get; set; } = null!;
}



public class UpdateEmployeeDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int PositionId { get; set; }
    public int DepartmentId { get; set; }
    public int EmployeeStatusId { get; set; }
    public int EducationLevelId { get; set; }
    public decimal Salary { get; set; }
    public string ProfessionalProfile { get; set; } = null!;
}
public class EmployeeResponseDto
{
    public int Id { get; set; }
    public string Document { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? PositionName { get; set; }
    public string? DepartmentName { get; set; }
    public string? EmployeeStatusName { get; set; }
    public string? EducationLevelName { get; set; }
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsEnabled { get; set; }
}
