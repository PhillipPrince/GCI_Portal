using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GCI_Admin.DBOperations;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GCI_Admin.Controllers
{
    public class TitlePrefixesController : Controller
    {
        private readonly ITitlePrefixService _service;
        private readonly AppDbContext _context;

        public TitlePrefixesController(ITitlePrefixService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            await EnsureDatabaseSetupAsync();
            var response = await _service.GetAllPrefixesAsync();
            return View(response.Data ?? new List<TitlePrefix>());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TitlePrefixDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return Json(new { success = false, message = "Title is required." });
            }

            var result = await _service.CreatePrefixAsync(dto);
            return Json(new { success = result.IsSuccess, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] TitlePrefixDto dto)
        {
            if (dto.Id <= 0 || string.IsNullOrWhiteSpace(dto.Title))
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            var result = await _service.UpdatePrefixAsync(dto);
            return Json(new { success = result.IsSuccess, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var result = await _service.ToggleStatusAsync(id, isActive);
            return Json(new { success = result.IsSuccess, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeletePrefixAsync(id);
            return Json(new { success = result.IsSuccess, message = result.Message });
        }

        private async Task EnsureDatabaseSetupAsync()
        {
            try
            {
                var sql1 = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TitlePrefixes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [TitlePrefixes] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_TitlePrefixes] PRIMARY KEY ([Id])
    );

    INSERT INTO [TitlePrefixes] (Title, Description, IsActive, CreatedAt) VALUES 
    ('Rev.', 'Reverend', 1, GETDATE()),
    ('Bishop', 'Bishop', 1, GETDATE()),
    ('Elder', 'Elder', 1, GETDATE()),
    ('Pastor', 'Pastor', 1, GETDATE()),
    ('Apostle', 'Apostle', 1, GETDATE()),
    ('Evangelist', 'Evangelist', 1, GETDATE()),
    ('Deacon', 'Deacon', 1, GETDATE()),
    ('Deaconess', 'Deaconess', 1, GETDATE());
END;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[GECMembers]') AND name = 'TitlePrefixId')
BEGIN
    ALTER TABLE [GECMembers] ADD [TitlePrefixId] int NULL;
END;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[AssemblyLeaders]') AND name = 'TitlePrefixId')
BEGIN
    ALTER TABLE [AssemblyLeaders] ADD [TitlePrefixId] int NULL;
END;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Elders]') AND name = 'TitlePrefixId')
BEGIN
    ALTER TABLE [Elders] ADD [TitlePrefixId] int NULL;
END;
";
                await _context.Database.ExecuteSqlRawAsync(sql1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EnsureDatabaseSetupAsync Error: {ex.Message}");
            }
        }
    }
}
