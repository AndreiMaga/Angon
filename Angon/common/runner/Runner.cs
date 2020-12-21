using Angon.common.comprotocols;
using Angon.common.comprotocols.requests;
using Angon.common.headers;
using Angon.common.runner.runners;

namespace Angon.common.runner
{
    /// <summary>
    /// Runner for requests
    /// </summary>
    class Runner
    {
        /// <summary>
        /// Starts the correct runner
        /// </summary>
        /// <param name="request"></param>
        public static void Start(Request request)
        {
            switch (request)
            {
                case GenericHello<ClientHelloHeader> ch:
                    ClientHelloRunner.Run(ch);
                    break;
                case GenericHello<ServerHelloHeader> sh:
                    ServerHelloRunner.Run(sh);
                    break;
                case JobRequest jr:
                    JobRequestRunner.Run(jr);
                    break;
            }
        }

    }
}
