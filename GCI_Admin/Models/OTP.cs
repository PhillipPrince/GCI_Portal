using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCI_Admin.Models
{
    public class OTP
    {
        public int Id { get; set; }

        public string EmailOrPhone { get; set; } = string.Empty;

        public string OTPCode { get; set; } = string.Empty;

        public bool IsConfirmed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ExpiresAt { get; set; }
    }
}
