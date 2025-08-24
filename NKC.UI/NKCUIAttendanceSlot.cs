using System.Collections;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIAttendanceSlot : MonoBehaviour
{
	public NKCUISlot m_NKCUISlot;

	public Animator m_aniReceive;

	public GameObject m_objInduce;

	public Text m_NKM_UI_EVENT_ICON_SLOT_DAY_TEXT;

	public GameObject m_NKM_UI_EVENT_ICON_SLOT_CHECK;

	public GameObject m_objNextMark;

	private bool bInitComplete;

	private void Init()
	{
		m_NKCUISlot?.Init();
		m_aniReceive.enabled = false;
		bInitComplete = true;
	}

	public void SetData(NKMAttendanceRewardTemplet rewardTemplet, bool bShowCheckMark, bool bShowCheckNext)
	{
		if (!bInitComplete)
		{
			Init();
		}
		NKCUtil.SetGameobjectActive(m_aniReceive.gameObject, bValue: true);
		m_aniReceive.enabled = false;
		m_NKCUISlot.SetData(NKCUISlot.SlotData.MakeRewardTypeData(rewardTemplet.RewardType, rewardTemplet.RewardID, rewardTemplet.RewardValue));
		m_NKM_UI_EVENT_ICON_SLOT_DAY_TEXT.text = rewardTemplet.LoginDate.ToString();
		NKCUtil.SetGameobjectActive(m_NKM_UI_EVENT_ICON_SLOT_CHECK, bShowCheckMark);
		NKCUtil.SetGameobjectActive(m_objNextMark, bValue: false);
		m_aniReceive.enabled = false;
		NKCUtil.SetGameobjectActive(m_objInduce, bValue: false);
	}

	public IEnumerator ShowSlotCheckMarkAnimation(GameObject effectGo)
	{
		yield return null;
		NKCUtil.SetGameobjectActive(m_aniReceive.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_EVENT_ICON_SLOT_CHECK, bValue: true);
		m_aniReceive.enabled = true;
		m_aniReceive.Play("NKM_UI_ATTENDANCE_ICON_SLOT_ITEM_RECIEVED");
		if (effectGo != null)
		{
			effectGo.transform.position = base.transform.position;
		}
		NKCUtil.SetGameobjectActive(effectGo, bValue: true);
		NKCSoundManager.PlaySound("FX_UI_ATTENDANCE_CHECK", 1f, 0f, 0f);
		while (m_aniReceive.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			yield return null;
		}
		NKCUtil.SetGameobjectActive(effectGo, bValue: false);
		m_aniReceive.enabled = false;
	}

	public void ShowInduceAnimation(bool bShow)
	{
		NKCUtil.SetGameobjectActive(m_objInduce, bShow);
	}

	public IEnumerator ShowNextMark(bool bPlayAnimation)
	{
		yield return null;
		NKCUtil.SetGameobjectActive(m_objNextMark, bValue: true);
		if (bPlayAnimation)
		{
			m_aniReceive.enabled = true;
			m_aniReceive.Play("NKM_UI_ATTENDANCE_ICON_SLOT_ITEM_NEXT");
			if (m_objNextMark != null)
			{
				m_objNextMark.transform.position = base.transform.position;
			}
			NKCUtil.SetGameobjectActive(m_objNextMark, bValue: true);
			while (m_aniReceive.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
			{
				yield return null;
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objNextMark, bValue: true);
		}
		m_aniReceive.enabled = false;
	}
}
