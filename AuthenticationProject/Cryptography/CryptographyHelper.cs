using System.Security.Cryptography;
using System.Text;

namespace AuthenticationProject.Cryptography;
public class CryptographyHelper
{
    public static byte[] Encrypt(string plainText, ICryptoTransform cryptoTransform)
    {
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");

        using (MemoryStream ms = new MemoryStream())
        {
            using (CryptoStream cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
            }
            return ms.ToArray();
        }
    }

    public static string Decrypt(byte[] chipperText, ICryptoTransform cryptoTransform)
    {
        if (chipperText == null || chipperText.Length <= 0)
            throw new ArgumentNullException("plainText");

        using (MemoryStream ms = new MemoryStream(chipperText))
        {
            using (CryptoStream cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Read))
            {
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}


public interface ICryptographyService
{
    string Encrypt(string plainText, string tenancyName);
    string Decrypt(string chipperText, string tenancyName);
}

public class CryptographyService : ICryptographyService
{

    private const string key = "6A3CEDF20C164A30815282E84A9ED096";
    private byte[] keyBytes = new byte[32];
    private byte[] ivBytes = new byte[16];


    public string Decrypt(string chipperText, string tenancyName)
    {
        string iv = $"{tenancyName}{key}";

        Array.Copy(new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(key)), keyBytes, keyBytes.Length);
        Array.Copy(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(iv)), ivBytes, ivBytes.Length);

        using (var aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = ivBytes;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            return CryptographyHelper.Decrypt(Convert.FromBase64String(chipperText), decryptor);
        }
    }

    public string Encrypt(string plainText, string tenancyName)
    {
        throw new NotImplementedException();
    }
}
