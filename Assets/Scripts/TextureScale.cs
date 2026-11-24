using System.Threading;
using UnityEngine;

public class TextureScale
{
    public class ThreadData
    {
        public int start;
        public int end;

        public ThreadData(int s, int e)
        {
            start = s;
            end = e;
        }
    }

    static Color[] texColors;
    static Color[] newColors;
    static int w;
    static float ratioX;
    static float ratioY;
    static int w2;
    static int finishCount;
    static Mutex mutex;

    public static void Point(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, false);
    }

    public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, true);
    }

    static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
    {
        texColors = tex.GetPixels();
        newColors = new Color[newWidth * newHeight];
        if (useBilinear)
        {
            ratioX = 1f / ((float)newWidth / (float)(tex.width - 1));
            ratioY = 1f / ((float)newHeight / (float)(tex.height - 1));
        }
        else
        {
            ratioX = (float)tex.width / (float)newWidth;
            ratioY = (float)tex.height / (float)newHeight;
        }
        w = tex.width;
        w2 = newWidth;
        int cores = Mathf.Min(SystemInfo.processorCount, newHeight);
        int slice = newHeight / cores;
        finishCount = 0;
        if (mutex == null)
        {
            mutex = new Mutex(false);
        }
        if (cores > 1)
        {
            int i = 0;
            ThreadData parameter;
            for (i = 0; i < cores - 1; i++)
            {
                parameter = new ThreadData(slice * i, slice * (i + 1));
                var start = (!useBilinear) ? new ParameterizedThreadStart(PointScale) : new ParameterizedThreadStart(BilinearScale);
                var thread = new Thread(start);
                thread.Start(parameter);
            }
            parameter = new ThreadData(slice * i, newHeight);
            if (useBilinear)
            {
                BilinearScale(parameter);
            }
            else
            {
                PointScale(parameter);
            }
            while (finishCount < cores)
            {
                Thread.Sleep(1);
            }
        }
        else
        {
            var threadData = new ThreadData(0, newHeight);
            if (useBilinear)
            {
                BilinearScale(threadData);
            }
            else
            {
                PointScale(threadData);
            }
        }
        tex.Resize(newWidth, newHeight);
        tex.SetPixels(newColors);
        tex.Apply();
        texColors = null;
        newColors = null;
    }

    public static void BilinearScale(object obj)
    {
        var threadData = (ThreadData)obj;
        for (int i = threadData.start; i < threadData.end; i++)
        {
            int yFloor = (int)Mathf.Floor((float)i * ratioY);
            int row = yFloor * w;
            int nextRow = (yFloor + 1) * w;
            int newRow = i * w2;
            for (int j = 0; j < w2; j++)
            {
                int x = (int)Mathf.Floor((float)j * ratioX);
                float value = (float)j * ratioX - (float)x;
                newColors[newRow + j] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[row + x], texColors[row + x + 1], value), ColorLerpUnclamped(texColors[nextRow + x], texColors[nextRow + x + 1], value), (float)i * ratioY - (float)yFloor);
            }
        }
        mutex.WaitOne();
        finishCount++;
        mutex.ReleaseMutex();
    }

    public static void PointScale(object obj)
    {
        var threadData = (ThreadData)obj;
        for (int i = threadData.start; i < threadData.end; i++)
        {
            int rowStart = (int)(ratioY * (float)i) * w;
            int newRow = i * w2;
            for (int j = 0; j < w2; j++)
            {
                newColors[newRow + j] = texColors[(int)((float)rowStart + ratioX * (float)j)];
            }
        }
        mutex.WaitOne();
        finishCount++;
        mutex.ReleaseMutex();
    }

    static Color ColorLerpUnclamped(Color c1, Color c2, float value)
    {
        return new Color(c1.r + (c2.r - c1.r) * value, c1.g + (c2.g - c1.g) * value, c1.b + (c2.b - c1.b) * value, c1.a + (c2.a - c1.a) * value);
    }
}


