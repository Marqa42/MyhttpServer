using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using MyHttpServer;
using MyHttpServer.Services;

namespace MyHttpServer
{
    internal sealed class HttpServer
    {
        private readonly HttpListener _listener;
        private readonly string _staticDirectoryPath;

        public HttpServer(string[] prefixes, string staticDirectoryPath)
        {
            _listener = new HttpListener();
            foreach (var prefix in prefixes)
            {
                Console.WriteLine($"Server started on {prefix}");
                _listener.Prefixes.Add(prefix);
            }

            _staticDirectoryPath = staticDirectoryPath;
        }

        public async Task StartAsync()
        {
            _listener.Start();
            while (_listener.IsListening)
            {
                var context = await _listener.GetContextAsync();
                await ProcessRequestAsync(context);
            }
        }
        

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            string? relativePath = context.Request.Url?.AbsolutePath.Trim('/');
            
            var filePath = Path.Combine(_staticDirectoryPath,
                string.IsNullOrEmpty(relativePath) ? "index.html" : relativePath);

            string path = filePath.Split('/').Last();

            
            Console.WriteLine("Server path: " + filePath);
            Console.WriteLine("Requested url: " + context.Request.Url);
            
            
            if (context.Request.HttpMethod == "POST")
            {
                ProcessPostMethod(context, path);
                return;
            }

            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(_staticDirectoryPath, "err404.html");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            byte[] responseFile = await File.ReadAllBytesAsync(filePath);
            context.Response.ContentType = GetContentType(Path.GetExtension(filePath));
            context.Response.ContentLength64 = responseFile.Length;
            await context.Response.OutputStream.WriteAsync(responseFile, 0, responseFile.Length);
            context.Response.OutputStream.Close();
        }


        private string GetContentType(string? extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension), "Extension cannot be null.");
            }

            return extension.ToLower() switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }

        private async void ProcessPostMethod(HttpListenerContext context, string path)
        {
            var request = context.Request;
            var body = new StreamReader(request.InputStream).ReadToEnd();
            var content = body.Split("&"); 
            if (content.Length != 2) return;
            
            var email = content[0].Replace("login=", "").Replace("%40", "@");
            var message = content[1].Replace("password=", "");
            
            if (email.Length == 0 || message.Length == 0 || (email.Contains("@") == false && email.Contains("%40")) ||
                !email.Contains("."))
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }

            Console.WriteLine($"Sending email to: {email}");
            
            MailService mailService = new MailService();

            // Console.WriteLine(email + " | " + message);

            await mailService.SendAsync(email, message, path: _staticDirectoryPath);
            
            context.Response.StatusCode = 200;
            context.Response.Close();
        }

        public void Stop()
        {
            _listener.Stop();
            Console.WriteLine("Server closed");
        }
    }
}