using ACS_API.Tools;
using System.Net.Http;
using System.Net.Http.Json;

class Program
{
    static HttpClient httpClient = new HttpClient();
    static async Task Main(string[] args)
    {
        var jsonSigur = new ResponseSigur()
        {
            confirmedLogId = 222,
        };
        if (Console.ReadKey().Key == ConsoleKey.Enter)
        {
            try
            {
                var post = httpClient.PostAsJsonAsync("http://10.10.1.7:7123/api/Events/GetEvent?jsonSigur=", jsonSigur);
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
