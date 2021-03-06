﻿using Angon.common.auth;
using Angon.common.comprotocols.requests;
using Angon.common.headers;
using Angon.common.sender;
using Angon.common.storage;
using Angon.common.utils;
using Serilog;
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
        private static ServerHelloHeader CreateServerHelloHeader(bool aproval, string sha, string message) => new ServerHelloHeader
        {
            AcceptedRequest = aproval,
            Sha = sha,
            Message = message
        };

        /// <summary>
        /// Gets the existing sha of the pending order
        /// </summary>
        /// <param name="chh"><see cref="ClientHelloHeader"/> containing all the information needed</param>
        /// <returns><see cref="SHA256"/> of pending order</returns>
        private static string GetExistingSha(ClientHelloHeader chh)
        {
            return StorageProvider.GetInstance().GetSHAOfExistingOrder(chh.ClientIP);
        }

        /// <summary>
        /// Runs the logic for Client Hello
        /// </summary>
        /// <param name="ch"><see cref="ClientHello"/></param>
        public static void Run(RequestWithHeader<ClientHelloHeader> ch)
        {
#if DEBUG
            Tuple<string, bool> auth = new Tuple<string, bool>("", true);
#else
            Tuple<string, bool> auth = Authentification.AuthenticateClient(ch.header.ClientIP, ch.header.ClientToken);
#endif
            string sha = "";
            bool aproval = false;
            if (auth.Item2 == true)
            {
                aproval = DecideAproval(ch.header);

                if (aproval == false)
                {
                    Log.Warning("Order was not approved, fetching already existing sha!");
                    sha = GetExistingSha(ch.header);
                    auth = new Tuple<string, bool>("Error: Existing order with sha:" + sha, false);
                }
                else
                {
                    sha = CreateSha(ch.header);
                    Log.Information("Order was approved, new sha :{0}", sha);
                }
            }

            if (auth.Item1.Contains("Token:"))
            {
                StorageProvider.GetInstance().RegisterClientToken(ch.header.ClientIP, auth.Item1.Split(':')[1]);
            }

            // Wrap the ServerHello Header
            WraperHeader wraper = new WraperHeader
            {
                Type = HeaderTypes.ServerHelloHeader,
                Data = ByteArrayUtils.ToByteArray(CreateServerHelloHeader(aproval, sha, auth.Item1))
            };

            Sender.Send(wraper, ch.Client);

            // Wait for the orderpost
            if (aproval == true)
            {
                OrderReciever.ReicieveClientHello(ch, sha);
            }
            // If aproval is false, the connection will close

        }

    }
}
