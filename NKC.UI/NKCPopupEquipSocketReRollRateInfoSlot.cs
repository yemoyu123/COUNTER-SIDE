using UnityEngine;

namespace NKC.UI;

public class NKCPopupEquipSocketReRollRateInfoSlot : MonoBehaviour
{
	public GameObject m_objStatNum_1;

	public NKCComText m_lbStat_1;

	public GameObject m_objStat_2;

	public NKCComText m_lbStat_2;

	public void SetData(string stat_1, string stat_2)
	{
		bool bValue = !string.IsNullOrEmpty(stat_2);
		NKCUtil.SetGameobjectActive(m_objStatNum_1, bValue);
		NKCUtil.SetGameobjectActive(m_objStat_2, bValue);
		NKCUtil.SetLabelText(m_lbStat_1, stat_1);
		NKCUtil.SetLabelText(m_lbStat_2, stat_2);
	}
}
