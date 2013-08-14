using System;
using System.IO;
using System.Linq;
using ID3Sharp;
using MIndexer.Core.DataTypes;
using MIndexer.Core.Interfaces;

namespace MIndexer.Core
{
    public class MContentProvider : IContentProvider
    {
        protected ID3Tag GetID3Tag(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;
            if (!File.Exists(filePath))
                return null;
            if (!IOHelper.GetMExtentions().Any(e => e == Path.GetExtension(filePath)))
                return null;
            if (!ID3v2Tag.HasTag(filePath))
                return null;
            ID3v2Tag id3V2Tag = ID3v2Tag.ReadTag(filePath);
            if (!(id3V2Tag.HasAlbum || id3V2Tag.HasArtist || id3V2Tag.HasTitle || id3V2Tag.HasGenre))
                return null;

            return id3V2Tag;
        }



        private string GetStreamLineTag(ID3Tag id3Tag)
        {
            return
                (string.Format("{0} {1} {2} {3}", (id3Tag.HasArtist) ? id3Tag.Artist : "",
                               (id3Tag.HasAlbum) ? id3Tag.Album : "", (id3Tag.HasTitle) ? id3Tag.Title : "",
                               (id3Tag.HasGenre) ? id3Tag.Genre : ""));
        }

        public ContentAndTarget GetContentAndTarget(string filePath)
        {
            return new ContentAndTarget {Content = GetStreamLineTag(GetID3Tag(filePath)), TargetFileName = filePath};
        }

    }
}
