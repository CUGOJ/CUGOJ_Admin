using CUGOJ.CUGOJ_Admin.Data;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;

namespace CUGOJ.CUGOJ_Admin.Shared
{
    public class LoginInfo
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
   
    public class LoginMiddleware
    {
        private static IDictionary<Guid, LoginInfo> Buffer = new ConcurrentDictionary<Guid,LoginInfo>();

        private readonly RequestDelegate _next;

        public LoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public static Guid PreLogin(LoginInfo loginInfo)
        {
            Guid key = Guid.NewGuid();
            Buffer.Add(key, loginInfo);
            return key;
        }
        
        public async Task Invoke(HttpContext context, SignInManager<IdentityUser> signInManager)
        {
            if(context.Request.Path=="/Login"&& context.Request.Query.ContainsKey("key"))
            {
                Guid key;
                var res = Guid.TryParse(context.Request.Query["key"], out key);
                if(!res)
                {
                    context.Response.Redirect("/Error");
                    return;
                }
                LoginInfo? info;
                res = Buffer.TryGetValue(key, out info);
                if (!res||info==null)
                {
                    context.Response.Redirect("/Error");
                    return;
                }
                else
                {
                    var result = await signInManager.PasswordSignInAsync(info.Username, info.Password, false, false);
                    if(result.Succeeded)
                    {
                        Buffer.Remove(key);
                        context.Response.Redirect("/Login?suc=true");
                        return;
                    }
                    else
                    {
                        context.Response.Redirect("/Login?err=" + result.ToString());
                        return;
                    }
                }
            }
            else
            {
                await _next.Invoke(context);
            }
        }


    }
}
