using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ConsoleLog : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    struct Log
    {
        public string Message
        {
            get;
            set;
        }

        public string StackTrace
        {
            get;
            set;
        }

        public LogType Type
        {
            get;
            set;
        }
    }

    List<Log> m_logs = new List<Log>();
    Vector2 m_scrollPosition;
    bool m_show;
    bool m_collapse;
    bool m_showStack;
    bool m_showErrors;

    static Dictionary<LogType, Color> s_logTypeColors = new Dictionary<LogType, Color> {
        {
            LogType.Assert,
            Color.white
        },
        {
            LogType.Error,
            Color.red
        },
        {
            LogType.Exception,
            Color.red
        },
        {
            LogType.Log,
            Color.white
        },
        {
            LogType.Warning,
            Color.yellow
        }
    };

    const int Margin = 20;

    Rect m_windowRect = new Rect(20f, 20f, (float)(Screen.width - 40), (float)(Screen.height - 40));
    Rect m_titleBarRect = new Rect(0f, 0f, 10000f, 20f);
    GUIContent m_clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    GUIContent m_collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
    GUIContent m_showStackLabel = new GUIContent("Show Stack", "Hide/Show Stack Tree.");
    GUIContent m_showErrorsLabel = new GUIContent("Only Errors", "Hide/Show Only Errors.");

    public static ConsoleLog Inst { get; private set; }

    public bool IsShown
    {
        get
        {
            return m_show;
        }
        set
        {
            m_show = value;
        }
    }

    void Awake()
    {
        Inst = this;
        Object.DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        Application.logMessageReceived += new Application.LogCallback(HandleLog);
    }

    void OnDisable()
    {
        Application.logMessageReceived -= new Application.LogCallback(HandleLog);
    }

    void Update()
    {
    }

    void OnGUI()
    {
        //if (DebugController.Instance.EnableDebugTexts)
        //{
        //	GUILayout.Space(50f);
        //	if (ScreenToolWrapper.IsIphoneX)
        //	{
        //		GUILayout.Space(100f);
        //	}
        //	if (GUILayout.Button("Console", GUILayout.Width(500f)))
        //	{
        //		this.m_show = !this.m_show;
        //	}
        //	if (this.m_show)
        //	{
        //		this.m_windowRect = GUILayout.Window(123456, this.m_windowRect, new GUI.WindowFunction(this.ConsoleWindow), "Console");
        //	}
        //}
    }

    void ConsoleWindow(int windowId)
    {
        m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);
        for (int i = 0; i < m_logs.Count; i++)
        {
            var log = m_logs[i];
            if ((!m_collapse || i <= 0 || !(log.Message == m_logs[i - 1].Message)) && (!m_showErrors || IsError(log.Type)))
            {
                GUI.contentColor = s_logTypeColors[log.Type];
                GUILayout.Label(log.Message);
                GUI.color = Color.grey;
                if (m_showStack)
                {
                    GUILayout.Label(log.StackTrace);
                }
                GUI.color = Color.white;
            }
        }
        GUILayout.EndScrollView();
        GUI.contentColor = Color.white;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(m_clearLabel))
        {
            m_logs.Clear();
        }
        m_showStack = GUILayout.Toggle(m_showStack, m_showStackLabel, GUILayout.ExpandWidth(false));
        m_collapse = GUILayout.Toggle(m_collapse, m_collapseLabel, GUILayout.ExpandWidth(false));
        m_showErrors = GUILayout.Toggle(m_showErrors, m_showErrorsLabel, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();
        GUI.DragWindow(m_titleBarRect);
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        if (message.Length > 1000)
        {
            message = message.Substring(0, 5000) + "...";
        }
        m_logs.Add(new Log
        {
            Message = message,
            StackTrace = stackTrace,
            Type = type
        });
    }

    bool IsError(LogType logType)
    {
        return logType == LogType.Error || logType == LogType.Exception || logType == LogType.Assert || logType == LogType.Warning;
    }
}


