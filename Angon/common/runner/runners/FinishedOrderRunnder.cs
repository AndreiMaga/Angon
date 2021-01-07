using Angon.common.comprotocols.requests;
using Angon.common.headers;
using Serilog;

namespace Angon.common.runner.runners
{
    class FinishedOrderRunnder
    {
        public static void Run(RequestWithHeader<FinishedOrderHeader> foh)
        {
            Log.Warning(foh.header.Message);
            if (foh.header.Size == 0)
            {
                Log.Warning(foh.header.Message);
                return;
            }

            OrderReciever.RecieveFinishedOrder(foh);
        }
    }
}
