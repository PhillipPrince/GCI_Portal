using GCI_Admin.Models;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class BenevolenceController : Controller
    {
       private readonly IBenevolenceService _benevolenceService;
        public BenevolenceController(IBenevolenceService benevolenceService)
        {
            _benevolenceService = benevolenceService;
        }
        public async Task<ActionResult> Index()
        {
            BenevolenceData data = new BenevolenceData();

            try
            {
                var response = await _benevolenceService.GetAllBenevolenceMembersAsync();

                if (response != null && response.Data != null)
                {
                    List<BenevolenceMember> members = response.Data;

                    data.BenevolenceMembers = members;
                    data.TotalMembers = members.Count;
                    data.TotalActiveMembers = members.Count(x => x.IsActive); 
                    data.TotalAmountDue = members.Sum(x => x.TotalAmountDue);
                    data.TotalAmountPaid = members.Sum(x => x.AmountPaid);
                    data.TotalBalance = members.Sum(x => x.BalanceAmount);
                }
                else
                {
                    data.BenevolenceMembers = new List<BenevolenceMember>();
                }
            }
            catch (Exception ex)
            {
                // Optional: log error here

                data.BenevolenceMembers = new List<BenevolenceMember>();

                ViewBag.ErrorMessage = "An error occurred while loading benevolence data: " + ex.Message;
            }

            return View(data);
        }
        [Authorize(Roles = "1")]

        public ActionResult MemberDetails(int id)
        {
            BenevolenceDetails benevolenceDetails = new BenevolenceDetails();
            BenevolenceMember benevolenceMember = _benevolenceService.GetBenevolenceMemberByIdAsync(id).Result.Data;
            if(benevolenceMember == null)
            {
                return NotFound();
            }
            List<BenevolenceBeneficiary> benevolenceBeneficiary = _benevolenceService.GetBenevolenceBeneficiariesAsync(benevolenceMember.Id).Result.Data;
            benevolenceDetails.Member = benevolenceMember;
            benevolenceDetails.TotalBeneficiaries = benevolenceBeneficiary.Count;
            benevolenceDetails.Beneficiaries = benevolenceBeneficiary;



            return View(benevolenceDetails);
        }

        // GET: BenevolenceController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BenevolenceController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BenevolenceController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BenevolenceController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BenevolenceController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BenevolenceController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
