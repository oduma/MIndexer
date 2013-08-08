using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using MIndexer.Core.DataTypes;
using MIndexer.Core.IndexSearchers;
using Sciendo.Common.Logging;

namespace MIndexer.Core.IndexMaintainers
{
    public abstract class BaseIndexManager:FileSearcher,IIndexManager
    {
        private FSWatcher _fsWatcher;
        private IndexWriter _writer;

        public BaseIndexManager()
        {
            _writer = GetIndexWriter();
            
        }
        public void StartMonitoringFolder(string folderPath)
        {
            _fsWatcher = new FSWatcher(folderPath, OperationProcess, RenameOperationProcess);
        }

        protected abstract void RenameOperationProcess(string oldPath, string newPath);

        protected abstract void OperationProcess(string path, OperationType operationType);

        internal abstract Document PrepareDocument(string filePath);


        protected Document PrepareDocument(string filePath, string content, string targetFileName)
        {
            Document document = new Document();

            document.Add(new Field("tagged", content, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));
            document.Add(new Field("filename",
                                   filePath.ReplaceAny(new string[] { "\\", " ", "." }, "").Substring(
                                       filePath.IndexOf(Path.VolumeSeparatorChar) + 1), Field.Store.YES,
                                   Field.Index.ANALYZED, Field.TermVector.YES));
            document.Add(new Field("targetfilename", targetFileName, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));

            return document;
        }



        private IndexWriter GetIndexWriter()
        {
            var indexFolder = new DirectoryInfo(Utils.GetFolderFromConfiguration("IndexDir"));
            var indexDir = FSDirectory.Open(indexFolder, _lockFactory);
            var writer = new IndexWriter(indexDir, new StandardAnalyzer(
                                                    Lucene.Net.Util.Version.LUCENE_30),
                                      IndexWriter.MaxFieldLength.UNLIMITED);
            writer.SetMergePolicy(new LogDocMergePolicy(_writer));
            writer.MergeFactor = 5;
            return writer;
        }

        public bool IndexAFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new ArgumentException("File does not exist "+ filePath);
            if(!AcceptableExtentions.Any(e=>e==Path.GetExtension(filePath)))
                throw new ArgumentException("Wrong file for this Manager");
            try
            {
                if (Search("filename:" + filePath.ReplaceAny(new string[] { "\\", " ", "." }, "").Substring(filePath.IndexOf(Path.VolumeSeparatorChar) + 1), 1, new[] { "filename" }).Count() <= 0)
                    _writer.AddDocument(PrepareDocument(filePath));
                return true;
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                throw;
            }
        }

        protected abstract string[] AcceptableExtentions { get;}

        public List<FileData> IndexFiles(object downloadedFiles)
        {
            List<FileData> results = (List<FileData>)downloadedFiles;
            foreach (FileData fileData in results)
            {
                if (fileData.LName.IndexFile(this))
                    fileData.LState = State.Indexed;

            }
            return results;
        }

        public void CommitAndOptimize()
        {
            _writer.Commit();
            _writer.Optimize();
        }

        public void CloseWriter()
        {
            if(_writer!=null)
            {
                _writer.Flush(true,true,true);
                _writer.Dispose();
            }
        }

        public void RemoveAFileFromIndex(string filePath)
        {
            try
            {
                    _writer.DeleteDocuments(PrepareQuery(
                        "filename:" +
                        filePath.ReplaceAny(new string[] {"\\", " ", "."}, "").Substring(
                            filePath.IndexOf(Path.VolumeSeparatorChar) + 1), 1, new[] {"filename"}));
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                throw;
            }

        }

        public void UpdateIndexForAFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new ArgumentNullException("filePath");
            try
            {
                if (Search("filename:" + filePath.ReplaceAny(new string[] { "\\", " ", "." }, "").Substring(filePath.IndexOf(Path.VolumeSeparatorChar) + 1), 1, new[] { "filename" }).Count() <= 0)
                    _writer.UpdateDocument(new Term("filename",filePath.ReplaceAny(new string[] { "\\", " ", "." }, "").Substring(filePath.IndexOf(Path.VolumeSeparatorChar) + 1)),PrepareDocument(filePath));
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                throw;
            }
        }
    }
}
