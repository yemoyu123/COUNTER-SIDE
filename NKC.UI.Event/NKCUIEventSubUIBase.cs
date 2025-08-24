using NKM;
using NKM.Event;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NKC.UI.Event;

[RequireComponent(typeof(NKCUIEventSubUI))]
public abstract class NKCUIEventSubUIBase : MonoBehaviour
{
	[Header("이벤트 기간")]
	[FormerlySerializedAs("m_lbTime")]
	[FormerlySerializedAs("m_txtTime")]
	[FormerlySerializedAs("m_lbEventDate")]
	[FormerlySerializedAs("m_lbEventTime")]
	public Text m_lbEventLimitDate;

	[Header("이벤트 숏컷 이동 버튼")]
	public NKCUIComStateButton m_csbtnEventShortcut;

	protected NKMEventTabTemplet m_tabTemplet;

	public virtual void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnEventShortcut, OnMoveShortcut);
	}

	public abstract void Open(NKMEventTabTemplet tabTemplet);

	public abstract void Refresh();

	public virtual void Close()
	{
	}

	public virtual void Hide()
	{
	}

	public virtual void UnHide()
	{
	}

	public virtual bool OnBackButton()
	{
		return false;
	}

	public virtual void OnInventoryChange(NKMItemMiscData itemData)
	{
	}

	protected void OnMoveShortcut()
	{
		if (m_tabTemplet.m_ShortCutType != NKM_SHORTCUT_TYPE.SHORTCUT_NONE && CheckEventTime())
		{
			NKCContentManager.MoveToShortCut(m_tabTemplet.m_ShortCutType, m_tabTemplet.m_ShortCut);
		}
	}

	protected bool CheckEventTime(bool bPopup = true)
	{
		if (m_tabTemplet == null)
		{
			return false;
		}
		if (m_tabTemplet.HasTimeLimit && !m_tabTemplet.IsAvailable)
		{
			if (bPopup)
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_EVENT_END, delegate
				{
					NKCUIEvent.Instance.Close();
				});
			}
			return false;
		}
		return true;
	}

	protected void SetDateLimit()
	{
		if (m_tabTemplet != null && m_tabTemplet.HasDateLimit)
		{
			if (NKCSynchronizedTime.GetTimeLeft(m_tabTemplet.EventDateEndUtc).TotalDays > (double)NKCSynchronizedTime.UNLIMITD_REMAIN_DAYS)
			{
				NKCUtil.SetLabelText(m_lbEventLimitDate, NKCUtilString.GET_STRING_EVENT_DATE_UNLIMITED_TEXT);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbEventLimitDate, NKCUtilString.GetTimeIntervalString(m_tabTemplet.EventDateStart, m_tabTemplet.EventDateEnd, NKMTime.INTERVAL_FROM_UTC));
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEventLimitDate, "");
		}
	}
}
