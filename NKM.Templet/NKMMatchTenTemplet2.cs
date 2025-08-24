using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMMatchTenTemplet2 : NKMMiniGameTemplet
{
	public int m_PlayTimeSec;

	public int m_PlayScoreMid;

	public int m_PlayScoreHigh;

	public int m_BoardSizeX;

	public int m_BoardSizeY;

	public int m_PerfectScoreValue => m_BoardSizeX * m_BoardSizeY;

	public new static IEnumerable<NKMMatchTenTemplet2> Values => NKMTempletContainer<NKMMatchTenTemplet2>.Values;

	public new static NKMMatchTenTemplet2 Find(int key)
	{
		return NKMTempletContainer<NKMMatchTenTemplet2>.Find((NKMMatchTenTemplet2 x) => x.m_Id == key);
	}

	public new static NKMMatchTenTemplet2 LoadFromLua(NKMLua lua)
	{
		NKMMatchTenTemplet2 nKMMatchTenTemplet = new NKMMatchTenTemplet2();
		if (!nKMMatchTenTemplet.Load(lua))
		{
			return null;
		}
		return nKMMatchTenTemplet;
	}

	protected override bool Load(NKMLua lua)
	{
		if (!base.Load(lua))
		{
			return false;
		}
		lua.GetData("m_PlayScoreMid", ref m_PlayScoreMid);
		lua.GetData("m_PlayScoreHigh", ref m_PlayScoreHigh);
		return (byte)(1u & (lua.GetData("m_BoardSizeX", ref m_BoardSizeX) ? 1u : 0u) & (lua.GetData("m_BoardSizeY", ref m_BoardSizeY) ? 1u : 0u)) != 0;
	}
}
