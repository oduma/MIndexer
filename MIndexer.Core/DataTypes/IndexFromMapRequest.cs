using MIndexer.Core.IndexMaintainers;

namespace MIndexer.Core.DataTypes
{
    public class IndexFromMapRequest
    {
        public FileMap FileMap { get; set; }

        public IIndexManager IndexMaintainer { get; set; }
    }
}
