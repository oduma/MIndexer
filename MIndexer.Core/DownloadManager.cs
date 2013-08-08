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
    public class DownloadManager : IDownloadManager
    {
        private readonly string _mRootFolder;
        private readonly string _lRootFolder;
        private readonly string[] _lExtentionsFilter;
        private readonly ITagReaderHelper _tagReaderHelper;
        private readonly FileMap _fileMap;

        public DownloadManager(string mRootFolder, 
            string lRootFolder, 
            string[] lExtentionsFilter, 
            ITagReaderHelper tagReaderHelper, 
            FileMap fileMap)
        {
            if (string.IsNullOrEmpty(mRootFolder))
                throw new ArgumentNullException("mRootFolder");
            if (string.IsNullOrEmpty(lRootFolder))
                throw new ArgumentNullException("lRootFolder");
            if (lExtentionsFilter == null || lExtentionsFilter.Length == 0)
                throw new ArgumentNullException("lExtentionsFilter");
            if (tagReaderHelper == null)
                throw new ArgumentNullException("tagReaderHelper");
            if (!Directory.Exists(mRootFolder))
                throw new ArgumentException("Folder does not exist", "mRootFolder");
            if (!Directory.Exists(lRootFolder))
                throw new ArgumentException("Folder does not exist", "lRootFolder");
            _mRootFolder = mRootFolder;
            _lRootFolder = lRootFolder;
            _lExtentionsFilter = lExtentionsFilter;
            _tagReaderHelper = tagReaderHelper;
            _fileMap = fileMap;
        }

        public List<FileData> DownloadFolder(object folderPath)
        {
            FileData currentFolder = (FileData)folderPath;
            List<FileData> result= new List<FileData>();
            foreach(FileMap fileMap in 
                _fileMap.GetSubFileMap(currentFolder.Name).SubFiles.Where(f => string.IsNullOrEmpty(f.FileData.LName)))
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

        private readonly string rootUrl = "http://www.songlyrics.com/";

        public virtual string DownloadLyrics(string relativeUrl)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(relativeUrl))
                throw new ArgumentNullException("relativeUrl");
            try
            {
                HttpWebRequest httpWebRequest =
                    (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", rootUrl, relativeUrl));
                WebResponse myResponse = httpWebRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream());
                result = ScrapLyrics(sr.ReadToEnd());
                sr.Close();
                myResponse.Close();
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
            }
            return result;
        }

        public string ScrapLyrics(string pageContent)
        {
            int startOfDiv = pageContent.IndexOf(@"id=""songLyricsDiv""");
            int startOfText = pageContent.IndexOf(@">", startOfDiv) + 1;
            int stopOfText = pageContent.IndexOf(@"</p>", startOfDiv);
            var rawLyrics = pageContent.Substring(startOfText, stopOfText - startOfText);
            rawLyrics = rawLyrics.Replace(@"<br />", " ");

            return ConvertFromASCII(rawLyrics).Replace("\r", "").Replace("\n", "");
        }

        public string ConvertFromASCII(string rawLyrics)
        {
            for (int i = 0; i < 256; i++)
                rawLyrics = rawLyrics.Replace("&#" + i + ";", Convert.ToString((char)i));
            return rawLyrics;
        }

        private string RetrieveLyricsForAnMFile(string filePath)
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
                    Lyrics lyrics = new Lyrics();
                    var id3Tag = _tagReaderHelper.GetID3Tag(filePath);
                    if (id3Tag == null)
                        return null;
                    lyrics.Content = DownloadLyrics(GetRelativeUrl(id3Tag));
                    if (string.IsNullOrEmpty(lyrics.Content))
                        return null;
                    lyrics.TargetFileName = filePath;
                    Serializer.SerializeOneToFile(lyrics, lyricsFilePath);
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

        private string GetRelativeUrl(ID3Tag id3Tag)
        {
            if (!id3Tag.HasTitle || !id3Tag.HasArtist)
                return null;
            return string.Format("{0}/{1}-lyrics/", id3Tag.Artist.Replace(" ", "-").Replace("/", ""), id3Tag.Title.Replace(" ", "-"));
        }

    }
}
