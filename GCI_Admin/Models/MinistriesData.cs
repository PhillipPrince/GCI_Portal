using System.Collections.Generic;

namespace GCI_Admin.Models
{
    public class MinistriesData
    {
        public List<Ministry> Ministries { get; set; } = new List<Ministry>();

        public List<MinistryLeader> MinistryLeaders { get; set; } = new List<MinistryLeader>();
    }
}