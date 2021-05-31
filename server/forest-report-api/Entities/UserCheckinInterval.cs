using System;

namespace forest_report_api.Entities
{
    public class UserCheckinInterval : BaseEntity
    {
        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int Year { get; set; }
        public string PeriodId { get; set; }
        public Period Period { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? EndDate { get; set; }
        public string Name { get; set; }
    }
}