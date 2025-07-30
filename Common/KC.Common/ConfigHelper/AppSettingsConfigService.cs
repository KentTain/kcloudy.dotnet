using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Com.Framework.Util;
using Com.Framework.Util.Config;
using Microsoft.Extensions.Configuration;

namespace Com.Common.ConfigHelper
{
    public class AppSettingsConfigService : IConfig
    {
        public string GetConfigItem(string name)
        {
            return GetConfigurationSetting(name, null, true);
        }

        public string GetConfigurationSetting(string configurationSetting, string defaultValue, bool throwIfNull)
        {
            if (string.IsNullOrEmpty(configurationSetting))
            {
                throw new Exception("ConfigurationSetting cannot be empty or null: " + configurationSetting);
            }

            // first, try to read from appsettings
            string ret = TryGetAppSetting(configurationSetting);

            if (ret == null && throwIfNull)
            {
                throw new Exception("Cannot find configuration string: " + configurationSetting);
            }

            return ret;
        }

        private static IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        private static string TryGetAppSetting(string configName)
        {
            string ret;
            try
            {
                var Configuration = builder.Build();
                ret = Configuration.GetSection("AppSettings")[configName];
                //ret = ConfigurationManager.AppSettings[configName];
            }
            catch (Exception ex)
            {
                return null;
            }

            return ret;
        }
    }
}