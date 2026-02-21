using Microsoft.AspNetCore.Mvc;
using GCI_Admin.Models;
using System;
using System.Threading.Tasks;
using GCI_Admin.Services.IService;

namespace GCI_Admin.Controllers
{
    public class GrowthCentersController : Controller
    {
        private readonly IGrowthCentersService _growthCentersService;

        public GrowthCentersController(IGrowthCentersService growthCentersService)
        {
            _growthCentersService = growthCentersService;
        }

        // GET: /GrowthCenters
        public async Task<IActionResult> Index()
        {
            try
            {
                GrowthCentersData data = new GrowthCentersData();

                // Get all growth centers
                var centerRes = await _growthCentersService.GetAllGrowthCentersAsync();
                if (centerRes != null && centerRes.Data != null)
                {
                    data.GrowthCenters = centerRes.Data;
                }

                // Get all growth center leaders
                var leaderRes = await _growthCentersService.GetAllGrowthCenterLeadersAsync();
                if (leaderRes != null && leaderRes.Data != null)
                {
                    data.GrowthCenterLeaders = leaderRes.Data;
                }

                return View(data);
            }
            catch (Exception ex)
            {
                // Optional: log exception
                Console.WriteLine(ex.Message);
                return View(new GrowthCentersData());
            }
        }

        // Optional: Details, Create, Edit, Delete actions can follow the same pattern as MinistriesController
    }
}