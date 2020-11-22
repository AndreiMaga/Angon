using Angon.common.comprotocols.requests;
using Angon.common.headers;
using Angon.common.storage;
using Angon.common.utils;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Angon.common.runner.runners
{
    /// <summary>
    /// Runner for the Client Hello Header.
    /// Server side
    /// </summary>
    class ClientHelloRunner
    {
        /// <summary>
        /// Decides if the client can place a new order
        /// </summary>
        /// <param name="ch"><see cref="ClientHelloHeader"/> the client hello header containing all information</param>
        /// <returns>true if the client does not have any pending orders</returns>
        private static bool DecideAproval(ClientHelloHeader ch)
        {
            return !StorageProvider.GetInstance().ClientHasOrder(ch.ClientIP);
        }

        /// <summary>
        /// Creates a SHA256 string from the Client Hello Header
        /// </summary>
        /// <param name="ch"><see cref="ClientHelloHeader"/> the client hello header containing all information</param>
        /// <returns><see cref="SHA256"/> represented as a string</returns>
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

        /// <summary>
        /// Creates a Server Hello Header
        /// </summary>
        /// <param name="aproval">can the client start place a new order</param>
        /// <param name="sha">the sha of the order, the new or old one</param>
        /// <returns></returns>
        private static ServerHelloHeader CreateServerHelloHeader(bool aproval, string sha) => new ServerHelloHeader
        {
            AcceptedRequest = aproval,
            Sha = sha
        };

        /// <summary>
        /// Gets the existing sha of the pending order
        /// </summary>
        /// <param name="chh"><see cref="ClientHelloHeader"/> containing all the information needed</param>
        /// <returns><see cref="SHA256"/> of pending order</returns>
        private static string GetExistingSha(ClientHelloHeader chh)
        {
            return StorageProvider.GetInstance().GetClientSha(chh.ClientIP);
        }

        /// <summary>
        /// Runs the logic for Client Hello
        /// </summary>
        /// <param name="ch"><see cref="ClientHello"/></param>
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

            ServerHelloHeader shh = CreateServerHelloHeader(aproval, sha);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        public static void ContinueExisting(ClientHello ch)
        {

        }

    }
}
