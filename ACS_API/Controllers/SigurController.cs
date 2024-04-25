using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace InteractionSigurAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SigurController : ControllerBase
    {
        [HttpGet("OpenGate")]
        public async Task<IActionResult> OpenGate(int td) // td - принимаемое значение(id точки доступа на сервере Sigur)
        {
            string response; // переменная хранит в себе ответ от сервера Sigur
            string path = "settings.txt"; // переменная хранит в себе путь до .txt файла с ip-адресом сервера Sigur
            string ipAdress; // переменная хранит в себе ip-адресс из прочитанного файла
            string loginSigur = null; // переменная хранит в себе логин для авторизации
            string passwordSigur = null; // переменная хранит в себе пароль для авторизации

            try
            {
                StreamReader reader = new StreamReader(path); // читаем файл по пути из переменной
                var settings = await reader.ReadToEndAsync(); // читаем содержимое файла
                string[] splitSettings = settings.Split(new char[] { '/' }); // разделяем строку из прочитанного файла разделителем пробел и запихиваем элементы в массив
                ipAdress = splitSettings[0]; // берём нулевой элемент массива, в котром должен находится ip-адрес
                loginSigur = splitSettings[1]; // берём первый элемент массива, в котром должен находится логин от сервера Sigur
                passwordSigur = splitSettings[2]; //берём второй элемент массива, в котром должен находится пароль от сервера Sigur
            }
            catch (Exception ex)
            {
                response = ex.Message; // присваивание текста ошибки переменной ответа от API
                return BadRequest(response); // ответ от сервера с текстом ошибки
            }

            try
            {
                using (TcpClient client = new TcpClient(ipAdress, 3312)) // создание telnet клиента
                using (NetworkStream stream = client.GetStream()) // создание потока с telnet соединением
                using (StreamWriter writer = new StreamWriter(stream)) // создание потока для чтения/отправки байтов в telnet соединении
                {
                    try
                    {
                        string messageLogin = $"LOGIN 1.8 {loginSigur} {passwordSigur}\r\n"; // \r\n обозначает символы возврата каретки и перевода строки, необходимые для telnet протокола
                        byte[] dataLogin = Encoding.ASCII.GetBytes(messageLogin); // конфертация сообщения серверу(utf8) в байты
                        stream.Write(dataLogin, 0, dataLogin.Length); // отправка сообщения в байтовом виде
                    }
                    catch
                    {
                        response = "Ошибка авторизации. Возможно на сервере Sigur установлен пароль, поэтому проверь файл настроек"; //присваивание текста ошибки переменной ответа от API
                        return BadRequest(response); // Ответ от сервера с текстом ошибки
                    }

                    string messagePass = $"ALLOWPASS {td} 3 IN\r\n"; // \r\n обозначает символы возврата каретки и перевода строки, необходимые для telnet протокола
                    byte[] dataPass = Encoding.ASCII.GetBytes(messagePass); // конфертация сообщения серверу(utf8) в байты
                    stream.Write(dataPass, 0, dataPass.Length); // отправка сообщения в байтовом виде

                    byte[] buffer = new byte[client.ReceiveBufferSize]; // создание массива для приема данных от клиента
                    int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize); // Чтение данных из массива в потоке
                    response = Encoding.ASCII.GetString(buffer, 0, bytesRead); // байты, прочитанные из потока, конвертируются в строку

                    client.Close(); // закрытие telnet клиента
                }
                if (response == "OK\r\n") // Если ответ от сервера "OK", то возращается:
                {
                    return Ok(response); // код 200(Ok) с сообщением от сервера
                }
                else // если нет, то возвращается:
                {
                    return BadRequest(response); // код 400(Bad request) с сообщением от сервера
                }
            }
            catch(Exception ex) // формрование ошибки программой
            {
                response = ex.Message; // преобразование ошибки в текст
                return BadRequest(response); // возврат кода 400(BadRequest) с текстом ошибки
            }
        }
    }
}
