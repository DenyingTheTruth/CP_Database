using Microsoft.AspNetCore.Identity;

namespace forest_report_api.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
    }
}