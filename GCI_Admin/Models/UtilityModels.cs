namespace GCI_Admin.Models
{
    public class DropdownItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; } = false;

        public string Subtext { get; set; }  // Optional secondary text
        public string Icon { get; set; }      // Optional icon class
        public string SearchText { get; set; } // Custom search text

        // Additional data
        public Dictionary<string, object> ExtraData { get; set; }
    }
}
