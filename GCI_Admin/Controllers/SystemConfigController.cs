using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Utils;

namespace GCI_Admin.Controllers
{
    public class SystemConfigController : Controller
    {
        private readonly ISystemConfigService _configService;

        public SystemConfigController(ISystemConfigService configService)
        {
            _configService = configService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _configService.GetAllConfigsAsync();
                var configs = response.Data ?? new List<SystemConfig>();
                return View(configs);
            }
            catch
            {
                return View(new List<SystemConfig>());
            }
        }

       

        [HttpGet]
        public IActionResult CreateConfig()
        {
            var dto = new SystemConfigDto();
            return View("_CreateConfig", dto);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitNewConfig([FromBody] SystemConfigDto dto)
        {
            try
            {
                var response = await _configService.CreateConfigAsync(dto);
                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DbResponse<SystemConfig>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByKey(string key)
        {
            try
            {
                var response = await _configService.GetConfigByKeyAsync(key);
                if (!response.Success)
                    return NotFound(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DbResponse<SystemConfig>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConfig(int id, [FromBody] SystemConfigDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.ConfigKey))
                {
                    return BadRequest(new DbResponse<SystemConfig>
                    {
                        Success = false,
                        Message = "Config key is required"
                    });
                }

                // Pass the ID to your service method
                var response = await _configService.UpdateConfigAsync( dto);
                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DbResponse<SystemConfig>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteConfig(int id)
        {
            try
            {
                var response = await _configService.DeleteConfigAsync(id);
                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DbResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}