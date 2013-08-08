using System;
using MIndexer.Core.DataTypes;

namespace MIndexer.Core
{
    public class BulkFileProgressEventArgs:EventArgs
    {
        public string DirectoryName { get; private set; }
        public int FilesProcessed { get; private set; }
        public State ToState { get; private set; }
        public FileType FileType { get; private set; }

        public BulkFileProgressEventArgs(string directoryName, int filesProcessed, State toState, FileType fileType)
        {
            DirectoryName = directoryName;
            FilesProcessed = filesProcessed;
            ToState = toState;
            FileType = fileType;
        }
    }
}
