using Microsoft.AspNetCore.Mvc;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Utils;

namespace GCI_Admin.Controllers
{
    public class LeadershipController : Controller
    {
        private readonly ILeadershipService _leadershipService;
        private readonly IMembersService _members;
        private readonly IMinistriesService _ministries;

        public LeadershipController(ILeadershipService leadershipService, IMembersService membersService, IMinistriesService ministriesService)
        {
            _leadershipService = leadershipService;
            _members = membersService;
            _ministries = ministriesService;
        }

        public async Task<IActionResult> Deacons()
        {
            try
            {
                DeaconsData deaconsData = new DeaconsData();
                var response = await _leadershipService.GetAllDeaconsAsync();

                if (response != null && response.IsSuccess)
                {
                    deaconsData.TotalDeacons = response.Data.Count;
                    deaconsData.Deacons = response.Data;
                    deaconsData.CurrentOnDutyDeacon = GetCurrentOnDutyDeacon(response.Data);
                }

                return View(deaconsData);
            }
            catch (Exception)
            {
                return View(new List<Deacon>());
            }
        }
        private DeaconOnDuty GetCurrentOnDutyDeacon(IEnumerable<Deacon> deacons)
        {
            var onDutyDeacon = deacons.FirstOrDefault(d => d.OnDuty && d.IsActive);

            if (onDutyDeacon != null)
            {
                return new DeaconOnDuty
                {
                    DeaconId = onDutyDeacon.DeaconId,
                    MemberId = onDutyDeacon.MemberId,
                    FullName = onDutyDeacon.Member != null
                        ? $"{onDutyDeacon.Member.FirstName} {onDutyDeacon.Member.OtherNames}"
                        : $"Deacon {onDutyDeacon.DeaconId}",
                    Ministry = onDutyDeacon.Ministry ?? "Not Assigned",
                    Phone = onDutyDeacon.Member?.Phone ?? "No phone",
                    Email = onDutyDeacon.Member?.Email ?? "No email",
                    Bio = onDutyDeacon.Bio,
                };
            }

            return null;
        }
        public async Task<IActionResult> DeaconDetails(int id)
        {
            try
            {
                var deacon = await _leadershipService.GetDeaconByIdAsync(id);

                if (deacon == null)
                {
                    TempData["ErrorMessage"] = "Deacon not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new DeaconDetailsViewModel
                {
                    Deacon = deacon.Data,
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading deacon details.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> AddDeaconPartial()
        {
            var model = new NewDeacon
            {
                MembersList = new List<DropdownItem>()
            };

            var response = await _members.GetAllMembersAsync();
            var ministriesResponse = await _ministries.GetAllMinistriesAsync();
            var ministries = ministriesResponse?.Data ?? new List<Ministry>();

            var members = response?.Data?
                .Where(m => m.StatusId == 1)
                .ToList() ?? new List<Member>();

            model.MinistriesList = ministries.Select(m => new DropdownItem
            {
                Value = m.MinistryId.ToString(),
                Text = m.MinistryName
            }).ToList();
            model.MembersList = members.Select(m => new DropdownItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.FirstName} {m.OtherNames}"
            }).ToList();

            return PartialView("_AddDeacon", model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDeacon([FromBody] DeaconDto dto)
        {
            var result = await _leadershipService.CreateDeaconAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDeacon(int id)
        {
            var result = await _leadershipService.GetDeaconByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDeacon(int id, [FromBody] DeaconDto dto)
        {
            var result = await _leadershipService.UpdateDeaconAsync(id, dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDeacon(int id)
        {
            var result = await _leadershipService.DeleteDeaconAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleDutyStatus(int id, bool onDuty)
        {
            var result = await _leadershipService.ToggleDutyStatusAsync(id, onDuty);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        public async Task<IActionResult> Elders()
        {
            try
            {
                var response = await _leadershipService.GetAllEldersAsync();

                if (response != null && response.IsSuccess)
                {
                    return View(response.Data);
                }

                return View(new List<Elder>());
            }
            catch (Exception)
            {
                return View(new List<Elder>());
            }
        }
        public async Task<IActionResult> ElderDetails(int id)
        {
            try
            {
                var elder = await _leadershipService.GetElderByIdAsync(id);

                if (elder == null || !elder.IsSuccess)
                {
                    TempData["ErrorMessage"] = "Elder not found.";
                    return RedirectToAction(nameof(Elders));
                }

                return View(elder.Data);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while loading elder details.";
                return RedirectToAction(nameof(Elders));
            }
        }
        public async Task<IActionResult> AddElderPartial()
        {
            var model = new NewElder
            {
                MembersList = new List<DropdownItem>()
            };

            var response = await _members.GetAllMembersAsync();

            var members = response?.Data?
                .Where(m => m.StatusId == 1)
                .ToList() ?? new List<Member>();

            model.MembersList = members.Select(m => new DropdownItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.FirstName} {m.OtherNames}"
            }).ToList();

            return PartialView("_AddElder", model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateElder([FromBody] ElderDto dto)
        {
            var result = await _leadershipService.CreateElderAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetElder(int id)
        {
            var result = await _leadershipService.GetElderByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateElder(int id, [FromBody] ElderDto dto)
        {
            var result = await _leadershipService.UpdateElderAsync(id, dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteElder(int id)
        {
            var result = await _leadershipService.DeleteElderAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

    }
}