using MIndexer.Core.Interfaces;

namespace MIndexer.Core.DataTypes
{
    public class DownloadAndIndexFromMapRequest:IndexFromMapRequest
    {
        public IDownloadManager DownloadManager { get; set; }
    }
}
