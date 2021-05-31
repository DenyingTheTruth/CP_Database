using forest_report_api.Entities;

namespace forest_report_api.Extensions
{
    public static class ApplicationUserExtensions
    {
        public static object GetFrontObject(this ApplicationUser user)
        {
            return new
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Organization = user.Organization.GetFrontObject(),
                Password = user.PasswordEncrypt,
                Fio = user.Fio
            };
        }
    }
}