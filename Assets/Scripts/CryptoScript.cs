using System.Globalization;
using System.Text;

public static class CryptoScript
{
    public static string Crypt(string text)
    {
        int length = text.Length;
        string very = "very";
        string veryUnder = "very_";
        string hard = "hard";
        string password = "password";
        string sixty = 60.ToString(CultureInfo.InvariantCulture);
        //key is built in pieces: veryvery_very_very60hard
        string key = very + veryUnder + veryUnder + very + sixty + hard;
        return Algorithm(Encoding.UTF8.GetBytes(text), Encoding.UTF8.GetBytes(key), key.Length, Encoding.UTF8);
    }

    public static string Crypt(byte[] bytes)
    {
        int byteCount = bytes.Length;
        string very = "very";
        string veryUnder = "very_";
        string hard = "hard";
        string password = "password";
        string sixty = 60.ToString(CultureInfo.InvariantCulture);
        string key = very + veryUnder + veryUnder + very + sixty + hard;
        return Algorithm(bytes, Encoding.UTF8.GetBytes(key), key.Length, Encoding.UTF8);
    }

    static string Algorithm(byte[] text, byte[] password, int passworLength, Encoding encoding)
    {
        int num = 1;
        var result = new byte[text.Length];
        //repeating key xor, the same call encrypts and decrypts
        for (int i = 0; i < text.Length; i++)
        {
            result[i] = (byte)(text[i] ^ password[i % passworLength]);
        }
        return encoding.GetString(result, 0, result.Length);
    }

    public static string Crypt(string text, Encoding encoding)
    {
        int length = text.Length;
        string very = "very";
        string veryUnder = "very_";
        string hard = "hard";
        string password = "password";
        string sixty = 60.ToString(CultureInfo.InvariantCulture);
        string key = very + veryUnder + veryUnder + very + sixty + hard;
        return Algorithm(encoding.GetBytes(text), encoding.GetBytes(key), key.Length, encoding);
    }

    public static string Crypt(byte[] bytes, Encoding encoding)
    {
        int length = bytes.Length;
        string very = "very";
        string veryUnder = "very_";
        string hard = "hard";
        string password = "password";
        string sixty = 60.ToString(CultureInfo.InvariantCulture);
        string key = very + veryUnder + veryUnder + very + sixty + hard;
        return Algorithm(bytes, encoding.GetBytes(key), key.Length, encoding);
    }
}


