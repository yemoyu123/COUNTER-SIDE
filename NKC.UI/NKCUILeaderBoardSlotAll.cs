using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUILeaderBoardSlotAll : MonoBehaviour
{
	public NKCUILeaderBoardSlotTop3 m_SlotTop3Only;

	public NKCUILeaderBoardSlotTop3 m_SlotTop3;

	public NKCUILeaderBoardSlot m_Slot;

	public LayoutElement m_layout;

	public void InitUI()
	{
		m_SlotTop3?.InitUI();
		m_SlotTop3Only?.InitUI();
		m_Slot?.InitUI();
	}

	public void SetData(LeaderBoardSlotData data1, LeaderBoardSlotData data2, LeaderBoardSlotData data3, int criteria, bool bTop3Only, NKCUILeaderBoardSlot.OnDragBegin onDragBegin)
	{
		NKCUtil.SetGameobjectActive(m_SlotTop3Only, bTop3Only);
		NKCUtil.SetGameobjectActive(m_SlotTop3, !bTop3Only);
		NKCUtil.SetGameobjectActive(m_Slot, bValue: false);
		m_layout.preferredHeight = (bTop3Only ? m_SlotTop3Only.GetComponent<LayoutElement>().preferredHeight : m_SlotTop3.GetComponent<LayoutElement>().preferredHeight);
		m_SlotTop3.SetData(data1, data2, data3, criteria, onDragBegin);
	}

	public void SetData(LeaderBoardSlotData data, int criteria, NKCUILeaderBoardSlot.OnDragBegin onDragBegin)
	{
		NKCUtil.SetGameobjectActive(m_SlotTop3Only, bValue: false);
		NKCUtil.SetGameobjectActive(m_SlotTop3, bValue: false);
		NKCUtil.SetGameobjectActive(m_Slot, bValue: true);
		m_layout.preferredHeight = m_Slot.GetComponent<LayoutElement>().preferredHeight;
		m_Slot.SetData(data, criteria, onDragBegin);
	}
}
