using Angon.common.comprotocols.requests;
using Angon.common.storage.data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Angon.common.storage
{
    // Signleton
    class StorageProvider
    {
        private static StorageProvider instance;

        private readonly SqliteConnection connection;

        StorageProvider()
        {
            connection = new SqliteConnection("Data Source=storage.db");
            //SQLitePCL.raw.SetProvider(new SQLitePCL.);
            connection.Open();
        }
        ~StorageProvider()
        {
            this.connection.Close();
        }

        public static StorageProvider GetInstance()
        {
            if (instance == null)
            {
                instance = new StorageProvider();
            }

            return instance;
        }


        public void SaveClient(Client c)
        {
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO slaves ip, port VALUES($ip, $port);";
                command.Parameters.AddWithValue("$ip", c.Ip);
                command.Parameters.AddWithValue("$port", c.Port);
                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }


        public List<Client> GetClients()
        {
            List<Client> clients = new List<Client>();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT ip, port FROM clients";

            using (var reader = command.ExecuteReader())
            {
                int index = 0;
                while (reader.Read())
                {
                    clients.Add(new Client(reader.GetString(index++), reader.GetString(index++)));
                }
            }

            return clients;
        }

        internal string GetClientSha(string ip)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT sha FROM orders WHERE ip=$ip";
            command.Parameters.AddWithValue("$ip", ip);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            return "";
        }

        public Boolean ClientHasOrder(string ip)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM orders WHERE ip=$ip";
            command.Parameters.AddWithValue("$ip", ip);
            using(var reader = command.ExecuteReader())
            {
                if(reader.Read()){
                    return true;
                }
            }
            return false;
        }

        public void ClientRegisteredOrder(ClientHello ch, string sha)
        {
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO orders ip, time, size, sha, version, status VALUES($ip, $time, $size, $sha, $version, $status);";
                command.Parameters.AddWithValue("$ip", ch.header.ClientIP);
                command.Parameters.AddWithValue("$time", ch.header.ClientUTCTime);
                command.Parameters.AddWithValue("$size", ch.header.SizeInBytes);
                command.Parameters.AddWithValue("$sha", sha);
                command.Parameters.AddWithValue("$version", ch.header.ClientVersion);
                command.Parameters.AddWithValue("$status", "just started");
                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }
    }
}
