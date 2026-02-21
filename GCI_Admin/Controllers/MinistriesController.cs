using Microsoft.AspNetCore.Mvc;
using GCI_Admin.Models;
using System;
using System.Threading.Tasks;
using GCI_Admin.Services.IService;

namespace GCI_Admin.Controllers
{
    public class MinistriesController : Controller
    {
        private readonly IMinistriesService _ministriesService;

        public MinistriesController(IMinistriesService ministriesService)
        {
            _ministriesService = ministriesService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                MinistriesData ministriesData = new MinistriesData();

                var ministryRes = await _ministriesService.GetAllMinistriesAsync();
                var leaderRes = await _ministriesService.GetAllMinistryLeadersAsync();

                if (ministryRes != null)
                {
                    ministriesData.Ministries = ministryRes.Data;
                }

                if (leaderRes != null)
                {
                    ministriesData.MinistryLeaders = leaderRes.Data;
                }

                return View(ministriesData);
            }
            catch (Exception ex)
            {
                // Optional: Log the exception
                Console.WriteLine(ex.Message);
                return View(new MinistriesData());
            }
        }
    }
}