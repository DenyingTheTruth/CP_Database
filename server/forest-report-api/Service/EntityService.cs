using forest_report_api.Repositories;

namespace forest_report_api.Service
{
    public class EntityService : IEntityService
    {
        private readonly IEntityRepository _repository;

        public EntityService(IEntityRepository repository)
        {
            _repository = repository;
        }

        public object Execute(string query)
        {
            return _repository.Execute(query);
        }
    }

    public interface IEntityService
    {
        object Execute(string query);
    }
}