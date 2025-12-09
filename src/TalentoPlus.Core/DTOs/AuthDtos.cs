using System;
using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Core.DTOs
{
    public class LoginDto
    {
        [Required]
        public string Document { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        [Required]
        public string Document { get; set; } = string.Empty;
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string? Phone { get; set; }
        
        public string? Address { get; set; }
        
        public DateTime? BirthDate { get; set; }
        
        public decimal Salary { get; set; }
        
        public string? ProfessionalProfile { get; set; }
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
