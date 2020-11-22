using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Angon.common.utils
{
    /// <summary>
    /// Suite of byte array utilities
    /// </summary>
    class ByteArrayUtils
    {
        /// <summary>
        /// Generic method to serialize <typeparamref name="obj"/> 
        /// </summary>
        /// <typeparam name="T">Type of object to serialize</typeparam>
        /// <param name="obj">The object to be serialized</param>
        /// <returns>byte[] with all the bytes of obj</returns>
        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Generic method to deserialize <paramref name="data"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize</typeparam>
        /// <param name="data">byte[] to be deserialized</param>
        /// <returns><typeparamref name="T"/> deserialized</returns>
        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }

        }
        /// <summary>
        /// Method to write file, from parts or whole
        /// </summary>
        /// <param name="byteArray">The array to be written</param>
        /// <param name="fileName">The file name where the <paramref name="byteArray"/> should be saved</param>
        /// <returns>true if everything went ok</returns>
        public static bool ByteArrayToFile(byte[] byteArray, string fileName)
        {
            try
            {
                using (var fs = new FileStream(fileName, File.Exists(fileName) ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
