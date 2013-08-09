using MIndexer.Core.IndexSearchers;

namespace MIndexer.Core.IndexMaintainers
{
    public interface IIndexManager:IFileSearcher
    {
        void StartMonitoringFolder(string folderPath);

        bool IndexAFile(string filePath);

        void CommitAndOptimize();

        void CloseWriter();

    }
}
