using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using forest_report_api.Entities;
using forest_report_api.Facade;
using forest_report_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace forest_report_api.Controllers
{
    [Route("account")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AccountServiceFacade _accountServiceFacade;
        private readonly ReportServiceFacade _reportServiceFacade;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AccountServiceFacade accountServiceFacade,
            IConfiguration configuration,
            ReportServiceFacade reportServiceFacade
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _accountServiceFacade = accountServiceFacade;
            _reportServiceFacade = reportServiceFacade;
        }

        [HttpPost("login")]
        public async Task<object> Login([FromBody] AccountModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    _configuration["JWT:ValidIssuer"],
                    _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddDays(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var organization = await _accountServiceFacade.GetOrganization(user.OrganizationId);
                var revisionCount = await _reportServiceFacade.GetCountReportsRevision(user.Id);
                var unreadReportsCount = await _reportServiceFacade.GetCountUnreadReports();
                
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    user = new
                    {
                        id = user.Id,
                        login = user.UserName,
                        role = userRoles.FirstOrDefault(),
                        organization = organization,
                        revisionReports = revisionCount != 0 ? revisionCount : (int?) null,
                        unreadReportsCount = unreadReportsCount != 0 ? unreadReportsCount : (int?) null,
                    }
                });
            }

            return Unauthorized();
        }

        [HttpGet("token-valid")]
        public async Task<object> TokenValid()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            try
            {
                string token = Request.Headers["Authorization"];
                if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = token.Substring("Bearer ".Length).Trim();
                }
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var userRoles = await _userManager.GetRolesAsync(user);
                
                var organization = await _accountServiceFacade.GetOrganization(user.OrganizationId);
                var revisionCount = await _reportServiceFacade.GetCountReportsRevision(user.Id);
                var unreadReportsCount = await _reportServiceFacade.GetCountUnreadReports();

                return Ok(new
                {
                    expiration = jwtToken.ValidTo,
                    user = new
                    {
                        id = user.Id,
                        login = user.UserName,
                        role = userRoles.FirstOrDefault(),
                        organization = organization,
                        revisionReports = revisionCount != 0 ? revisionCount : (int?) null,
                        unreadReportsCount = unreadReportsCount != 0 ? unreadReportsCount : (int?) null,
                    }
                });
            }
            catch
            {
                return new ResponseResult(false, "No valid");
            }
        }
    }
}