using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HenryMilesLicenceOnlineWpf.Helper
{
    internal static class ConfigHelper
    {
        public static void UpdateValue(Configuration configuration, string key, dynamic value, string section)
        {
            if (configuration != null)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    string valueAsString = GiveBackString(value);
                    TryUpdateKey(configuration, key, valueAsString);

                    if (TryUpdateKey(configuration, key, valueAsString))
                    {
                        TrySaveChanges(configuration, section);
                    }
                }
            }
        }

        private static void TrySaveChanges(Configuration configuration, string section)
        {
            try
            {
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(section);
            }
            catch (Exception exept)
            {
                // throw;
            }
        }

        private static bool TryUpdateKey(Configuration configuration, string key, string valueAsString)
        {
            bool result = false;

            try
            {
                configuration.AppSettings.Settings[key].Value = valueAsString;
                result = true;
            }
            catch (Exception exept)
            {
                result = false;
                //throw;
            }

            return result;
        }

        private static string GiveBackString(dynamic value)
        {
            string result = string.Empty;

            if (value is not String)
            {
                result = value.ToString();
            }
            else
            {
                result = value;
            }

            return result;
        }
    }
}