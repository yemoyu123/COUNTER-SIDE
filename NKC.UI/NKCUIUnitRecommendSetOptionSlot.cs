using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitRecommendSetOptionSlot : MonoBehaviour
{
	public Image m_imgSetOption;

	public Text m_lbSetOption;

	public NKCUIComStateButton m_btnDetail;

	public NKCUIComStateButton m_btnEquip;

	private NKCUIUnitRecommendSetOption.OnEquip m_dOnEquip;

	private int m_SetOptionID;

	public void Init(NKCUIUnitRecommendSetOption.OnEquip onEquip)
	{
		m_dOnEquip = onEquip;
		m_btnDetail.PointerClick.RemoveAllListeners();
		m_btnDetail.PointerClick.AddListener(OnClickDetail);
		m_btnEquip.PointerClick.RemoveAllListeners();
		m_btnEquip.PointerClick.AddListener(OnClickEquip);
	}

	public void SetData(int setOptionID)
	{
		m_SetOptionID = setOptionID;
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(setOptionID);
		if (equipSetOptionTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetImageSprite(m_imgSetOption, NKCUtil.GetSpriteEquipSetOptionIcon(equipSetOptionTemplet));
		NKCUtil.SetLabelText(m_lbSetOption, NKCStringTable.GetString(equipSetOptionTemplet.m_EquipSetName));
	}

	private void OnClickDetail()
	{
		if (NKCUIUnitInfo.IsInstanceOpen)
		{
			NKCUIUnitInfo.Instance.OpenRecommendEquipList(m_SetOptionID);
		}
	}

	private void OnClickEquip()
	{
		m_dOnEquip?.Invoke(m_SetOptionID);
	}
}
