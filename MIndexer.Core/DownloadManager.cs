using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ID3Sharp;
using MIndexer.Core.DataTypes;
using MIndexer.Core.Interfaces;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;

namespace MIndexer.Core
{
    public class DownloadManager
    {
        private readonly string _mRootFolder;
        private readonly string _lRootFolder;
        private readonly string[] _lExtentionsFilter;
        private readonly IContentProvider _contentProvider;
        private readonly FileMap _fileMap;

        public DownloadManager(string mRootFolder, 
            string lRootFolder, 
            string[] lExtentionsFilter, 
            IContentProvider contentProvider, 
            FileMap fileMap)
        {
            if (string.IsNullOrEmpty(mRootFolder))
                throw new ArgumentNullException("mRootFolder");
            if (string.IsNullOrEmpty(lRootFolder))
                throw new ArgumentNullException("lRootFolder");
            if (lExtentionsFilter == null || lExtentionsFilter.Length == 0)
                throw new ArgumentNullException("lExtentionsFilter");
            if (contentProvider == null)
                throw new ArgumentNullException("contentProvider");
            if (fileMap == null)
                throw new ArgumentNullException("fileMap");
            if (!Directory.Exists(mRootFolder))
                throw new ArgumentException("Folder does not exist", "mRootFolder");
            if (!Directory.Exists(lRootFolder))
                throw new ArgumentException("Folder does not exist", "lRootFolder");
            _mRootFolder = mRootFolder;
            _lRootFolder = lRootFolder;
            _lExtentionsFilter = lExtentionsFilter;
            _contentProvider = contentProvider;
            _fileMap = fileMap;
        }

        public List<FileData> DownloadFolder(object folderPath)
        {
            string currentFolder = (string)folderPath;
            List<FileData> result= new List<FileData>();
            foreach(FileMap fileMap in 
                _fileMap.GetSubFileMap(currentFolder).SubFiles.Where(f => string.IsNullOrEmpty(f.FileData.LName)))
            {
                var lFileName = RetrieveLyricsForAnMFile(fileMap.FileData.Name);
                if (!string.IsNullOrEmpty(lFileName))
                    result.Add(new FileData
                                   {
                                       IsFolder = false,
                                       LName = lFileName,
                                       LState = State.Downloaded,
                                       Name = fileMap.FileData.Name,
                                       State = fileMap.FileData.State
                                   });
            }
            return result;
        }

        private string CalculateLyricsFilePath(string mFileName)
        {
            var mRootFolderParts = _mRootFolder.Split(Path.PathSeparator);
            var lRootCalculatedFolder = Path.Combine(_lRootFolder, mRootFolderParts[mRootFolderParts.Length - 1]);
            return string.Format("{0}.{1}", mFileName.Replace(_mRootFolder, lRootCalculatedFolder), _lExtentionsFilter[0]);
        }

        internal string RetrieveLyricsForAnMFile(string filePath)
        {
            string[] mExtentions = IOHelper.GetMExtentions();

            if (mExtentions.Any(e => Path.GetExtension(filePath) == e))
            {
                try
                {
                    string lyricsFilePath=CalculateLyricsFilePath(filePath);
                    var lyricsFolder = Path.GetDirectoryName(lyricsFilePath);

                    if (!string.IsNullOrEmpty(lyricsFolder))
                        if (!Directory.Exists(lyricsFolder))
                            Directory.CreateDirectory(lyricsFolder);
                    ContentAndTarget content = _contentProvider.GetContentAndTarget(filePath);
                    if (content==null || string.IsNullOrEmpty(content.Content))
                        return null;
                    Serializer.SerializeOneToFile(content, lyricsFilePath);
                    return lyricsFilePath;
                }
                catch (Exception ex)
                {
                    LoggingManager.LogSciendoSystemError(ex);
                    return null;
                }
            }
            return null;
        }

    }
}
