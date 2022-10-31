using Microsoft.AspNetCore.Mvc;

namespace CUGOJ.CUGOJ_Admin.Config
{
    public class ConfigService
    {
        public static Dictionary<string,string>GetConfig([FromBody]string env)
        {
            Dictionary<string, string> res = new();
            var envConf = ConfigManager.GetConfig(env);
            if (envConf!=null)
            {
                foreach (var (key, value) in envConf.ConfigList)
                {
                    res[key] = value;
                }
            }
            var defaultConf = ConfigManager.GetConfig("default");
            if (defaultConf != null)
            {
                foreach (var (key, value) in defaultConf.ConfigList)
                {
                    if (!res.ContainsKey(key))
                        res[key] = value;
                }
            }
            return res;
        }
        public static void Init(WebApplication app)
        {
            app.MapPost("/api/conf/getconfig", GetConfig);
        }
    }
}
