using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Trim;

public class NKCUITrimSlot : MonoBehaviour
{
	public enum SlotState
	{
		Default,
		Selected,
		Disable,
		Locked
	}

	public delegate void OnClick(int slotIndex, int trimId);

	public NKCUIComStateButton m_csbtnButton;

	public Animator m_animator;

	public Text m_lbTrimName;

	public Text m_lbTrimLevel;

	[Header("\ufffd\ufffdƲ \ufffd\ufffd\ufffd\ufffd\ufffd")]
	public Transform m_battleCondParent;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\u05b4ϸ\ufffd\ufffd\u033c\ufffd \ufffd\u0338\ufffd")]
	public string m_baseAnimation;

	public string m_selectAnimation;

	public string m_disableAnimation;

	public string m_lockedAnimation;

	[Space]
	public GameObject m_objIntervalTime;

	public GameObject m_objEventDrop;

	private OnClick m_dOnClick;

	private int m_iSlotIndex;

	private int m_trimId;

	private SlotState m_eSlotState;

	public SlotState TrimSlotState => m_eSlotState;

	public void Init(int slotIndex, OnClick onClick)
	{
		m_iSlotIndex = slotIndex;
		m_dOnClick = onClick;
		NKCUtil.SetButtonClickDelegate(m_csbtnButton, OnClickSlot);
		NKCUITrimUtility.InitBattleCondition(m_battleCondParent, showToolTip: false);
		m_animator.keepAnimatorControllerStateOnDisable = true;
	}

	public void SetData(int trimId)
	{
		m_trimId = trimId;
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(trimId);
		NKMUserData userData = NKCScenManager.CurrentUserData();
		string text = null;
		int trimLevel = 0;
		if (nKMTrimTemplet != null)
		{
			text = NKCStringTable.GetString(nKMTrimTemplet.TirmGroupName);
			int clearedTrimLevel = NKCUITrimUtility.GetClearedTrimLevel(userData, trimId);
			trimLevel = Mathf.Min(nKMTrimTemplet.MaxTrimLevel, clearedTrimLevel + 1);
		}
		else
		{
			text = " - ";
		}
		NKCUtil.SetGameobjectActive(m_objIntervalTime, nKMTrimTemplet?.ShowInterval ?? false);
		NKCUtil.SetLabelText(m_lbTrimName, text);
		NKCUtil.SetLabelText(m_lbTrimLevel, trimLevel.ToString());
		NKCUITrimUtility.SetBattleCondition(m_battleCondParent, nKMTrimTemplet, trimLevel, showToolTip: false);
		NKCUtil.SetGameobjectActive(m_objEventDrop, NKCUITrimUtility.HaveEventDrop(trimId));
	}

	public void SetSlotState(SlotState slotState)
	{
		m_eSlotState = slotState;
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(m_trimId);
		NKMUserData cNKMUserData = NKCScenManager.CurrentUserData();
		if (nKMTrimTemplet == null || !NKMContentUnlockManager.IsContentUnlocked(cNKMUserData, in nKMTrimTemplet.m_UnlockInfo))
		{
			m_eSlotState = SlotState.Locked;
		}
		switch (m_eSlotState)
		{
		case SlotState.Default:
			m_animator?.Play(m_baseAnimation);
			break;
		case SlotState.Selected:
			m_animator?.Play(m_selectAnimation);
			break;
		case SlotState.Disable:
			m_animator?.Play(m_disableAnimation);
			break;
		case SlotState.Locked:
			m_animator?.Play(m_lockedAnimation);
			break;
		}
	}

	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public void LetChangeClickState(int trimId)
	{
		if (m_trimId == trimId)
		{
			OnClickSlot();
		}
	}

	private void OnClickSlot()
	{
		if (m_dOnClick != null)
		{
			m_dOnClick(m_iSlotIndex, m_trimId);
		}
	}

	private void OnDestroy()
	{
		m_dOnClick = null;
	}
}
