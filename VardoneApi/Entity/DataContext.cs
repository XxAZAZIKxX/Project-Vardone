using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models;

namespace VardoneApi.Entity
{
    public sealed class DataContext : DbContext
    {
        private readonly string _connectionString;
        public DbSet<Users> Users { get; set; }
        public DbSet<UserInfos> UserInfos { get; set; }
        public DbSet<Tokens> Tokens { get; set; }
        public DbSet<FriendsList> FriendsList { get; set; }

        public DataContext(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Default Property
            modelBuilder.Entity<UserInfos>().Property(p => p.Avatar).HasDefaultValue();
            modelBuilder.Entity<UserInfos>().Property(p => p.Description).HasDefaultValue();
            //Unique
            modelBuilder.Entity<Users>().HasIndex(p => p.Username).IsUnique();
            //Foreign
            modelBuilder.Entity<Users>().HasOne(p => p.Info).WithOne().OnDelete(DeleteBehavior.SetNull);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}