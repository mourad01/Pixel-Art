using System.Globalization;
using System.Text;

public static class CryptoScript
{
    public static string Crypt(string text)
    {
        int length = text.Length;
        string text2 = "very";
        string text3 = "very_";
        string text4 = "hard";
        string text5 = "password";
        string text6 = 60.ToString(CultureInfo.InvariantCulture);
        string text7 = text2 + text3 + text3 + text2 + text6 + text4;
        return Algorithm(Encoding.UTF8.GetBytes(text), Encoding.UTF8.GetBytes(text7), text7.Length, Encoding.UTF8);
    }

    public static string Crypt(byte[] bytes)
    {
        int byteCount = bytes.Length;
        string text = "very";
        string text2 = "very_";
        string text3 = "hard";
        string text4 = "password";
        string text5 = 60.ToString(CultureInfo.InvariantCulture);
        string text6 = text + text2 + text2 + text + text5 + text3;
        return Algorithm(bytes, Encoding.UTF8.GetBytes(text6), text6.Length, Encoding.UTF8);
    }

    static string Algorithm(byte[] text, byte[] password, int passworLength, Encoding encoding)
    {
        int num = 1;
        var result = new byte[text.Length];
        for (int i = 0; i < text.Length; i++)
        {
            result[i] = (byte)(text[i] ^ password[i % passworLength]);
        }
        return encoding.GetString(result, 0, result.Length);
    }

    public static string Crypt(string text, Encoding encoding)
    {
        int length = text.Length;
        string text2 = "very";
        string text3 = "very_";
        string text4 = "hard";
        string text5 = "password";
        string text6 = 60.ToString(CultureInfo.InvariantCulture);
        string text7 = text2 + text3 + text3 + text2 + text6 + text4;
        return Algorithm(encoding.GetBytes(text), encoding.GetBytes(text7), text7.Length, encoding);
    }

    public static string Crypt(byte[] bytes, Encoding encoding)
    {
        int length = bytes.Length;
        string text = "very";
        string text2 = "very_";
        string text3 = "hard";
        string text4 = "password";
        string text5 = 60.ToString(CultureInfo.InvariantCulture);
        string text6 = text + text2 + text2 + text + text5 + text3;
        return Algorithm(bytes, encoding.GetBytes(text6), text6.Length, encoding);
    }
}


