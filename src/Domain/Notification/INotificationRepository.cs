namespace DT_ASPNET.Domain.Notification;

public interface INotificationRepository
{
    Task<List<Notification>> GetByUserAsync(Guid userId);
    Task AddAsync(Notification notification);
    Task MarkAsReadAsync(Guid notificationId);
    Task SaveChangesAsync();
}
