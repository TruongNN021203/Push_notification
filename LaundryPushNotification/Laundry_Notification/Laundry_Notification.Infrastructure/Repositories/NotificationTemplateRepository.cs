using Google.Cloud.Firestore;
using Laundry_Notification.Application.Interface;
using Laundry_Notification.Domain.Entities;
using Laundry_Notification.ShareLibary.Helper;
using Laundry_Notification.ShareLibary.Responses;
using Microsoft.Extensions.Configuration;

namespace Laundry_Notification.Infrastructure.Repositories
{
    public class NotificationTemplateRepository : INotificationTemplateRepository
    {
        private readonly FirestoreDb _firestoreDb;

        public NotificationTemplateRepository(IConfiguration config)
        {
            FirebaseInitializer.Initialize(config);
            _firestoreDb = FirestoreDb.Create(config["Firebase:ProjectId"]);
        }

        public async Task<Response> SaveNotificationTemplateAsync(NotificationTemplate template)
        {
            try
            {
                DocumentReference docRef = _firestoreDb
                    .Collection("notification_templates")
                    .Document(template.Id);
                var data = new Dictionary<string, object>
                {
                    { "id", template.Id },
                    { "title", template.Title },
                    { "body", template.Body },
                    { "data", template.Data! },
                    { "createdAt", Timestamp.FromDateTime(template.CreatedAt.ToUniversalTime()) },
                };

                await docRef.SetAsync(data);
                return new Response(true, "Template saved successfully");
            }
            catch (Exception ex)
            {
                return new Response(false, "Failed to save template: " + ex.Message);
            }
        }

        public async Task<NotificationTemplate> GetNotificationTemplateAsync(string templateId)
        {
            try
            {
                DocumentReference docRef = _firestoreDb
                    .Collection("notification_templates")
                    .Document(templateId);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                    return null!;

                var dict = snapshot.ToDictionary();

                var template = new NotificationTemplate
                {
                    Id = templateId,
                    Title = dict.TryGetValue("title", out var title) ? title?.ToString() ?? "" : "",
                    Body = dict.TryGetValue("body", out var body) ? body?.ToString() ?? "" : "",
                    Data = dict.TryGetValue("data", out var data) ? data?.ToString() : null,
                    CreatedAt = snapshot.ContainsField("createdAt")
                        ? snapshot.GetValue<Timestamp>("createdAt").ToDateTime()
                        : DateTime.UtcNow,
                };

                return template;
            }
            catch
            {
                return null!;
            }
        }
    }
}
