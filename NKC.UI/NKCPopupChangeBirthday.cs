using System.Collections;
using NKC.UI.Component;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupChangeBirthday : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_user_info";

	private const string UI_ASSET_NAME = "NKM_UI_USER_INFO_POPUP_BIRTHDAY";

	private static NKCPopupChangeBirthday m_Instance;

	public NKCPopupChangeBirthdaySlot m_pfbSlot;

	public ScrollRect m_srMonth;

	public NKCUIComSnapScroll m_snapMonth;

	public ScrollRect m_srDay;

	public NKCUIComSnapScroll m_snapDay;

	public NKCUIComStateButton m_btnOK;

	public NKCUIComStateButton m_btnCancel;

	private int m_Month;

	private int m_Day;

	private const int MONTH_COUNT = 12;

	private const int DAY_COUNT = 31;

	public static NKCPopupChangeBirthday Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupChangeBirthday>("ab_ui_nkm_ui_user_info", "NKM_UI_USER_INFO_POPUP_BIRTHDAY", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupChangeBirthday>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void Init()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_btnOK.PointerClick.RemoveAllListeners();
		m_btnOK.PointerClick.AddListener(OnClickOK);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(base.Close);
		for (int i = 0; i < 12; i++)
		{
			NKCPopupChangeBirthdaySlot nKCPopupChangeBirthdaySlot = null;
			nKCPopupChangeBirthdaySlot = ((i >= m_srMonth.content.childCount) ? Object.Instantiate(m_pfbSlot, m_srMonth.content) : m_srMonth.content.GetChild(i).GetComponent<NKCPopupChangeBirthdaySlot>());
			nKCPopupChangeBirthdaySlot.SetData(i + 1);
		}
		for (int j = 0; j < 31; j++)
		{
			NKCPopupChangeBirthdaySlot nKCPopupChangeBirthdaySlot2 = null;
			nKCPopupChangeBirthdaySlot2 = ((j >= m_srDay.content.childCount) ? Object.Instantiate(m_pfbSlot, m_srDay.content) : m_srDay.content.GetChild(j).GetComponent<NKCPopupChangeBirthdaySlot>());
			nKCPopupChangeBirthdaySlot2.SetData(j + 1);
		}
		m_snapMonth.dOnCurrentSlotChanged = OnMonthChanged;
		m_snapDay.dOnCurrentSlotChanged = OnDayChanged;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(int month = 1, int day = 1)
	{
		m_Month = month;
		m_Day = day;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UIOpened();
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform.GetComponent<RectTransform>());
		StartCoroutine(DelayLayout());
	}

	private IEnumerator DelayLayout()
	{
		yield return new WaitForEndOfFrame();
		SetScrollPosition(m_Month, m_Day);
		yield return null;
	}

	private void SetScrollPosition(int month, int day)
	{
		m_snapMonth.ScrollToSnapIndex(month - 1, 0.1f);
		m_snapDay.ScrollToSnapIndex(day - 1, 0.1f);
	}

	private void OnMonthChanged(RectTransform rt)
	{
		NKCPopupChangeBirthdaySlot component = rt.GetComponent<NKCPopupChangeBirthdaySlot>();
		m_Month = component.GetData();
		m_Day = 1;
		SetDayCount(m_Month);
		m_snapDay.ScrollToSnapIndex(0, 0.1f);
	}

	private void OnDayChanged(RectTransform rt)
	{
		NKCPopupChangeBirthdaySlot component = rt.GetComponent<NKCPopupChangeBirthdaySlot>();
		m_Day = component.GetData();
		if (m_Day > GetMaxDayCount(m_Month))
		{
			m_Day = GetMaxDayCount(m_Month);
		}
	}

	private void SetDayCount(int month)
	{
		for (int i = 0; i < 31; i++)
		{
			NKCUtil.SetGameobjectActive(m_srDay.content.GetChild(i).gameObject, i < GetMaxDayCount(month));
		}
		if (m_Day > GetMaxDayCount(month))
		{
			m_Day = 1;
			m_snapDay.ScrollToSnapIndex(m_Day - 1, 0.1f);
		}
	}

	private int GetMaxDayCount(int month)
	{
		switch (month)
		{
		default:
			return 31;
		case 4:
		case 6:
		case 9:
		case 11:
			return 30;
		case 2:
			return 29;
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnClickOK()
	{
		BirthDayDate birthDay = new BirthDayDate(m_Month, m_Day);
		string monthString = NKCUtilString.GetMonthString(m_Month);
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCUtilString.GET_STRING_PROFILE_BIRTHDAY_POPUP_DESC, monthString, m_Day), delegate
		{
			NKCPacketSender.Send_NKMPacket_UPDATE_BIRTHDAY_REQ(birthDay);
			Close();
		});
	}
}
