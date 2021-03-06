using System;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models;

namespace VardoneApi.Entity
{
    public sealed class DataContext : DbContext
    {
        private readonly string _connectionString;
        public DbSet<Users> Users { get; set; }
        public DbSet<UserInfos> UsersInfos { get; set; }
        public DbSet<Tokens> Tokens { get; set; }

        public DataContext(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().HasOne(p => p.Info).WithOne().OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Users>().HasIndex(p => p.Username).IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}