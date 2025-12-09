using Microsoft.EntityFrameworkCore;
using TalentoPlus.Core.Entities;

namespace TalentoPlus.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<EducationLevel> EducationLevels { get; set; }
        public DbSet<EmployeeStatus> EmployeeStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Employee configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Required properties
                entity.Property(e => e.Document)
                    .IsRequired()
                    .HasMaxLength(20);
                    
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(e => e.PasswordHash)
                    .IsRequired();
                    
                // Optional properties
                entity.Property(e => e.Phone)
                    .HasMaxLength(20);
                    
                entity.Property(e => e.Address)
                    .HasMaxLength(200);
                    
                entity.Property(e => e.ProfessionalProfile)
                    .HasMaxLength(500);
                    
                // Foreign key relationships
                entity.HasOne(e => e.Position)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(e => e.PositionId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Employees)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.EducationLevel)
                    .WithMany()
                    .HasForeignKey(e => e.EducationLevelId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.EmployeeStatus)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeStatusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Position configuration
            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // Department configuration
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // EducationLevel configuration
            modelBuilder.Entity<EducationLevel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // EmployeeStatus configuration
            modelBuilder.Entity<EmployeeStatus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}
