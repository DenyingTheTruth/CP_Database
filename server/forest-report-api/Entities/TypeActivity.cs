namespace forest_report_api.Entities
{
    public class TypeActivity : BaseEntity
    {
        public string Name { get; set; }
        public bool IsIndustrial { get; set; }
        public int Position { get; set; }
    }
}