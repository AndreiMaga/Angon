using Angon.common.config;
using Angon.common.headers;
using Angon.common.reciever;
using Angon.common.sender;
using Angon.common.storage;
using Angon.common.storage.data;
using Angon.common.utils;
using Angon.master.splitter;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Order Order { get; set; }
        public OrderConfig OrderConfig { get; set; }

        private string GetOrderPath { get => Path.Combine(ConfigReader.GetInstance().Config.SavePath, Order.Sha); }

        public Scheduler() { }

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
            Order = StorageProvider.GetInstance().GetOldestNotFinishedOrder();
            Log.Information("Running order {0}", Order.Sha);

            UnzipOrder();
            GetOrderConfig();

#if DEBUG
            Log.Debug("Checking if the order zip should be deleted.");
#endif
            if (OrderConfig.ShouldDeleteAfterUnzip || ConfigReader.GetInstance().Config.DeleteAfterUnzip)
                File.Delete(Path.Combine(GetOrderPath, "temp.zip"));

            // check if it's running as Slave
            if( ConfigReader.GetInstance().Config.Type == 1)
            {
                // The scheduler it's inside a slave
                ContinueSlave();
                return; // Slave does not need to split the input folder
            }

            if (OrderConfig.ShouldBeSplit && !Order.Splitted)
            {
#if DEBUG
                Log.Debug("Splitting the order.");
#endif
                new Splitter(Order, OrderConfig).Split();

                if (Splitter.MaliciousReturn)
                {
#if DEBUG
                    Log.Debug("As Geralt would put it:");
                    Log.Error("...Fuck!...");
#endif
                    // TODO
                    Splitter.MaliciousReturn = false;
                    return; // Untill this is done, all malicious runs will end up in an infinite loop
                }
            }

            SlaveManager();
        }

        private void ContinueSlave()
        {
            // Check if the program exists in the exe folder
            string pathToExecutable = Path.Combine(GetOrderPath, "unzip", "exe", OrderConfig.NameOfExecutable);
            FileInfo fi = new FileInfo(pathToExecutable);
            if (fi.FullName.Length != pathToExecutable.Length) // if the exe is reffered to with .. or in another folder
            {
                // don't run anything
                return;
            }

            if (!fi.Exists) // if the exe does not exist
            {
                // don't run anything
                return;
            }

            if(ConfigReader.GetInstance().Config.VerifySignature)
            {
                if (!SecurityUtils.FileIsSigned(pathToExecutable))
                    return;
            }

            // if all checks are fine
            // run exe with arguments passed in the order config
            Process p = Process.Start(pathToExecutable, OrderConfig.ArgumentsForExecutable);
            p.WaitForExit();

            // the process is finished send it back to the master
            if(p.ExitCode == OrderConfig.SuccessExitCode)
            {
                // Everything is ok
                // Zip result and send it to master
            }
        }

        private JobsConfig GetJobsConfig()
        {
            string path = Path.Combine(GetOrderPath, "unzip", "jobconfig.json");
            if (File.Exists(path))
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    return JsonSerializer.DeserializeAsync<JobsConfig>(fs).Result;
                }

            }
            return CreateJobsConfig();
        }

        private JobsConfig CreateJobsConfig()
        {
            var jobs = new List<string>(Directory.GetDirectories(Path.Combine(GetOrderPath, "unzip", "input", "jobs")));
            return new JobsConfig
            {
                Jobs = jobs,
                FinishedJobs = new List<string>(),
                numberOfJobs = jobs.Count,
                SentJobs = new List<string>()
            };
        }

        private void SaveStateOfJobsConfig()
        {
            string path = Path.Combine(GetOrderPath, "unzip", "jobconfig.json");
            using (FileStream fs = File.Open(path, FileMode.OpenOrCreate))
            {
                JsonSerializer.SerializeAsync(fs, JobsConfig);
            }
        }

        private void GetOrderConfig()
        {

            string path = Path.Combine(GetOrderPath, "unzip", "exe", "orderconfig.json");
            if (File.Exists(path))
            {
#if DEBUG
                Log.Debug("Order config exists.");
#endif
                try
                {
                    using (FileStream fs = File.OpenRead(path))
                    {
                        OrderConfig = JsonSerializer.DeserializeAsync<OrderConfig>(fs).Result;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    OrderConfig = new OrderConfig();
                }

            }
            else
            {
#if DEBUG
                Log.Debug("Order config exists.");
#endif
                OrderConfig = new OrderConfig();
            }
        }

        private void UnzipOrder()
        {
#if DEBUG
            Log.Debug("Unzipping order");
#endif

            try
            {
                ZipFile.ExtractToDirectory(Path.Combine(GetOrderPath, "temp.zip"), Path.Combine(GetOrderPath, "unzip"));

            }
            catch (IOException)
            {
                // already unzipped
            }
        }

        public void SlaveManager()
        {
            // expose JobsConfig so the runner for result can finish jobs
            JobsConfig = GetJobsConfig();
            SaveStateOfJobsConfig();
            ZipJobs();
            // clear JobsConfig.SentJobs as this might be a resume
            foreach (string s in JobsConfig.SentJobs)
            {
                JobsConfig.Jobs.Add(s);
            }
            JobsConfig.SentJobs = new List<string>();

            List<Slave> listOfSlaves = CheckWhichSlavesAreAvailable();
            while (JobsConfig.FinishedJobs.Count != JobsConfig.numberOfJobs)
            {
                // if jobs are pending to be sent
                if (JobsConfig.Jobs.Count != 0)
                {
                    foreach (Slave slave in listOfSlaves)
                    {
                        SendJob(slave);
                        // if all jobs were sent
                        if (JobsConfig.Jobs.Count == 0)
                        {
#if DEBUG
                            Log.Debug("All jobs were sent");
#endif
                            break;
                        }
                    }
                }
                // The response will come to the server as a "Result" request

                List<Slave> updatedlistOfSlaves = CheckWhichSlavesAreAvailable();
                foreach (Slave slave in listOfSlaves)
                {
                    // if the slave had a job and it's not on the new list
                    if (slave.HasJob && !updatedlistOfSlaves.Any(ns => ns.UniqueToken.Equals(slave.UniqueToken)))
                    {
                        JobsConfig.SentJobs.Remove(slave.AssignedJob);
                        JobsConfig.Jobs.Add(slave.AssignedJob); // to be reassigned
                        SaveStateOfJobsConfig();
                    }
                    // if the slave has a job, pass it
                    // will be replaced with database info
                    if (slave.HasJob)
                    {
                        updatedlistOfSlaves.Find(ns => ns.UniqueToken.Equals(slave.UniqueToken)).HasJob = true;
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

            string zipspath = Path.Combine(GetOrderPath, "jobs");
            string job = JobsConfig.Jobs[0]; // take the first job

#if DEBUG
            Log.Debug("Sending job {0} to {1}.", job, slave.UniqueToken);
#endif
            FileInfo fi = new FileInfo(Path.Combine(zipspath, job));

            WraperHeader wraperHeader = new WraperHeader
            {
                Type = HeaderTypes.JobHeader,
                Data = ByteArrayUtils.ToByteArray(new JobHeader
                {
                    JobID = job,
                    Size = fi.Length
                })
            };

            TcpClient serverConnection = new TcpClient(slave.Ip, slave.Port);
            Sender.Send(wraperHeader, serverConnection);
            Sender.SendZip(fi.FullName, serverConnection.GetStream());
            serverConnection.Close();

            slave.HasJob = true;
            slave.AssignedJob = job;

            // after it's sent
            JobsConfig.Jobs.Remove(job);
            SaveStateOfJobsConfig();
        }

        private void ZipJobs()
        {
#if DEBUG
            Log.Debug("Zipping jobs.");
#endif
            string inputbasepath = Path.Combine(GetOrderPath, "unzip", "input", "jobs");
            string resultpath = Path.Combine(GetOrderPath, "jobs");

            int inbaselen = Directory.GetDirectories(inputbasepath).Length;
            if (!Directory.Exists(resultpath))
            {
                Directory.CreateDirectory(resultpath);
            }
            if (inbaselen == 0 || inbaselen == Directory.GetDirectories(resultpath).Length)
            {
                // Already zipped
                return;
            }

            string exepath = Path.Combine(GetOrderPath, "unzip", "exe");
            string temppath = Path.Combine(GetOrderPath, "temp");

            // clean temp path and copy the exe inside
            if (Directory.Exists(temppath))
            {
                Directory.Delete(resultpath, true);
            }

            Directory.CreateDirectory(temppath);
            IOUtils.DirectoryCopy(exepath, Path.Combine(temppath, "exe"), true);

            // for each job, take the job input and the exe , pack them into a zip and move them
            // into resultpath
            foreach (string job in JobsConfig.Jobs)
            {
                IOUtils.DirectoryCopy(job, Path.Combine(temppath, "input"), true);
                ZipFile.CreateFromDirectory(temppath, Path.Combine(resultpath, new DirectoryInfo(job).Name));

                if (ConfigReader.GetInstance().Config.DeleteSplitFolderAfterJobZip)
                {
                    Directory.Delete(job, true);
                }

                // delete the input folder from the temp path as another might take it's place
                Directory.Delete(Path.Combine(temppath, "input"), true);
            }

            // all jobs are in resultpath
        }

        private List<Slave> CheckWhichSlavesAreAvailable()
        {
            StorageProvider.GetInstance().GetSlaves().ForEach(CheckerStart); // this will change the list

            return StorageProvider.GetInstance().GetSlaves().FindAll(s => s.AvailableForWork != 2);

        }

        private void CheckerStart(Slave s)
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
