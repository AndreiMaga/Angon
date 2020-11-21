using System;
using System.Net;
using System.Net.Sockets;

namespace Angon.common.utils
{
    class Networking
    {
        public static string GetIPAddress()
        {
            return new System.Net.WebClient().DownloadString("https://api.ipify.org");
        }
    }
}
