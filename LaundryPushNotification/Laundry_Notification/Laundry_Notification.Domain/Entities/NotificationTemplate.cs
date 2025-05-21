using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Laundry_Notification.Domain.Entities
{
    public class NotificationTemplate
    {
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Body { get; set; } = null!;

        public string? Data { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Dictionary<string, object>? GetDataAsDictionary()
        {
            return string.IsNullOrEmpty(Data) ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(Data);
        }

    }
}
