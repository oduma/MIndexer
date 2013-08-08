using System.Collections.Generic;
using MIndexer.Core.DataTypes;
using MIndexer.Core.IndexSearchers;

namespace MIndexer.Core.IndexMaintainers
{
    public interface IIndexManager:IFileSearcher
    {
        void StartMonitoringFolder(string folderPath);

        bool IndexAFile(string filePath);

        List<FileData> IndexFiles(object downloadedFiles);

        void CommitAndOptimize();

        void CloseWriter();

    }
}
