using System;
using System.IO;
using System.Linq;
using MIndexer.Core.IndexMaintainers;
using MIndexer.Core.IndexSearchers;
using MIndexer.Core.Interfaces;
using Sciendo.Common.Logging;

namespace MIndexer.Core
{
    public static class ExtentionMethods
    {
        public static string ReplaceAny(this string inString,string[] replace, string replaceWith)
        {
            if(replace==null || !replace.Any() || replaceWith==null)
                return inString;
            replace.ToList().ForEach((s) =>
                                         {
                                             inString = inString.Replace(s, replaceWith);
                                         });
            return inString;
        }

        //public static string IndexMFile(this string filePath)
        //{
        //    try
        //    {
        //        MIndexMaintainer mIndexMaintainer = new MIndexMaintainer();
        //        mIndexMaintainer.IndexAFile(filePath);
        //        return filePath;
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggingManager.LogSciendoSystemError("Music file not indexed " + filePath,ex);
        //        return null;
        //    }
        //}

        public static bool IndexFile(this string filePath,IIndexManager indexMaintainer)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            if (indexMaintainer == null)
                return false;
            if (!File.Exists(filePath))
                return false;
            try
            {
                return indexMaintainer.IndexAFile(filePath);
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError("File not indexed " + filePath, ex);
                return false;
            }
        }


        //public static string RemoveFileFromIndex(this string filePath)
        //{
        //    if (string.IsNullOrEmpty(filePath))
        //        return null;
        //    try
        //    {
        //        MIndexMaintainer mIndexMaintainer = new MIndexMaintainer();
        //        mIndexMaintainer.RemoveAFileFromIndex(filePath);
        //        return filePath;
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggingManager.LogSciendoSystemError(ex);
        //        return null;
        //    }

        //}
        
        public static string DeleteLyricsFile(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;
            try
            {
                return IOHelper.DeleteLyricsFileForAnMFile(filePath);

            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                return null;
            }
        }

        //public static string UpdateIndexForMFile(this string filePath)
        //{
        //    if (string.IsNullOrEmpty(filePath))
        //        return null;
        //    try
        //    {
        //        MIndexMaintainer mIndexMaintainer = new MIndexMaintainer();
        //        mIndexMaintainer.UpdateIndexForAFile(filePath);
        //        return filePath;
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggingManager.LogSciendoSystemError(ex);
        //        return null;
        //    }

        //}

        public static string UpdateIndexForLFile(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;
            try
            {
                LIndexManager lIndexMaintainer = new LIndexManager();
                lIndexMaintainer.UpdateIndexForAFile(filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                return null;
            }

        }
    }
}
