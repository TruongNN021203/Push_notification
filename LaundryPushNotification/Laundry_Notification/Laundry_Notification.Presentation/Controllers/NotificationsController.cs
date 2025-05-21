using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Laundry_Notification.Application.DTOs;
using Laundry_Notification.Application.Interfaces;
using Laundry_Notification.Domain.Entities;
using Laundry_Notification.Application.Interface;

namespace Laundry_Notification.Presentation.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly INotificationTemplateRepository _notificationTemplateRepository;

        public NotificationsController(
            INotificationService notificationService,
            INotificationTemplateRepository notificationTemplateRepository)
        {
            _notificationService = notificationService;
            _notificationTemplateRepository = notificationTemplateRepository;
        }

        [HttpPost("pushNotification")]
        public async Task<IActionResult> SendNotification(
       [FromBody] NotificationRequest notificationRequest)
        {
            var result = await _notificationService.SendNotificationAsync(notificationRequest);
            return result.Flag ? Ok("Notification sent") : BadRequest("Failed to send");
        }

        [HttpPost("saveNotificationTemplate")]
        public async Task<IActionResult> PushNotificationTemplate([FromBody] NotificationTemplate template)
        {
            var result = await _notificationTemplateRepository.SaveNotificationTemplateAsync(template);
            return result.Flag ? Ok("Notification template saved") : BadRequest("Failed to save template");
        }
    }
}
