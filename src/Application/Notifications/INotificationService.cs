namespace DT_ASPNET.Application.Notifications;

public interface INotificationService
{
    Task<List<NotificationDto>> GetMyNotificationsAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId);
}