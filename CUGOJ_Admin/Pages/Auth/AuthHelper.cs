using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Identity;

namespace CUGOJ.CUGOJ_Admin.Pages.Auth
{
    public class AuthHelperService
    {
        readonly UserManager<IdentityUser> _userManager;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly RoleManager<IdentityRole> _roleManager;
        readonly SignInManager<IdentityUser> _signInManager;

        public AuthHelperService(IServiceProvider provider, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = provider.CreateScope().ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            _roleManager = provider.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _signInManager = provider.CreateScope().ServiceProvider.GetRequiredService<SignInManager<IdentityUser>>();
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> InitAdmin()
        {
            if ((await _userManager.FindByNameAsync("Admin")) == null)
            {
                var user = new IdentityUser { UserName = "Admin", Email = "Admin" };
                await _userManager.CreateAsync(user, "CUGOJcugoj_01");
                user = await _userManager.FindByNameAsync("Admin");
                await _userManager.ConfirmEmailAsync(user, await _userManager.GenerateEmailConfirmationTokenAsync(user));
            }
            return true;
        }

        public async Task<bool> IsAdmin()
        {
            if (_httpContextAccessor.HttpContext == null) return false;
            var user = _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User).Result;
            if (user == null)
                return false;
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<bool> AddAdmin(string? username = null)
        {
            IdentityUser? user;
            if (username == null)
            {
                if (_httpContextAccessor.HttpContext == null) return false;
                user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            }
            else
            {
                user = await _userManager.FindByNameAsync(username);
            }
            if (user == null) return false;
            if (await _roleManager.FindByNameAsync("Admin") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            await _userManager.AddToRoleAsync(user, "Admin");
            return true;
        }

        public async Task<string?> ChangePassword(string oldPassword, string newPassword)
        {
            var user = await GetUser();
            if (user == null) return "请先登录";
            var res = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!res.Succeeded)
            {
                return res.Errors.First().Description;
            }
            return null;
        }
        public async Task<IdentityUser?> GetBaseUser()
        {
            var user = await GetUser();
            if (user == null) return null;
            return new IdentityUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }
        public async Task<IdentityUser?> GetUser()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return null;
            }
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return user;
        }
        public async Task<IdentityUser?> GetUserByUserName(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }
        public async Task<string?> GetUsername()
        {
            var user = await GetUser();
            if (user == null) return null;
            return user.UserName;
        }
        public async Task<string?> GetUserId()
        {
            var user = await GetUser();
            if (user == null) return null;
            return user.Id;
        }
    }
}
