using System.Collections.Generic;

namespace GCI_Admin.Models
{
    public class MinistryDetailsData
    {
        public Ministry Ministry { get; set; }
        public List<MinistryLeader> Leaders { get; set; } = new List<MinistryLeader>();
        public List<MinistryMember> Members { get; set; } = new List<MinistryMember>();
    }
}
