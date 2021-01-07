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
                case RequestWithHeader<ClientHelloHeader> ch:
                    ClientHelloRunner.Run(ch);
                    break;
                case RequestWithHeader<ServerHelloHeader> sh:
                    ServerHelloRunner.Run(sh);
                    break;
                case RequestWithHeader<JobHeader> jr:
                    JobRequestRunner.Run(jr);
                    break;
                case RequestWithHeader<ServerAvailableHeader> sah:
                    ServerAvailableRunner.Run(sah);
                    break;
                case RequestWithHeader<JobResultHeader> jrh:
                    JobResultRunner.Run(jrh);
                    break;
                case RequestWithHeader<RegisterHeader> rh:
                    RegisterHeaderRunner.Run(rh);
                    break;
                case RequestWithHeader<RegisterResponseHeader> rrh:
                    RegisterResponseRunner.Run(rrh);
                    break;
                case RequestWithHeader<RequestFinishedOrderHeader> rfo:
                    RequestFinishedOrderRunner.Run(rfo);
                    break;
                case RequestWithHeader<FinishedOrderHeader> foh:
                    FinishedOrderRunnder.Run(foh);
                    break;
            }
        }

    }
}
