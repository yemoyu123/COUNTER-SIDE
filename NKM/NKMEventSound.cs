using System.Collections.Generic;
using Cs.Logging;
using NKM.Unit;

namespace NKM;

public class NKMEventSound : NKMUnitEffectStateEventOneTime, INKMUnitStateEvent, IEventConditionOwner
{
	public int m_SkinID = -1;

	private List<string> m_listSoundName = new List<string>();

	public bool m_bVoice;

	public float m_fLocalVol = 1f;

	public float m_fFocusRange = 1200f;

	public bool m_b3D;

	public bool m_bLoop;

	public float m_PlayRate = 1f;

	public bool m_bStopSound;

	public Dictionary<int, List<string>> m_dicSkinSound;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Client;

	public List<string> GetTargetSoundList(int skinID)
	{
		if (m_dicSkinSound != null && m_dicSkinSound.TryGetValue(skinID, out var value) && value != null && value.Count > 0)
		{
			return value;
		}
		return m_listSoundName;
	}

	public List<string> GetTargetSoundList(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return m_listSoundName;
		}
		return GetTargetSoundList(unitData.m_SkinID);
	}

	public List<string> GetTargetSoundList(NKMDamageEffectData DEData)
	{
		if (DEData == null)
		{
			return m_listSoundName;
		}
		return GetTargetSoundList(DEData.GetMasterSkinID());
	}

	public bool GetRandomSound(int skinID, out string soundName)
	{
		List<string> targetSoundList = GetTargetSoundList(skinID);
		if (targetSoundList.Count > 0)
		{
			int index = NKMRandom.Range(0, targetSoundList.Count);
			soundName = targetSoundList[index];
			return true;
		}
		soundName = null;
		return false;
	}

	public bool GetRandomSound(NKMUnitData unitData, out string soundName)
	{
		if (unitData == null)
		{
			return GetRandomSound(0, out soundName);
		}
		return GetRandomSound(unitData.m_SkinID, out soundName);
	}

	public bool GetRandomSound(NKMDamageEffectData DEData, out string soundName)
	{
		if (DEData == null)
		{
			return GetRandomSound(0, out soundName);
		}
		return GetRandomSound(DEData.GetMasterSkinID(), out soundName);
	}

	public bool IsRightSkin(int unitSkinID)
	{
		if (m_SkinID < 0)
		{
			return true;
		}
		return m_SkinID == unitSkinID;
	}

	public void DeepCopyFromSource(NKMEventSound source)
	{
		DeepCopy(source);
		m_SkinID = source.m_SkinID;
		m_listSoundName.Clear();
		for (int i = 0; i < source.m_listSoundName.Count; i++)
		{
			m_listSoundName.Add(source.m_listSoundName[i]);
		}
		m_bVoice = source.m_bVoice;
		m_fLocalVol = source.m_fLocalVol;
		m_fFocusRange = source.m_fFocusRange;
		m_b3D = source.m_b3D;
		m_bLoop = source.m_bLoop;
		m_PlayRate = source.m_PlayRate;
		m_bStopSound = source.m_bStopSound;
		if (source.m_dicSkinSound != null)
		{
			m_dicSkinSound = new Dictionary<int, List<string>>();
			{
				foreach (KeyValuePair<int, List<string>> item in source.m_dicSkinSound)
				{
					m_dicSkinSound.Add(item.Key, new List<string>(item.Value));
				}
				return;
			}
		}
		m_dicSkinSound = null;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_SkinID", ref m_SkinID);
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
		cNKMLua.GetData("m_bVoice", ref m_bVoice);
		cNKMLua.GetData("m_fLocalVol", ref m_fLocalVol);
		cNKMLua.GetData("m_fFocusRange", ref m_fFocusRange);
		cNKMLua.GetData("m_b3D", ref m_b3D);
		cNKMLua.GetData("m_bLoop", ref m_bLoop);
		cNKMLua.GetData("m_PlayRate", ref m_PlayRate);
		cNKMLua.GetData("m_bStopSound", ref m_bStopSound);
		if (cNKMLua.OpenTable("m_dicSkinSound"))
		{
			if (m_SkinID >= 0)
			{
				Log.Warn($"m_dicSkinSound이 m_SkinID {m_SkinID}과 함께 사용되었습니다.. m_dicSkinSound가 무시될 가능성이 높습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 2986);
			}
			m_dicSkinSound = new Dictionary<int, List<string>>();
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				if (!cNKMLua.GetData(1, out var rValue2, 0))
				{
					Log.Error("Bad data from m_dicSkinSound for skinID " + m_SkinID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 2997);
					continue;
				}
				List<string> list = new List<string>();
				string rValue3;
				for (int j = 2; cNKMLua.GetData(j, out rValue3, ""); j++)
				{
					list.Add(rValue3);
				}
				m_dicSkinSound.Add(rValue2, list);
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMDamageEffect cNKMDamageEffect)
	{
		cNKMDamageEffect.ApplyEventSound(this);
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.ApplyEventSound(this);
	}
}
