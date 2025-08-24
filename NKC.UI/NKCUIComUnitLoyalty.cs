using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComUnitLoyalty : MonoBehaviour
{
	public Text m_lbLoyalty;

	public Text m_lbLoyaltyMax;

	public Text m_lbLoyaltyTitle;

	public GameObject m_objLoyaltyMax;

	public GameObject m_objLoyaltyLifetime;

	public void SetLoyalty(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			NKCUtil.SetLabelText(m_lbLoyalty, "0");
			NKCUtil.SetGameobjectActive(m_objLoyaltyMax, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLoyaltyLifetime, bValue: false);
			NKCUtil.SetLabelText(m_lbLoyaltyTitle, NKCUtilString.GET_STRING_LOYALTY);
			NKCUtil.SetLabelText(m_lbLoyaltyMax, "/{0}", 100);
			return;
		}
		NKCUtil.SetLabelText(m_lbLoyalty, (unitData.loyalty / 100).ToString());
		NKCUtil.SetGameobjectActive(m_objLoyaltyMax, unitData.loyalty >= 10000);
		NKCUtil.SetGameobjectActive(m_objLoyaltyLifetime, unitData.IsPermanentContract);
		if (unitData.IsPermanentContract)
		{
			NKCUtil.SetLabelText(m_lbLoyaltyTitle, NKCUtilString.GET_STRING_LOYALTY_LIFETIME);
			NKCUtil.SetLabelText(m_lbLoyaltyMax, "/{0}", 100);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbLoyaltyTitle, NKCUtilString.GET_STRING_LOYALTY);
			NKCUtil.SetLabelText(m_lbLoyaltyMax, "/{0}", 100);
		}
	}
}
