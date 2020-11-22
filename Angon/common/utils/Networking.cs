﻿namespace Angon.common.utils
{
    /// <summary>
    /// Suite of network utilities
    /// </summary>
    class Networking
    {
        /// <summary>
        /// Get the public IP address
        /// </summary>
        /// <returns>public ip address</returns>
        public static string GetIPAddress()
        {
            return new System.Net.WebClient().DownloadString("https://api.ipify.org");
        }
    }
}
