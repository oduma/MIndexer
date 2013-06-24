using Lucene.Net.Documents;

namespace MIndexer.Core.Interfaces
{
    public interface IFileIndexer
    {
        Document PrepareDocument(string filePath, string targetFilePath=null);
    }
}
