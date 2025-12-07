using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RealEstateManagement.Core.Interfaces;
using RealEstateManagement.Core.DTOs;

namespace RealEstateManagement.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAgentRepository _agentRepo;

        public AccountController(IAgentRepository agentRepo)
        {
            _agentRepo = agentRepo;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(AgentLoginDto request)
        {
            // 1. 使用 Repository 查詢使用者
            var agent = await _agentRepo.GetByEmployeeCodeAsync(request.Username);

            // 2. 驗證密碼 (使用 BCrypt)
            if (agent != null && BCrypt.Net.BCrypt.Verify(request.Password, agent.Password))
            {
                // 3. 建立身分聲明 (Claims)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, agent.Name),
                    new Claim(ClaimTypes.Role, "Agent"),
                    new Claim(ClaimTypes.NameIdentifier, agent.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    // 不記住，session cookie
                    IsPersistent = false,
                    // 伺服器端的票證過期時間 (即使瀏覽器沒關，掛網 30 分鐘也會失效)
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                    // 是否允許滑動過期？(使用者有在操作，時間就自動延長)
                    AllowRefresh = true
                };

                // 4. 登入 (寫入 Cookie)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Houses");
            }

            ViewBag.Error = "帳號或密碼錯誤";
            return View();
        }

        // POST: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            // 登出 (清除 Cookie)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
