using System.Collections.Generic;
using forest_report_api.Entities;
using forest_report_api.Entities.Enums;

namespace forest_report_api.Models
{
    public class ApplicationUserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string OrganizationId { get; set; }
        public string Fio { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}