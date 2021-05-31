using forest_report_api.Service;

namespace forest_report_api.Facade
{
    public class EntityFacade
    {
        private readonly IEntityService _service;

        public EntityFacade(IEntityService service)
        {
            _service = service;
        }

        public object Execute(string query)
        {
            return _service.Execute(query);
        }
    }
}