using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace GCI_Admin.Models.DTOs
{
    public class CreateRcpsCountyCoordinatorDto
    {
        public List<Member> Members { get; set; }
        public List<SelectListItem> RcpsList { get; set; }
        public RcpsCountyCoordinatorDto Coordinator { get; set; }
    }

    public class RcpsCountyCoordinatorDto
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int RcpsId { get; set; }
        public string Bio { get; set; }
        public bool IsActive { get; set; }
        public string? ProfileImageBase64 { get; set; }
    }
}
