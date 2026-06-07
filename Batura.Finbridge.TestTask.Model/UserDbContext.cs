using Batura.Finbridge.TestTask.Model.Entities.Outbox;
using Batura.Finbridge.TestTask.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Batura.Finbridge.TestTask.Model;

/// <summary>
/// Контекст базы данных для работы с пользователями
/// </summary>
public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    public DbSet<BalanceHistory> BalanceHistories { get; set; }

    public DbSet<IntegrationEventToSend> IntegrationEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Определяем версию строки для оптимистической блокировки
        modelBuilder.Entity<User>()
            .Property(e => e.Version).IsRowVersion();

        modelBuilder.Entity<BalanceHistory>()
            .HasOne(b => b.User)
            .WithMany(u => u.BalanceHistories)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ((EntityBase)entry.Entity).CreatedAtUtc = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    ((EntityBase)entry.Entity).ModifiedAtUtc = DateTime.UtcNow;
                    break;

                default:
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}