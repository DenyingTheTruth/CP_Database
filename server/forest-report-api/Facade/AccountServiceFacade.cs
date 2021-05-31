using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using forest_report_api.Entities;
using forest_report_api.Extensions;
using forest_report_api.Helper;
using forest_report_api.Models;
using forest_report_api.Repositories;
using forest_report_api.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace forest_report_api.Facade
{
    public class AccountServiceFacade
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IMailer _mailer;

        public AccountServiceFacade(RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IOrganizationRepository organizationRepository,
            IMailer mailer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _organizationRepository = organizationRepository;
            _mailer = mailer;
        }

        public IEnumerable<ApplicationRole> GetRoles()
        {
            return _roleManager.Roles;
        }

        public Task<IdentityResult> AddRole(ApplicationRole role)
        {
            return _roleManager.CreateAsync(role);
        }

        public async IAsyncEnumerable<object> GetUsers()
        {
            var users = await _userManager.Users
                .Include(x => x.Organization)
                .ToListAsync();
            foreach (var user in users)
            {
                var role = await _userManager.GetRolesAsync(user);
                if (role.FirstOrDefault() == "Admin") continue;

                yield return new
                {
                    Id = user.Id,
                    Fio = user.Fio,
                    Password = user.PasswordEncrypt != null
                        ? Encrypter.DecryptString(user.PasswordEncrypt)
                        : null,
                    Role = role.FirstOrDefault(),
                    OrganizationId = user.OrganizationId,
                    UserName = user.UserName,
                    Organization = user.Organization.GetFrontObject()
                };
            }
        }

        public async Task<ApplicationUserModel> GetUser(string userId)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            return user != null
                ? new ApplicationUserModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                    Fio = user.Fio
                }
                : null;
        }

        public async Task<string> GetRoleByUser(string name)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.UserName == name);
            return (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        }

        public string GetUserIdByName(string name)
        {
            return _userManager.Users.FirstOrDefault(x => x.UserName == name)?.Id;
        }

        public string GetOrganizationIdByName(string name)
        {
            return _userManager.Users.FirstOrDefault(x => x.UserName == name)?.OrganizationId;
        }
        
        public async Task<string> GetOrganizationByUserName(string name)
        {
            var id = _userManager.Users.FirstOrDefault(x => x.UserName == name)?.OrganizationId;
            return (await _organizationRepository.GetById(id)).Name;
        }

        private readonly string[] _organizationType = {"ооо", "оао", "руп", "сооо", "пуп", "зао", "хк"};
        private readonly char[] _charsToDelete = {' ','-','_','\'','\"','.',',',};
        public async Task<object> CreateUser(ApplicationUserModel userModel)
        {
            var organization = await _organizationRepository.GetById(userModel.OrganizationId);
            if (organization.Users.Any())
                return new ResponseResult(false, "User exists for this organization");
            
            var password = GenerateRandomPassword();
            var regex = new Regex(@$"(({string.Join("|",_organizationType)}|\s)*)(.*)");
            var param = new string(
                regex.Match(organization.Name.ToLower()).Groups[3].Value
                    .Where(x=>!_charsToDelete.Contains(x)).ToArray());
            var login = new Transliter().GetTranslite(param);
            userModel.Password = password;
            userModel.Role = "User";

            userModel.UserName = login;

            var identityResult = await _userManager.CreateAsync(new ApplicationUser()
            {
                Email = "example@gmail.com",
                UserName = userModel.UserName,
                OrganizationId = userModel.OrganizationId,
                PasswordEncrypt = Encrypter.EncryptString(userModel.Password),
                Fio = userModel.Fio
            }, userModel.Password);

            if (identityResult.Succeeded)
            {
                var user = _userManager.Users.FirstOrDefault(x => x.UserName == userModel.UserName);
                var roleResult = await _userManager.AddToRoleAsync(user, userModel.Role);

                return identityResult.Succeeded && roleResult.Succeeded
                    ? new ResponseResult(true, "Saved user", new
                    {
                        login,
                        password
                    })
                    : new ResponseResult(false, "Role don't created");
            }

            return new ResponseResult(false, identityResult.Errors.FirstOrDefault()?.Description);
        }

        public async Task<bool> UpdateUser(ApplicationUserModel userModel)
        {
            var user = await _userManager.FindByIdAsync(userModel.Id);

            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles.ToArray());

                var roleResult = await _userManager.AddToRoleAsync(user, userModel.Role);

                user.Fio = userModel.Fio;
                var userResult = await _userManager.UpdateAsync(user);
                return roleResult.Succeeded && userResult.Succeeded;
            }

            return false;
        }

        public async Task<ResponseResult> RenameUser(string id, string name)
        {
            var existUser = _userManager.Users.FirstOrDefault(x => x.UserName == name);
            if (existUser != null)
                return new ResponseResult(false, "Username exist", 1488);

            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                var result = await _userManager.SetUserNameAsync(user, name);
                return new ResponseResult(result.Succeeded, "POST", result.Errors);
            }

            return new ResponseResult(false, "Cannot find by id");
        }

        public async Task<object> GetOrganizations()
        {
            return (await _organizationRepository.GetAll()).Select(x => x.GetFrontObject());
        }

        public async Task<object> GetFreeOrganizations()
        {
            return (await _organizationRepository.GetFree()).Select(x => x.GetFrontObject());
        }

        public async Task<object> GetOrganization(string id)
        {
            return (await _organizationRepository.GetById(id)).GetFrontObject();
        }

        public async Task<object> RemoveOrganization(string id)
        {
            try
            {
                var organization = await _organizationRepository.GetById(id);
                foreach (var user in organization.Users)
                {
                    await RemoveUser(user.Id);
                }

                await _organizationRepository.Remove(id);
                return new ResponseResult(true, "Removed");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<object> RemoveUser(string userId)
        {
            try
            {
                var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                await _userManager.DeleteAsync(user);
                return new ResponseResult(true, "Removed user");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<object> SentLoginInfo(string id)
        {
            try
            {
                var organization = await _organizationRepository.GetById(id);
                var user = organization.Users.FirstOrDefault();
                if (user != null)
                {
                    var password = Encrypter.DecryptString(user.PasswordEncrypt);
                    await _mailer.SendEmailAsync(user.Email, $"Your login:{user.UserName}<br>" +
                                                             $"Your password: {password}");
                    return new ResponseResult(true, "Send");
                }
                else
                {
                    return new ResponseResult(false, "User is null");
                }
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<object> SaveOrganization(Organization model)
        {
            try
            {
                if (!model.IsNew)
                {
                    var user = _userManager.Users.FirstOrDefault(x => x.OrganizationId == model.Id);
                    var regex = new Regex(@$"(({string.Join("|",_organizationType)}|\s)*)(.*)");
                    var param = new string(
                        regex.Match(model.Name.ToLower()).Groups[3].Value
                            .Where(x=>!_charsToDelete.Contains(x)).ToArray());
                    var login = new Transliter().GetTranslite(param);
                    
                    await _userManager.SetUserNameAsync(user, login);
                }

                await _organizationRepository.Save(model);
                return new ResponseResult(true, "Check email");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null)
                opts = new PasswordOptions()
                {
                    RequiredLength = 8,
                    RequiredUniqueChars = 4,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireNonAlphanumeric = false,
                    RequireUppercase = true
                };

            var randomChars = new[]
            {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ", // uppercase 
                "abcdefghijkmnopqrstuvwxyz", // lowercase
                "0123456789", // non-alphanumeric
            };
            var rand = new Random(Environment.TickCount);
            var chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (var i = chars.Count;
                i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars;
                i++)
            {
                var rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        public async Task<object> ResetPassword(string userId)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var newPassword = GenerateRandomPassword();
                var resetPasswordAsync = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
                user.PasswordEncrypt = Encrypter.EncryptString(newPassword);
                await _userManager.UpdateAsync(user);
                return new ResponseResult(true, "Password cleared");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }

        public async Task<object> GetOrganizationGeneralInfo(string id)
        {
            try
            {
                var generalInfo = (await _organizationRepository.GetById(id)).GetGeneralInfo();
                return new ResponseResult(generalInfo != null, "Get general info", generalInfo);
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }
        public async Task<object> SaveOrganizationGeneralInfo(Organization organization)
        {
            try
            {
                await _organizationRepository.SaveGeneralInfo(organization);
                return new ResponseResult(true, "Save organization");
            }
            catch (Exception e)
            {
                return new ResponseResult(false, e.Message);
            }
        }
    }
}