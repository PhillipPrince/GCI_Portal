using System.Collections.Generic;

namespace GCI_Admin.Models.ViewModels
{
    public class MemberSelectorViewModel
    {
        public string ControlId { get; set; } = "memberSelector";
        public string LabelText { get; set; } = "Select Member";
        public string PlaceholderText { get; set; } = "-- Select a member --";
        public string MemberIdFieldName { get; set; } = "MemberId";
        public int? SelectedMemberId { get; set; }
        public List<Member> Members { get; set; }
        public bool IsRequired { get; set; } = true;
        public bool IsEdit { get; set; } = false;
        public bool ShowPhoneAndGender { get; set; } = true;
    }
}