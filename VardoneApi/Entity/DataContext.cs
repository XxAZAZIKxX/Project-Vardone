using System;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models;

namespace VardoneApi.Entity
{
    public sealed class DataContext : DbContext
    {
        private readonly string _connectionString;
        private readonly object _locker = new();
        public DbSet<UsersTable> Users { get; set; }
        public DbSet<UserInfosTable> UserInfos { get; set; }
        public DbSet<TokensTable> Tokens { get; set; }
        public DbSet<FriendsListTable> FriendsList { get; set; }
        public DbSet<PrivateChatsTable> PrivateChats { get; set; }
        public DbSet<PrivateMessagesTable> PrivateMessages { get; set; }

        public DataContext(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Default Property
            //------------------------------------------------------------------------------------Null
            modelBuilder.Entity<UserInfosTable>().Property(p => p.Avatar).HasDefaultValue();
            modelBuilder.Entity<UserInfosTable>().Property(p => p.Description).HasDefaultValue();
            modelBuilder.Entity<PrivateMessagesTable>().Property(p => p.Image).HasDefaultValue();
            //Unique
            modelBuilder.Entity<UsersTable>().HasIndex(p => p.Email).IsUnique();
            //Foreign
            modelBuilder.Entity<UsersTable>().HasOne(p => p.Info).WithOne().OnDelete(DeleteBehavior.SetNull);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }

        public override int SaveChanges()
        {
            lock (_locker) return base.SaveChanges();
        }
    }
}