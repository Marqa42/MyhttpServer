namespace MyHttpServer.Models
{
    internal sealed class AppConfig
    {
        public string Domain { get; set; } = "localhost";
        public uint Port { get; set; } = 6529;
        public string StaticDirectoryPath { get; set; } = @"public\";
    }
}