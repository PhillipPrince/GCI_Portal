namespace GCI_Admin.Models.DTOs
{
    public class SystemConfigDto
    {
        public string ConfigKey { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsEditable { get; set; } = true;
        public bool IsActive { get; set; } = true;
    }
}
