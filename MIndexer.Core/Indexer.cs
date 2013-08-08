using System;
using System.IO;
using System.Linq;
using Sciendo.Common.Logging;
using Directory = System.IO.Directory;

namespace MIndexer.Core
{
    //public class Indexer
    //{
    //    private DirectoryInfo _mDataFolder;

    //    public Indexer(string folderName)
    //    {
    //        if (string.IsNullOrEmpty(folderName))
    //            throw new ArgumentNullException("folderName");
    //        if (!Directory.Exists(folderName))
    //            throw new ArgumentException("Directory does not exist", "folderName");
    //        _mDataFolder = new DirectoryInfo(folderName);
    //    }

    //    public void BuildIndexOnFolder()
    //    {
    //        LoggingManager.Debug("Discovering from root folder:" + _mDataFolder.FullName);
    //        IndexFolder(_mDataFolder);
    //    }

    //    private void IndexFolder(DirectoryInfo currentFolder)
    //    {
    //        var files = currentFolder.EnumerateFiles();
    //        if (files.Any())
    //            foreach (FileInfo file in files)
    //                file.FullName.IndexMFile().RetrieveLyricsFile(TODO).IndexFile();

    //        currentFolder.EnumerateDirectories().AsParallel().ForAll(IndexFolder);
    //    }
    //}
}
