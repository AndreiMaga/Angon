using CommandLine;

namespace Angon.common.cmd
{
    /// <summary>
    /// Class that has all the command line arguments
    /// </summary>
    class Options
    {
        [Option('m', "mastermode", Default = false, Required = false, HelpText = "Force it to run in master mode even if the config says otherwise.")]
        public bool MasterMode { get; set; }

        [Option('s', "slavemode", Default = false, Required = false, HelpText = "Force it to run in slave mode even if the config says otherwise.")]
        public bool SlaveMode { get; set; }

        [Option('e', "exefolder", Default = "", Required = false, HelpText = "The path to the folder containing the executable.")]
        public string PathToExeFolder { get; set; }

        [Option('i', "inputfolder", Default = "", Required = false, HelpText = "The path to the folder containing the executable.")]
        public string PathToInputFolder { get; set; }

        [Option('k', "sha", Default = "", Required = false, HelpText = "The sha to request from the master.")]
        public string Sha { get; set; }
    }
}
