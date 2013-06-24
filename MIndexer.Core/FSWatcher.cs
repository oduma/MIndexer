using System;
using System.IO;
using System.Threading;
using MIndexer.Core.DataTypes;
using Sciendo.Common.Logging;

namespace MIndexer.Core
{
    public class FSWatcher
    {
        public string Path { get; private set; }

        private Action<string, OperationType> _queueOperation;

        private Action<string, string> _queueRenameOperation;

        public FSWatcher(string localRootFolder, Action<string, OperationType> queueOperation, Action<string, string> queueRenameOperation)
        {
            LoggingManager.Debug("Initializing the FS Watcher with publisher: " + localRootFolder);
            if (string.IsNullOrEmpty(localRootFolder))
                throw new ArgumentNullException("localRootFolder");
            if (!Directory.Exists(localRootFolder))
                throw new ArgumentException("localRootFolder does not exist");
            if (queueOperation == null)
                throw new ArgumentNullException("queueOperation");
            _queueOperation = queueOperation;
            if (queueRenameOperation == null)
                throw new ArgumentNullException("queueRenameOperation");
            _queueRenameOperation = queueRenameOperation;

            FileSystemWatcher fsWatcher = new FileSystemWatcher(localRootFolder);
            fsWatcher.IncludeSubdirectories = true;


            Path = fsWatcher.Path;

            fsWatcher.Created += fsWatcher_Created;
            fsWatcher.Changed += fsWatcher_Changed;
            fsWatcher.Deleted += fsWatcher_Deleted;
            fsWatcher.Renamed += fsWatcher_Renamed;
            fsWatcher.EnableRaisingEvents = true;
            LoggingManager.Debug("Initilization done waiting for changes in the FS.");
        }


        private void fsWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            LoggingManager.Debug("A File renamed from " + e.OldFullPath + " to " + e.FullPath);
            //if it is a directory ignore it
            if (!File.Exists(e.FullPath))
                return;
            //queue an insert;
            // Wait if file is still open
            FileInfo fileInfo = new FileInfo(e.FullPath);
            while (IsFileLocked(fileInfo))
            {
                Thread.Sleep(500);
            }

            _queueRenameOperation(e.OldFullPath, e.FullPath);

        }

        private void fsWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            LoggingManager.Debug("A file deleted: " + e.FullPath);
            if (Directory.Exists(e.FullPath))
                return;
            _queueOperation(e.FullPath, OperationType.Delete);
        }

        private void fsWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            LoggingManager.Debug("A file changed: " + e.FullPath);
            //if it is a directory ignore it
            if (!File.Exists(e.FullPath))
                return;
            //queue an insert;
            // Wait if file is still open
            FileInfo fileInfo = new FileInfo(e.FullPath);
            while (IsFileLocked(fileInfo))
            {
                Thread.Sleep(500);
            }
            //queue an update;
            _queueOperation(e.FullPath, OperationType.Update);
        }

        private void fsWatcher_Created(object sender, FileSystemEventArgs e)
        {
            LoggingManager.Debug("A new file created: " + e.FullPath);
            //if it is a directory ignore it
            if (!File.Exists(e.FullPath))
                return;
            //queue an insert;
            // Wait if file is still open
            FileInfo fileInfo = new FileInfo(e.FullPath);
            while (IsFileLocked(fileInfo))
            {
                Thread.Sleep(500);
            }
            _queueOperation(e.FullPath, OperationType.Insert);
        }

        static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open,
                         FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

    }
}
