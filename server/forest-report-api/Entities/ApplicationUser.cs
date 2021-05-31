using forest_report_api.Entities.Enums;
using forest_report_api.Models;
using Microsoft.AspNetCore.Identity;

namespace forest_report_api.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string OrganizationId { get; set; } 
        public Organization Organization { get; set; }
        public string PasswordEncrypt { get; set; }
        public string Fio { get; set; }
    }
}