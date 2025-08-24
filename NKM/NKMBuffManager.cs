using System;
using NKM.Templet.Base;

namespace NKM;

public class NKMBuffManager
{
	public static void LoadFromLUA()
	{
		string[] fileNames = new string[3] { "LUA_BUFF_TEMPLET", "LUA_BUFF_TEMPLET2", "LUA_BUFF_TEMPLET3" };
		NKMTempletContainer<NKMBuffTemplet>.Load("AB_SCRIPT", fileNames, "m_dicNKMBuffTemplet", NKMBuffTemplet.LoadFromLUA, (NKMBuffTemplet e) => e.m_BuffStrID);
	}

	public static NKMBuffTemplet GetBuffTempletByID(short buffID)
	{
		buffID = Math.Abs(buffID);
		return NKMTempletContainer<NKMBuffTemplet>.Find(buffID);
	}

	public static NKMBuffTemplet GetBuffTempletByStrID(string buffStrID)
	{
		return NKMTempletContainer<NKMBuffTemplet>.Find(buffStrID);
	}
}
