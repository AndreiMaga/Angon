
using System.Collections.Generic;

namespace Angon.master.scheduler
{
    class OrderConfig
    {
#pragma warning disable 0649
        /// <summary>
        /// Should the Splitter even run or it's already splitted
        /// True -> Splitter will run
        /// False -> Will assume every job it's in it's own folder
        /// </summary>
        public bool ShouldBeSplit = true;

        /// <summary>
        /// Clients can send their own splitter.
        /// SECURITY RISK, disabled by default inside master's config
        /// </summary>
        public string NameOfExternalSplitter = "";

        public string ExternalSplitterArguments = "";

        /// <summary>
        /// Should the splitter delete the zip file after unzipping it
        /// </summary>
        public bool ShouldDeleteAfterUnzip = false;

        public float DefaultSplitterDeviation = 0.05f;

        public string NameOfExecutable = "Angon.exe";
        public string ArgumentsForExecutable = "";

        public int SuccessExitCode = 0;
#pragma warning restore 0649
    }
}
