namespace GCI_Admin.Models
{
    public class Deacon
    {
        public int DeaconId { get; set; }

        public int MemberId { get; set; }

        public string Ministry { get; set; }
        public string Bio { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }
        public bool OnDuty { get; set; }

        public bool HasSpecialDuties { get; set; }
        public bool IsEmeritus { get; set; }

        public DateTime CreatedAt { get; set; }

        // 🔹 Navigation Property (Optional but recommended)
        public Member Member { get; set; }
    }

    public class DeaconsData
    {
        public int TotalDeacons { get; set; }
        public List<Deacon> Deacons { get; set; }
        public List<string> MinistryOptions { get; set; }
        public DeaconOnDuty CurrentOnDutyDeacon { get; set; } // Changed to DeaconOnDuty type
    }


    public class DeaconOnDuty
    {
        public int DeaconId { get; set; }
        public int MemberId { get; set; }
        public string FullName { get; set; }
        public string Ministry { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public DateTime StartedAt { get; set; }

        public string DutyDuration
        {
            get
            {
                var duration = DateTime.Now - StartedAt;
                if (duration.TotalMinutes < 1)
                    return "Just started";
                else if (duration.TotalHours < 1)
                    return $"{duration.Minutes} minute{(duration.Minutes != 1 ? "s" : "")}";
                else if (duration.TotalHours < 24)
                    return $"{duration.Hours} hour{(duration.Hours != 1 ? "s" : "")}, {duration.Minutes} minute{(duration.Minutes != 1 ? "s" : "")}";
                else
                    return $"{duration.Days} day{(duration.Days != 1 ? "s" : "")}, {duration.Hours} hour{(duration.Hours != 1 ? "s" : "")}";
            }
        }

    }
    public class DeaconDetailsViewModel
    {
        public Deacon Deacon { get; set; }
    }
    public class NewDeacon
    {
        public List<DropdownItem> MembersList { get; set; }
        public List<DropdownItem> MinistriesList { get; set; }
    }
}