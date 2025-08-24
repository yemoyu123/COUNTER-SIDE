using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

namespace NKC.Dev;

public class NKCMemoryStats : MonoBehaviour
{
	private struct StatInfo
	{
		public ProfilerCategory Cat;

		public string Name;

		public ProfilerMarkerDataUnit Unit;
	}

	public int fontSize = 24;

	public Color fontColor = Color.green;

	public Rect rect = new Rect(10f, 10f, 400f, 150f);

	private static NKCMemoryStats instance;

	private string statsText;

	public bool ShowGUI;

	private GUIStyle style = new GUIStyle();

	private Dictionary<string, ProfilerRecorder> dicProfiler = new Dictionary<string, ProfilerRecorder>();

	private StringBuilder sb = new StringBuilder(500);

	public string Stat => statsText;

	public static NKCMemoryStats Instance
	{
		get
		{
			if (instance == null)
			{
				MakeInstance();
			}
			return instance;
		}
	}

	public static bool HasInstance()
	{
		return instance != null;
	}

	public static void MakeInstance()
	{
		if (!(instance != null))
		{
			GameObject obj = new GameObject("MemoryStat");
			instance = obj.AddComponent<NKCMemoryStats>();
			Object.DontDestroyOnLoad(obj);
		}
	}

	public static void DestoryInstance()
	{
		if (!(instance == null))
		{
			Object.Destroy(instance.gameObject);
		}
	}

	private long GetValue(ProfilerRecorder recorder)
	{
		if (recorder.Valid)
		{
			return recorder.LastValue;
		}
		return 0L;
	}

	private void OnEnable()
	{
		AddProfiler(ProfilerCategory.Memory, "Total Reserved Memory");
		AddProfiler(ProfilerCategory.Memory, "GC Reserved Memory");
		AddProfiler(ProfilerCategory.Memory, "System Used Memory");
		AddProfiler(ProfilerCategory.Memory, "Total Used Memory");
		AddProfiler(ProfilerCategory.Memory, "Texture Memory");
		AddProfiler(ProfilerCategory.Memory, "Mesh Memory");
	}

	private void AddProfiler(ProfilerCategory category, string name)
	{
		dicProfiler.Add(name, ProfilerRecorder.StartNew(category, name));
	}

	private void OnDisable()
	{
		foreach (KeyValuePair<string, ProfilerRecorder> item in dicProfiler)
		{
			item.Value.Dispose();
		}
		dicProfiler.Clear();
	}

	private void Update()
	{
		sb.Clear();
		foreach (KeyValuePair<string, ProfilerRecorder> item in dicProfiler)
		{
			ProfilerRecorder value = item.Value;
			if (value.Valid)
			{
				sb.Append(item.Key);
				sb.Append(" : ");
				sb.AppendLine(GetByteString(value.LastValueAsDouble));
			}
		}
		statsText = sb.ToString();
	}

	private static string GetByteString(double value)
	{
		int i;
		for (i = 0; i <= 4; i++)
		{
			if (value < 1024.0)
			{
				break;
			}
			value /= 1024.0;
		}
		return $"{value:#.###}{GetByteSize(i)}";
	}

	private static string GetByteSize(int pow)
	{
		return pow switch
		{
			0 => "b", 
			1 => "kb", 
			2 => "mb", 
			3 => "gb", 
			_ => "tb", 
		};
	}

	private void OnGUI()
	{
		if (ShowGUI)
		{
			style.fontSize = fontSize;
			style.normal.textColor = fontColor;
			GUI.Label(rect, statsText, style);
		}
	}

	public Dictionary<string, long> SnapShot()
	{
		Dictionary<string, long> dictionary = new Dictionary<string, long>();
		foreach (KeyValuePair<string, ProfilerRecorder> item in dicProfiler)
		{
			ProfilerRecorder value = item.Value;
			if (value.Valid)
			{
				dictionary.Add(item.Key, value.LastValue);
			}
		}
		return dictionary;
	}

	public static Dictionary<string, long> DiffSnapshot(Dictionary<string, long> before, Dictionary<string, long> after)
	{
		Dictionary<string, long> dictionary = new Dictionary<string, long>();
		foreach (KeyValuePair<string, long> item in before)
		{
			long value = item.Value;
			if (after.TryGetValue(item.Key, out var value2))
			{
				dictionary.Add(item.Key, value2 - value);
			}
		}
		return dictionary;
	}

	public Dictionary<string, long> SnapShotChanges()
	{
		Dictionary<string, long> dictionary = new Dictionary<string, long>();
		foreach (KeyValuePair<string, ProfilerRecorder> item in dicProfiler)
		{
			ProfilerRecorder value = item.Value;
			if (value.Valid)
			{
				dictionary.Add(item.Key, value.CurrentValue - value.LastValue);
			}
		}
		return dictionary;
	}

	public static string GetSnapshotString(Dictionary<string, long> snapshot)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, long> item in snapshot)
		{
			stringBuilder.Append(item.Key);
			stringBuilder.Append(" : ");
			stringBuilder.AppendLine(GetByteString(item.Value));
		}
		return stringBuilder.ToString();
	}

	public static string EnumerateProfilerStats()
	{
		List<ProfilerRecorderHandle> list = new List<ProfilerRecorderHandle>();
		ProfilerRecorderHandle.GetAvailable(list);
		List<StatInfo> list2 = new List<StatInfo>(list.Count);
		foreach (ProfilerRecorderHandle item2 in list)
		{
			ProfilerRecorderDescription description = ProfilerRecorderHandle.GetDescription(item2);
			StatInfo item = new StatInfo
			{
				Cat = description.Category,
				Name = description.Name,
				Unit = description.UnitType
			};
			list2.Add(item);
		}
		list2.Sort(delegate(StatInfo a, StatInfo b)
		{
			int num = string.Compare(a.Cat.ToString(), b.Cat.ToString());
			return (num != 0) ? num : string.Compare(a.Name, b.Name);
		});
		StringBuilder stringBuilder = new StringBuilder("Available stats:\n");
		foreach (StatInfo item3 in list2)
		{
			stringBuilder.AppendLine($"{(int)(ushort)item3.Cat}\t\t - {item3.Name}\t\t - {item3.Unit}");
		}
		return stringBuilder.ToString();
	}
}
