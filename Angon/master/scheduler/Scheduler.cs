using Angon.common.config;
using Angon.common.headers;
using Angon.common.reciever;
using Angon.common.sender;
using Angon.common.storage;
using Angon.common.storage.data;
using Angon.common.utils;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Angon.master.scheduler
{
    class Scheduler
    {
        public Scheduler()
        {
            Task t = new Task(() =>
            {
                Run();
            });

            t.Start();
        }

        public void Run()
        {
            while (true)
            {
                while (StorageProvider.GetInstance().NumberOfJobsToBeDone() > 0)
                {
                    RunOrder();
                }

                // No more orders to run
                // sleep for time specified in config and try again
                Task.Delay(ConfigReader.GetInstance().Config.MilisecondsToSleep);
            }
        }


        public void RunOrder()
        {
            Order orderToRun = StorageProvider.GetInstance().GetOldestNotFinishedOrder();
            
            // the order was not split yet
            if(orderToRun.Splitted == false)
            {
                // TODO splitter
            }

            SlaveManager(orderToRun);
            
        }

        public void SlaveManager(Order orderToRun)
        {
            List<Slave> listofSlaves = CheckWhichSlavesAreAvailable();
            // TODO : Send jobs to the ones that are available for work
            // The response will come to the server as a "Result" request
            // TODO : Wait for all the jobs to finish, check if a job is hanged or still running on the client
            // and take action
            // TODO : while waiting, update the slaves list
            // if all jobs are completed mark the order as done on the database and concat the results
            // might be able to have people create their own merger tool that get's uploaded with the exe
        }

        public List<Slave> CheckWhichSlavesAreAvailable()
        {
            StorageProvider.GetInstance().GetSlaves().ForEach(CheckerStart); // this will change the list

            return StorageProvider.GetInstance().GetSlaves().FindAll(s => s.AvailableForWork);

        }

        public void CheckerStart(Slave s)
        {
            try
            {
                TcpClient tcp = new TcpClient(s.Ip, s.Port);

                ServerAvailableHeader sah = new ServerAvailableHeader();
                WraperHeader wh = new WraperHeader()
                {
                    Type = 'K',
                    Data = ByteArrayUtils.ToByteArray(sah)
                };
                Sender.Send(wh, tcp);

                // continue in reciever

                new Reciever().ProcessClient(tcp);

                // this will end when the recieving process is done

            }
            catch { }
        }

    }
}
