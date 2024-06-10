using System.Net.Mail;
using System.Net;
using LibraryBD.BD;
using System.Net.Http;

namespace ACS_API.Tools
{
    public class Mail
    {
        static HttpClient httpClient = new HttpClient();
        public static async Task SendEmailAsync(string toEmail, string login, string password)
        {
            try
            {
                MailAddress from = new MailAddress("sanya.kokorin.04@mail.ru", "Admin");
                MailAddress to = new MailAddress($"{toEmail}");
                MailMessage m = new MailMessage(from, to);
                m.Subject = "СКУД Беатон Владивосток";
                m.Body = "Вам создали аккаунт для администрирования СКУД\n" +
                    $"Ссылка на сайт: http://10.10.1.102:7151/\n" +
                    $"Ваш логин: {login}\n" +
                    $"Ваш временный пароль: {password}\n" +
                    $"Не забудьте сменить пароль в личном кабинете!";
                SmtpClient smtp = new SmtpClient("smtp.mail.ru", 25);
                smtp.Credentials = new NetworkCredential("sanya.kokorin.04@mail.ru", "S3nTM0DyVjE5rQNFcurQ");
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(m);
            }
            catch(Exception ex)
            {
                var log = new Log() { Title = $"Ошибка отправки email: {ex.Message}", DateTime = DateTime.Now };
                httpClient.PostAsJsonAsync("http://10.10.1.102:7123/api/Logs/WriteLog", log);
            }
        }
    }
}
