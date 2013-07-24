using System;
using System.Collections.Generic;
using System.IO;
using ID3Sharp;
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
    public class MFileIndexerSearcher:IFileIndexerSearcher
    {
        readonly static SimpleFSLockFactory _lockFactory = new SimpleFSLockFactory();

        public Document PrepareDocument(string filePath)
        {
            if(string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");
            if(!File.Exists(filePath))
                throw new ArgumentException("File does not exist:"+filePath);
            ID3Tag id3Tag = GetID3Tag(filePath);
            if (id3Tag == null)
                throw new ArgumentException("Cannot read tag from file: " + filePath);
            Document document= new Document();
            
            document.Add(new Field("tagged",StreamLineTag(id3Tag),Field.Store.YES,Field.Index.ANALYZED,Field.TermVector.YES));
            document.Add(new Field("filename",filePath,Field.Store.YES,Field.Index.ANALYZED,Field.TermVector.YES));

            return document;
        }

        public IEnumerable<string> Search(string query, int maxResults)
        {
            var indexFileLocation = new DirectoryInfo(Utils.GetFolderFromConfiguration("IndexDir"));
            LuceneDirectory dir = FSDirectory.Open(indexFileLocation,_lockFactory);

            IndexSearcher searcher = new IndexSearcher(dir);

            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
            
            MultiFieldQueryParser queryParser = new MultiFieldQueryParser(Version.LUCENE_30,
                                        new string[] { "tagged", "filename" },
                                        analyzer);
            //parse the query string into a Query object
            Query luceneQuery = queryParser.Parse(query);

            //execute the query
            TopDocs topDocs = searcher.Search(luceneQuery,maxResults);

            if (topDocs == null)
                yield return null;

            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                Document doc = searcher.Doc(scoreDoc.Doc);
                yield return doc.GetField("filename").StringValue;
            }
        }

        private string StreamLineTag(ID3Tag id3Tag)
        {
            return
                (string.Format("{0} {1} {2} {3}", (id3Tag.HasArtist) ? id3Tag.Artist : "",
                               (id3Tag.HasAlbum) ? id3Tag.Album : "", (id3Tag.HasTitle) ? id3Tag.Title : "",
                               (id3Tag.HasGenre) ? id3Tag.Genre : ""));
        }

        internal ID3Tag GetID3Tag(string filePath)
        {
            if (!ID3v2Tag.HasTag(filePath))
                return null;
            ID3v2Tag id3V2Tag = ID3v2Tag.ReadTag(filePath);
            if (!(id3V2Tag.HasAlbum || id3V2Tag.HasArtist || id3V2Tag.HasTitle || id3V2Tag.HasGenre))
                return null;

            return id3V2Tag;
        }
    }
}
