using GCI_Admin.Models;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class CareRequestRepository
    {
        private readonly AppDbContext _context;

        public CareRequestRepository(AppDbContext context)
        {
            _context = context;
        }

     
        public async Task<DbResponse<List<CareRequest>>> GetAllAsync()
        {
            var response = new DbResponse<List<CareRequest>>();

            try
            {
                response.Data = await _context.CareRequests
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("CareRequestRepository->GetAllAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // 🔥 PICK REQUEST (CRITICAL LOGIC)
        public async Task<DbResponse<bool>> PickRequestAsync(int requestId, int pastorId)
        {
            var response = new DbResponse<bool>();

            try
            {
                var updated = await _context.CareRequests
                    .Where(x => x.Id == requestId && !x.IsPicked)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.IsPicked, true)
                        .SetProperty(x => x.PickedByPastorId, pastorId)
                        .SetProperty(x => x.PickedAt, DateTime.Now)
                    );

                if (updated == 0)
                {
                    response.Success = false;
                    response.Message = "Request already picked by another pastor";
                    return response;
                }

                response.Success = true;
                response.Data = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("CareRequestRepository->PickRequestAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        //public async Task<DbResponse<bool>> RespondAsync(RespondCareRequestDto dto)
        //{
        //    var response = new DbResponse<bool>();

        //    try
        //    {
        //        var entity = await _context.CareRequests
        //            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        //        if (entity == null)
        //        {
        //            response.Success = false;
        //            response.Message = "Request not found";
        //            return response;
        //        }

        //        entity.Response = dto.Response;
        //        entity.RespondedAt = DateTime.Now;
        //        entity.UpdatedAt = DateTime.Now;

        //        await _context.SaveChangesAsync();

        //        response.Success = true;
        //        response.Data = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Loggers.DoLogs("CareRequestRepository->RespondAsync->" + ex.Message);
        //        response.Success = false;
        //        response.Message = ex.Message;
        //    }

        //    return response;
        //}
    }
}
