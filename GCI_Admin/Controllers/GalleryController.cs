using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Controllers
{
    [SessionAuthorize]
    public class GalleryController : Controller
    {
        private readonly IGalleryService _galleryService;

        public GalleryController(IGalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        // ── Index ─────────────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var response = await _galleryService.GetGalleryImagesAsync();
            
            if (!response.IsSuccess)
            {
                TempData["Error"] = $"Could not load gallery: {response.Message}";
            }
            
            return View(response.Data ?? new List<GalleryImageDto>());
        }

        // ── Upload ────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Upload([FromBody] GalleryUploadRequestDto request)
        {
            var response = await _galleryService.UploadGalleryImageAsync(request);
            
            return Json(new { isSuccess = response.IsSuccess, message = response.Message });
        }

        // ── Delete ────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Delete(string filename)
        {
            var response = await _galleryService.DeleteGalleryImageAsync(filename);
            
            return Json(new { isSuccess = response.IsSuccess, message = response.Message });
        }
    }
}
