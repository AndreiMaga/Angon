using Angon.common.cmd;
using Angon.common.config;
using Angon.common.storage;
using Angon.master.server;
using Angon.client;
using CommandLine;
using System;
using System.Collections.Generic;

namespace Angon
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(Run).WithNotParsed(HandleErrors);
        }

        static void Run (Options options)
        {
            // Force load Storage
            StorageProvider.GetInstance();

            if (options.MasterMode)
            {
                RunMaster(options);
            }
            if(options.SlaveMode)
            {
                RunSlave(options);
            }

            if(options.PathToExeFolder != "" && options.PathToInputFolder != "")
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

        static void HandleErrors(IEnumerable<Error> errs)
        {
            foreach(Error e in errs)
            {
                // log e
                Console.WriteLine(e);
            }
        }


        static void RunMaster(Options options)
        {
            new Server();
        }

        static void RunSlave(Options options)
        {

        }

        static void RunClient(Options options)
        {
            Requester.RunFromFolders(options.PathToExeFolder, options.PathToInputFolder);
        }
    }
}
