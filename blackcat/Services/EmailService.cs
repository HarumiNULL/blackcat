using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace blackcat.Services;

public class EmailService
{
    public static async Task EnviarCorreo(string email, string subject, string body)
    {
        var e = new MimeMessage();
        e.From.Add(MailboxAddress.Parse("oficial7blackcat@gmail.com"));
        e.To.Add(MailboxAddress.Parse(email));
        e.Subject = subject;
        e.Body = new TextPart(TextFormat.Html) { Text = body };
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync("oficial7blackcat@gmail.com", "jnhdxrxrixqvalen");
        await smtp.SendAsync(e);
        await smtp.DisconnectAsync(true);
    }

    public async Task EnviarCorreoRecuperacion(string email, string name, string activationToken)
    {
        // Construye la URL completa con el token
        string urlRecuperacion = $"https://blackcat-dnc7cye0hagabucb.eastus2-01.azurewebsites.net/User/RecuperarContrasena?token={activationToken}";

        // Usa $ para string interpolation y @ para verbatim string (evita escapar comillas)
        string body = $@"<!DOCTYPE html>
        <html>
        <head>
            <meta charset=""utf-8"" />
            <title>Recuperación de contraseña</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    line-height: 1.6;
                    color: #48ADBC;
                }}
                .container {{
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 20px;
                }}
                .button {{
                    display: inline-block;
                    padding: 10px 20px;
                    background-color: #48ADBC;
                    color: white !important;
                    text-decoration: none;
                    border-radius: 5px;
                    margin: 15px 0;
                }}
                .footer {{
                    margin-top: 30px;
                    font-size: 0.9em;
                    color: #A1D7D0;
                }}
            </style>
        </head>
        <body>
            <div class=""container"">
                <h2>Hola, {name}</h2>
                
                <p>Hemos recibido una solicitud para restablecer tu contraseña. Haz clic en el siguiente botón para continuar con el proceso:</p>
                
                <p>
                    <a href=""{urlRecuperacion}"" class=""button"">Restablecer contraseña</a>
                </p>
                
                <p>Si no puedes hacer clic en el botón, copia y pega la siguiente URL en tu navegador:</p>
                <p><small>{urlRecuperacion}</small></p>
                
                <p>Si no solicitaste este cambio, puedes ignorar este mensaje.</p>
                
                <div class=""footer"">
                    <p>Atentamente,</p>
                    <p>El equipo de soporte</p>
                    <img src=""https://blackcat-dnc7cye0hagabucb.eastus2-01.azurewebsites.net/staticFiles/correo/logo.png"" alt=""Logo"" style=""max-width: 200px; display: block; margin-bottom: 20px;"" />

                </div>
            </div>
        </body>
        </html>";

        await EnviarCorreo(email, "Recupera tu contraseña - BlackCat", body);
    }
}