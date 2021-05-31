namespace forest_report_api.Entities
{
    public class BaseEntity
    {
        public string Id { get; set; }
        public bool IsNew => Id == default;
    }
}