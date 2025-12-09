using System;

namespace TalentoPlus.Core.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Document { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        
        // Foreign keys
        public int PositionId { get; set; }
        public int DepartmentId { get; set; }
        public int EmployeeStatusId { get; set; }
        public int EducationLevelId { get; set; }
        
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public string ProfessionalProfile { get; set; }
        public string PasswordHash { get; set; }
        public bool IsEnabled { get; set; } = false;
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Position Position { get; set; }
        public virtual Department Department { get; set; }
        public virtual EmployeeStatus EmployeeStatus { get; set; }
        public virtual EducationLevel EducationLevel { get; set; }
    }
}
