
using BootstrapBlazor.Components;
using CUGOJ.CUGOJ_Admin.Config;
using CUGOJ.CUGOJ_Admin.Data;
using System.Net;

namespace CUGOJ.CUGOJ_Admin.Pages.Config
{
    public partial class Config
    {
        public class ConfigItem
        {
            public string Key { get; set; } = null!;
            public string Value { get; set; } = null!;
            public bool IsInherited { get; set; }
        }
        public class ConfigModel
        {
            public string Env { get; set; } = null!;
            public bool HasPermission { get; set; }
            public List<ConfigItem> ConfigItems
            {
                get
                {
                    var res = new HashSet<ConfigItem>();
                    var EnvConfig = ConfigManager.MustGetConfig(Env);
                    var DefaultConfig = ConfigManager.MustGetConfig("default");
                    return (from e in EnvConfig.ConfigList
                            select new ConfigItem { Key = e.Key, Value = e.Value, IsInherited = false })
                     .Concat(from d in DefaultConfig.ConfigList
                             where !EnvConfig.ConfigList.ContainsKey(d.Key)
                             select new ConfigItem { Key = d.Key, Value = d.Value, IsInherited = true })
                     .OrderBy(e => e.Key).ToList();
                }
            }
        }
        List<ConfigItem> ConfigItems = new();
        List<ConfigModel> ConfigList = new();
        Modal? ViewModal { get; set; }
        Modal? EditModal { get; set; }
        Modal? EditKVModal { get; set; }
        ConfigItem? editingConfig { get; set; }
        string? editingKey { get; set; }
        string currentEnv { get; set; } = string.Empty;
        async Task OpenModal(bool isEdit,string env)
        {
            currentEnv = env;
            var conf = ConfigList.FirstOrDefault(c => c.Env == env);
            if(conf==null)
            {
                await ToastService.Error("未找到对应环境的配置");
                return;
            }
            ConfigItems = conf.ConfigItems;
            if (isEdit)
            {
                await EditModal!.Toggle();
            }
            else await ViewModal!.Toggle();
        }
        async Task SaveEdit()
        {
            var conf = ConfigList.FirstOrDefault(c => c.Env == currentEnv);
            if (conf==null)
            {
                await ToastService.Error("未找到对应环境的配置");
                return;
            }
            await ConfigManager.UpdateConfig(currentEnv, new CUGOJ_Admin.Config.Config { Env = currentEnv, ConfigList = (from c in ConfigItems where !c.IsInherited select c).ToDictionary(c => c.Key, c => c.Value) }); ;
            await EditModal!.Close();
        }
        async Task EditConfigItem(string key)
        {
            var conf = ConfigItems.FirstOrDefault(c => c.Key == key);
            if (conf != null)
            {
                editingConfig = new ConfigItem
                {
                    Key = conf.Key,
                    Value = conf.Value,
                    IsInherited = false
                };
                editingKey = conf.Key;
            }
            else 
                editingConfig = null;
            await EditKVModal!.Toggle();
        }

        async Task EditEnd ()
        {
            if (editingConfig == null || editingKey == null)
            {
                await ToastService.Error("编辑失败");
                return;
            }
            if (editingConfig.Key!=editingKey&& ConfigItems.Any(c=>c.Key==editingConfig.Key))
            {
                await ToastService.Error("键已存在");
                return;
            }
            var conf = ConfigItems.FirstOrDefault(c => c.Key == editingKey);
            if (conf==null)
            {
                await ToastService.Error("编辑失败");
                return;
            }
            ConfigItems.Remove(conf);
            ConfigItems.Add(editingConfig);
            editingConfig = null;
            ConfigItems.Sort((a, b) => a.Key.CompareTo(b.Key));
            await EditKVModal!.Close();
        }
        void DeleteConfig (string key)
        {
            var removeItem = ConfigItems.FirstOrDefault(c => c.Key == key);
            if (removeItem != null)
                ConfigItems.Remove(removeItem);
        }
        async Task AddConfig()
        {
            ConfigItems.Add(new ConfigItem
            {
                Key = await Task.Run(() =>
                {
                    for (int i = 1; ; i++)
                    {
                        var key = "配置项" + i.ToString();
                        if (ConfigItems.FirstOrDefault(c => c.Key == key) == null)
                            return key;
                    }
                }),
                IsInherited = false
            });
            ConfigItems.Sort((a, b) => a.Key.CompareTo(b.Key));
        }   
        
        async Task LoadConfigList()
        {
            var user = await AuthHelper.GetUser();
            if (user == null)
            {
                await ToastService.Error("请先登录");
                return;
            }
            using var context = new ApplicationDbContext();
            var userEnvs = (from u in context.UserEnvs where u.UserId == user.Id select u.EnvId).ToHashSet();
            ConfigList = new List<ConfigModel> { new ConfigModel { Env = "default", HasPermission = await AuthHelper.IsAdmin() } }.Concat(from u in context.Envs select new ConfigModel { Env = u.Name, HasPermission = userEnvs.Contains(u.Id) }).ToList();
        }
        protected override async Task OnInitializedAsync()
        {
            await LoadConfigList();
            await base.OnInitializedAsync();
        }
        async Task AsyncConfigs()
        {
            await ConfigManager.CreateConfigForAllEnv();

        }
    }
}
