using System;
using System.IO;
using System.Net;
using ID3Sharp;
using MIndexer.Core.DataTypes;
using Sciendo.Common.Logging;

namespace MIndexer.Core
{
    public class LContentProviderRemote:MContentProvider
    {

        private readonly string rootUrl = "http://www.songlyrics.com/";

        private string DownloadLyrics(string relativeUrl)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(relativeUrl))
                return result;
            try
            {
                HttpWebRequest httpWebRequest =
                    (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", rootUrl, relativeUrl));
                WebResponse myResponse = httpWebRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream());
                result = ScrapLyrics(sr.ReadToEnd());
                sr.Close();
                myResponse.Close();
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
            }
            return result;
        }
        
        private string ConvertFromASCII(string rawLyrics)
        {
            for (int i = 0; i < 256; i++)
                rawLyrics = rawLyrics.Replace("&#" + i + ";", Convert.ToString((char)i));
            return rawLyrics;
        }



        private string ScrapLyrics(string pageContent)
        {
            int startOfDiv = pageContent.IndexOf(@"id=""songLyricsDiv""");
            int startOfText = pageContent.IndexOf(@">", startOfDiv) + 1;
            int stopOfText = pageContent.IndexOf(@"</p>", startOfDiv);
            var rawLyrics = pageContent.Substring(startOfText, stopOfText - startOfText);
            rawLyrics = rawLyrics.Replace(@"<br />", " ");

            return ConvertFromASCII(rawLyrics).Replace("\r", "").Replace("\n", "");
        }

        public new ContentAndTarget GetContentAndTarget(string filePath)
        {
            ContentAndTarget content = new ContentAndTarget();
            var id3Tag = GetID3Tag(filePath);
            if (id3Tag == null)
                return null;
            content.Content =
                DownloadLyrics(string.Format("{0}/{1}-lyrics/", id3Tag.Artist.Replace(" ", "-").Replace("/", ""),
                                             id3Tag.Title.Replace(" ", "-")));
            if (string.IsNullOrEmpty(content.Content))
                return null;
            content.TargetFileName = filePath;
            return content;
        }
    }
}
