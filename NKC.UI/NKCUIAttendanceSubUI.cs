using System.Collections;
using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIAttendanceSubUI : MonoBehaviour
{
	private const string BG_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_attendance_texture";

	public const float UI_OPEN_DELAY = 0.2f;

	[Header("프리팹 신규 생성 시 연결 필요")]
	public List<NKCUIAttendanceSlot> m_lstSlot = new List<NKCUIAttendanceSlot>();

	public Image m_NKM_UI_EVENT_DAILYCHECK_MONTH_IMG;

	public GameObject m_AB_FX_UI_EVENT_ICON_SLOT_RECIEVE_FX;

	private NKCUIAttendanceSlot m_slotToday;

	private NKCUIAttendanceSlot m_slotNext;

	private bool m_bInitComplete;

	private void InitUI()
	{
		if (m_lstSlot == null || m_lstSlot.Count == 0)
		{
			Debug.LogError(base.gameObject.name + " - m_lstSlot is null");
		}
		if (m_NKM_UI_EVENT_DAILYCHECK_MONTH_IMG == null)
		{
			Debug.LogError(base.gameObject.name + " - m_NKM_UI_EVENT_DAILYCHECK_MONTH_IMG is null");
		}
		if (m_AB_FX_UI_EVENT_ICON_SLOT_RECIEVE_FX == null)
		{
			Debug.LogError(base.gameObject.name + " - AB_FX_UI_EVENT_ICON_SLOT_RECIEVE_FX is null");
		}
		NKCUtil.SetGameobjectActive(m_AB_FX_UI_EVENT_ICON_SLOT_RECIEVE_FX.gameObject, bValue: false);
		m_bInitComplete = true;
	}

	public void SetData(NKMAttendanceTabTemplet tabTemplet)
	{
		if (!m_bInitComplete)
		{
			InitUI();
		}
		m_slotToday = null;
		m_slotNext = null;
		if (tabTemplet == null)
		{
			Debug.LogError("tabTemplet is null");
			return;
		}
		NKMAttendance nKMAttendance = NKCScenManager.CurrentUserData().m_AttendanceData.AttList.Find((NKMAttendance x) => x.IDX == tabTemplet.IDX);
		if (nKMAttendance == null)
		{
			Debug.LogError($"attendance is null - key : {tabTemplet.IDX}");
			return;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		for (int num = 0; num < m_lstSlot.Count; num++)
		{
			if (num < tabTemplet.RewardTemplets.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstSlot[num], bValue: true);
				int num2 = num + 1;
				bool bShowCheckMark = num2 <= nKMAttendance.Count;
				bool bShowCheckNext = num2 == nKMAttendance.Count + 1;
				m_lstSlot[num].SetData(tabTemplet.RewardTemplets[num2], bShowCheckMark, bShowCheckNext);
				if (num2 == nKMAttendance.Count)
				{
					m_slotToday = m_lstSlot[num];
				}
				else if (num2 == nKMAttendance.Count + 1)
				{
					m_slotNext = m_lstSlot[num];
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstSlot[num], bValue: false);
			}
		}
		m_NKM_UI_EVENT_DAILYCHECK_MONTH_IMG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_attendance_texture", tabTemplet.BackgroundImage);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public IEnumerator ProcessSubUI(bool bShowSlotAnimation)
	{
		if (!m_bInitComplete)
		{
			InitUI();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		yield return new WaitForSeconds(0.2f);
		if (m_slotToday != null)
		{
			m_slotToday.ShowInduceAnimation(bShow: true);
		}
		if (bShowSlotAnimation)
		{
			yield return ShowSlotCheckAnimation();
		}
		if (m_slotNext != null)
		{
			yield return m_slotNext.ShowNextMark(bShowSlotAnimation);
		}
		yield return null;
	}

	private IEnumerator ShowSlotCheckAnimation()
	{
		if (m_slotToday == null)
		{
			Debug.LogWarning("아이템 획득 연출이 필요한데 대상 슬롯이 없음 - SetData 체크");
			yield break;
		}
		yield return m_slotToday.ShowSlotCheckMarkAnimation(m_AB_FX_UI_EVENT_ICON_SLOT_RECIEVE_FX);
		NKCUtil.SetGameobjectActive(m_AB_FX_UI_EVENT_ICON_SLOT_RECIEVE_FX, bValue: false);
	}
}
