using System.Collections.Generic;
using Lucene.Net.Documents;

namespace MIndexer.Core.Interfaces
{
    public interface IFileIndexerSearcher
    {
        Document PrepareDocument(string filePath);

        IEnumerable<string> Search(string query, int maxResults);
    }
}
