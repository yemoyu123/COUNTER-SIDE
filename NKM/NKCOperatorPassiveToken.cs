namespace NKM;

public class NKCOperatorPassiveToken
{
	public NKM_ITEM_GRADE m_itemGrade;

	public int m_itemID;

	public long m_itemCount;

	public int m_passiveSkillID;

	public NKM_ITEM_GRADE ItemGrade => m_itemGrade;

	public int ItemID => m_itemID;

	public long ItemCount => m_itemCount;

	public int PassiveSkillID => m_passiveSkillID;

	public NKCOperatorPassiveToken(NKM_ITEM_GRADE itemGrade, int itemID, long itemCnt, int passiveSkillID)
	{
		m_itemGrade = itemGrade;
		m_itemID = itemID;
		m_itemCount = itemCnt;
		m_passiveSkillID = passiveSkillID;
	}
}
