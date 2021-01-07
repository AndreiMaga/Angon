using Angon.common.headers;
using System.Collections.Generic;

namespace Angon.common.comprotocols
{
    class AllowedRequests
    {
        public static List<char> Master { get; } = new List<char> { HeaderTypes.ClientHelloHeader, HeaderTypes.JobResultHeader, HeaderTypes.RegisterHeader, HeaderTypes.RequestFinishedOrderHeader };
        public static List<char> Client { get; } = new List<char> { HeaderTypes.ServerHelloHeader, HeaderTypes.FinishedOrderHeader };
        public static List<char> Slave { get; } = new List<char> { HeaderTypes.ServerAvailableHeader, HeaderTypes.JobHeader, HeaderTypes.RegisterResponseHeader };

        public static List<char> GetType(int index)
        {
            return index > 0 ? (index == 1 ? Slave : Client) : Master;
        }
    }
}
