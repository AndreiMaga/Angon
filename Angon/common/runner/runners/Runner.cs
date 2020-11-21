using Angon.common.comprotocols;
using Angon.common.comprotocols.requests;

namespace Angon.common.runner.runners
{
    class Runner
    {
        public static void Start(Request request)
        {
            switch (request)
            {
                case ClientHello ch:
                    ClientHelloRunner.Run(ch);
                    break;
                case ServerHello sh:
                    ServerHelloRunner.Run(sh);
                    break;
            }
        }

    }
}
