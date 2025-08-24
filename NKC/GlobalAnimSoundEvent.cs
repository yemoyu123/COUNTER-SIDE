using System.Collections.Generic;
using NKM;

namespace NKC;

public class GlobalAnimSoundEvent
{
	public string m_AssetName = "";

	public string m_AniName = "";

	public float m_fTime;

	public List<string> m_listSoundName = new List<string>();

	public void DeepCopyFromSource(GlobalAnimSoundEvent source)
	{
		m_AssetName = source.m_AssetName;
		m_AniName = source.m_AniName;
		m_fTime = source.m_fTime;
		m_listSoundName.Clear();
		for (int i = 0; i < source.m_listSoundName.Count; i++)
		{
			m_listSoundName.Add(source.m_listSoundName[i]);
		}
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_AssetName", ref m_AssetName);
		cNKMLua.GetData("m_AniName", ref m_AniName);
		cNKMLua.GetData("m_fTime", ref m_fTime);
		if (cNKMLua.OpenTable("m_listSoundName"))
		{
			m_listSoundName.Clear();
			int i = 1;
			for (string rValue = ""; cNKMLua.GetData(i, ref rValue); i++)
			{
				m_listSoundName.Add(rValue);
			}
			cNKMLua.CloseTable();
		}
		return true;
	}
}
