using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCPopupEquipSetOptionListSlot : MonoBehaviour
{
	public Image m_SET_ICON;

	public Text m_SET_NAME;

	public GameObject m_LIST_01;

	public Text m_STAT_TEXT_01;

	public GameObject m_LIST_02;

	public Text m_STAT_TEXT_02;

	public void SetData(int setOptionID)
	{
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(setOptionID);
		if (equipSetOptionTemplet != null)
		{
			NKCUtil.SetImageSprite(m_SET_ICON, NKCUtil.GetSpriteEquipSetOptionIcon(equipSetOptionTemplet));
			NKCUtil.SetLabelText(m_SET_NAME, $"{NKCStringTable.GetString(equipSetOptionTemplet.m_EquipSetName)} (0/{equipSetOptionTemplet.m_EquipSetPart})");
			NKCUtil.SetGameobjectActive(m_LIST_01, equipSetOptionTemplet.m_StatType_1 != NKM_STAT_TYPE.NST_RANDOM);
			NKCUtil.SetGameobjectActive(m_LIST_02, equipSetOptionTemplet.m_StatType_2 != NKM_STAT_TYPE.NST_RANDOM);
			string setOptionDescription = NKMItemManager.GetSetOptionDescription(equipSetOptionTemplet.m_StatType_1, equipSetOptionTemplet.m_StatValue_1);
			NKCUtil.SetLabelText(m_STAT_TEXT_01, setOptionDescription);
			if (equipSetOptionTemplet.m_StatType_2 != NKM_STAT_TYPE.NST_RANDOM)
			{
				string setOptionDescription2 = NKMItemManager.GetSetOptionDescription(equipSetOptionTemplet.m_StatType_2, equipSetOptionTemplet.m_StatValue_2);
				NKCUtil.SetLabelText(m_STAT_TEXT_02, setOptionDescription2);
			}
		}
	}
}
