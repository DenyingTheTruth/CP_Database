using System.Collections.Generic;
using System.Threading.Tasks;
using forest_report_api.Entities;
using forest_report_api.Facade;
using forest_report_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace forest_report_api.Controllers
{
    [Authorize]
    [Route("organizations")]
    [ApiController]
    public class OrganizationController : Controller
    {
        private readonly AccountServiceFacade _accountServiceFacade;

        public OrganizationController(AccountServiceFacade accountServiceFacade)
        {
            _accountServiceFacade = accountServiceFacade;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<object> Organizations()
        {
            return await _accountServiceFacade.GetOrganizations();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("free")]
        public async Task<object> FreeOrganizations()
        {
            return await _accountServiceFacade.GetFreeOrganizations();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<object> GetOrganization(string id)
        {
            return await _accountServiceFacade.GetOrganization(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<object> RemoveOrganization(string id)
        {
            return await _accountServiceFacade.RemoveOrganization(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<object> SaveOrganization(Organization model)
        {
            return await _accountServiceFacade.SaveOrganization(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("sent-login-info/{id}")]
        public async Task<object> GetLoginInfo(string id)
        {
            return await _accountServiceFacade.SentLoginInfo(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public IAsyncEnumerable<object> GetUsers()
        {
            return _accountServiceFacade.GetUsers();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("users")]
        public async Task<object> Users(ApplicationUserModel userModel)
        {
            return await _accountServiceFacade.CreateUser(userModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("users/{id},{name}")]
        public async Task<object> ApplicationUser(string id, string name)
        {
            return await _accountServiceFacade.RenameUser(id, name);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("users/{id}")]
        public async Task<object> RemoveUser(string id)
        {
            return await _accountServiceFacade.RemoveUser(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("users/reset-password/{userId}")]
        public async Task<object> ResetPassword(string userId)
        {
            return await _accountServiceFacade.ResetPassword(userId);
        }

        [HttpPost("save-general-info")]
        public async Task<object> SaveGeneralInfo(Organization organization)
        {
            return await _accountServiceFacade.SaveOrganizationGeneralInfo(organization);
        }

        [HttpGet("get-general-info/{id}")]
        public async Task<object> GetGeneralInfo(string id)
        {
            return await _accountServiceFacade.GetOrganizationGeneralInfo(id);
        }
    }
}