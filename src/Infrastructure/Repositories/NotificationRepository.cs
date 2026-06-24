using DT_ASPNET.Domain.Notifications;
using DT_ASPNET.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DT_ASPNET.Infrastructure.Repositories;

public class NotificationRepository(AppDbContext db) : INotificationRepository
{
    public Task<List<Domain.Notifications.Notification>> GetByUserAsync(Guid userId) =>
        db.Notifications.Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public async Task AddAsync(Domain.Notifications.Notification notification) =>
        await db.Notifications.AddAsync(notification);

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var n = await db.Notifications.FindAsync(notificationId);
        if (n is not null)
            n.IsRead = true;
    }

    public Task SaveChangesAsync() =>
        db.SaveChangesAsync();
}
