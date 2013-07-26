using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MIndexer.Core.Interfaces;
using LuceneDirectory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;


namespace MIndexer.Core
{
    public abstract class BaseFileIndexerSearcher:IFileIndexerSearcher
    {
        protected readonly static SimpleFSLockFactory _lockFactory = new SimpleFSLockFactory();


        public virtual Document PrepareDocument(string filePath)
        {
            return null;
        }


        protected Document PrepareDocument(string filePath, string content,string targetFileName)
        {
            Document document = new Document();

            document.Add(new Field("tagged", content, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));
            document.Add(new Field("filename", filePath.ReplaceAny(new string[] {"\\"," ","."}, "").Substring(filePath.IndexOf(Path.VolumeSeparatorChar) + 1), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES))
            ;
            document.Add(new Field("targetfilename", targetFileName, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));

            return document;
        }



        public IEnumerable<string> Search(string query, int maxResults, string[] searchFields)
        {
            var indexFileLocation = new DirectoryInfo(Utils.GetFolderFromConfiguration("IndexDir"));
            LuceneDirectory dir = FSDirectory.Open(indexFileLocation, _lockFactory);

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
    }
}
