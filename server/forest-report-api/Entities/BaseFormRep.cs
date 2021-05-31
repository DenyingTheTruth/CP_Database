namespace forest_report_api.Entities
{
    public class BaseFormRep : BaseEntity
    {
        public string CollectionId { get; set; }
        public string Json { get; set; }
        public string NoValid { get; set; }
        public int TabId { get; set; }
    }
}