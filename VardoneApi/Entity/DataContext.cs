using System;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models.Guilds;
using VardoneApi.Entity.Models.PrivateChats;
using VardoneApi.Entity.Models.Users;

namespace VardoneApi.Entity
{
    public sealed class DataContext : DbContext
    {
        private readonly string _connectionString;
        public DbSet<UsersTable> Users { get; set; }
        public DbSet<UserInfosTable> UserInfos { get; set; }
        public DbSet<TokensTable> Tokens { get; set; }
        public DbSet<FriendsListTable> FriendsList { get; set; }
        public DbSet<PrivateChatsTable> PrivateChats { get; set; }
        public DbSet<PrivateMessagesTable> PrivateMessages { get; set; }
        public DbSet<UsersOnlineTable> UsersOnline { get; set; }
        public DbSet<GuildsTable> Guilds { get; set; }
        public DbSet<ChannelsTable> Channels { get; set; }
        public DbSet<MembersTable> Members { get; set; }
        public DbSet<GuildInfosTable> GuildInfos { get; set; }

        public DataContext(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Default Property
            modelBuilder.Entity<UserInfosTable>().Property(p => p.Avatar).HasDefaultValue();
            modelBuilder.Entity<UserInfosTable>().Property(p => p.Description).HasDefaultValue();
            modelBuilder.Entity<GuildInfosTable>().Property(p => p.Avatar).HasDefaultValue();
            modelBuilder.Entity<PrivateMessagesTable>().Property(p => p.Image).HasDefaultValue();
            //Unique
            modelBuilder.Entity<UsersTable>().HasIndex(p => p.Email).IsUnique();
            modelBuilder.Entity<UsersTable>().HasIndex(p => p.Username).IsUnique();
            //Foreign
            modelBuilder.Entity<UsersTable>().HasOne(p => p.Info).WithOne().OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<GuildsTable>().HasOne(p => p.Info).WithOne().OnDelete(DeleteBehavior.SetNull);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}