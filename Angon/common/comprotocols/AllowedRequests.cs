using Angon.common.headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angon.common.comprotocols
{
    class AllowedRequests
    {
        public static List<char> Master { get; } = new List<char> { HeaderTypes.ClientHelloHeader, HeaderTypes.JobResultHeader };
        public static List<char> Client { get; } = new List<char> { HeaderTypes.ServerHelloHeader };
        public static List<char> Slave { get; } = new List<char> { HeaderTypes.ServerAvailableHeader };
    
        public static List<char> GetType(int index)
        {
            return index > 0 ? (index == 1 ? Slave : Client) : Master;
        }
    }
}
