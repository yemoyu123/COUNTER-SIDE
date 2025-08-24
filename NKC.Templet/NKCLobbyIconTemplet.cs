using System;
using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCLobbyIconTemplet : INKMTemplet
{
	public int IDX;

	public NKM_SHORTCUT_TYPE m_ShortCutType;

	public string m_shortCutParam = string.Empty;

	public string m_IconName = string.Empty;

	public string m_Desc = string.Empty;

	public DateTime m_StartTime;

	public DateTime m_EndTime;

	public int m_OrderList;

	public STAGE_UNLOCK_REQ_TYPE m_UnlockReqType;

	public int m_UnlockReqValue;

	public DateTime m_StartTimeUTC => NKMTime.LocalToUTC(m_StartTime);

	public DateTime m_EndTimeUTC => NKMTime.LocalToUTC(m_EndTime);

	public int Key => IDX;

	public bool HasDateLimit
	{
		get
		{
			if (m_StartTime.Ticks > 0)
			{
				return m_EndTime.Ticks > 0;
			}
			return false;
		}
	}

	public static NKCLobbyIconTemplet Find(int idx)
	{
		return NKMTempletContainer<NKCLobbyIconTemplet>.Find(idx);
	}

	public static NKCLobbyIconTemplet LoadFromLUA(NKMLua lua)
	{
		NKCLobbyIconTemplet nKCLobbyIconTemplet = new NKCLobbyIconTemplet();
		int num = (int)(1u & (lua.GetData("IDX", ref nKCLobbyIconTemplet.IDX) ? 1u : 0u)) & (lua.GetData("m_ShortCutType", ref nKCLobbyIconTemplet.m_ShortCutType) ? 1 : 0);
		lua.GetData("m_ShortCut", ref nKCLobbyIconTemplet.m_shortCutParam);
		int num2 = num & (lua.GetData("m_LobbyIconName", ref nKCLobbyIconTemplet.m_IconName) ? 1 : 0);
		lua.GetData("m_LobbyIconDesc", ref nKCLobbyIconTemplet.m_Desc);
		int num3 = (int)((uint)num2 & (lua.GetData("m_StartTime", ref nKCLobbyIconTemplet.m_StartTime) ? 1u : 0u)) & (lua.GetData("m_EndTime", ref nKCLobbyIconTemplet.m_EndTime) ? 1 : 0);
		lua.GetData("m_OrderList", ref nKCLobbyIconTemplet.m_OrderList);
		lua.GetData("m_UnlockReqType", ref nKCLobbyIconTemplet.m_UnlockReqType);
		lua.GetData("m_UnlockReqValue", ref nKCLobbyIconTemplet.m_UnlockReqValue);
		if (num3 == 0)
		{
			return null;
		}
		return nKCLobbyIconTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public string GetDesc()
	{
		if (string.IsNullOrEmpty(m_Desc))
		{
			return string.Empty;
		}
		return NKCStringTable.GetString(m_Desc);
	}
}
