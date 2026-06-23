namespace GCI_Admin.Models.DTOs
{
    public class DataTableResponse<T>
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<T> data { get; set; }
        public string error { get; set; }
    }
}
