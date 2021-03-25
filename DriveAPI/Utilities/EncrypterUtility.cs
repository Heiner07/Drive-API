using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DriveAPI.Utilities
{
    public class EncrypterUtility
    {
        /// <summary>
        /// Get a string and return the string representation of apply SHA256 algorithm
        /// to the string provided
        /// </summary>
        /// <param name="value">The string to apply the algorithm</param>
        /// <returns>A string representation of the result of SHA256 algorithm applied to
        /// the string provided</returns>
        public static string StringToSHA256String(string value)
        {
            SHA256 sha256 = SHA256.Create();
            var encodedBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(value));
            StringBuilder stringBuilder = new StringBuilder();

            foreach(byte encodedByte in encodedBytes)
            {
                stringBuilder.Append(value: encodedByte.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}
