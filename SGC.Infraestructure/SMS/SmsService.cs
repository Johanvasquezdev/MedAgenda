using Microsoft.Extensions.Configuration;
using SGC.Domain.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace SGC.Infraestructure.SMS
{
    // Servicio de SMS para enviar notificaciones a los pacientes utilizando Twilio
    public class SmsService : ISmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;

        public SmsService(IConfiguration config)
        {
            _accountSid = config["Twilio:AccountSid"]!;
            _authToken = config["Twilio:AuthToken"]!;
            _fromNumber = config["Twilio:FromNumber"]!;
            TwilioClient.Init(_accountSid, _authToken);
        }

        // Método privado para enviar un SMS genérico utilizando Twilio
        private async Task EnviarAsync(string telefono, string mensaje)
        {
            await MessageResource.CreateAsync(
                body: mensaje,
                from: new Twilio.Types.PhoneNumber(_fromNumber),
                to: new Twilio.Types.PhoneNumber(telefono));
        }

        // Métodos para enviar diferentes tipos de mensajes SMS a los pacientes
        public async Task EnviarRecordatorioAsync(
            string telefono, string mensaje)
        {
            await EnviarAsync(telefono, mensaje);
        }

        public async Task EnviarAlertaUrgente(
            string telefono, string mensaje)
        {
            await EnviarAsync(telefono, $"🚨 URGENTE: {mensaje}");
        }

        public async Task EnviarVerificacionAsync(
            string telefono, string codigo)
        {
            await EnviarAsync(telefono,
                $"Tu código de verificación MedAgenda es: {codigo}");
        }
    }
}
