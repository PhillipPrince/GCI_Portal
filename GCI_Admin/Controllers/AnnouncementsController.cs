using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
    using Microsoft.AspNetCore.Mvc;
using Utils;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]

    public class AnnouncementsController : Controller
    {
        private readonly IAnnouncementsService _announcementsService;
        private readonly IMembersService _membersService;
        private readonly IMinistriesService _ministriesService;
        private readonly IGrowthCentersService _growthCentersService;
        private readonly IRcpsService _rcpsService;

        public AnnouncementsController(
            IAnnouncementsService announcementsService, 
            IMembersService membersService,
            IMinistriesService ministriesService,
            IGrowthCentersService growthCentersService,
            IRcpsService rcpsService)
        {
            _announcementsService = announcementsService;
            _membersService = membersService;
            _ministriesService = ministriesService;
            _growthCentersService = growthCentersService;
            _rcpsService = rcpsService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var res = await _announcementsService.GetAllAnnouncementsAsync();
                return View(res.Data ?? new List<Notification>());
            }
            catch
            {
                return View(new List<Notification>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AnnouncementsTable()
        {
            try
            {
                var res = await _announcementsService.GetAllAnnouncementsAsync();
                return PartialView("_AnnouncementsTable", res.Data ?? new List<Notification>());
            }
            catch
            {
                return PartialView("_AnnouncementsTable", new List<Notification>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateAnnouncement()
        {
            NotificationDto notification = new NotificationDto();
            var notificationGroupsResponse = await _announcementsService.GetAllNotificationGroupsAsync();
            
            notification.NotificationGroups = (notificationGroupsResponse.Data ?? new List<NotificationGroup>()).Select(m => new DropdownItem
                      {
                          Value = m.GroupId.ToString(),
                          Text = m.GroupName.ToString()
                      }).ToList();
            
            var membersResponse = await _membersService.GetAllMembersAsync();
            notification.Members = (membersResponse.Data ?? new List<Member>()).Select(m => new DropdownItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.FirstName} {m.OtherNames}"
            }).ToList();
            notification.RawMembers = membersResponse.Data;


            var ministriesResponse = await _ministriesService.GetAllMinistriesAsync();
            notification.MinistriesList = (ministriesResponse.Data ?? new List<Ministry>()).Select(m => new DropdownItem
            {
                Value = m.MinistryId.ToString(),
                Text = m.MinistryName
            }).ToList();

            var growthCentersResponse = await _growthCentersService.GetAllGrowthCentersAsync();
            notification.GrowthCentersList = (growthCentersResponse.Data ?? new List<GrowthCenter>()).Select(g => new DropdownItem
            {
                Value = g.GrowthCenterId.ToString(),
                Text = g.CenterName
            }).ToList();

            var rcpsResponse = await _rcpsService.GetAllRcpsAsync();
            notification.RcpsList = (rcpsResponse.Data ?? new List<Rcps>()).Select(r => new DropdownItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();

            return View("_CreateAnnouncement",  notification);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitNewAnnouncement([FromBody] NotificationDto dto)
        {
            try
            {
                var response = await _announcementsService.CreateAnnouncementAsync(dto);
                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Notification> { IsSuccess = false, Code = "500", Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int notificationId)
        {
            try
            {
                var response = await _announcementsService.GetAnnouncementByIdAsync(notificationId);
                if (!response.IsSuccess)
                    return NotFound(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Notification> { IsSuccess = false, Code = "500", Message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int notificationId, [FromBody] NotificationDto dto)
        {
            try
            {
                var response = await _announcementsService.UpdateAnnouncementAsync(notificationId, dto);
                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Notification> { IsSuccess = false, Code = "500", Message = ex.Message });
            }
        }

        public class DeleteAnnouncementRequest
        {
            public int NotificationId { get; set; }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteAnnouncementRequest req)
        {
            try
            {
                var response = await _announcementsService.DeleteAnnouncementAsync(req.NotificationId);
                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool> { IsSuccess = false, Code = "500", Message = ex.Message });
            }
        }

        public class ToggleStatusRequest
        {
            public int NotificationId { get; set; }
            public bool IsActive { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus([FromBody] ToggleStatusRequest req)
        {
            try
            {
                var response = await _announcementsService.ToggleAnnouncementStatusAsync(req.NotificationId, req.IsActive);
                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool> { IsSuccess = false, Code = "500", Message = ex.Message });
            }
        }
    }
}
