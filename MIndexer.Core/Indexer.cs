using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using MIndexer.Core.DataTypes;
using MIndexer.Core.Interfaces;
using Sciendo.Common.Logging;
using Directory = System.IO.Directory;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace MIndexer.Core
{
    public class Indexer
    {
        private MFileIndexer _mFileIndexer;
        private FSWatcher _fsWatcher;
        private DirectoryInfo _dataFolder;
        private LuceneDirectory _indexDir;
        readonly static SimpleFSLockFactory _lockFactory = new SimpleFSLockFactory();

        public Indexer(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException("folderName");
            if (!Directory.Exists(folderName))
                throw new ArgumentException("Directory does not exist", "folderName");
            _dataFolder = new DirectoryInfo(folderName);
            var indexFolder = new DirectoryInfo(GetConfiguration("IndexDir"));
            _indexDir = FSDirectory.Open(indexFolder, _lockFactory);
            _mFileIndexer= new MFileIndexer();
        }

        private string GetConfiguration(string indexDirKey)
        {
            string folderName = string.Empty;
            try
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(indexDirKey))
                {
                    folderName = ConfigurationManager.AppSettings[indexDirKey];
                    if (string.IsNullOrEmpty(folderName))
                        return AppDomain.CurrentDomain.BaseDirectory;
                    if (Directory.Exists(folderName))
                        return folderName;
                    return AppDomain.CurrentDomain.BaseDirectory;
                }
                return AppDomain.CurrentDomain.BaseDirectory;
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public void BuildIndexOnFolder(Action<string,MFileIndexer,IndexWriter> processTheFile)
        {
            LoggingManager.Debug("Discovering from root folder:" + _dataFolder.FullName);
            if(processTheFile==null)
                throw new ArgumentNullException("processTheFile");
            var writer = new IndexWriter(_indexDir, new StandardAnalyzer(
                                      Lucene.Net.Util.Version.LUCENE_30),
                                      IndexWriter.MaxFieldLength.UNLIMITED);
            writer.SetMergePolicy(new LogDocMergePolicy(writer));
            writer.MergeFactor = 5;
 
            IndexFolder(_dataFolder, processTheFile, writer);
            writer.Optimize();
            writer.Dispose();
        }

        private void IndexFolder(DirectoryInfo currentFolder, Action<string, MFileIndexer,IndexWriter> processTheFile,IndexWriter writer)
        {
            var files = currentFolder.EnumerateFiles();
            if (files.Any())
                foreach (FileInfo file in files)
                {
                    processTheFile(file.FullName, _mFileIndexer, writer);
                }
            currentFolder.EnumerateDirectories().AsParallel().ForAll(f => IndexFolder(f, processTheFile,writer));
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

        internal void ProcessTheFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public static void IndexAnMFile(string filePath, MFileIndexer mFileIndexer,IndexWriter indexWriter)
        {
            try
            {
                var indexedDoc = mFileIndexer.PrepareDocument(filePath);
                indexWriter.AddDocument(indexedDoc);
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                throw;
            }
        }
    }
}
