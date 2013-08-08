using System.Collections.Generic;
using Lucene.Net.Search;

namespace MIndexer.Core.IndexSearchers
{
    public interface IFileSearcher
    {
        IEnumerable<string> Search(string query, int maxResults,string[] searchFields);

        Query[] PrepareQuery(string query, int maxResults, string[] searchFields);

    }
}
