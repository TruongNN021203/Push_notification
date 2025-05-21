using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laundry_Notification.Application.DTOs
{
    public class NotificationRequest
    {
        public string FirebaseToken { get; set; } = default!;
        public string TemplateId { get; set; } = default!;
        public string Body { get; set; } = default!;
        public Dictionary<string, string>? Data
        {
            get; set;
        } = default!;
    }
}
