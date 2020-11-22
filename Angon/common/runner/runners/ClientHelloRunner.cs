using Angon.common.comprotocols.requests;
using Angon.common.config;
using Angon.common.headers;
using Angon.common.storage;
using Angon.common.utils;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Angon.common.runner.runners
{
    class ClientHelloRunner
    {

        private static bool DecideAproval(ClientHelloHeader ch)
        {
            return !StorageProvider.GetInstance().ClientHasOrder(ch.ClientIP);
        }

        public static String CreateSha(ClientHelloHeader ch)
        {
            String input = ch.ClientIP + ch.ClientUTCTime + ch.ClientVersion + ch.SizeInBytes;

            String now = DateTime.Now.ToString();
            String nowUTC = DateTime.UtcNow.ToString();
            input = now + input + nowUTC;

            input = new string(input.ToCharArray().OrderBy(x => Guid.NewGuid()).ToArray());

            using (SHA256 hash = SHA256Managed.Create())
            {
                return string.Concat(hash
                  .ComputeHash(Encoding.UTF8.GetBytes(input))
                  .Select(item => item.ToString("x2")));
            }
        }

        private static ServerHelloHeader createServerHelloHeader(bool aproval, string sha)
        {
            ServerHelloHeader shh = new ServerHelloHeader
            {
                AcceptedRequest = aproval,
                Sha = sha
            };
            return shh;
        }

        private static string GetExistingSha(ClientHelloHeader chh)
        {
            return StorageProvider.GetInstance().GetClientSha(chh.ClientIP);
        }

        public static void Run(ClientHello ch)
        {
            
            bool aproval = true;
            string sha = CreateSha(ch.header);
            /* SKIP FOR NOW 
            bool aproval = DecideAproval(ch.header);
            string sha;
            if (aproval == false)
            {
                sha = GetExistingSha(ch.header);
            }
            else
            {
                sha = CreateSha(ch.header);
            }
            */

            ServerHelloHeader shh = createServerHelloHeader(aproval, sha);
            byte[] dataArray = ByteArrayUtils.ToByteArray(shh);

            // Wrap the ServerHello Header
            WraperHeader wraper = new WraperHeader
            {
                Type = 'S',
                Data = dataArray
            };

            byte[] byteArray = ByteArrayUtils.ToByteArray(wraper);
            byte[] sizeArray = BitConverter.GetBytes(byteArray.Length);
            Console.WriteLine("Will send {0} bytes to the client", byteArray.Length);
            ch.Client.GetStream().Write(sizeArray, 0, sizeof(int)); // long has 8 bytes
            ch.Client.GetStream().Write(byteArray, 0, byteArray.Length);

            // Wait for the orderpost
            if (aproval == false)
            {
                ContinueExisting(ch);
            }
            else
            {
                OrderReciever.Recieve(ch, sha);
            }
        }

        public static void ContinueExisting(ClientHello ch)
        {

        }

    }
}
