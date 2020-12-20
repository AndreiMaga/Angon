using System.Diagnostics;
using System.IO;

namespace Angon.common.utils
{
    class SecurityUtils
    {
        private static string PathToSignTool()
        {
            string binfolder = "C:\\Program Files (x86)\\Windows Kits\\10\bin\\";
            if (Directory.Exists(binfolder))
            {
                if (Directory.Exists(Path.Combine(binfolder, "x86")))
                {
                    return Path.Combine(binfolder, "x86", "signtool.exe");
                }
                else if (Directory.Exists(Path.Combine(binfolder, "x64")))
                {
                    Path.Combine(binfolder, "x64", "signtool.exe");
                }
            }
            return "";
        }

        public static bool FileIsSigned(string path)
        {
            string signtool = PathToSignTool();
            if (signtool != "")
            {
                Process p = Process.Start(signtool, "verify " + path);
                p.WaitForExit();
                return p.ExitCode == 0;
            }
            return false;
        }
    }
}
