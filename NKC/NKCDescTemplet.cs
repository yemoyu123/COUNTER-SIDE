using NKM;

namespace NKC;

public class NKCDescTemplet
{
	public enum NKC_DESC_TEMPLET_TYPE
	{
		NDTT_RESULT_WIN,
		NDTT_RESULT_LOSE,
		NDTT_GET_UNIT,
		NDTT_SUPER,
		NDTT_RESULT_WIN_LIFE,
		NDTT_RESULT_LOSE_LIFE,
		NDTT_COUNT
	}

	public class NKCDescData
	{
		private string m_Desc;

		public string m_Ani;

		public NKCDescData(string desc, string ani)
		{
			m_Desc = desc;
			m_Ani = ani;
		}

		public string GetDesc()
		{
			return NKCStringTable.GetString(m_Desc);
		}
	}

	public int m_UnitID;

	public int m_SkinID;

	public NKCDescData[] m_arrDescData = new NKCDescData[6];

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_UnitID", ref m_UnitID);
		cNKMLua.GetData("m_SkinID", ref m_SkinID);
		SetData(cNKMLua, NKC_DESC_TEMPLET_TYPE.NDTT_RESULT_WIN, "m_ResultWinDesc", "m_ResultWinAni");
		SetData(cNKMLua, NKC_DESC_TEMPLET_TYPE.NDTT_RESULT_LOSE, "m_ResultLoseDesc", "m_ResultLoseAni");
		SetData(cNKMLua, NKC_DESC_TEMPLET_TYPE.NDTT_GET_UNIT, "m_GetUnitDesc", "m_GetUnitAni");
		SetData(cNKMLua, NKC_DESC_TEMPLET_TYPE.NDTT_SUPER, "m_SuperDesc", "m_SuperAni");
		SetData(cNKMLua, NKC_DESC_TEMPLET_TYPE.NDTT_RESULT_WIN_LIFE, "m_ResultWinLifeDesc", "m_ResultWinLifeAni");
		SetData(cNKMLua, NKC_DESC_TEMPLET_TYPE.NDTT_RESULT_LOSE_LIFE, "m_ResultLoseLifeDesc", "m_ResultLoseLifeAni");
		return true;
	}

	private void SetData(NKMLua cNKMLua, NKC_DESC_TEMPLET_TYPE type, string descPropName, string aniPropName)
	{
		string rValue = "";
		string rValue2 = "";
		cNKMLua.GetData(descPropName, ref rValue);
		cNKMLua.GetData(aniPropName, ref rValue2);
		m_arrDescData[(int)type] = new NKCDescData(rValue, rValue2);
	}
}
