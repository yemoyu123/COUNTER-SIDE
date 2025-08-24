using System;
using Cs.Logging;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public class NKCLoginCutSceneTemplet : INKMTempletEx, INKMTemplet
{
	private string dateStrId;

	public int m_Key;

	public string m_CutSceneStrID;

	public EventUnlockCond m_CondType;

	public string m_CondValue;

	public int m_OrderList;

	public int Key => m_Key;

	public NKMIntervalTemplet IntervalTemplet { get; private set; } = NKMIntervalTemplet.Invalid;

	public DateTime StartDateUTC => NKMTime.LocalToUTC(IntervalTemplet.StartDate);

	public DateTime EndDateUTC => NKMTime.LocalToUTC(IntervalTemplet.EndDate);

	public bool HasDateLimit => IntervalTemplet.IsValid;

	public static NKCLoginCutSceneTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLoginCutSceneManager.cs", 47))
		{
			return null;
		}
		NKCLoginCutSceneTemplet nKCLoginCutSceneTemplet = new NKCLoginCutSceneTemplet();
		int num = (int)(1u & (lua.GetData("m_Key", ref nKCLoginCutSceneTemplet.m_Key) ? 1u : 0u) & (lua.GetData("m_CutSceneStrID", ref nKCLoginCutSceneTemplet.m_CutSceneStrID) ? 1u : 0u)) & (lua.GetData("m_DateStrID", ref nKCLoginCutSceneTemplet.dateStrId) ? 1 : 0);
		lua.GetData("m_CondType", ref nKCLoginCutSceneTemplet.m_CondType);
		lua.GetData("m_CondValue", ref nKCLoginCutSceneTemplet.m_CondValue);
		lua.GetData("m_OrderList", ref nKCLoginCutSceneTemplet.m_OrderList);
		if (num == 0)
		{
			Debug.LogError("NKCLoginCutSceneTemplet LoadFromLUA fail");
			return null;
		}
		return nKCLoginCutSceneTemplet;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	public void JoinIntervalTemplet()
	{
		IntervalTemplet = NKMIntervalTemplet.Find(dateStrId);
		if (IntervalTemplet == null)
		{
			IntervalTemplet = NKMIntervalTemplet.Unuseable;
			Log.ErrorAndExit("잘못된 interval id :" + dateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLoginCutSceneManager.cs", 81);
		}
		else if (IntervalTemplet.IsRepeatDate)
		{
			Log.ErrorAndExit($"[LoginCutscene:{Key}] 반복 기간설정 사용 불가. id:{dateStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLoginCutSceneManager.cs", 87);
		}
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}

	public void Validate()
	{
	}
}
