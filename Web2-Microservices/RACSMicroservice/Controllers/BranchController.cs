using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RACSMicroservice.DTOs;
using RACSMicroservice.Models;
using RACSMicroservice.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RACSMicroservice.Controllers
{
    public class BranchController : Controller
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly HttpClient httpClient;

        public BranchController(IUnitOfWork _unitOfWork, HttpClient httpClient)
        {
            unitOfWork = _unitOfWork;
            this.httpClient = httpClient;
        }

        #region Branch methods
        [HttpGet]
        [Route("get-racs-branches")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetRACSBranches()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RentACarServiceAdmin"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var res = await unitOfWork.RentCarServiceRepository.Get(racs => racs.AdminId == userId, null, "Branches");
                var racs = res.FirstOrDefault();

                if (racs == null)
                {
                    return BadRequest("Rent Service not found");
                }

                var branches = racs.Branches;

                List<object> objs = new List<object>();

                foreach (var item in branches)
                {
                    objs.Add(new { item.City, item.BranchId, item.State });
                }

                return Ok(objs);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return branches");
            }
        }

        [HttpPost]
        [Route("add-branch")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> AddBranch(BranchDto branchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RentACarServiceAdmin"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var res = await unitOfWork.RentCarServiceRepository.Get(racs => racs.AdminId == userId);
                var racs = res.FirstOrDefault();

                if (racs == null)
                {
                    return NotFound("Racs not found");
                }

                var branch = new Branch()
                {
                    RentACarService = racs,
                    City = branchDto.City,
                    State = branchDto.State
                };

                try
                {
                    await unitOfWork.BranchRepository.Insert(branch);
                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to insert branch");
                }

                var allBranches = await unitOfWork.BranchRepository.Get(b => b.RentACarServiceId == racs.RentACarServiceId);

                List<object> objs = new List<object>();

                foreach (var item in allBranches)
                {
                    objs.Add(new { item.City, item.BranchId, item.State });
                }

                return Ok(objs);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to insert branch");
            }
        }

        [HttpDelete]
        [Route("delete-branch/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            if (String.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest();
            }
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RentACarServiceAdmin"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var findBranch = await unitOfWork.BranchRepository.Get(b => b.BranchId == id, null, "Cars");
                var branch = findBranch.FirstOrDefault();

                if (branch == null)
                {
                    return NotFound("Branch not found");
                }

                var res = await unitOfWork.RentCarServiceRepository.Get(racs => racs.AdminId == userId);
                var racs = res.FirstOrDefault();

                if (racs == null)
                {
                    return NotFound("RACS not found");
                }

                foreach (var car in branch.Cars)
                {
                    racs.Cars.Add(car);
                }

                try
                {
                    unitOfWork.RentCarServiceRepository.Update(racs);
                    unitOfWork.BranchRepository.Delete(branch);
                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to delete branch. One of transaction failed");
                }

                List<object> objs = new List<object>();

                foreach (var item in racs.Branches)
                {
                    objs.Add(new { item.City, item.BranchId, item.State });
                }

                return Ok(objs);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to delete branch");
            }

        }
        #endregion
    }
}
