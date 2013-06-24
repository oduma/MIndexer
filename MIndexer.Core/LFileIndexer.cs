using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using MIndexer.Core.Interfaces;

namespace MIndexer.Core
{
    public class LFileIndexer:IFileIndexer
    {
        public Document PrepareDocument(string filePath, string targetFilePath=null)
        {
            throw new NotImplementedException();
        }
    }
}
