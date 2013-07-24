using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Documents;
using MIndexer.Core.DataTypes;
using MIndexer.Core.Interfaces;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;

namespace MIndexer.Core
{
    public class LFileIndexerSearcher:IFileIndexerSearcher
    {
        public Document PrepareDocument(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");
            if (!File.Exists(filePath))
                throw new ArgumentException("File does not exist:" + filePath);

            Lyrics lyrics= new Lyrics();
            try
            {
                lyrics = Serializer.DeserializeOneFromFile<Lyrics>(filePath);
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
            }
            if (lyrics == null || string.IsNullOrEmpty(lyrics.Content))
                throw new ArgumentException("Cannot read lyrics from file: " + filePath);
            if(string.IsNullOrEmpty(lyrics.TargetFileName))
                throw new ArgumentException("Cannot read target file name from file: " + filePath);
            if(!File.Exists(lyrics.TargetFileName))
                throw new ArgumentException("Target file does not exist:" + lyrics.TargetFileName);

            Document document = new Document();

            document.Add(new Field("tagged", lyrics.Content.Replace("\r\n"," ").Replace("\r"," ").Replace("\n"," "), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));
            document.Add(new Field("filename", lyrics.TargetFileName, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));

            return document;
        }

        public IEnumerable<string> Search(string query, int maxResults)
        {
            throw new NotImplementedException();
        }
    }
}
