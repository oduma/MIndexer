using System;
using System.Collections.Generic;

namespace MIndexer.Core
{
    public class FileMapBuildProgressEventArgs:EventArgs
    {
        public FileMapBuildProgressEventArgs(List<string> files, string folder)
        {
            Files = files;
            Folder = folder;
        }

        public string Folder { get; private set; }

        public List<string> Files { get; private set; }
    }
}
