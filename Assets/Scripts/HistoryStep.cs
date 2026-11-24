using System;
using System.Collections.Generic;
using System.Text;

[Serializable]
public class HistoryStep
{
    public List<ShortVector2> Vectors { get; private set; }

    public void Add(ShortVector2 vector)
    {
        if (Vectors == null)
        {
            Vectors = new List<ShortVector2>();
        }
        Vectors.Add(vector);
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < Vectors.Count; i++)
        {
            stringBuilder.Append(Vectors[i]).Append(";");
        }
        return string.Format("{0}", stringBuilder);
    }
}


