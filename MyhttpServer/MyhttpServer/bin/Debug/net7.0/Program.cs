using MyHttpServer.Models;
using MyHttpServer.Services;
using System.Net;
using System.Text;
using System.Text.Json;

namespace MyHttpServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //var mailService = new MailService();
            //mailService.SendAsync();

           
            AppConfig config = null;
            if (File.Exists("config.json"))
            {
                var fileConfig = await File.ReadAllTextAsync("config.json");
                config = JsonSerializer.Deserialize<AppConfig>(fileConfig);
            }
            else
            {
                Console.WriteLine("файл конфигурации сервера 'config.json' не найден");
                config = new AppConfig();
            }
            var prefixes = new[] { $"http://{config.Domain}:{config.Port}/" };
            string path = Directory.GetCurrentDirectory();
            string sum = $"{path}\\{config.StaticDirectoryPath}";
            var server = new HttpServer(prefixes, sum);

            await server.StartAsync();
            
        }
    }
}