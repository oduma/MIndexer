using System;
using System.IO;
using ID3Sharp;
using Lucene.Net.Documents;
using MIndexer.Core.DataTypes;
using MIndexer.Core.IndexSearchers;
using MIndexer.Core.Interfaces;

namespace MIndexer.Core.IndexMaintainers
{
    public class MIndexManager : BaseIndexManager
    {
        private readonly ITagReaderHelper _tagReaderHelper;

        public MIndexManager(ITagReaderHelper tagReaderHelper)
        {
            _tagReaderHelper = tagReaderHelper;
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


        internal override Document PrepareDocument(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");
            if (!File.Exists(filePath))
                throw new ArgumentException("File does not exist:" + filePath);
            ID3Tag id3Tag = _tagReaderHelper.GetID3Tag(filePath);
            if (id3Tag == null)
                throw new ArgumentException("Cannot read tag from file: " + filePath);
            string content = StreamLineTag(id3Tag);
            return PrepareDocument(filePath, content, filePath);
        }

        protected override string[] AcceptableExtentions
        {
            get { return IOHelper.GetMExtentions(); }
        }

        private string StreamLineTag(ID3Tag id3Tag)
        {
            return
                (string.Format("{0} {1} {2} {3}", (id3Tag.HasArtist) ? id3Tag.Artist : "",
                               (id3Tag.HasAlbum) ? id3Tag.Album : "", (id3Tag.HasTitle) ? id3Tag.Title : "",
                               (id3Tag.HasGenre) ? id3Tag.Genre : ""));
        }
    }
}
