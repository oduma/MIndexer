using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MIndexer.Core.DataTypes;
using MIndexer.Core.IndexMaintainers;
using MIndexer.Core.Interfaces;
using Sciendo.Common.Logging;

namespace MIndexer.Core
{
    public class FileMapBuilder
    {
        public event EventHandler<FileMapBuildProgressEventArgs> FileMapBuildProgress; 

        public FileMap StartFromFolder(string folder, string[] extentionsFilter)
        {
            if(string.IsNullOrEmpty(folder))
                throw new ArgumentNullException("folder");
            if(extentionsFilter==null || extentionsFilter.Length==0)
                throw new ArgumentNullException("extentionsFilter");
            if(!Directory.Exists(folder))
                throw new ArgumentException("folder");
            return new FileMap
                       {
                           FileData = new FileData {IsFolder = true, Name = folder},
                           SubFiles = GetSubFiles(folder, extentionsFilter)
                       };
        }

        private List<FileMap> GetSubFiles(string folder, IEnumerable<string> extentionsFilter)
        {
            List<FileMap> mFileMaps= new List<FileMap>();
            List<string> files = new List<string>();
            foreach(string extentionFilter in extentionsFilter)
                files.AddRange(Directory.EnumerateFiles(folder,"*" + extentionFilter));
            mFileMaps.AddRange(files.Select(
                f =>
                new FileMap
                    {
                        FileData = new FileData {IsFolder = false, Name = f, State = State.NotTouched},
                        SubFiles = null
                    }).ToList());
            if(FileMapBuildProgress!=null)
                FileMapBuildProgress(this,new FileMapBuildProgressEventArgs(files,folder));
            var subfolders = Directory.EnumerateDirectories(folder);
            mFileMaps.AddRange(
                subfolders.Select(
                    d => new FileMap {FileData = new FileData {IsFolder = true, Name = d}, SubFiles = GetSubFiles(d,extentionsFilter)}));
            return mFileMaps;
        }
    }
}
