using Angon.common.comprotocols.requests;
using Angon.common.headers;
using Angon.common.storage.data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

///////////////////////////////////////////
/// WORK IN PROGRESS, JUST PROTOTYPING  ///
///////////////////////////////////////////

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

        internal string GetSHAOfExistingOrder(string ip)
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
            command.CommandText = @"SELECT * FROM orders WHERE ip=$ip and status not like 'done'";
            command.Parameters.AddWithValue("$ip", ip);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return true;
                }
            }
            return false;
        }

        public void ClientRegisteredOrder(GenericHello<ClientHelloHeader> ch, string sha)
        {
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO orders ip, time, size, sha, version, status, created_at VALUES($ip, $time, $size, $sha, $version, $status, $$created_at);";
                command.Parameters.AddWithValue("$ip", ch.header.ClientIP);
                command.Parameters.AddWithValue("$time", ch.header.ClientUTCTime);
                command.Parameters.AddWithValue("$size", ch.header.SizeInBytes);
                command.Parameters.AddWithValue("$sha", sha);
                command.Parameters.AddWithValue("$version", ch.header.ClientVersion);
                command.Parameters.AddWithValue("$status", "just started");
                command.Parameters.AddWithValue("$created_at", DateTime.UtcNow.ToString());
                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        public int NumberOfJobsToBeDone()
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT count(status) FROM orders WHERE status not like 'done'";
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            }
            return -1;
        }

        public Order GetOldestNotFinishedOrder()
        {
            return null;
        }

        public List<Slave> GetSlaves()
        {
            return null;
        }

    }
}
