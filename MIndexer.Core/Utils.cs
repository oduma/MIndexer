using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Sciendo.Common.Logging;

namespace MIndexer.Core
{
    public static class Utils
    {

        public static string GetFolderFromConfiguration(string dirKey)
        {
            string folderName = string.Empty;
            try
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(dirKey))
                {
                    folderName = ConfigurationManager.AppSettings[dirKey];
                    if (string.IsNullOrEmpty(folderName))
                        return AppDomain.CurrentDomain.BaseDirectory;
                    if (Directory.Exists(folderName))
                        return folderName;
                    return AppDomain.CurrentDomain.BaseDirectory;
                }
                return AppDomain.CurrentDomain.BaseDirectory;
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public static string GetExtentionsFromConfiguration(string extensionsKey)
        {
            try
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(extensionsKey))
                {
                   return ConfigurationManager.AppSettings[extensionsKey];
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                return string.Empty;
            }
        }
    }
}
