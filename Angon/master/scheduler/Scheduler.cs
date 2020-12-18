using Angon.common.config;
using Angon.common.headers;
using Angon.common.reciever;
using Angon.common.sender;
using Angon.common.storage;
using Angon.common.storage.data;
using Angon.common.utils;
using Angon.master.splitter;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text.Json;
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

            UnzipOrder(orderToRun);
            OrderConfig orderConfig = GetOrderConfig(orderToRun);

            if (orderConfig.ShouldDeleteAfterUnzip || ConfigReader.GetInstance().Config.DeleteAfterUnzip)
                File.Delete(Path.Combine(GetOrderPath(orderToRun),"temp.zip"));

            if (orderConfig.ShouldBeSplit == true && orderToRun.Splitted == false)
            {
                new Splitter(orderToRun, orderConfig).Split();

                if (Splitter.MaliciousReturn)
                {
                    // TODO
                    Splitter.MaliciousReturn = false;
                    return; // Untill this is done, all malicious runs will end up in an infinite loop
                }
            }

            // Create JSON with the jobs, will store them inside database after the order is finished

            JobsConfig jobsConfig = GetJobsConfig(orderToRun);

            SlaveManager(orderToRun, orderConfig, jobsConfig);

        }

        private JobsConfig GetJobsConfig(Order order)
        {
            string path = Path.Combine(GetOrderPath(order), "unzip", "jobconfig.json");
            if (File.Exists(path))
            {
                return JsonSerializer.Deserialize<JobsConfig>(File.ReadAllText(path));
            }
            JobsConfig jobsConfig = CreateJobsConfig(order);
            File.WriteAllText(path, JsonSerializer.Serialize(jobsConfig));
            return jobsConfig;
        }

        private JobsConfig CreateJobsConfig(Order order) => new JobsConfig
        {
            Jobs = new List<string>(Directory.GetDirectories(Path.Combine(GetOrderPath(order), "unzip", "input", "jobs")))
        };

        private string GetOrderPath(Order order)
        {
            return Path.Combine(ConfigReader.GetInstance().Config.SavePath, order.Sha);
        }

        private OrderConfig GetOrderConfig(Order orderToRun)
        {
            return JsonSerializer.Deserialize<OrderConfig>(File.ReadAllText(Path.Combine(GetOrderPath(orderToRun) + "unzip","exe","orderconfig.json")));
        }
        
        private void UnzipOrder(Order order)
        {
            try
            {
                string path = GetOrderPath(order);
                ZipFile.ExtractToDirectory(Path.Combine(path, "temp.zip"), Path.Combine(path + "unzip"));

            }
            catch (IOException)
            {
                // already unzipped
            }
        }

        public void SlaveManager(Order order, OrderConfig orderConfig, JobsConfig jobsConfig)
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
