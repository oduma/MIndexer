using CommandLine;
using CommandLine.Text;

namespace MIndexer.Builder
{
    public class Options
    {
        [Option('c', "create", DefaultValue = "map.xml", HelpText = "Creates a new map file", MutuallyExclusiveSet="action")]
        public string CreateNewMap { get; set; }

        [Option('u', "use", DefaultValue = "", HelpText = "Use a map file", MutuallyExclusiveSet = "action")]
        public string UseMap { get; set; }

        [Option('i', "inFolder", DefaultValue = "", HelpText = "Generates a xml file based on data from the folder structure.", Required=true)]
        public string StartFromFolder { get; set; }

        [HelpOption('?', null, HelpText = "Display this help on the screen.")]
        public string GetUssage()
        {
            HelpText help = new HelpText(Program.headingInfo);
            help.AddOptions(this);
            return help.ToString();
        }
    }
}
