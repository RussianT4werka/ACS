using ACS_API.Tools;
using System.Net.Http;
using System.Net.Http.Json;

class Program
{
    static HttpClient httpClient = new HttpClient();
    static async Task Main(string[] args)
    {
        string jsonSigur = "{\r\n\"logs\": [\r\n{\r\n\"logId\": 925244,\r\n\"time\": 1510826281,\r\n\"empId\": \"\",\r\n\"internalEmpId\": 0,\r\n\"accessPoint\": 1,\r\n\"direction\": 2,\r\n\"keyHex\": \"5AB860\"\r\n}}";
        Console.WriteLine("Нажмите \"Enter\" для начала теста");
        if (Console.ReadKey().Key == ConsoleKey.Enter)
        {
            try
            {
                var post = await httpClient.PostAsJsonAsync($"http://10.10.1.7:7123/api/Events/GetEvent?jsonSigur={jsonSigur}", "");
                Console.WriteLine(post);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex}");
            }
            Console.ReadLine();
        }
        
    }

}
