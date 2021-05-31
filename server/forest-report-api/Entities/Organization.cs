using System.Collections.Generic;
using forest_report_api.Entities.Enums;

namespace forest_report_api.Entities
{
    public class Organization : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsHolding { get; set; }
        public Regions Region { get; set; }
        public bool IsState { get; set; }
        public string TypeActivityId { get; set; }
        public TypeActivity TypeActivity { get; set; }
        public List<ApplicationUser> Users { get; set; }

        public string UNP { get; set; }
        public string TypeEconomicActivity { get; set; }
        public string OrganizationalLegalForm { get; set; }
        public string GovermentForReport { get;  set; }
        public string UnitForReport { get; set; }
        public string Address { get; set; }
        public string Position1 { get; set; }
        public string FullName1 { get; set; }
        public string Position2 { get; set; }
        public string FullName2 { get; set; }
    }
}