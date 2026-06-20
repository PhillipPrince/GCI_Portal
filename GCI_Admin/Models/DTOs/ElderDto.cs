using System;

namespace GCI_Admin.Models.DTOs
{
    public class ElderDto
    {
        public int MemberId { get; set; }
        public string Description { get; set; }
        public DateTime? DateOrdained { get; set; }
        public string ProfileImageBase64 { get; set; }
    }

    public class NewElder
    {
        public List<DropdownItem> MembersList { get; set; }
    }
}