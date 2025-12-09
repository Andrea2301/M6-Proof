using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using TalentoPlus.Core.Interfaces;
using TalentoPlus.Infrastructure.Configuration;

namespace TalentoPlus.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string employeeName)
        {
            var subject = "¡Bienvenido a TalentoPlus!";
            var body = GetWelcomeEmailTemplate(employeeName);
            
            return await SendEmailAsync(toEmail, subject, body, true);
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                if (isHtml)
                {
                    bodyBuilder.HtmlBody = body;
                }
                else
                {
                    bodyBuilder.TextBody = body;
                }

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                
                // Conectar al servidor SMTP
                await client.ConnectAsync(
                    _emailSettings.SmtpServer, 
                    _emailSettings.SmtpPort, 
                    _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None
                );

                // Autenticarse
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);

                // Enviar el correo
                await client.SendAsync(message);

                // Desconectar
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                // Log del error (puedes usar ILogger si lo prefieres)
                Console.WriteLine($"Error al enviar correo: {ex.Message}");
                return false;
            }
        }

        private string GetWelcomeEmailTemplate(string employeeName)
        {
            return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .container {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border-radius: 10px;
            padding: 30px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
        }}
        .content {{
            background: white;
            border-radius: 8px;
            padding: 30px;
            margin-top: 20px;
        }}
        h1 {{
            color: white;
            margin: 0;
            font-size: 28px;
        }}
        h2 {{
            color: #667eea;
            margin-top: 0;
        }}
        .highlight {{
            background-color: #f0f4ff;
            padding: 15px;
            border-left: 4px solid #667eea;
            margin: 20px 0;
        }}
        .footer {{
            text-align: center;
            color: white;
            margin-top: 20px;
            font-size: 14px;
        }}
        .button {{
            display: inline-block;
            padding: 12px 30px;
            background-color: #667eea;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1> TalentoPlus</h1>
        
        <div class='content'>
            <h2>¡Bienvenido/a, {employeeName}!</h2>
            
            <p>Nos complace informarte que tu registro en la plataforma <strong>TalentoPlus</strong> se ha completado exitosamente.</p>
            
            <div class='highlight'>
                <strong>✅ Tu cuenta ha sido creada correctamente</strong>
                <p>Podrás acceder a la plataforma una vez que el administrador de recursos humanos habilite tu cuenta.</p>
            </div>
            
            <p><strong>¿Qué puedes hacer en TalentoPlus?</strong></p>
            <ul>
                <li>Consultar tu información personal</li>
                <li>Descargar tu hoja de vida en PDF</li>
                <li>Mantener actualizados tus datos</li>
            </ul>
            
            <p>Si tienes alguna pregunta o necesitas asistencia, no dudes en contactar al departamento de Recursos Humanos.</p>
            
            <p style='margin-top: 30px;'>
                Saludos cordiales,<br>
                <strong>Equipo TalentoPlus</strong>
            </p>
        </div>
        
        <div class='footer'>
            <p>Este es un correo automático, por favor no responder.</p>
            <p>© 2024 TalentoPlus - Sistema de Gestión de Recursos Humanos</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}