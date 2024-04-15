using Dental_Manager.Models;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace Dental_Manager.PatientApiController.Mail
{
    public class SendMail
    {
        private readonly QlkrContext _context;

        public SendMail(QlkrContext context)
        {
            _context = context;
        }

        public async Task SendAppoinmentConfirmationEmail(string clientEmail, Appointment registrationModel)
        {
            try
            {
                var clientMessage = new MimeMessage();

                clientMessage.From.Add(new MailboxAddress("Appoinmet Confirmation", "voankhanh0@gmail.com"));
                clientMessage.Subject = "Appoinmet Confirmation";

                var bookingDate = registrationModel.AppointmentDate?.ToString("yyyy-MM-dd");
                var currentTime = DateTime.Now.ToString("HH:mm");

                var branch = await _context.Clinics.FindAsync(registrationModel.ClinicId);

                clientMessage.Body = new TextPart("html")
                {
                    Text = $@"
                    <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                    <html xmlns=""http://www.w3.org/1999/xhtml"">
                    <head>
                        <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
                        <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title></title>
                    </head>
                    <body style=""margin:0;"">
                        <center style=""width: 100%; table-layout: fixed; background-color: #e5dec7;"" class=""wrapper"">
                            <table class=""main"" style=""border-spacing: 0;width: 100%; max-width: 500px; background-color: black; font-family: sans-serif; color: #4a4a4a; box-shadow: 0 0 25px rgba(0, 0, 0, .15);"" width=""100%"">
                                <tr>
                                    <td style=""padding: 5px; background-color: white;"">
                                        <table width=""100%"">
                                            <tr>
                                                <h3 style=""font-family: 'Roboto Condensed', sans-serif; font-size: 28px; text-align: center; color: #c89800;"">CUSTOMER INFORMATION</h3>
                                                <td class=""two-cols"" style=""padding: 0 0 0;"">
                                                    <table style=""width: 100%; max-width: 242px; display: inline-block; vertical-align: top;"" class=""col"">
                                                        <tr>
                                                            <td style=""padding: 0;"" class=""padding"">
                                                                <table style=""border-spacing: 0;font-size: 0;"" class=""content"">
                                                                    <tr>
                                                                        <p style=""font-family: 'Roboto Condensed', sans-serif; font-size: 16px; color: #4a4a4a;""><strong>Name:</strong> {registrationModel.Name}</p>
                                                                        <p style=""font-family: 'Roboto Condensed', sans-serif; font-size: 16px; color: #4a4a4a;""><strong>Phone number:</strong>{registrationModel.Phone}</p>
                                                                        <p style=""font-family: 'Roboto Condensed', sans-serif; font-size: 16px; color: #4a4a4a;""><strong>Booking date: </strong>{bookingDate} / {currentTime}</p>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                       <table style=""border-spacing: 0;width: 100%; max-width: 242px; display: inline-block; vertical-align: top;"" class=""col"">
                                                        <tr>
                                                            <td style=""padding: 0;""  class=""padding"">
                                                                <table style=""border-spacing: 0; font-size: 0;"" class=""content"">
                                                                    <tr>
                                                                        <p style=""font-family: 'Roboto Condensed', sans-serif; font-size: 16px; color: #4a4a4a;""><strong>Notes:</strong> {registrationModel.Note}</p>
                                                                        <p style=""font-family: 'Roboto Condensed', sans-serif; font-size: 16px; color: #4a4a4a;""><strong>Branch:</strong> {branch?.ClinicAddress}</p>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </center>
                    </body>
                    </html>"
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("voankhanh0@gmail.com", "seef sxno bsef ufyq");

                    clientMessage.To.Add(new MailboxAddress(clientEmail, clientEmail));

                    await client.SendAsync(clientMessage);

                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }

        public async Task SendAppoinmentNotificationEmail(string staffEmail, Appointment registrationModel)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress("Appoinment Notify", "voankhanh0@gmail.com"));
                message.Subject = "Appoinment Notification";

                var branch = await _context.Clinics.FindAsync(registrationModel.ClinicId);
                var bookingDate = registrationModel.AppointmentDate?.ToString("yyyy-MM-dd");
                var currentTime = DateTime.Now.ToString("HH:mm");

                message.Body = new TextPart("html")
                {
                    Text = $@"
                    <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                    <html xmlns=""http://www.w3.org/1999/xhtml"">
                    <head>
                        <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
                        <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title></title>
                    </head>
                    <body style=""margin:0;"">
                        <center style=""width: 100%; table-layout: fixed; background-color: #e5dec7;"" class=""wrapper"">
                            <table class=""main"" style=""border-spacing: 0;width: 100%; max-width: 500px; background-color: black; font-family: sans-serif; color: #4a4a4a; box-shadow: 0 0 25px rgba(0, 0, 0, .15);"" width=""100%"">
                                <tr>
                                    <td style=""padding: 5px; background-color: white;"">
                                        <table width=""100%"">
                                            <tr>
                                                <h3 style=""font-family: 'Roboto Condensed', sans-serif; font-size: 28px; text-align: center; color: #c89800; text-transform: uppercase;"">you have a customer booking</h3>
                                                <td class=""two-cols"" style=""padding: 0 0 0;"">
                                                    <table style=""width: 100%; max-width: 242px; display: inline-block; vertical-align: top;"" class=""col"">
                                                        <tr>
                                                            <td style=""padding: 0;"" class=""padding"">
                                                                <table style=""border-spacing: 0;font-size: 0;"" class=""content"">
                                                                    <tr>
                                                                        <p style=""font-family: 'Roboto Condensed', sans-serif; font-size: 16px; color: #4a4a4a;""><strong>Name:</strong> {registrationModel.Name}</p>
                                                                        <p style=""font-family: 'Roboto Condensed', sans-serif; font-size: 16px; color: #4a4a4a;""><strong>Phone number:</strong>{registrationModel.Phone}</p>
                                                                        <p style=""font-family: 'Roboto Condensed', sans-serif; font-size: 16px; color: #4a4a4a;""><strong>Booking date: </strong>{bookingDate} /  {currentTime}</p>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                       <table style=""border-spacing: 0;width: 100%; max-width: 242px; display: inline-block; vertical-align: top;"" class=""col"">
                                                        <tr>
                                                            <td style=""padding: 0;""  class=""padding"">
                                                                <table style=""border-spacing: 0; font-size: 0;"" class=""content"">
                                                                    <tr>
                                                                        <p style=""font-family: 'Roboto Condensed', sans-serif; font-size: 16px; color: #4a4a4a;""><strong>Notes:</strong> {registrationModel.Note}</p>
                                                                        <p style=""font-family: 'Roboto Condensed', sans-serif; font-size: 16px; color: #4a4a4a;""><strong>Branch:</strong> {branch?.ClinicAddress}</p>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </center>
                    </body>
                    </html>"
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("voankhanh0@gmail.com", "seef sxno bsef ufyq");

                    message.To.Add(new MailboxAddress(staffEmail, staffEmail));

                    await client.SendAsync(message);

                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }
    }
}
