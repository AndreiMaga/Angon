﻿using System;

namespace Angon.common.storage.data
{
    class Order
    {
        string ip;
        string time;
        int size;
        string sha;
        string version;
        string status;
        DateTimeOffset created_at;
        bool splitted;

        public Order(string ip, string time, string size, string sha, string version, string status, string created_at, int splitted)
        {
            this.Ip = ip;
            this.Time = time;
            this.Size = int.Parse(size);
            this.Sha = sha;
            this.Version = version;
            this.Status = status;
            this.Created_at = DateTimeOffset.FromUnixTimeSeconds(long.Parse(created_at));
            this.Splitted = splitted != 0;
        }

        public string Ip { get => ip; set => ip = value; }
        public string Time { get => time; set => time = value; }
        public int Size { get => size; set => size = value; }
        public string Sha { get => sha; set => sha = value; }
        public string Version { get => version; set => version = value; }
        public string Status { get => status; set => status = value; }
        public DateTimeOffset Created_at { get => created_at; set => created_at = value; }
        public bool Splitted { get => splitted; set => splitted = value; }
    }
}
