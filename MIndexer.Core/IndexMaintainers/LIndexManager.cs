using MIndexer.Core.DataTypes;
using MIndexer.Core.Interfaces;

namespace MIndexer.Core.IndexMaintainers
{
    public class LIndexManager : BaseIndexManager
    {
        public LIndexManager(IContentProvider contentProvider) : base(contentProvider)
        {
        }

        protected override void RenameOperationProcess(string oldPath, string newPath)
        {
            //oldPath.RemoveFileFromIndex();
            //newPath.IndexFile();
        }

        protected override void OperationProcess(string path, OperationType operationType)
        {
            switch (operationType)
            {
                case OperationType.Insert:
                    path.IndexFile(this);
                    break;
                case OperationType.Update:
                    path.UpdateIndexForLFile();
                    break;
                case OperationType.Delete:
//                    path.RemoveFileFromIndex();
                    break;
            }
        }

        protected override string[] AcceptableExtentions
        {
            get { return new string[] {".lyrics"}; }
        }
    }
}
