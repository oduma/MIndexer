using System;
using System.IO;
using System.Net;
using Sciendo.Common.Logging;

namespace MIndexer.Core
{
    public class DownloaderHelper
    {
        private readonly string rootUrl = "http://www.songlyrics.com/";

        public virtual string DownloadLyrics(string relativeUrl)
        {
            string result= string.Empty;
            if(string.IsNullOrEmpty(relativeUrl))
                throw new ArgumentNullException("relativeUrl");
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

        public string ScrapLyrics(string pageContent)
        {
            int startOfDiv= pageContent.IndexOf(@"id=""songLyricsDiv""");
            int startOfText = pageContent.IndexOf(@">", startOfDiv)+1;
            int stopOfText = pageContent.IndexOf(@"</p>", startOfDiv);
            var rawLyrics = pageContent.Substring(startOfText, stopOfText - startOfText);
            rawLyrics = rawLyrics.Replace(@"<br />", " ");

            return ConvertFromASCII(rawLyrics).Replace("\r","").Replace("\n","");
        }

        private string ConvertFromASCII(string rawLyrics)
        {
            for (int i = 0; i < 256; i++)
                rawLyrics = rawLyrics.Replace("&#" + i + ";", Convert.ToString((char) i));
            return rawLyrics;
        }
    }
}
