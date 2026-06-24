using DT_ASPNET.Domain.Notifications;

namespace DT_ASPNET.Application.Notifications;

public record NotificationDto(Guid Id, string Type, string Title, string Body, bool IsRead, DateTime CreatedAt);

public class NotificationService(INotificationRepository notifications) : INotificationService
{
    public async Task<List<NotificationDto>> GetMyNotificationsAsync(Guid userId)
    {
        var list = await notifications.GetByUserAsync(userId);
        return list.Select(n => new NotificationDto(
            n.Id, n.Type.ToString(), n.Title, n.Body, n.IsRead, n.CreatedAt)).ToList();
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        await notifications.MarkAsReadAsync(notificationId);
        await notifications.SaveChangesAsync();
    }
}
