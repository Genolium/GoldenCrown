using Microsoft.EntityFrameworkCore;
using GoldenCrown.Models;

namespace GoldenCrown.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи User -> Account (1 ко многим)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Accounts)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);

            // Настройка связи Transaction -> User (Отправитель)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Sender)
                .WithMany() // У юзера нет списка "исходящих транзакций" в модели, поэтому пусто
                .HasForeignKey(t => t.SenderId)
                .OnDelete(DeleteBehavior.Restrict); // Запрещаем каскадное удаление

            // Настройка связи Transaction -> User (Получатель)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Receiver)
                .WithMany()
                .HasForeignKey(t => t.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Точность для денег
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            // Seed данные
            var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var userId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            var adminAccountId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var userAccountId = Guid.Parse("44444444-4444-4444-4444-444444444444");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminId,
                    Login = "admin",
                    Name = "Мистер Администратор",
                    Password = "adminpassword"
                },
                new User
                {
                    Id = userId,
                    Login = "user",
                    Name = "Обычный Пользователь",
                    Password = "userpassword"
                }
            );

            modelBuilder.Entity<Account>().HasData(
                 new Account
                 {
                     Id = adminAccountId,
                     UserId = adminId, // Привязываем к админу
                     Balance = 1000000.00m, // Миллион на счету
                     Currency = "RUB"
                 },
                 new Account
                 {
                     Id = userAccountId,
                     UserId = userId, // Привязываем к обычному юзеру
                     Balance = 100.00m,
                     Currency = "RUB"
                 }
            );
        }
    }
}
