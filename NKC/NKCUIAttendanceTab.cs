using System;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIAttendanceTab : MonoBehaviour
{
	public delegate void OnClickEvent(int tabID);

	private const string ICON_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_attendance_sprite";

	public Text m_lbTitleOn;

	public Text m_lbSubTitleOn;

	public Image m_imgIconOn;

	public Text m_lbRemainDaysOn;

	public Text m_lbTitleOff;

	public Text m_lbSubTitleOff;

	public Image m_imgIconOff;

	public Text m_lbRemainDaysOff;

	public NKCUIComStateButton m_btn;

	public NKCUIComToggle m_tgl;

	public GameObject m_objRedDot;

	private OnClickEvent dOnClick;

	private bool bInitComplete;

	private int m_tabIDX;

	private float m_fDeltaTime;

	private void InitUI()
	{
		m_btn?.PointerDown.RemoveAllListeners();
		m_btn?.PointerDown.AddListener(delegate
		{
			OnBtnClick();
		});
		bInitComplete = true;
	}

	public void SetData(NKMAttendanceTabTemplet tabInfo, NKCUIComToggleGroup toggleGroup, bool bEnableRedDot, OnClickEvent onClick)
	{
		if (!bInitComplete)
		{
			InitUI();
		}
		m_fDeltaTime = 0f;
		m_tabIDX = tabInfo.IDX;
		m_lbTitleOn.text = tabInfo.GetTabNameMain();
		m_lbSubTitleOn.text = tabInfo.GetTabNameSub();
		m_imgIconOn.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_attendance_sprite", tabInfo.TabIconName);
		m_lbTitleOff.text = tabInfo.GetTabNameMain();
		m_lbSubTitleOff.text = tabInfo.GetTabNameSub();
		m_imgIconOff.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_attendance_sprite", tabInfo.TabIconName);
		m_tgl.SetToggleGroup(toggleGroup);
		NKCUtil.SetGameobjectActive(m_objRedDot, bEnableRedDot);
		SetRemainTime();
		if (onClick != null)
		{
			dOnClick = onClick;
		}
	}

	private void SetRemainTime()
	{
		DateTime eventEndDate = NKCScenManager.CurrentUserData().m_AttendanceData.AttList.Find((NKMAttendance x) => x.IDX == m_tabIDX).EventEndDate;
		NKCUtil.SetLabelText(m_lbRemainDaysOn, string.Format(NKCUtilString.GET_STRING_REMAIN_TIME_LEFT_ONE_PARAM, NKCUtilString.GetRemainTimeString(eventEndDate, 1)));
		NKCUtil.SetLabelText(m_lbRemainDaysOff, string.Format(NKCUtilString.GET_STRING_REMAIN_TIME_LEFT_ONE_PARAM, NKCUtilString.GetRemainTimeString(eventEndDate, 1)));
	}

	public void Select(bool bSelect)
	{
		m_tgl.Select(bSelect, bForce: true, bImmediate: true);
		if (bSelect)
		{
			NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		}
	}

	private void OnBtnClick()
	{
		if (dOnClick != null)
		{
			dOnClick(m_tabIDX);
		}
	}

	public int GetTabIDX()
	{
		return m_tabIDX;
	}

	private void Update()
	{
		if (base.gameObject.activeSelf && m_tabIDX > 0)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				SetRemainTime();
			}
		}
	}
}
