using Laundry_Notification.Application.DTOs;
using Laundry_Notification.ShareLibary.Responses;

namespace Laundry_Notification.Application.Interfaces
{
    public interface INotificationService
    {
        public Task<Response> SendNotificationAsync(
           NotificationRequest notificationRequest
        );
    }
}
