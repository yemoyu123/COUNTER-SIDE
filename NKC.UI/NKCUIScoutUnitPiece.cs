using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIScoutUnitPiece : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public Image m_imgUnit;

	public Text m_lbCount;

	public Slider m_slCount;

	private int m_miscItemID;

	public void SetData(NKMPieceTemplet templet, int scoutCount = 1)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		bool num = nKMUserData.m_ArmyData.IsCollectedUnit(templet.m_PieceGetUintId);
		long num2 = (num ? templet.m_PieceReq : templet.m_PieceReqFirst);
		if (scoutCount < 1)
		{
			scoutCount = 1;
		}
		num2 = ((!num) ? (templet.m_PieceReq * (scoutCount - 1) + templet.m_PieceReqFirst) : (num2 * scoutCount));
		long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(templet.m_PieceId);
		NKMItemMiscTemplet itemMiscTemplet = ((templet == null) ? null : NKMItemManager.GetItemMiscTempletByID(templet.m_PieceId));
		NKCUtil.SetImageSprite(m_imgUnit, NKCResourceUtility.GetOrLoadMiscItemIcon(itemMiscTemplet));
		if (countMiscItem < num2)
		{
			NKCUtil.SetLabelText(m_lbCount, $"<color=#ff0000>{countMiscItem}</color>/{num2}");
		}
		else
		{
			NKCUtil.SetLabelText(m_lbCount, $"{countMiscItem}/{num2}");
		}
		if (m_slCount != null)
		{
			m_slCount.minValue = 0f;
			m_slCount.maxValue = num2;
			if (countMiscItem >= num2)
			{
				m_slCount.normalizedValue = 1f;
			}
			else
			{
				m_slCount.value = countMiscItem;
			}
		}
		m_miscItemID = templet.m_PieceId;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (m_miscItemID != 0)
		{
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(m_miscItemID, 1L);
			NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, data);
		}
	}
}
