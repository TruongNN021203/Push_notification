using Laundry_Notification.Domain.Entities;
using Laundry_Notification.ShareLibary.Responses;
using Microsoft.Extensions.Configuration;

namespace Laundry_Notification.Application.Interface
{
    public interface INotificationTemplateRepository
    {
        public Task<Response> SaveNotificationTemplateAsync(NotificationTemplate template);
        public Task<NotificationTemplate> GetNotificationTemplateAsync(string templateId);
    }
}
