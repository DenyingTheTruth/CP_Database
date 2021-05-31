using forest_report_api.Facade;
using Microsoft.AspNetCore.Mvc;

namespace forest_report_api.Controllers
{
    [Route("entity")]
    [ApiController]
    public class EntityController : Controller
    {
        private readonly EntityFacade _facade;

        public EntityController(EntityFacade facade)
        {
            _facade = facade;
        }

        [HttpPost("execute")]
        public object Execute(string query)
        {
            return _facade.Execute(query);
        }
    }
}