using JohnChum.SharedKernel.Domain.Common;
using Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Laundry_Notification.Domain.Entities
{
    public class Notification : AggregateRoot
    {

        public string UserId { get; set; } = null!;

        public string? TemplateId { get; set; }

        public string FirebaseToken { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Body { get; set; } = null!;

        public string? Data { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Sent";

        //public NotificationTemplate? Template { get; set; }

        public Dictionary<string, object>? GetDataAsDictionary()
        {
            return string.IsNullOrEmpty(Data) ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(Data);
        }

        protected override bool TryApplyDomainEvent(INotification domainEvent)
        {
            throw new NotImplementedException();
        }
    }
}
