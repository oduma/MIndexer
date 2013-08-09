using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Documents;
using MIndexer.Core.DataTypes;
using MIndexer.Core.IndexSearchers;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;

namespace MIndexer.Core.IndexMaintainers
{
    public class LIndexManager : BaseIndexManager
    {
        protected override void RenameOperationProcess(string oldPath, string newPath)
        {
            //oldPath.RemoveFileFromIndex();
            //newPath.IndexFile();
        }

        protected override void OperationProcess(string path, OperationType operationType)
        {
            switch (operationType)
            {
                case OperationType.Insert:
                    path.IndexFile(this);
                    break;
                case OperationType.Update:
                    path.UpdateIndexForLFile();
                    break;
                case OperationType.Delete:
//                    path.RemoveFileFromIndex();
                    break;
            }
        }

        internal override Document PrepareDocument(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");
            if (!File.Exists(filePath))
                throw new ArgumentException("File does not exist:" + filePath);
            Lyrics lyrics = new Lyrics();
            try
            {
                lyrics = Serializer.DeserializeOneFromFile<Lyrics>(filePath);
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
            }
            if (lyrics == null || string.IsNullOrEmpty(lyrics.Content))
                throw new ArgumentException("Cannot read lyrics from file: " + filePath);
            if (string.IsNullOrEmpty(lyrics.TargetFileName))
                throw new ArgumentException("Cannot read target file name from file: " + filePath);
            if (!File.Exists(lyrics.TargetFileName))
                throw new ArgumentException("Target file does not exist:" + lyrics.TargetFileName);
            return PrepareDocument(filePath, lyrics.Content, lyrics.TargetFileName);
        }

        protected override string[] AcceptableExtentions
        {
            get { return new string[] {".lyrics"}; }
        }
    }
}
