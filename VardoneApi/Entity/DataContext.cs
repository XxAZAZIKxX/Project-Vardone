using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models;

namespace VardoneApi.Entity
{
    public sealed class DataContext : DbContext
    {
        private readonly string _connectionString;
        public DbSet<Users> Users { get; set; }
        public DbSet<Tokens> Tokens { get; set; }

        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}