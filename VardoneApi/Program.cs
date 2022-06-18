using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using VardoneApi.Entity;
using VardoneApi.Tcp;

namespace VardoneApi
{
    public static class Program
    {
        private static string ConnectionString { get; set; }
        public static DataContext DataContext => new(ConnectionString);
        public static TcpServerObject TcpServer { get; private set; }

        public static void Main(string[] args)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                UserID = "root",
                Password = "root",
                Database = "VardoneApi"
            };
            ConnectionString = builder.ConnectionString;
            TcpServer = new TcpServerObject();
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}