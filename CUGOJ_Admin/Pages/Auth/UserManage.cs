using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Identity;

namespace CUGOJ.CUGOJ_Admin.Pages.Auth
{
    public partial class UserManage
    {
        string username = string.Empty;
        bool IsAdmin = false;
        bool ReNewPassword = false;
        async Task ConfirmUser(IdentityUser user)
        {
            if (!user.EmailConfirmed)
            {
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                await UserManager.ConfirmEmailAsync(user, code);
            }
        }
        async Task SaveUser(bool create)
        {
            if (string.IsNullOrEmpty(username))
            {
                await ToastService.Error("用户名不可为空");
                return;
            }
            if (!create)
            {
                var user = await UserManager.FindByNameAsync(username);
                if (user == null)
                {
                    await ToastService.Error("用户不存在");
                    return;
                }
                if (IsAdmin)
                    await UserManager.AddToRoleAsync(user, "Admin");
                else
                    await UserManager.RemoveFromRoleAsync(user, "Admin");
                if (ReNewPassword)
                    await UserManager.ResetPasswordAsync(user, await UserManager.GeneratePasswordResetTokenAsync(user), "CUGOJcugoj_01");
                await ConfirmUser(user);
            }
            else
            {
                var user = await UserManager.FindByNameAsync(username);
                if (user != null)
                {
                    await ToastService.Error("用户已存在");
                    return;
                }
                user = new IdentityUser { UserName=username,Email=username};
                var result = await UserManager.CreateAsync(user, "CUGOJcugoj_01");
                if(!result.Succeeded)
                {
                    var error = string.Empty;
                    foreach (var err in result.Errors)
                    {
                        error += err.Description;
                    }
                    await ToastService.Error(error);
                    return;
                }
                if (IsAdmin)
                    await UserManager.AddToRoleAsync(user, "Admin");
                await ConfirmUser(user);
            }
            await ToastService.Success("保存成功");
        }
    }
}
