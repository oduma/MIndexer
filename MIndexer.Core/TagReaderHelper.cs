using System.IO;
using System.Linq;
using ID3Sharp;
using MIndexer.Core.Interfaces;

namespace MIndexer.Core
{
    public class TagReaderHelper : ITagReaderHelper
    {
        public virtual ID3Tag GetID3Tag(string filePath)
        {
            if (!IOHelper.GetMExtentions().Any(e => e == Path.GetExtension(filePath)))
                return null;
            if (!ID3v2Tag.HasTag(filePath))
                return null;
            ID3v2Tag id3V2Tag = ID3v2Tag.ReadTag(filePath);
            if (!(id3V2Tag.HasAlbum || id3V2Tag.HasArtist || id3V2Tag.HasTitle || id3V2Tag.HasGenre))
                return null;

            return id3V2Tag;
        }


    }
}
