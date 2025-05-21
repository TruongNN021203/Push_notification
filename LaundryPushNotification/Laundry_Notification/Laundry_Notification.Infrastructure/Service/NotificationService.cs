using System.Text.RegularExpressions;
using FirebaseAdmin.Messaging;
using Google.Cloud.Firestore;
using Laundry_Notification.Application.DTOs;
using Laundry_Notification.Application.Interface;
using Laundry_Notification.Application.Interfaces;
using Laundry_Notification.Domain.Entities;
using Laundry_Notification.Infrastructure.Repositories;
using Laundry_Notification.ShareLibary.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Laundry_Notification.Infrastructure.Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationTemplateRepository _templateRepository;
        private readonly ILogger<NotificationTemplateRepository> _logger;
        private readonly FirestoreDb _firestoreDb;

        public NotificationService(
            INotificationTemplateRepository templateRepository,
            ILogger<NotificationTemplateRepository> logger,
            FirestoreDb firestoreDb,
            IConfiguration config
        )
        {
            _templateRepository = templateRepository;
            _logger = logger;
            _firestoreDb = FirestoreDb.Create(config["Firebase:ProjectId"]);
        }

        public async Task<Response> SendNotificationAsync(
           NotificationRequest notificationRequest
        )
        {
            var notificationTemplate = await _templateRepository.GetNotificationTemplateAsync(notificationRequest.TemplateId);
            if (notificationTemplate is null)
            {
                return new Response(false, "Template is null, please try again!");
            }

            string result = Regex.Replace(notificationTemplate.Body, @"\{\{\.(\w+)\}\}", match =>
            {
                var key = match.Groups[1].Value;
                return notificationRequest.Data != null && notificationRequest.Data.TryGetValue(key, out var value) ? value : match.Value;
            });


            Domain.Entities.Notification notification = new Domain.Entities.Notification();
            notification.TemplateId = notificationRequest.TemplateId;
            notification.Title = notificationTemplate.Title;
            notification.Body = result;
            notification.Data = result;
            notification.SentAt = DateTime.Now;
            notification.Status = "Sent";

            var message = new Message
            {
                Token = notificationRequest.FirebaseToken,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = notificationTemplate.Title,
                    Body = result,
                },
            };
            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation($"Notification sent: {response}");
                await SaveNotificationAsync(notification!);
                return new Response(true, "Notification sent!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send notification: {ex.Message}");
                return new Response(false, "Sent notification failed, try againF!");
                ;
            }
        }

        public async Task<Response> SaveNotificationAsync(Domain.Entities.Notification notification)
        {
            try
            {
                DocumentReference docRef = _firestoreDb.Collection("notifications").Document();
                var notificationSave = new Dictionary<string, object>
                {
                    { "templateId", notification.TemplateId! },
                    { "title", notification.Title },
                    { "body", notification.Body },
                    { "data", notification.Data! },
                    { "sentAt", DateTime.UtcNow },
                    { "status", "Sent" },
                };

                await docRef.SetAsync(notificationSave);
                return new Response(true, "Template saved successfully");
            }
            catch (Exception ex)
            {
                return new Response(false, "Failed to save template: " + ex.Message);
            }
        }
    }
}
