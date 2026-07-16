using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCI_Admin.Models
{
    [Table("Counties")]
    public class County
    {
        [Key]
        public int Id { get; set; }

        public string CountyName { get; set; }

        public string CountyCode { get; set; }
    }
}
