using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using MIndexer.Core;
using MIndexer.Core.DataTypes;
using MIndexer.Core.IndexMaintainers;
using MIndexer.Core.Interfaces;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;

namespace MIndexer.Builder
{
    public class Program
    {
        internal static readonly HeadingInfo headingInfo =
            new HeadingInfo(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

        static void Main(string[] args)
        {
            var options = new Options();

            Parser parser = new Parser(new ParserSettings{MutuallyExclusive=true});
            if (parser.ParseArguments(args, options))
            {
                if (!string.IsNullOrEmpty(options.UseMap))
                    CreateNewMap(options.StartFromFolder, options.CreateNewMap);
                else
                    ProcessMap(options.StartFromFolder, options.LyricsRootFolder, options.UseMap);
            }
            else
            {
                Console.WriteLine(options.GetUssage());
            }


        }

        public static void ProcessMap(string startFromFolder, string outputFile, string lRootFolder)
        {
            BulkFileProcessor bulkFileProcessor= new BulkFileProcessor();
            IContentProvider tagReaderHelper= new MContentProvider();
            FileMap fileMap= Serializer.DeserializeOneFromFile<FileMap>(outputFile);
            DownloadManager downloadManager = new DownloadManager(startFromFolder, lRootFolder,
                                                                   new string[] {".lyrics"}, tagReaderHelper, fileMap);
            IContentProvider contentProvider=new LContentProviderLocal();
            IIndexManager indexManager= new LIndexManager(contentProvider);
            DownloadAndIndexFromMapRequest downloadAndIndexFromMapRequest = new DownloadAndIndexFromMapRequest
                                                                                {
                                                                                    DownloadManager = downloadManager,
                                                                                    FileMap = fileMap,
                                                                                    IndexMaintainer = indexManager
                                                                                };
            Task lyricsTask= new Task(bulkFileProcessor.ProcessLFilesFromMapThreaded,downloadAndIndexFromMapRequest );
            IIndexManager mIndexManager= new MIndexManager(tagReaderHelper);
            IndexFromMapRequest indexFromMapRequest = new IndexFromMapRequest
                                                          {FileMap = fileMap, IndexMaintainer = mIndexManager};
            Task musicTask = new Task(bulkFileProcessor.ProcessMFilesFromMapThreaded, indexFromMapRequest);
            lyricsTask.Start();
            musicTask.Start();
            musicTask.Wait();
            lyricsTask.Wait();
        }

        public static FileMap CreateNewMap(string rootFolder, string mapFileName)
        {
            FileMapBuilder fileMapBuilder=new FileMapBuilder();
            fileMapBuilder.FileMapBuildProgress += new EventHandler<FileMapBuildProgressEventArgs>(fileMapBuilder_FileMapBuildProgress);
            FileMap map;
            try
            {
                map = fileMapBuilder.StartFromFolder(rootFolder, IOHelper.GetMExtentions());
                Serializer.SerializeOneToFile(map, mapFileName);
                return map;
            }
            catch (ArgumentNullException anex)
            {
                LoggingManager.LogSciendoSystemError(anex);
                Console.WriteLine("Almost impossible but still {0} is null", anex.ParamName);
                return new FileMap();
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                Console.WriteLine(ex.Message);
                return new FileMap();
            }
        }

        static void fileMapBuilder_FileMapBuildProgress(object sender, FileMapBuildProgressEventArgs e)
        {
            Console.WriteLine("In Folder {0} found {1} files.",e.Folder,e.Files.Count);
        }
    }
}
