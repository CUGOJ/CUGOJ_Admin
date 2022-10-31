using BootstrapBlazor.Components;
using CUGOJ.CUGOJ_Admin.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace CUGOJ.CUGOJ_Admin.Config
{
    public static class ConfigManager
    {
        private static IDictionary<string, Config> configList = new ConcurrentDictionary<string, Config>();
        static ConfigManager()
        {

        }
        private static void LoadConfig(string env)
        {
            if (configList.ContainsKey(env)) return;
            using var context = new ConfigContext();
            var conf = (from c in context.Configs where c.Env == env select c).FirstOrDefault();
            if (conf != null)
            {
                configList[env] = conf;
            }
        }

        private static void CreateConfigEnv(string env)
        {
            using var context = new ConfigContext();
            if ((from c in context.Configs where c.Env == env select c).Any())
            {
                throw new Exception("对应环境的配置已存在");
            }
            context.Configs.Add(new Config()
            {
                Env = env
            });
            context.SaveChanges();
            LoadConfig(env);
        }

        public static Config? GetConfig(string env)
        {
            if (!configList.ContainsKey(env))
                LoadConfig(env);
            configList.TryGetValue(env, out Config? conf);
            return conf;
        }

        public static Config MustGetConfig(string env)
        {
            if (!configList.ContainsKey(env))
                LoadConfig(env);
            configList.TryGetValue(env, out Config? conf);
            if (conf == null)
                return new Config();
            return conf;
        }

        public static async Task<List<Config>> GetConfigList()
        {
            using var context = new ConfigContext();
            return await context.Configs.ToListAsync();
        }
        
        public static async Task UpdateConfig(string env, Config config)
        {
            using var context = new ConfigContext();
            var conf = (from c in context.Configs where c.Env == env select c).FirstOrDefault();
            if (conf == null)
                context.Configs.Add(config);
            else
                conf.ConfigList = config.ConfigList;
            await context.SaveChangesAsync();
            configList[env] = config;
        }
        
        public static async Task CreateConfigForAllEnv()
        {
            using var configContext = new ConfigContext();
            using var envContext = new ApplicationDbContext();
            var envList = await envContext.Envs.ToListAsync();
            var configList = configContext.Configs.ToHashSet();
            foreach (var env in envList)
            {
                if (!configList.Any(c => c.Env == env.Name))
                {
                    configContext.Configs.Add(new Config()
                    {
                        Env = env.Name
                    });
                }
            }
            await configContext.SaveChangesAsync();
        }
    }
}

