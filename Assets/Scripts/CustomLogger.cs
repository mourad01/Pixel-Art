public class CustomLogger
{
    static CustomLogger instance;

    public static CustomLogger Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CustomLogger();
            }
            return instance;
        }
    }

    public void Log(string s)
    {
    }
}


