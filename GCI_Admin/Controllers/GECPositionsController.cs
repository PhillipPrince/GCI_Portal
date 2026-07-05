using GCI_Admin.Models;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class GECPositionsController : Controller
    {
        private readonly IGECPositionService _positionService;
        private readonly DBOperations.AppDbContext _context;

        public GECPositionsController(IGECPositionService positionService, DBOperations.AppDbContext context)
        {
            _positionService = positionService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _positionService.GetAllPositionsAsync();
            return View(response.Data ?? new List<GECPosition>());
        }

        [HttpGet("GECPositions/InitDb")]
        public async Task<IActionResult> InitDb()
        {
            try
            {
                var sql1 = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[GECPositions]') AND type in (N'U'))
BEGIN
CREATE TABLE [GECPositions] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_GECPositions] PRIMARY KEY ([Id])
);

INSERT INTO [GECPositions] (Title, Description, IsActive, CreatedAt) VALUES 
('General Overseer', 'Overall leader', 1, GETDATE()),
('Deputy General Overseer', 'Deputy leader', 1, GETDATE()),
('The General Secretary', 'Secretary', 1, GETDATE()),
('The National Treasurer', 'Treasurer', 1, GETDATE()),
('Board Member', 'Member of the board', 1, GETDATE()),
('Chairperson', '', 1, GETDATE()),
('Vice Chairperson', '', 1, GETDATE()),
('Secretary', '', 1, GETDATE()),
('Treasurer', '', 1, GETDATE()),
('Member', '', 1, GETDATE()),
('Observer', '', 1, GETDATE());
END;
";
                await Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.ExecuteSqlRawAsync(_context.Database, sql1);

                var sql2 = @"
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[GECMembers]') AND name = 'PositionTitle')
BEGIN
    ALTER TABLE [GECMembers] ADD [GECPositionId] int NOT NULL DEFAULT 1;
END;
";
                await Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.ExecuteSqlRawAsync(_context.Database, sql2);

                var sql3 = @"
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[GECMembers]') AND name = 'PositionTitle')
BEGIN
    EXEC('UPDATE [GECMembers] SET [GECPositionId] = ISNULL((SELECT TOP 1 Id FROM [GECPositions] WHERE Title = [PositionTitle]), 1);');
    ALTER TABLE [GECMembers] DROP COLUMN [PositionTitle];
    CREATE INDEX [IX_GECMembers_GECPositionId] ON [GECMembers] ([GECPositionId]);
    ALTER TABLE [GECMembers] ADD CONSTRAINT [FK_GECMembers_GECPositions_GECPositionId] FOREIGN KEY ([GECPositionId]) REFERENCES [GECPositions] ([Id]) ON DELETE CASCADE;
END;
";
                await Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.ExecuteSqlRawAsync(_context.Database, sql3);

                return Content("DB Initialized");
            }
            catch(System.Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        public IActionResult LoadCreateForm()
        {
            ViewBag.IsEdit = false;
            return PartialView("_CreateGECPositionPartial", new GECPosition());
        }

        public async Task<IActionResult> LoadEditForm(int id)
        {
            var response = await _positionService.GetPositionByIdAsync(id);
            if (!response.IsSuccess || response.Data == null)
            {
                return NotFound("Position not found");
            }

            ViewBag.IsEdit = true;
            return PartialView("_CreateGECPositionPartial", response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePosition([FromBody] GECPosition position)
        {
            if (string.IsNullOrWhiteSpace(position.Title))
            {
                return BadRequest("Title is required");
            }

            var response = await _positionService.CreatePositionAsync(position);
            if (response.IsSuccess)
                return Ok(new { success = true, message = response.Message });
            
            return BadRequest(new { success = false, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePosition([FromBody] GECPosition position)
        {
            if (string.IsNullOrWhiteSpace(position.Title))
            {
                return BadRequest("Title is required");
            }

            var response = await _positionService.UpdatePositionAsync(position);
            if (response.IsSuccess)
                return Ok(new { success = true, message = response.Message });
            
            return BadRequest(new { success = false, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePosition(int id)
        {
            var response = await _positionService.DeletePositionAsync(id);
            if (response.IsSuccess)
                return Ok(new { success = true, message = response.Message });
            
            return BadRequest(new { success = false, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var response = await _positionService.TogglePositionStatusAsync(id, isActive);
            if (response.IsSuccess)
                return Ok(new { success = true, message = response.Message });
            
            return BadRequest(new { success = false, message = response.Message });
        }
    }
}
