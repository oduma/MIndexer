using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using MIndexer.Core;
using MIndexer.Core.DataTypes;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;

namespace MIndexer.Builder
{
    class Program
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
                    ProcessMap(options.StartFromFolder, options.UseMap);
            }
            else
            {
                Console.WriteLine(options.GetUssage());
            }


        }

        private static void ProcessMap(string startFromFolder, string outputFile)
        {
            BulkFileProcessor bulkFileProcessor= new BulkFileProcessor();
            DownloadAndIndexFromMapRequest downloadAndIndexFromMapRequest= new DownloadAndIndexFromMapRequest();
            Task lyricsTask= new Task(bulkFileProcessor.ProcessLFilesFromMapThreaded,downloadAndIndexFromMapRequest );
            IndexFromMapRequest indexFromMapRequest= new IndexFromMapRequest();
            Task musicTask = new Task(bulkFileProcessor.ProcessMFilesFromMapThreaded, indexFromMapRequest);
        }

        private static FileMap CreateNewMap(string rootFolder, string mapFileName)
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
