using Angon.common.config;
using Angon.common.storage.data;
using Angon.common.utils;
using Angon.master.scheduler;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Angon.master.splitter
{
    
    class Splitter
    {
        private readonly Order Order;

        public static bool MaliciousReturn = false;
        private string PathToOrder { get => Path.Combine(ConfigReader.GetInstance().Config.SavePath,Order.Sha,"unzip"); }

        private string PathToInputFolder { get => Path.Combine(PathToOrder, "input"); }
        private string PathToSplitResult { get => Path.Combine(PathToInputFolder, "jobs"); }

        private readonly OrderConfig OrderConfig;

        public Splitter(Order order, OrderConfig orderConfig)
        {
            Order = order;
            OrderConfig = orderConfig;
        }

        public void Split()
        {
            if(!ConfigReader.GetInstance().Config.DisableExternalSplitter && OrderConfig.NameOfExternalSplitter != "")
            {
                if (ConfigReader.GetInstance().Config.VerifySignature && !SecurityUtils.FileIsSigned(OrderConfig.NameOfExternalSplitter))
                {
                    Log.Warning("The splitter {0} is not signed by a valid authority ! Please check the order {1} !", SecurityUtils.FileIsSigned(OrderConfig.NameOfExternalSplitter), Order.Sha);
                    Log.Error("The splitter {0} is not signed by a valid authority ! Please check the order {1} !", SecurityUtils.FileIsSigned(OrderConfig.NameOfExternalSplitter), Order.Sha);

                    // TODO : Mark order as malicious

                    MaliciousReturn = true;
                    return;
                }
                Process p = Process.Start(OrderConfig.NameOfExternalSplitter, OrderConfig.ExternalSplitterArguments);
                p.WaitForExit();
                return;
            }

            // run default splitter
            List<FileInfo> fileInfos = new List<FileInfo>();
            
            foreach(string inputFilePath in Directory.GetFiles(PathToInputFolder))
            {
                fileInfos.Add(new FileInfo(inputFilePath));
            }

            // sanity check
            if (fileInfos.Count == 0)
            {
                return;
            }

            Directory.CreateDirectory(PathToSplitResult);

            // No groups to be made with 1 file
            if( fileInfos.Count == 1)
            {
                MoveFileInfoToDirectory(fileInfos[0], CreateJobDirectory());
                return;
            }

            fileInfos.Sort(CompareFileInfo);

            SplitFileInfos(fileInfos);
            

            // Splitting is done

        }

        private int CompareFileInfo(FileInfo x, FileInfo y)
        {
            return x.Length.CompareTo(y.Length);
        }

        private void SplitFileInfos(List<FileInfo> fileInfos)
        {
            List<List<FileInfo>> groups = new List<List<FileInfo>>();

            int max_groups = (fileInfos.Count / 2) + 1;

            // splitting algorithm
            groups.Add(new List<FileInfo>());
            groups[0].Add(Pop(fileInfos));
            groups.Add(new List<FileInfo>());
            groups[0].Add(Pop(fileInfos));


            // while there are still file infos to split
            while (fileInfos.Count > 0)
            {
                // pop from the front
                FileInfo fo = Pop(fileInfos);
                float avg = groups[0].Sum(x => x.Length);
                int i = 1; // start from the second one
                float current_sum = groups[i].Sum(x => x.Length);
                while (fo.Length + current_sum > avg * OrderConfig.DefaultSplitterDeviation && i < max_groups)
                {
                    avg = (avg + current_sum) / 2;
                    i++;
                    current_sum = groups[i].Sum(x => x.Length);
                }
                // if i == max groups, append to the first group
                groups[i == max_groups ? 0 : i].Add(fo);
            }
            
            foreach(List<FileInfo> group in groups)
            {
                MoveGroupToDirectory(group);
            }
        }

        private void MoveGroupToDirectory(List<FileInfo> group)
        {
            string dir = CreateJobDirectory();
            
            foreach(FileInfo fi in group)
            {
                MoveFileInfoToDirectory(fi, dir);
            }

        }

        private void MoveFileInfoToDirectory(FileInfo fi, string dir)
        {
            fi.CopyTo(Path.Combine(dir, fi.Name));
        }

        private string CreateJobDirectory()
        {
            string dir = Path.Combine(PathToSplitResult, Path.GetRandomFileName());
            // for that 1 in 2^57
            while (Directory.Exists(dir))
            {
                dir = Path.Combine(PathToSplitResult, Path.GetRandomFileName());
            }
            Directory.CreateDirectory(dir);

            return dir;
        }
        
        private T Pop<T>(List<T> list)
        {
            T r = list[0];
            list.RemoveAt(0);
            return r;
        }
    }
}
