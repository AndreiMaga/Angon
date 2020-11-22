using Angon.client;
using Angon.common.cmd;
using Angon.common.config;
using Angon.common.storage;
using Angon.master.server;
using CommandLine;
using System;
using System.Collections.Generic;

namespace Angon
{
    /// <summary>
    /// Entry point to Angon.
    /// </summary>
    class Program
    {
        
        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(Run).WithNotParsed(HandleErrors);
        }

        /// <summary>
        /// This will run if the program is run correctly
        /// </summary>
        /// <param name="options"><see cref="Options"/> parsed arguments from the command line</param>
        static void Run(Options options)
        {
            // Force load Storage
            StorageProvider.GetInstance();

            if (options.MasterMode)
            {
                RunMaster(options);
            }
            if (options.SlaveMode)
            {
                RunSlave(options);
            }

            if (options.PathToExeFolder != "" && options.PathToInputFolder != "")
            {
                RunClient(options);
            }


            // Normal mode, without any arguments
            if (ConfigReader.GetInstance().Config.Type == 0) // Master
            {
                // start new server
                RunMaster(null);
            }

            else if (ConfigReader.GetInstance().Config.Type == 1)
            {
                // TODO client
            }
        }

        /// <summary>
        /// This will run if the program is not run correctly
        /// </summary>
        /// <param name="errs"> List of errors</param>
        static void HandleErrors(IEnumerable<Error> errs)
        {
            foreach (Error e in errs)
            {
                // log e
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Start up the master program.
        /// </summary>
        /// <param name="options"><see cref="Options"/> parsed arguments from the command line</param>
        static void RunMaster(Options options)
        {
            new Server();
        }

        /// <summary>
        /// Start up the slave program.
        /// </summary>
        /// <param name="options"><see cref="Options"/> parsed arguments from the command line</param>
        static void RunSlave(Options options)
        {

        }

        /// <summary>
        /// Start up the client program
        /// </summary>
        /// <param name="options"><see cref="Options"/> parsed arguments from the command line</param>
        static void RunClient(Options options)
        {
            Requester.RunFromFolders(options.PathToExeFolder, options.PathToInputFolder);
        }
    }
}
