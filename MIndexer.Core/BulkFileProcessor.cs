using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MIndexer.Core.DataTypes;
using MIndexer.Core.IndexMaintainers;
using MIndexer.Core.Interfaces;

namespace MIndexer.Core
{
    public class BulkFileProcessor
    {
        public event EventHandler<BulkFileProgressEventArgs> BulkFileProgress;

        public void ProcessLFilesFromMap(FileMap fileMap,
            IDownloadManager downloadManager,
            IIndexManager indexManager)
        {
            if (fileMap == null)
                throw new ArgumentNullException("fileMap");
            if (downloadManager == null)
                throw new ArgumentNullException("downloadManager");
            if (indexManager == null)
                throw new ArgumentNullException("indexManager");
            IndexingThreadedLauncher indexLauncher= new IndexingThreadedLauncher(indexManager);
            List<FileData> allSubDirectoriesWithFiles = fileMap.GetAllFolders(true);
            Task<List<FileData>> downloadTask = new Task<List<FileData>>(downloadManager.DownloadFolder, allSubDirectoriesWithFiles[0]);
            downloadTask.Start();
            for (int i = 1; i <= allSubDirectoriesWithFiles.Count; i++)
            {
                downloadTask.Wait();
                List<FileData> downloadedFiles = downloadTask.Result;
                if (BulkFileProgress != null)
                    BulkFileProgress(this,
                                     new BulkFileProgressEventArgs(Path.GetDirectoryName(downloadedFiles[0].Name),
                                                                   downloadedFiles.Count(
                                                                       d => d.LState == State.Downloaded),
                                                                   State.Downloaded, FileType.Lyrics));
                Task<List<FileData>> indexTask = new Task<List<FileData>>(indexLauncher.IndexLFiles, downloadedFiles);
                indexTask.Start();
                if (i < allSubDirectoriesWithFiles.Count)
                {
                    downloadTask = new Task<List<FileData>>(downloadManager.DownloadFolder, allSubDirectoriesWithFiles[i]);
                    downloadTask.Start();
                }
                indexTask.Wait();
                indexManager.CommitAndOptimize();
                List<FileData> indexedFiles = indexTask.Result;
                if (BulkFileProgress != null)
                    BulkFileProgress(this,
                                     new BulkFileProgressEventArgs(Path.GetDirectoryName(indexedFiles[0].Name),
                                                                   downloadedFiles.Count(
                                                                       d => d.LState == State.Indexed),
                                                                   State.Indexed, FileType.Lyrics));
                MarkDownloadedAndIndexedFiles(indexedFiles, fileMap);

            }
        }

        private void MarkDownloadedAndIndexedFiles(List<FileData> indexedFiles, FileMap fileMap)
        {
            foreach (FileData indexedFile in indexedFiles)
            {
                var subFileMap = fileMap.GetSubFileMap(indexedFile.Name);
                subFileMap.FileData.LName = indexedFile.LName;
                subFileMap.FileData.LState = indexedFile.LState;
            }
        }

        public void ProcessMFilesFromMap(FileMap fileMap, IIndexManager indexMaintainer)
        {
            if(fileMap==null)
                throw new ArgumentNullException("fileMap");
            if(indexMaintainer == null)
                throw new ArgumentNullException("indexMaintainer");
            MFilesProcessing(fileMap, indexMaintainer);
        }

        private void MFilesProcessing(FileMap fileMap, IIndexManager indexMaintainer)
        {
            foreach (FileMap subFileMap in fileMap.SubFiles.Where(s => !s.FileData.IsFolder))
            {
                if (subFileMap.FileData.Name.IndexFile(indexMaintainer))
                    subFileMap.FileData.State = State.Indexed;
            }
            if (BulkFileProgress != null)
                BulkFileProgress(this,
                                 new BulkFileProgressEventArgs(Path.GetDirectoryName(fileMap.FileData.Name),
                                                               fileMap.SubFiles.Count(
                                                                   s => s.FileData.State == State.Indexed),
                                                               State.Indexed, FileType.Music));
            indexMaintainer.CommitAndOptimize();
            foreach (FileMap subFolderMap in fileMap.SubFiles.Where(s => s.FileData.IsFolder))
                MFilesProcessing(subFolderMap, indexMaintainer);
        }

        public void ProcessLFilesFromMapThreaded(object obj)
        {
            throw new NotImplementedException();
        }

        public void ProcessMFilesFromMapThreaded(object obj)
        {
            throw new NotImplementedException();
        }
    }

    public class IndexingThreadedLauncher
    {
        private readonly IIndexManager _indexManager;

        public IndexingThreadedLauncher(IIndexManager indexManager)
        {
            _indexManager = indexManager;
        }

        public List<FileData> IndexLFiles(object downloadedFiles)
        {
            List<FileData> results = (List<FileData>)downloadedFiles;
            foreach (FileData fileData in results)
            {
                if (fileData.LName.IndexFile(_indexManager))
                    fileData.LState = State.Indexed;
            }
            return results;
        }

        public List<FileData> IndexMFiles(object downloadedFiles)
        {
            List<FileData> results = (List<FileData>)downloadedFiles;
            foreach (FileData fileData in results)
            {
                if (fileData.LName.IndexFile(_indexManager))
                    fileData.LState = State.Indexed;
            }
            return results;
        }

    }
}
