using UnityEngine;

namespace NKC.UI;

public class NKCUILeaderBoardSlotTop3 : MonoBehaviour
{
	public NKCUILeaderBoardSlot m_Top1;

	public NKCUILeaderBoardSlot m_Top2;

	public NKCUILeaderBoardSlot m_Top3;

	public void InitUI()
	{
		m_Top1?.InitUI();
		m_Top2?.InitUI();
		m_Top3?.InitUI();
	}

	public void SetData(LeaderBoardSlotData data1, LeaderBoardSlotData data2, LeaderBoardSlotData data3, int criteria, NKCUILeaderBoardSlot.OnDragBegin onDragBegin)
	{
		m_Top1.SetData(data1, criteria, onDragBegin);
		m_Top2.SetData(data2, criteria, onDragBegin);
		m_Top3.SetData(data3, criteria, onDragBegin);
	}
}
