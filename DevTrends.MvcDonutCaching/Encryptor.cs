using System.Text;
using System.Web.Security;

namespace DevTrends.MvcDonutCaching
{
    public class Encryptor : IEncryptor
    {
        /// <summary>
        /// Encrypts the specified plain text.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns></returns>
        public string Encrypt(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return MachineKey.Encode(plainTextBytes, MachineKeyProtection.Encryption);
        }

        /// <summary>
        /// Decrypts the specified encrypted text.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <returns></returns>
        public string Decrypt(string encryptedText)
        {
            var decryptedBytes = MachineKey.Decode(encryptedText, MachineKeyProtection.Encryption);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
