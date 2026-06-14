using System.Collections.Generic;

namespace GCI_Admin.Models
{
    public class GrowthCentersData
    {
        public List<GrowthCenter> GrowthCenters { get; set; } = new List<GrowthCenter>();
        public List<GrowthCenterLeader> GrowthCenterLeaders { get; set; } = new List<GrowthCenterLeader>();
    }
}