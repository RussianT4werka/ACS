using System.Net.Mail;
using System.Net;
using LibraryBD.BD;
using System.Net.Http;

namespace ACS_API.Tools
{
    public class Mail
    {
        static HttpClient httpClient = new HttpClient();
        public static async Task SendEmailAsync(string toEmail)
        {
            try
            {
                MailAddress from = new MailAddress("sanya.kokorin.04@mail.ru", "Администратор");
                MailAddress to = new MailAddress(toEmail);
                MailMessage m = new MailMessage(from, to);
                m.Subject = "Уведомление от СКУД";
                m.Body = "Вам создали аккаунт администратора web-интерфейса СКУД";
                SmtpClient smtp = new SmtpClient("smtp.mail.ru", 465);
                smtp.Credentials = new NetworkCredential("sanya.kokorin.04@mail.ru", "wePYp0HfcyHEL21ua9En");
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(m);
            }
            catch(Exception ex)
            {
                var log = new Log() { Title = $"Ошибка отправки email: {ex.Message}", DateTime = DateTime.Now };
                httpClient.PostAsJsonAsync("http://10.10.1.7:7123/api/Logs/WriteLog", log);
            }
        }
    }
}
