using System;
using System.IO;
using System.Linq;
using ID3Sharp;
using MIndexer.Core.DataTypes;
using MIndexer.Core.Interfaces;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;

namespace MIndexer.Core
{
    public static class IOHelper
    {

        public static string[] GetMExtentions()
        {
            string[] mExtentions = Utils.GetExtentionsFromConfiguration("MExtensions").Split(new char[] { ',' });
            if (!mExtentions.Any(e => !string.IsNullOrEmpty(e)))
                mExtentions = new string[] { ".mp3", ".ogg", ".flac" };
            else
                mExtentions = mExtentions.Select(e => (e.StartsWith(".")) ? e : "." + e).ToArray();
            return mExtentions;
        }

        public static string GetLyricsFolder()
        {
            string lyricsFolder = Utils.GetExtentionsFromConfiguration("LyricsFolder");
            if (string.IsNullOrEmpty(lyricsFolder))
                lyricsFolder = "Lyrics";
            return lyricsFolder;
        }

        public static string DeleteLyricsFileForAnMFile(string filePath)
        {
            string lFilePath = Path.Combine(GetLyricsFolder(),
                                            Path.GetFileNameWithoutExtension(filePath) + ".lyrics");
            File.Delete(lFilePath);
            return lFilePath;
        }
    }
}
