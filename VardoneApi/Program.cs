using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using VardoneApi.Entity;
using VardoneApi.Tcp;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Models.TcpModels;

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
            Task.Run(() =>
            {
                Console.ReadKey();

                TcpServer.SendMessageTo(1,
                    new TcpResponseModel
                    {
                        type = TypeTcpResponse.NewChannel,
                        data = new Channel
                        {
                            ChannelId = 1,
                            Guild = new Guild
                            {
                                GuildId = 1,
                                Name = "Tururu"
                            },
                            Name = "Bla bla"
                        }
                    });
                TcpServer.RemoveConnection(1);
            });
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}