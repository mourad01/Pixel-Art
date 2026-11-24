using System;
using System.Collections.Generic;
using System.IO;

public class History
{
    string m_fileName;
    StreamWriter m_streamWriter;

    public List<HistoryStep> Steps { get; private set; }

    public void Init(string fileName)
    {
        m_fileName = fileName;
        Steps = new List<HistoryStep>();
        if (File.Exists(fileName))
        {
            var lines = File.ReadAllLines(fileName);
            Steps = new List<HistoryStep>(lines.Length);
            var allLines = lines;
            foreach (string text in allLines)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    //one step per line, pixels as x:y separated by ;
                    var historyStep = new HistoryStep();
                    var parts = text.Split(new char[1] {
                        ';'
                    }, StringSplitOptions.RemoveEmptyEntries);
                    var stepParts = parts;
                    foreach (string part in stepParts)
                    {
                        var coords = part.Split(new char[1] {
                            ':'
                        }, StringSplitOptions.RemoveEmptyEntries);
                        if (coords.Length == 2)
                        {
                            historyStep.Add(new ShortVector2(short.Parse(coords[0]), short.Parse(coords[1])));
                        }
                    }
                    Steps.Add(historyStep);
                }
            }
        }
        m_streamWriter = File.AppendText(fileName);
    }

    public void AddStep(HistoryStep historyStep)
    {
        Steps.Add(historyStep);
        m_streamWriter.WriteLine(historyStep);
    }

    public void Shuffle()
    {
        Steps.Shuffle();
    }

    public void Save()
    {
        //close and reopen the writer to flush it to disk
        if (m_streamWriter != null)
        {
            m_streamWriter.Close();
            m_streamWriter = File.AppendText(m_fileName);
        }
    }

    public void Close()
    {
        if (m_streamWriter != null)
        {
            m_streamWriter.Close();
        }
    }
}


