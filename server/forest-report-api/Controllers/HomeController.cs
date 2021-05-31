using System.Threading.Tasks;
using forest_report_api.Facade;
using forest_report_api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace forest_report_api.Controllers
{
    [Authorize]
    [Route("home")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly UserIntervalFacade _userIntervalFacade;
        private readonly AccountServiceFacade _accountServiceFacade;

        public HomeController(UserIntervalFacade userIntervalFacade,
            AccountServiceFacade accountServiceFacade)
        {
            _userIntervalFacade = userIntervalFacade;
            _accountServiceFacade = accountServiceFacade;
        }

        [HttpGet("calendar")]
        public async Task<object> GetCalendar()
        {
            var organizationId = _accountServiceFacade.GetOrganizationIdByName(User.Identity?.Name);
            return await _userIntervalFacade.GetUserIntervals(organizationId);
        }
    }
}