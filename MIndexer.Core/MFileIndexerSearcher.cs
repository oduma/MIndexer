using System;
using System.IO;
using ID3Sharp;
using Lucene.Net.Documents;

namespace MIndexer.Core
{
    public class MFileIndexerSearcher:BaseFileIndexerSearcher
    {
        public override Document PrepareDocument(string filePath)
        {
            if(string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");
            if(!File.Exists(filePath))
                throw new ArgumentException("File does not exist:"+filePath);
            ID3Tag id3Tag = GetID3Tag(filePath);
            if (id3Tag == null)
                throw new ArgumentException("Cannot read tag from file: " + filePath);
            string content = StreamLineTag(id3Tag);
            return PrepareDocument(filePath,content,filePath);
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
