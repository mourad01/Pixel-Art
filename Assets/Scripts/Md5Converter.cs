using System.Security.Cryptography;
using System.Text;

public class Md5Converter
{
    public static string CreateMD5(string input)
    {
        using (MD5 mD = MD5.Create())
        {
            var bytes = Encoding.ASCII.GetBytes(input);
            var hash = mD.ComputeHash(bytes);
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("X2"));
            }
            return stringBuilder.ToString().ToLower();
        }
    }
}


