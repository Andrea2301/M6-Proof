using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PdfService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateEmployeeResume(Employee employee)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // ===== HEADER =====
                    page.Header()
                        .Padding(15)
                        .Background(Colors.Blue.Medium)
                        .Text(text =>
                        {
                            text.Span($"Hoja de Vida - {employee.FirstName} {employee.LastName}")
                                .FontSize(22)
                                .Bold()
                                .FontColor(Colors.White);
                        });

                    // ===== CONTENT =====
                    page.Content().Column(column =>
                    {
                        column.Spacing(12);

                        // --- Información Personal ---
                        column.Item().Text("Información Personal").FontSize(16).Bold();

                        column.Item().Text(text =>
                        {
                            text.Line($"Documento: {employee.Document}");
                            text.Line($"Nacimiento: {employee.BirthDate:dd/MM/yyyy}");
                            text.Line($"Email: {employee.Email}");
                            text.Line($"Teléfono: {employee.Phone ?? "No especificado"}");
                            text.Line($"Dirección: {employee.Address ?? "No especificada"}");
                        });

                        column.Item().LineHorizontal(1, Unit.Point);

                        // --- Información Laboral ---
                        column.Item().Text("Información Laboral").FontSize(16).Bold();

                        column.Item().Text(text =>
                        {
                            text.Line($"Cargo: {employee.Position?.Name ?? "N/A"}");
                            text.Line($"Departamento: {employee.Department?.Name ?? "N/A"}");
                            text.Line($"Estado: {employee.EmployeeStatus?.Name ?? "N/A"}");
                            text.Line($"Salario: ${employee.Salary:N2}");
                            text.Line($"Ingreso: {employee.HireDate:dd/MM/yyyy}");
                        });

                        column.Item().LineHorizontal(1, Unit.Point);

                        // --- Educación ---
                        column.Item().Text("Educación").FontSize(16).Bold();
                        column.Item().Text($"Nivel Educativo: {employee.EducationLevel?.Name ?? "N/A"}");

                        column.Item().LineHorizontal(1, Unit.Point);

                        // --- Perfil Profesional ---
                        if (!string.IsNullOrWhiteSpace(employee.ProfessionalProfile))
                        {
                            column.Item().Text("Perfil Profesional").FontSize(16).Bold();

                            column.Item().Text(text =>
                            {
                                text.Line(employee.ProfessionalProfile);
                               
                            });

                            column.Item().LineHorizontal(1, Unit.Point);
                        }
                    });

                    // ===== FOOTER =====
                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generado por TalentoPlus - ").FontSize(8);
                            text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(8);
                        });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerateEmployeeResumeAsync(int employeeId)
        {
            var employee = await _unitOfWork.Employees.GetByIdWithDetailsAsync(employeeId)
                ?? throw new Exception($"Empleado con ID {employeeId} no encontrado");

            return GenerateEmployeeResume(employee);
        }
    }
}
