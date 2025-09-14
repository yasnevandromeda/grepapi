using System.Security.Cryptography;

namespace grepapi
{
    public class SecureManager
    {
        public const string DbValuesEncryptPassKey = "StmYZlho3x9P66BOhgeut2A@OFU&Y6";
        public const string Salt = "34234sfdsf435";
        public static string EncryptString(string plaintext, string password = DbValuesEncryptPassKey)
        {
            StringEncryptor se = new StringEncryptor(DbValuesEncryptPassKey, Salt);
            return se.Encrypt(plaintext);
        }

        public static string DecryptString(string encrypted, string password = DbValuesEncryptPassKey)
        {
            StringEncryptor se = new StringEncryptor(DbValuesEncryptPassKey, Salt);
            return se.Decrypt(encrypted);
        }
    }
}
