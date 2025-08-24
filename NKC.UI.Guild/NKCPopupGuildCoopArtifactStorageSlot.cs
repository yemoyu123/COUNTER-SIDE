using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildCoopArtifactStorageSlot : MonoBehaviour
{
	[Header("일반 슬롯")]
	public GameObject m_objNormalSlot;

	public NKCUISlot m_slot;

	public Text m_lbName;

	public Text m_lbDesc;

	public bool m_bIsArenaNum;

	[Header("텍스트 슬롯")]
	public GameObject m_objTextSlot;

	public Text m_lbArenaNum;

	public void Init()
	{
		if (m_slot != null)
		{
			m_slot.Init();
			m_slot.SetUseBigImg(bSet: false);
		}
	}

	public void SetData(NKCUIComGuildArtifactContent.ArtifactSlotData slotData)
	{
		if (!slotData.bIsArenaNum && slotData.id == 0)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_bIsArenaNum = slotData.bIsArenaNum;
		NKCUtil.SetGameobjectActive(m_objNormalSlot, !m_bIsArenaNum);
		NKCUtil.SetGameobjectActive(m_objTextSlot, m_bIsArenaNum);
		if (m_bIsArenaNum)
		{
			NKCUtil.SetLabelText(m_lbArenaNum, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_DUNGEON_UI_ARENA_INFO, slotData.id));
			return;
		}
		if (m_slot != null)
		{
			NKCUtil.SetGameobjectActive(m_slot, bValue: true);
			m_slot.SetData(NKCUISlot.SlotData.MakeGuildArtifactData(slotData.id));
		}
		NKCUtil.SetLabelText(m_lbName, slotData.name);
		NKCUtil.SetLabelText(m_lbDesc, slotData.desc);
	}
}
