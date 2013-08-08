using System.Collections.Generic;
using MIndexer.Core.DataTypes;

namespace MIndexer.Core.Interfaces
{
    public interface IDownloadManager
    {
        List<FileData> DownloadFolder(object folderPath);
        string DownloadLyrics(string relativeUrl);
        string ScrapLyrics(string pageContent);
        string ConvertFromASCII(string rawLyrics);

    }
}