using BootstrapBlazor.Components;
using CUGOJ.CUGOJ_Admin.Data;

namespace CUGOJ.CUGOJ_Admin.Pages
{
    public partial class Envs
    { 
        public class EnvModel : Env
        {
            public bool HasPermission { get; set; }
        }
        string NewEnvName = string.Empty;
        string NewPermissionUserName = string.Empty;
        long NewPermissionEnvId;
        List<EnvModel> EnvList = new();
        private Modal? AddPermissionModal;
        private Modal? AddEnvModal;
        protected override async Task OnInitializedAsync()
        {
            await LoadEnvs();
            await base.OnInitializedAsync();
        }
        public async Task OpenModal(long Id)
        {
            NewPermissionEnvId = Id;
            await AddPermissionModal!.Toggle();
        }
        public async Task LoadEnvs()
        {
            var user = await AuthHelper.GetUser();
            if (user == null)
            {
                await ToastService.Error("请先登录");
                return;
            }
            using var context = new ApplicationDbContext();
            var userEnvs = (from u in context.UserEnvs where u.UserId == user.Id select u.EnvId).ToHashSet();
            EnvList = (from u in context.Envs select new EnvModel { Id = u.Id, Name = u.Name, HasPermission = userEnvs.Contains(u.Id) }).ToList();
        }
        public async Task AddPermission()
        {
            var user = await AuthHelper.GetUser();
            if (user == null)
            {
                await ToastService.Error(null, "请先登录");
                return;
            }
            using var context = new ApplicationDbContext();
            if (!(from e in context.Envs where e.Id == NewPermissionEnvId select e).Any())
            {
                await ToastService.Error(null, "环境不存在");
                return;
            }
            if (!(from u in context.UserEnvs where u.EnvId == NewPermissionEnvId && u.UserId == user.Id select u).Any())
            {
                await ToastService.Error(null, "无权操作");
                return;
            }
            user = await AuthHelper.GetUserByUserName(NewPermissionUserName);
            if(user ==null)
            {
                await ToastService.Error(null, "用户不存在");
                return;
            }
            if ((from u in context.UserEnvs where u.EnvId == NewPermissionEnvId && u.UserId == user.Id select u).Any())
            {
                await ToastService.Error(null, "用户已有权限");
                return;
            }
            context.UserEnvs.Add(new UserEnv { EnvId = NewPermissionEnvId, UserId = user.Id });
            await context.SaveChangesAsync();
        }

        public async Task AddEnv()
        {
            var user = await AuthHelper.GetUser();
            if (user == null)
            {
                await ToastService.Error(null, "请先登录");
                return;
            }
            using var context = new ApplicationDbContext();
            if ((from e in context.Envs where e.Name == NewEnvName select e).Any())
            {
                await ToastService.Error(null, "同名环境已存在");
                return;
            }
            var newEnv = new Env { Name = NewEnvName };
            context.Envs.Add(newEnv);
            await context.SaveChangesAsync();
            context.UserEnvs.Add(new UserEnv { UserId = user.Id, EnvId = newEnv.Id });
            await context.SaveChangesAsync();
            await LoadEnvs();
        }

        public async Task DeleteEnv(long Id)
        {
            using var context = new ApplicationDbContext();
            if (!(from e in context.Envs where e.Id == Id select e).Any())
            {
                await ToastService.Error(null, "环境不存在");
                return;
            }
            context.Envs.Remove(new Env { Id = Id });
            context.UserEnvs.RemoveRange(from u in context.UserEnvs where u.EnvId == Id select u);
            await context.SaveChangesAsync();
            await LoadEnvs();
            StateHasChanged();
        }
    }
}
