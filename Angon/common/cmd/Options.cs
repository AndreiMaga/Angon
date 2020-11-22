using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angon.common.cmd
{
    class Options
    {
        [Option('m', "mastermode", Default = false, Required =false, HelpText = "Force it to run in master mode even if the config says otherwise.")]
        public bool MasterMode { get; set; }

        [Option('s', "slavemode", Default = false, Required = false, HelpText = "Force it to run in slave mode even if the config says otherwise.")]
        public bool SlaveMode { get; set; }

        [Option('e', "exefolder" , Default = "", Required = false, HelpText = "The path to the folder containing the executable.")]
        public string PathToExeFolder { get; set; }

        [Option('i', "inputfolder", Default = "", Required = false, HelpText = "The path to the folder containing the executable.")]
        public string PathToInputFolder { get; set; }
    }
}
