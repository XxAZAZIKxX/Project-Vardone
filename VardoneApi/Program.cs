using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using VardoneApi.Entity;
using VardoneApi.Entity.Models;

namespace VardoneApi
{
    public static class Program
    {
        public static DataContext DataContext { get; private set; }

        public static void Main(string[] args)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost", UserID = "root", Password = "root", Database = "VardoneApi"
            };
            DataContext = new DataContext(builder.ConnectionString);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
        }
    }
}