using Angon.common.comprotocols.requests;
using Angon.common.headers;
using Angon.common.storage.data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

///////////////////////////////////////////
///      THIS NEEDS OPTIMIZATIONS       ///
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

        public string GetSHAOfExistingOrder(string ip)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT sha FROM orders WHERE ip=$ip;";
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
            command.CommandText = @"SELECT id FROM orders WHERE ip=$ip and status not like 'done';";
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
                command.CommandText = @"INSERT INTO orders (ip, time, size, sha, version, status, created_at) VALUES($ip, $time, $size, $sha, $version, $status, $created_at);";
                command.Parameters.AddWithValue("$ip", ch.header.ClientIP);
                command.Parameters.AddWithValue("$time", new DateTimeOffset(ch.header.ClientUTCTime).ToUnixTimeSeconds());
                command.Parameters.AddWithValue("$size", ch.header.SizeInBytes);
                command.Parameters.AddWithValue("$sha", sha);
                command.Parameters.AddWithValue("$version", ch.header.ClientVersion);
                command.Parameters.AddWithValue("$status", "just started");
                command.Parameters.AddWithValue("$created_at", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        public int NumberOfJobsToBeDone()
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT count(status) FROM orders WHERE status not like 'done';";
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
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM orders where created_at=(SELECT MAX(CAST(created_at as integer))) FROM orders WHERE status not like 'done');";
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Order(reader.GetString(0), reader.GetString(1),
                                     reader.GetString(2), reader.GetString(3),
                                     reader.GetString(4), reader.GetString(5),
                                     reader.GetString(6), reader.GetInt32(7));
                }
            }
            return null;
        }

        public List<Slave> GetSlaves()
        {
            List<Slave> list = new List<Slave>();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM pool;";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Slave(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2) != 0));
                }
            }
            return list;
        }

        public bool ClientHasToken(string ip)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT ip FROM clients WHERE ip=$ip;";
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

        public bool ClientHasThisToken(string ip, string token)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT token FROM clients WHERE ip=$ip;";
            command.Parameters.AddWithValue("$ip", ip);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader.GetString(0).Equals(token);
                }
            }
            return false;
        }
        
        public void RegisterClientToken(string ip, string token)
        {
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO clients (ip, token) VALUES($ip,$token);";
                command.Parameters.AddWithValue("$ip", ip);
                command.Parameters.AddWithValue("$token", token);
                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }
    
        public void UpdateSha(string sha)
        {
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE orders SET sha=$sha WHERE id=(select seq from sqlite_sequence where name=\"orders\");";
                command.Parameters.AddWithValue("sha", sha);
                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        public void DeleteLatestClientOrder()
        {
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM orders WHERE id=(select seq from sqlite_sequence where name=\"orders\");";
                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        public string GetClientsToken()
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT token FROM clients WHERE ip like 'localhost'";
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            return "";
        }
    }
}
