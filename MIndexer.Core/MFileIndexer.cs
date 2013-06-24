using System;
using System.IO;
using ID3Sharp;
using Lucene.Net.Documents;
using MIndexer.Core.Interfaces;

namespace MIndexer.Core
{
    public class MFileIndexer:IFileIndexer
    {
        public Document PrepareDocument(string filePath, string targetFilePath=null)
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

        private string StreamLineTag(ID3Tag id3Tag)
        {
            return
                (string.Format("{0} {1} {2} {3}", (id3Tag.HasArtist) ? id3Tag.Artist : "",
                               (id3Tag.HasAlbum) ? id3Tag.Album : "", (id3Tag.HasTitle) ? id3Tag.Title : "",
                               (id3Tag.HasGenre) ? id3Tag.Genre : ""));
        }

        private ID3Tag GetID3Tag(string filePath)
        {
            ID3v2Tag id3V2Tag= new ID3v2Tag();
            if (!ID3v2Tag.HasTag(filePath))
                return null;
            id3V2Tag = ID3v2Tag.ReadTag(filePath);
            if (!(id3V2Tag.HasAlbum || id3V2Tag.HasArtist || id3V2Tag.HasTitle || id3V2Tag.HasGenre))
                return null;

            return id3V2Tag;
        }
    }
}
