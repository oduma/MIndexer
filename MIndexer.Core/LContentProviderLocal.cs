using System;
using System.IO;
using MIndexer.Core.DataTypes;
using MIndexer.Core.Interfaces;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;

namespace MIndexer.Core
{
    public class LContentProviderLocal:IContentProvider
    {
        public ContentAndTarget GetContentAndTarget(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");
            if (!File.Exists(filePath))
                throw new ArgumentException("File does not exist:" + filePath);
            try
            {
                return Serializer.DeserializeOneFromFile<ContentAndTarget>(filePath);
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                return null;
            }
        }
    }
}
