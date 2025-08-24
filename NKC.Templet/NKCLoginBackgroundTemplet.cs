using System.Collections.Generic;
using Cs.Logging;
using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCLoginBackgroundTemplet : INKMTemplet
{
	public int ID;

	public LoginScreenDataType m_type;

	public string BundleName;

	public string AssetName;

	public string m_MusicName;

	public float m_MusicStartTime;

	public int Key => ID;

	public static NKCLoginBackgroundTemplet Find(int key)
	{
		return NKMTempletContainer<NKCLoginBackgroundTemplet>.Find(key);
	}

	public static NKCLoginBackgroundTemplet GetCurrentBackgroundTemplet()
	{
		foreach (NKCLoginBackgroundTemplet value in NKMTempletContainer<NKCLoginBackgroundTemplet>.Values)
		{
			if (value.m_type == LoginScreenDataType.BACKGROUND)
			{
				return value;
			}
		}
		Log.Error("[LoginBackgroundTemplet] 활성화된 Login Background 가 없습니다!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCLoginBackgroundTemplet.cs", 42);
		return null;
	}

	public static List<NKCLoginBackgroundTemplet> GetEnabledPrefabList()
	{
		List<NKCLoginBackgroundTemplet> list = new List<NKCLoginBackgroundTemplet>();
		foreach (NKCLoginBackgroundTemplet value in NKMTempletContainer<NKCLoginBackgroundTemplet>.Values)
		{
			if (value.m_type == LoginScreenDataType.PREFAB)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static NKCLoginBackgroundTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCLoginBackgroundTemplet.cs", 62))
		{
			return null;
		}
		NKCLoginBackgroundTemplet nKCLoginBackgroundTemplet = new NKCLoginBackgroundTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("ID", ref nKCLoginBackgroundTemplet.ID);
		if (!cNKMLua.GetDataEnum<LoginScreenDataType>("m_type", out nKCLoginBackgroundTemplet.m_type))
		{
			flag = false;
			Log.Error($"[BACKGROUND_TEMPLET][{nKCLoginBackgroundTemplet.ID}] 데이터 타입을 지정해주세요", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCLoginBackgroundTemplet.cs", 71);
		}
		flag &= cNKMLua.GetData("BundleName", ref nKCLoginBackgroundTemplet.BundleName);
		flag &= cNKMLua.GetData("AssetName", ref nKCLoginBackgroundTemplet.AssetName);
		if (!cNKMLua.GetData("m_MusicName", ref nKCLoginBackgroundTemplet.m_MusicName) && nKCLoginBackgroundTemplet.m_type == LoginScreenDataType.BACKGROUND)
		{
			flag = false;
			Log.Error($"[BACKGROUND_TEMPLET][{nKCLoginBackgroundTemplet.ID}] BACKGROUND 타입에는 배경음악 설정이 필수입니다", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCLoginBackgroundTemplet.cs", 83);
		}
		cNKMLua.GetData("m_MusicStartTime", ref nKCLoginBackgroundTemplet.m_MusicStartTime);
		if (!flag)
		{
			return null;
		}
		return nKCLoginBackgroundTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
