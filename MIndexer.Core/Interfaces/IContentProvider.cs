using MIndexer.Core.DataTypes;

namespace MIndexer.Core.Interfaces
{
    public interface IContentProvider
    {

        ContentAndTarget GetContentAndTarget(string filePath);

    }
}