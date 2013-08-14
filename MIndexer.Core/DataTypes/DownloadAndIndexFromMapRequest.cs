using MIndexer.Core.Interfaces;

namespace MIndexer.Core.DataTypes
{
    public class DownloadAndIndexFromMapRequest:IndexFromMapRequest
    {
        public DownloadManager DownloadManager { get; set; }
    }
}
