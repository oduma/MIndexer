using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ID3Sharp;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using MIndexer.Core.DataTypes;
using MIndexer.Core.Interfaces;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;
using Directory = System.IO.Directory;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace MIndexer.Core
{
    public class Indexer:IDisposable
    {
        private Dictionary<string, IFileIndexerSearcher> _indexers;
        private MFileIndexerSearcher _mFileIndexer;
        private LFileIndexerSearcher _lFileIndexer;
        private FSWatcher _fsWatcher;
        private DirectoryInfo _dataFolder;
        private LuceneDirectory _indexDir;
        readonly static SimpleFSLockFactory _lockFactory = new SimpleFSLockFactory();
        private IndexWriter _writer;
        internal DownloaderHelper _downloaderHelper;

        public Indexer(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException("folderName");
            if (!Directory.Exists(folderName))
                throw new ArgumentException("Directory does not exist", "folderName");
            _dataFolder = new DirectoryInfo(folderName);
            var indexFolder = new DirectoryInfo(Utils.GetFolderFromConfiguration("IndexDir"));
            _indexDir = FSDirectory.Open(indexFolder, _lockFactory);
            _mFileIndexer= new MFileIndexerSearcher();
            _lFileIndexer= new LFileIndexerSearcher();
            _writer = new IndexWriter(_indexDir, new StandardAnalyzer(
                          Lucene.Net.Util.Version.LUCENE_30),
                          IndexWriter.MaxFieldLength.UNLIMITED);
            _writer.SetMergePolicy(new LogDocMergePolicy(_writer));
            _writer.MergeFactor = 5;
            _downloaderHelper = new DownloaderHelper();

        }

        public void BuildIndexOnFolder(List<Action<string>> processTheFile)
        {
            LoggingManager.Debug("Discovering from root folder:" + _dataFolder.FullName);
            if(processTheFile==null)
                throw new ArgumentNullException("processTheFile");
 
            IndexFolder(_dataFolder, processTheFile);
            _writer.Optimize();
            _writer.Dispose();
        }

        private void IndexFolder(DirectoryInfo currentFolder, List<Action<string>> processTheFile)
        {
            var files = currentFolder.EnumerateFiles();
            if (files.Any())
                foreach (FileInfo file in files)
                {
                    foreach(var processingMethod in processTheFile)
                        processingMethod(file.FullName);
                }
            currentFolder.EnumerateDirectories().AsParallel().ForAll(f => IndexFolder(f, processTheFile));
        }

        public void StartMonitoringFolder()
        {
            _fsWatcher = new FSWatcher(_dataFolder.FullName,OperationProcess,RenameOperationProcess);
        }

        private void RenameOperationProcess(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        internal void OperationProcess(string fileName,OperationType operationType)
        {
            throw new NotImplementedException();
        }

        public void IndexAnMFile(string filePath)
        {
            if(!File.Exists(filePath))
                throw new ArgumentNullException("filePath");
            string[] mExtentions = Utils.GetExtentionsFromConfiguration("MExtensions").Split(new char[] {','});
            if(!mExtentions.Any(e=>!string.IsNullOrEmpty(e)))
                mExtentions=new string[]{".mp3",".ogg",".flac"};
            else
                mExtentions = mExtentions.Select(e => (e.StartsWith(".")) ? e : "." + e).ToArray();

            if (mExtentions.Any(e => Path.GetExtension(filePath) == e))
            {
                try
                {
                    if(_mFileIndexer.Search("filename:" +filePath.Replace("\\","/").Replace(" ","").Substring(filePath.IndexOf(Path.VolumeSeparatorChar)+1) ,1,new []{"filename"}).Count()<=0)
                        _writer.AddDocument(_mFileIndexer.PrepareDocument(filePath));
                }
                catch (Exception ex)
                {
                    LoggingManager.LogSciendoSystemError(ex);
                    throw;
                }
            }
        }

        public void IndexAnLFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new ArgumentNullException("filePath");

            if (Path.GetExtension(filePath) == ".lyrics")
            {
                try
                {
                    if (_lFileIndexer.Search("filename:" + filePath.Replace("\\", "/").Replace(" ","").Substring(filePath.IndexOf(Path.VolumeSeparatorChar) + 1), 1,new string[]{"filename"}).Count() <= 0)
                        _writer.AddDocument(_lFileIndexer.PrepareDocument(filePath));
                }
                catch (Exception ex)
                {
                    LoggingManager.LogSciendoSystemError(ex);
                    throw;
                }
            }
        }

        public void RetrieveLyricsForAnMFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new ArgumentNullException("filePath");
            string[] mExtentions = Utils.GetExtentionsFromConfiguration("MExtensions").Split(new char[] { ',' });
            if (!mExtentions.Any(e => !string.IsNullOrEmpty(e)))
                mExtentions = new string[] { ".mp3", ".ogg", ".flac" };
            else
                mExtentions = mExtentions.Select(e => (e.StartsWith(".")) ? e : "." + e).ToArray();

            if (mExtentions.Any(e => Path.GetExtension(filePath) == e))
            {
                try
                {
                    Lyrics lyrics = new Lyrics();
                    var id3Tag = _mFileIndexer.GetID3Tag(filePath);
                    if (id3Tag == null)
                        return;
                    lyrics.Content=_downloaderHelper.DownloadLyrics(GetRelativeUrl(id3Tag));
                    if (string.IsNullOrEmpty(lyrics.Content))
                        return;
                    lyrics.TargetFileName = filePath;
                    Serializer.SerializeOneToFile(lyrics,
                                                  Path.Combine(Utils.GetFolderFromConfiguration("LyricsDir"),
                                                               Path.GetFileNameWithoutExtension(filePath) + ".lyrics"));
                }
                catch (Exception ex)
                {
                    LoggingManager.LogSciendoSystemError(ex);
                }
            }
            
        }

        private string GetRelativeUrl(ID3Tag id3Tag)
        {
            if (!id3Tag.HasTitle || !id3Tag.HasArtist)
                return null;
            return string.Format("{0}/{1}-lyrics/", id3Tag.Artist.Replace(" ", "-").Replace("/",""), id3Tag.Title.Replace(" ", "-"));
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
