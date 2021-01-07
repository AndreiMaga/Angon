using Angon.client;
using Angon.common.cmd;
using Angon.common.config;
using Angon.common.server;
using Angon.common.storage;
using Angon.master.scheduler;
using Angon.slave;
using CommandLine;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .WriteTo.Console().MinimumLevel.Debug()
#endif
                .WriteTo.File("log.txt").CreateLogger();
            Log.Information("Starting with arguments " + string.Join(", ", args));
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
                RunMaster();
            }
            if (options.SlaveMode)
            {
                RunSlave();
            }

            if ((options.PathToExeFolder != "" && options.PathToInputFolder != "") || options.Sha != "")
            {
                RunClient(options);
            }


            // Normal mode, without any arguments
            if (ConfigReader.GetInstance().Config.Type == 0) // Master
            {
                // start new server
                RunMaster();
            }

            else if (ConfigReader.GetInstance().Config.Type == 1)
            {
                RunSlave();
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
                Log.Error(e.ToString());
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Start up the master program.
        /// </summary>
        static void RunMaster()
        {
            Log.Information("Starting as master");

            Task t = new Task(() =>
            {
                new Scheduler().Run();
            });
            t.Start();

            new Server();
            Environment.Exit(0);
        }

        /// <summary>
        /// Start up the slave program.
        /// </summary>
        static void RunSlave()
        {
            Log.Information("Starting as slave");

            StorageProvider.GetInstance().ClearOrders();

            Register.RegisterToMaster();

            Task t = new Task(() =>
            {
                new Scheduler().Run();
            });
            t.Start();

            new Server();

            Environment.Exit(0);
        }

        /// <summary>
        /// Start up the client program
        /// </summary>
        /// <param name="options"><see cref="Options"/> parsed arguments from the command line</param>
        static void RunClient(Options options)
        {
            Log.Information("Starting as client");
            ConfigReader.GetInstance().Config.Type = 2; // force the type to 2
            if (options.Sha == "")
            {
                Requester.RunFromFolders(options.PathToExeFolder, options.PathToInputFolder);
            }
            else
            {
                Requester.RequestResultFromMaster(options.Sha);
            }
            Environment.Exit(0);
        }
    }
}
