public static class Helper
{
    public static short[] ReverseX(this short[] array, int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height / 2; j++)
            {
                short num = array[i + j * width];
                array[i + j * width] = array[i + (height - 1 - j) * width];
                array[i + (height - 1 - j) * width] = num;
            }
        }
        return array;
    }

    public static short[] Double(this short[] array, int width, int height)
    {
        var doubled = new short[array.Length * 4];
        int num = width * 2;
        int doubleHeight = height * 2;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                doubled[i * 2 + j * 2 * num] = (doubled[i * 2 + 1 + j * 2 * num] = (doubled[i * 2 + (j * 2 + 1) * num] = (doubled[i * 2 + 1 + (j * 2 + 1) * num] = array[i + j * width])));
            }
        }
        return doubled;
    }

    public static short[] Around(this short[] array, int width, int height)
    {
        var framed = new short[(width + 2) * (height + 2)];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                framed[i + 1 + (j + 1) * (width + 2)] = array[i + j * width];
            }
        }
        return framed;
    }
}


