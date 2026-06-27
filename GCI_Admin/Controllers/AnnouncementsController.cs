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
        public IActionResult CreateAnnouncement()
        {
            NotificationDto notification = new NotificationDto();
            var notificationGroupsResponse = _announcementsService.GetAllNotificationGroupsAsync().Result.Data;
            
            notification.NotificationGroups = notificationGroupsResponse.Select(m => new DropdownItem
                      {
                          Value = m.GroupId.ToString(),
                          Text = m.GroupName.ToString()
                      }).ToList();
            var membersResponse = _membersService.GetAllMembersAsync().Result.Data;
            notification.Members = membersResponse.Select(m => new DropdownItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.FirstName} {m.OtherNames}"
            }).ToList();

            var ministriesResponse = _ministriesService.GetAllMinistriesAsync().Result.Data;
            notification.MinistriesList = ministriesResponse?.Select(m => new DropdownItem
            {
                Value = m.MinistryId.ToString(),
                Text = m.MinistryName
            }).ToList() ?? new List<DropdownItem>();

            var growthCentersResponse = _growthCentersService.GetAllGrowthCentersAsync().Result.Data;
            notification.GrowthCentersList = growthCentersResponse?.Select(g => new DropdownItem
            {
                Value = g.GrowthCenterId.ToString(),
                Text = g.CenterName
            }).ToList() ?? new List<DropdownItem>();

            var rcpsResponse = _rcpsService.GetAllRcpsAsync().Result.Data;
            notification.RcpsList = rcpsResponse?.Select(r => new DropdownItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList() ?? new List<DropdownItem>();

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

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] int notificationId)
        {
            try
            {
                var response = await _announcementsService.DeleteAnnouncementAsync(notificationId);
                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool> { IsSuccess = false, Code = "500", Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int notificationId, bool isActive)
        {
            try
            {
                var response = await _announcementsService.ToggleAnnouncementStatusAsync(notificationId, isActive);
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
