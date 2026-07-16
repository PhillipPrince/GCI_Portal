using System.Collections.Generic;

namespace GCI_Admin.Models
{
    public class GrowthCenterDetailsData
    {
        public GrowthCenter GrowthCenter { get; set; }
        public List<GrowthCenterLeader> Leaders { get; set; } = new List<GrowthCenterLeader>();
        public List<GrowthCenterMember> Members { get; set; } = new List<GrowthCenterMember>();
    }
}
