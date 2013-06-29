using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using MIndexer.Core.Interfaces;

namespace MIndexer.Core
{
    public class LFileIndexer:IFileIndexerSearcher
    {
        public Document PrepareDocument(string filePath, string targetFilePath=null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Search(string query, int maxResults)
        {
            throw new NotImplementedException();
        }
    }
}
