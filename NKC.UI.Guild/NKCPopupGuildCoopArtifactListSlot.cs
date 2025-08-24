using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildCoopArtifactListSlot : MonoBehaviour
{
	public delegate void OnClickSlot(int slotIndex);

	public NKCUIComStateButton m_btnSlot;

	public Text m_lbOrderNum;

	public Image m_imgGradeBG;

	public GameObject m_objGradeGlow;

	public NKCUISlot m_slot;

	public Text m_lbName;

	public Text m_lbDesc;

	public GameObject m_objClear;

	public GameObject m_objCurrent;

	public GameObject m_objFlag;

	public GameObject m_objFlagSetting;

	private OnClickSlot m_dOnClickSlot;

	private int m_slotIdx;

	public void Init(OnClickSlot dOnClickSlot)
	{
		if (m_slot != null)
		{
			m_slot.Init();
			m_slot.SetUseBigImg(bSet: true);
		}
		m_btnSlot.PointerClick.RemoveAllListeners();
		m_btnSlot.PointerClick.AddListener(OnClickBtn);
		m_dOnClickSlot = dOnClickSlot;
	}

	public void SetData(GuildDungeonArtifactTemplet templet, int slotIdx)
	{
		m_slotIdx = slotIdx;
		NKCUtil.SetLabelText(m_lbOrderNum, templet.GetOrder().ToString());
		NKCUtil.SetImageSprite(m_imgGradeBG, NKCUtil.GetGuildArtifactBgProbImage(templet.GetBgProbImage()));
		NKCUtil.SetGameobjectActive(m_objGradeGlow, templet.GetBgProbImage() == GuildDungeonArtifactTemplet.ArtifactProbType.HIGH);
		m_slot.SetData(NKCUISlot.SlotData.MakeGuildArtifactData(templet.GetArtifactId()));
		NKCUtil.SetLabelText(m_lbName, templet.GetName());
		NKCUtil.SetLabelText(m_lbDesc, templet.GetDescFull());
		NKCUtil.SetGameobjectActive(m_objClear, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCurrent, bValue: false);
		NKCUtil.SetGameobjectActive(m_objFlag, bValue: false);
		NKCUtil.SetGameobjectActive(m_objFlagSetting, bValue: false);
	}

	public void SetClear(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objClear, bValue);
	}

	public void SetCurrent(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objCurrent, bValue);
	}

	public void SetFlag(bool bValue, bool bIsSetting)
	{
		NKCUtil.SetGameobjectActive(m_objFlag, bValue && !bIsSetting);
		NKCUtil.SetGameobjectActive(m_objFlagSetting, bValue && bIsSetting);
	}

	private void OnClickBtn()
	{
		m_dOnClickSlot?.Invoke(m_slotIdx);
	}
}
