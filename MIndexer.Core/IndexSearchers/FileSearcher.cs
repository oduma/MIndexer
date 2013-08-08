using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace MIndexer.Core.IndexSearchers
{
    public class FileSearcher:IFileSearcher
    {
        protected readonly static SimpleFSLockFactory _lockFactory = new SimpleFSLockFactory();

        public IEnumerable<string> Search(string query, int maxResults, string[] searchFields)
        {
            var indexFileLocation = new DirectoryInfo(Utils.GetFolderFromConfiguration("IndexDir"));
            Directory dir = FSDirectory.Open(indexFileLocation, _lockFactory);

            IndexSearcher searcher = new IndexSearcher(dir);

            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);

            MultiFieldQueryParser queryParser = new MultiFieldQueryParser(Version.LUCENE_30,
                                        searchFields,
                                        analyzer);
            //parse the query string into a Query object
            Query luceneQuery = queryParser.Parse(query);

            //execute the query
            TopDocs topDocs = searcher.Search(luceneQuery, maxResults);

            if (topDocs == null)
                yield return null;

            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                Document doc = searcher.Doc(scoreDoc.Doc);
                yield return doc.GetField("targetfilename").StringValue;
            }
        }

        public Query[] PrepareQuery(string query, int maxResults, string[] searchFields)
        {
            throw new NotImplementedException();
        }
    }
}
