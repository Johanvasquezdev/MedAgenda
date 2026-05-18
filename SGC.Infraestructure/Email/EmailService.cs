using Microsoft.Extensions.Configuration;
using SGC.Domain.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace SGC.Infraestructure.Email
{
    // Servicio de correo electrónico para enviar notificaciones a los pacientes
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private static readonly HttpClient _httpClient = new();

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        // Método privado para enviar un correo electrónico genérico utilizando la API HTTP de Brevo
        private async Task EnviarAsync(string destinatario,
            string asunto, string cuerpo)
        {
            // Siempre mostramos una vista previa en la consola de desarrollo para facilitar el debugging local sin depender de la red/IP
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine($"[EMAIL SIMULADO - MEDAGENDA]");
            Console.WriteLine($"Destinatario : {destinatario}");
            Console.WriteLine($"Asunto       : {asunto}");
            Console.WriteLine($"Vista Previa :");
            // Extraer link de reset si existe
            if (cuerpo.Contains("href='"))
            {
                var startIndex = cuerpo.IndexOf("href='") + 6;
                var endIndex = cuerpo.IndexOf("'", startIndex);
                var link = cuerpo.Substring(startIndex, endIndex - startIndex);
                Console.WriteLine($"👉 LINK DETECTADO: {link}");
            }
            Console.WriteLine(new string('=', 60) + "\n");

            var apiKey = _config["Brevo:ApiKey"]
                ?? throw new InvalidOperationException("Brevo:ApiKey no configurado");
            var senderEmail = _config["Brevo:SenderEmail"]
                ?? throw new InvalidOperationException("Brevo:SenderEmail no configurado");
            var senderName = _config["Brevo:SenderName"]
                ?? throw new InvalidOperationException("Brevo:SenderName no configurado");

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
            requestMessage.Headers.Add("accept", "application/json");
            requestMessage.Headers.Add("api-key", apiKey);

            var payload = new
            {
                sender = new { name = senderName, email = senderEmail },
                to = new[] { new { email = destinatario } },
                subject = asunto,
                htmlContent = cuerpo
            };

            var json = JsonSerializer.Serialize(payload);
            requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Error al enviar email por Brevo API. Status: {response.StatusCode}. Detalle: {errorResponse}");
            }
        }

        // Mtodos para enviar confirmaciones, recordatorios de cita y cancelaciones de cita a los pacientes
        public async Task EnviarConfirmacionCitaAsync(string email,
            string nombrePaciente, DateTime fechaCita)
        {
            var asunto = "Confirmacin de Cita  MedAgenda";
            var cuerpo = $@"
                <h2>Hola {nombrePaciente}</h2>
                <p>Tu cita ha sido confirmada para el 
                   <strong>{fechaCita:dd/MM/yyyy HH:mm}</strong></p>
                <p>Gracias por usar MedAgenda.</p>";
            await EnviarAsync(email, asunto, cuerpo);
        }

      
        public async Task EnviarRecordatorioCitaAsync(string email,
            string nombrePaciente, DateTime fechaCita)
        {
            var asunto = "Recordatorio de Cita  MedAgenda";
            var cuerpo = $@"
                <h2>Hola {nombrePaciente}</h2>
                <p>Te recordamos que tienes una cita maana 
                   <strong>{fechaCita:dd/MM/yyyy HH:mm}</strong></p>
                <p>Gracias por usar MedAgenda.</p>";
            await EnviarAsync(email, asunto, cuerpo);
        }

        public async Task EnviarCancelacionCitaAsync(string email,
            string nombrePaciente, DateTime fechaCita)
        {
            var asunto = "Cancelación de Cita — MedAgenda";
            var cuerpo = $@"
                <h2>Hola {nombrePaciente}</h2>
                <p>Tu cita del 
                   <strong>{fechaCita:dd/MM/yyyy HH:mm}</strong> 
                   ha sido cancelada.</p>
                <p>Gracias por usar MedAgenda.</p>";
            await EnviarAsync(email, asunto, cuerpo);
        }

        public async Task EnviarPasswordResetAsync(string email, string resetLink)
        {
            var asunto = "Recuperación de Contraseña — MedAgenda";
            var cuerpo = $@"
                <h2>Solicitud de cambio de contraseña</h2>
                <p>Hemos recibido una solicitud para restablecer tu contraseña en MedAgenda.</p>
                <p>Haz clic en el siguiente enlace para cambiar tu contraseña:</p>
                <p><a href='{resetLink}' style='padding: 10px 20px; background-color: #10b981; color: white; text-decoration: none; border-radius: 5px; display: inline-block; margin-top: 10px;'>Restablecer Contraseña</a></p>
                <p>Si no solicitaste este cambio, puedes ignorar este correo.</p>";
            await EnviarAsync(email, asunto, cuerpo);
        }
        public async Task EnviarBienvenidaAsync(string email, string nombrePaciente)
        {
            var asunto = "Bienvenido a MedAgenda";
            var cuerpo = $@"
                <div style='font-family:Inter,sans-serif;max-width:600px;margin:auto;background:#0f172a;color:#f1f5f9;border-radius:12px;overflow:hidden;'>
                  <div style='background:linear-gradient(135deg,#064e3b,#059669);padding:40px 32px;text-align:center;'>
                    <h1 style='margin:0;font-size:28px;color:#fff;'>Bienvenido, {nombrePaciente}</h1>
                    <p style='margin:8px 0 0;color:#a7f3d0;font-size:15px;'>Tu cuenta en MedAgenda está lista</p>
                  </div>
                  <div style='padding:32px;'>
                    <p style='color:#cbd5e1;'>Estamos emocionados de tenerte con nosotros. Con MedAgenda puedes:</p>
                    <ul style='color:#94a3b8;line-height:2;'>
                      <li>📅 Agendar citas médicas en minutos</li>
                      <li>🔔 Recibir recordatorios automáticos</li>
                      <li>💳 Gestionar tus pagos de forma segura</li>
                      <li>🤖 Consultar a nuestro asistente virtual 24/7</li>
                    </ul>
                    <div style='text-align:center;margin-top:28px;'>
                      <a href='http://localhost:3000/paciente/dashboard' style='background:#10b981;color:#fff;padding:14px 32px;border-radius:8px;text-decoration:none;font-weight:600;font-size:15px;display:inline-block;'>
                        Ir a mi dashboard
                      </a>
                    </div>
                  </div>
                  <div style='padding:20px 32px;border-top:1px solid #1e293b;text-align:center;color:#475569;font-size:12px;'>
                    MedAgenda · medagenda.me · Si no creaste esta cuenta, ignora este correo.
                  </div>
                </div>";
            await EnviarAsync(email, asunto, cuerpo);
        }

        public async Task EnviarCambioHorarioAsync(string email, string nombrePaciente, string nombreMedico, DateTime fechaAnterior, DateTime fechaNueva)
        {
            var asunto = "Cambio de horario en tu cita — MedAgenda";
            var cuerpo = $@"
                <div style='font-family:Inter,sans-serif;max-width:600px;margin:auto;background:#0f172a;color:#f1f5f9;border-radius:12px;overflow:hidden;'>
                  <div style='background:linear-gradient(135deg,#1e3a5f,#2563eb);padding:40px 32px;text-align:center;'>
                    <h1 style='margin:0;font-size:24px;color:#fff;'>Cambio de Horario</h1>
                    <p style='margin:8px 0 0;color:#bfdbfe;font-size:15px;'>Tu cita con Dr. {nombreMedico} ha sido reprogramada</p>
                  </div>
                  <div style='padding:32px;'>
                    <p style='color:#cbd5e1;'>Hola <strong>{nombrePaciente}</strong>, tu cita ha sido reprogramada con los siguientes cambios:</p>
                    <div style='background:#1e293b;border-radius:8px;padding:20px;margin:20px 0;'>
                      <div style='display:flex;gap:16px;'>
                        <div style='flex:1;'>
                          <p style='margin:0;color:#64748b;font-size:12px;text-transform:uppercase;'>Fecha anterior</p>
                          <p style='margin:4px 0 0;color:#f87171;font-weight:600;font-size:16px;'>{fechaAnterior:dd MMM yyyy · HH:mm}</p>
                        </div>
                        <div style='flex:1;'>
                          <p style='margin:0;color:#64748b;font-size:12px;text-transform:uppercase;'>Nueva fecha</p>
                          <p style='margin:4px 0 0;color:#34d399;font-weight:600;font-size:16px;'>{fechaNueva:dd MMM yyyy · HH:mm}</p>
                        </div>
                      </div>
                    </div>
                    <p style='color:#94a3b8;font-size:14px;'>Si tienes alguna duda, puedes cancelar la cita desde tu panel o contactarnos.</p>
                  </div>
                  <div style='padding:20px 32px;border-top:1px solid #1e293b;text-align:center;color:#475569;font-size:12px;'>
                    MedAgenda · medagenda.me
                  </div>
                </div>";
            await EnviarAsync(email, asunto, cuerpo);
        }

        public async Task EnviarVerificacionEmailAsync(string email, string nombrePaciente, string verificationLink)
        {
            var asunto = "Verifica tu correo — MedAgenda";
            var cuerpo = $@"
                <div style='font-family:Inter,sans-serif;max-width:600px;margin:auto;background:#0f172a;color:#f1f5f9;border-radius:12px;overflow:hidden;'>
                  <div style='background:linear-gradient(135deg,#064e3b,#059669);padding:40px 32px;text-align:center;'>
                    <div style='font-size:48px;'>✉️</div>
                    <h1 style='margin:12px 0 0;font-size:24px;color:#fff;'>Verifica tu correo</h1>
                  </div>
                  <div style='padding:32px;text-align:center;'>
                    <p style='color:#cbd5e1;'>Hola <strong>{nombrePaciente}</strong>, solo falta un paso para activar tu cuenta en MedAgenda.</p>
                    <p style='color:#94a3b8;font-size:14px;'>Este enlace expirará en 24 horas.</p>
                    <div style='margin:28px 0;'>
                      <a href='{verificationLink}' style='background:#10b981;color:#fff;padding:14px 32px;border-radius:8px;text-decoration:none;font-weight:600;font-size:15px;display:inline-block;'>
                        Verificar mi correo
                      </a>
                    </div>
                    <p style='color:#475569;font-size:13px;'>Si no creaste esta cuenta, puedes ignorar este correo de forma segura.</p>
                  </div>
                  <div style='padding:20px 32px;border-top:1px solid #1e293b;text-align:center;color:#475569;font-size:12px;'>
                    MedAgenda · medagenda.me
                  </div>
                </div>";
            await EnviarAsync(email, asunto, cuerpo);
        }
    }
}
