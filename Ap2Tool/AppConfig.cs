using ApCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ap2Tool
{
    public class AppConfig
    {
       
        public static void CreateAppConfigFile()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            KeyValueConfigurationCollection cfgCollection = config.AppSettings.Settings;

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("ShowUiGroup", "false");
            dict.Add("ShowDevelopGroup", "false");
            dict.Add("All_Font_Size", "14");
            dict.Add("Font_Name", "Arial");
            dict.Add("Lang", "Eng");

            foreach (KeyValuePair<string, string> kvp in dict)
            {
                cfgCollection.Add(kvp.Key, kvp.Value);
            }
            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }

        public static string GetValue(string strKey, string strDefault)
        {
            try
            {
                return ConfigurationManager.AppSettings[strKey].ToString();
            }            
            catch 
            {
                return strDefault;
            }   
        }


        public static void SetValue(string strKey, string strVal)
        {
            ConfigurationManager.AppSettings[strKey] = strVal;

            // 파일에 쓰기 
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(strKey);
            config.AppSettings.Settings.Add(strKey, strVal);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }
    }
}
