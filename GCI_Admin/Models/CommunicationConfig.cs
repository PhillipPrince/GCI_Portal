using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GCI_Admin.Models
{


    public class SmsConfig
        {
            public string BaseUrl { get; set; }
            public string ApiKey { get; set; }
            public string PartnerId { get; set; }
            public string Shortcode { get; set; }
        }

    public class EmailConfig
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string FromEmail { get; set; }
    }


    public class SmsApiResponse
    {
        [JsonPropertyName("responses")]
        public List<SmsResponseItem> Responses { get; set; }
    }

    public class SmsResponseItem
    {
        [JsonPropertyName("respose-code")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("response-description")]
        public string ResponseDescription { get; set; }

        [JsonPropertyName("mobile")]
        public long Mobile { get; set; }

        [JsonPropertyName("messageid")]
        public long MessageId { get; set; }

        [JsonPropertyName("networkid")]
        public string NetworkId { get; set; }
    }
    public class SendSmsDto
    {
        public bool IsChurchWide { get; set; }
        public bool SendSMS { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
