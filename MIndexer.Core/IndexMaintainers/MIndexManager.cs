using MIndexer.Core.DataTypes;
using MIndexer.Core.Interfaces;

namespace MIndexer.Core.IndexMaintainers
{
    public class MIndexManager : BaseIndexManager
    {
        public MIndexManager(IContentProvider contentProvider) : base(contentProvider)
        {
        }

        protected override void RenameOperationProcess(string oldPath, string newPath)
        {
            //oldPath.RemoveFileFromIndex().DeleteLyricsFile();
            //newPath.IndexMFile().RetrieveLyricsFile(TODO);
        }

        protected override void OperationProcess(string path, OperationType operationType)
        {
            //switch (operationType)
            //{
            //    case OperationType.Insert:
            //        path.IndexMFile().RetrieveLyricsFile(TODO);
            //        break;
            //    case OperationType.Update:
            //        path.UpdateIndexForMFile().RetrieveLyricsFile(TODO);
            //        break;
            //    case OperationType.Delete:
            //        path.RemoveFileFromIndex().DeleteLyricsFile();
            //        break;
            //}
        }

        protected override string[] AcceptableExtentions
        {
            get { return IOHelper.GetMExtentions(); }
        }
    }
}
