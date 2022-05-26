using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EasyUpdate.Crypto
{
    internal static class Hash
    {
        internal static string ComputeFileHash(string filePath, string hashName)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open))
            {
                using (HashAlgorithm hash = HashAlgorithm.Create(hashName))
                {
                    byte[] hashBuffer = hash.ComputeHash(file);
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var hashByte in hashBuffer)
                    {
                        stringBuilder.Append(hashByte.ToString("x2"));
                    }
                    return stringBuilder.ToString();
                }
            }
        }
    }
}
