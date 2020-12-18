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
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace Angon.master.scheduler
{
    class Scheduler
    {
        public static JobsConfig JobsConfig { get; private set; }

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


            SlaveManager(orderToRun, orderConfig);

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
            Jobs = new List<string>(Directory.GetDirectories(Path.Combine(GetOrderPath(order), "unzip", "input", "jobs"))),
            FinishedJobs = new List<string>()
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

        public void SlaveManager(Order order, OrderConfig orderConfig)
        {
            // expose JobsConfig so the runner for result can finish jobs
            JobsConfig = GetJobsConfig(order);

            List<Slave> listOfSlaves = CheckWhichSlavesAreAvailable();
            while(JobsConfig.FinishedJobs.Count != JobsConfig.numberOfJobs)
            {
                // if jobs are pending to be sent
                if(JobsConfig.Jobs.Count != 0)
                {
                    foreach (Slave slave in listOfSlaves)
                    {
                        SendJob(slave);
                        // if all jobs were sent
                        if (JobsConfig.Jobs.Count == 0)
                            break;
                    }
                }
                // The response will come to the server as a "Result" request

                List<Slave> updatedlistOfSlaves = CheckWhichSlavesAreAvailable();
                foreach(Slave slave in listOfSlaves)
                {
                    // if the slave had a job and it's not on the new list
                    if (slave.HasJob && !updatedlistOfSlaves.Any(ns => ns.UniqueToken.Equals(slave.UniqueToken)))
                    {
                        JobsConfig.SentJobs.Remove(slave.AssignedJob);
                        JobsConfig.Jobs.Add(slave.AssignedJob); // to be reassigned
                    }
                }
                listOfSlaves = updatedlistOfSlaves;
                Task.Delay(ConfigReader.GetInstance().Config.MilisecondsToSleep);
            }
            // if all jobs are completed mark the order as done on the database and concat the results
            // might be able to have people create their own merger tool that get's uploaded with the exe
        }

        private void SendJob(Slave slave)
        {
            // TODO : Send jobs to slaves
        }

        public List<Slave> CheckWhichSlavesAreAvailable()
        {
            StorageProvider.GetInstance().GetSlaves().ForEach(CheckerStart); // this will change the list

            return StorageProvider.GetInstance().GetSlaves().FindAll(s => s.AvailableForWork != 2);

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
