using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIndexer.Core.DataTypes
{
    public class FileMap
    {
        
        public FileData FileData { get; set; }

        public List<FileMap> SubFiles { get; set; }

        public List<FileData> GetAllFolders(bool recursive)
        {
            List<FileData> currentFolders = new List<FileData>();
            if(this.FileData.IsFolder)
                currentFolders.Add(this.FileData);
            if(!recursive)
                return currentFolders;
            //currentFolders.AddRange(this.SubFiles.Where(s=>s.FileData.IsFolder).Select(s=>s.FileData));
            foreach (FileMap fileMap in this.SubFiles.Where(s => s.FileData.IsFolder))
                currentFolders.AddRange(fileMap.GetAllFolders(true));
            return currentFolders;
        }

        public FileMap GetSubFileMap(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (this.FileData.Name == name)
                return this;
            if(this.SubFiles==null)
                return null;
            foreach (FileMap fileMap in this.SubFiles)
            {
                var result = fileMap.GetSubFileMap(name);
                if(result!=null)
                    return result;
            }
            return null;
        }
    }
}
