using ID3Sharp;

namespace MIndexer.Core.Interfaces
{
    public interface ITagReaderHelper
    {
        ID3Tag GetID3Tag(string filePath);
    }
}