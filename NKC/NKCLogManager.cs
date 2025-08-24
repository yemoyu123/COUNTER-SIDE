using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cs.Memory;
using UnityEngine;

namespace NKC;

public class NKCLogManager
{
	private const int MAX_LOG_LINE = 1000;

	private static FileStream m_LogFileStream;

	private static StreamWriter m_LogFileStreamWriter;

	private static BinaryWriter m_LogFileBinaryWriter;

	private static List<string> m_LogList = new List<string>();

	private static object m_LockObject = new object();

	private static bool m_logMessageHandled = false;

	private static ConcurrentQueue<Action> m_ReservedActionQueue = new ConcurrentQueue<Action>();

	private static int m_logFileNum = 0;

	private static string m_logFileTime;

	private static List<string> m_listLogFilePath = new List<string>();

	public static void Init()
	{
		OpenNewLogFile();
		DeleteOldLogFile();
		if (!m_logMessageHandled)
		{
			Application.logMessageReceivedThreaded += HandleLog;
			m_logMessageHandled = true;
		}
	}

	public static void Update()
	{
		Action result;
		while (m_ReservedActionQueue.TryDequeue(out result))
		{
			result();
		}
	}

	public static string[] FlushLogList()
	{
		string[] array = null;
		lock (m_LockObject)
		{
			array = m_LogList.ToArray();
			m_LogList.Clear();
			return array;
		}
	}

	public static string GetSavePath()
	{
		if (NKCDefineManager.DEFINE_STEAM())
		{
			return Application.dataPath;
		}
		return Application.persistentDataPath;
	}

	public static string GetLogSavePath()
	{
		string text = Path.Combine(GetSavePath(), "Log");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	public static string[] GetLogFileList()
	{
		return (from x in Directory.GetFiles(GetLogSavePath(), "*_Log*.TXT")
			orderby new FileInfo(x).CreationTimeUtc.Date
			select x).ToArray();
	}

	public static void CreateNewLogFile()
	{
		lock (m_LockObject)
		{
			m_LogList.Clear();
			OpenNewLogFile();
		}
	}

	public static List<string> GetCurrentOpenedLogs()
	{
		return m_listLogFilePath;
	}

	public static void OpenNewLogFile()
	{
		if (string.IsNullOrEmpty(m_logFileTime))
		{
			m_logFileTime = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
			m_logFileTime = m_logFileTime.Replace('/', '_');
			m_logFileTime = m_logFileTime.Replace(' ', '_');
			m_logFileTime = m_logFileTime.Replace(':', '_');
		}
		m_logFileNum++;
		string text = GetLogSavePath() + "/CounterSide_" + m_logFileTime + "_" + $"{m_logFileNum:D3}" + "_Log.TXT";
		FileMode mode = FileMode.Create;
		if (File.Exists(text))
		{
			mode = FileMode.Truncate;
		}
		if (m_LogFileBinaryWriter != null)
		{
			m_LogFileBinaryWriter.Close();
		}
		if (m_LogFileStreamWriter != null)
		{
			m_LogFileStreamWriter.Close();
		}
		if (m_LogFileStream != null)
		{
			m_LogFileStream.Close();
		}
		m_LogFileStream = File.Open(text, mode, FileAccess.Write, FileShare.Read);
		if (NeedsLogEncryption())
		{
			m_LogFileBinaryWriter = new BinaryWriter(m_LogFileStream);
		}
		else
		{
			m_LogFileStreamWriter = new StreamWriter(m_LogFileStream);
			m_LogFileStreamWriter.AutoFlush = true;
		}
		m_listLogFilePath.Add(text);
	}

	private static void AddLogAndWriteFile(string logText)
	{
		string[] array = logText.Split(new string[2] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		lock (m_LockObject)
		{
			for (int i = 0; i < array.Length; i++)
			{
				m_LogList.Add(array[i]);
				if (NeedsLogEncryption())
				{
					byte[] bytes = Encoding.UTF8.GetBytes(array[i]);
					Crypto2.Encrypt(bytes, bytes.Length);
					byte[] bytes2 = BitConverter.GetBytes(bytes.Length);
					if (m_LogFileBinaryWriter != null)
					{
						m_LogFileBinaryWriter.Write(bytes2, 0, bytes2.Length);
						m_LogFileBinaryWriter.Write(bytes, 0, bytes.Length);
					}
				}
				else if (m_LogFileStreamWriter != null)
				{
					m_LogFileStreamWriter.WriteLine(array[i]);
				}
				if (m_LogList.Count >= 1000)
				{
					m_LogList.Clear();
					OpenNewLogFile();
				}
			}
			if (m_LogFileBinaryWriter != null)
			{
				m_LogFileBinaryWriter.Flush();
			}
		}
	}

	private static void DeleteOldLogFile()
	{
		string[] files = Directory.GetFiles(GetLogSavePath(), "*_Log*.TXT");
		DateTime now = DateTime.Now;
		for (int i = 0; i < files.Length; i++)
		{
			DateTime lastAccessTime = File.GetLastAccessTime(files[i]);
			if ((now - lastAccessTime).TotalDays > 3.0)
			{
				File.Delete(files[i]);
			}
		}
		files = Directory.GetFiles(Application.persistentDataPath, "*_Log*.TXT");
		for (int j = 0; j < files.Length; j++)
		{
			DateTime lastAccessTime2 = File.GetLastAccessTime(files[j]);
			if ((now - lastAccessTime2).TotalDays > 3.0)
			{
				File.Delete(files[j]);
			}
		}
	}

	private static string ConvertToColoredMsg(string msg, LogType type)
	{
		string text = "";
		switch (type)
		{
		case LogType.Log:
			text = "#FFFFFFFF";
			break;
		case LogType.Error:
		case LogType.Assert:
		case LogType.Exception:
			text = "#FF0000FF";
			break;
		case LogType.Warning:
			text = "#FFE400FF";
			break;
		default:
			text = "#FFFFFFFF";
			break;
		}
		return $"<color={text}>{msg}</color>".Replace("\n", $"</color>\n<color={text}>");
	}

	private static void HandleLog(string msg, string stackTrace, LogType type)
	{
		if (!msg.Contains("set as Camera.targetTexture!"))
		{
			string text = "";
			text = ((type != LogType.Error && type != LogType.Exception) ? $"[{DateTime.Now.ToLongTimeString()}] {msg}" : string.Format("[{0}] {1}", DateTime.Now.ToLongTimeString(), string.Join("\n", msg, stackTrace)));
			AddLogAndWriteFile(ConvertToColoredMsg(text, type));
		}
	}

	private static bool NeedsLogEncryption()
	{
		if (NKCDefineManager.DEFINE_PURE_LOG())
		{
			return false;
		}
		if (!NKCDefineManager.DEFINE_SAVE_LOG())
		{
			return false;
		}
		if (NKCDefineManager.DEFINE_USE_CHEAT())
		{
			return false;
		}
		return true;
	}
}
