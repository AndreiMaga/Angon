using Angon.common.storage;
using Serilog;
using System;
using System.Security.Cryptography;

namespace Angon.common.auth
{
    class Authentification
    {

        private static string GenerateSha()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] tokenData = new byte[256];
            rng.GetBytes(tokenData);

            string token = Convert.ToBase64String(tokenData);

            return "Token:" + token;
        }
        public static Tuple<string, bool> AuthenticateClient(string ip, string token)
        {
            // check if the header token matches database token
            if (token.Equals("")) // empty only on the first request of the client
            {
                // check if the client has a token
                if (StorageProvider.GetInstance().ClientHasToken(ip))
                {
                    Log.Warning("Client with ip {0} tried to register but already has a token", ip);
                    return new Tuple<string, bool>("Error: invalid token!", false);
                }
                Log.Warning("Client with ip {0} generated a new token {1}", ip, token);
                return new Tuple<string, bool>(GenerateSha(), true);
            }

            // if token is provided check if it matches
            if (StorageProvider.GetInstance().ClientHasThisToken(ip, token))
            {
                return new Tuple<string, bool>("", true);
            }
            return new Tuple<string, bool>("Error: invalid token!", false);
        }
    }
}
