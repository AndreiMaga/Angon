using System;
using System.IO;

namespace Angon.common.zip
{
    class ZipUtils
    {

        public static bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }

            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
