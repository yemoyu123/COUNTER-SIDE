using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NKC.UI.Guide;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIAttendance : NKCUIBase
{
	public const string UI_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_attendance";

	public const string UI_ASSET_NAME = "NKM_UI_ATTENDANCE";

	private static NKCUIAttendance m_Instance;

	[Header("Prefabs")]
	public NKCUIAttendanceTab m_pfbTab;

	[Header("TopBar")]
	public Text m_lbTitle;

	public NKCUIComStateButton m_btnClose;

	[Header("Tab")]
	public Transform m_trTabParent;

	public NKCUIComToggleGroup m_tglGroup;

	[Header("SubUI")]
	public Transform m_trSubUIParent;

	public RectTransform m_rtRoot;

	public GameObject m_objBlockLayer;

	public NKCUIComStateButton m_btnBG;

	public NKCUIComStateButton m_HELP_BUTTON;

	public Text m_lbAttendanceDuration;

	public GameObject m_objEmpty;

	private Dictionary<int, NKCUIAttendanceTab> m_dicTab = new Dictionary<int, NKCUIAttendanceTab>();

	private Dictionary<int, NKCUIAttendanceSubUI> m_dicSubUI = new Dictionary<int, NKCUIAttendanceSubUI>();

	private static List<NKCAssetInstanceData> m_listNKCAssetResourceData = new List<NKCAssetInstanceData>();

	private DateTime m_tNextResetTime;

	private NKCUIAttendanceSubUI m_currentSubUI;

	private bool m_bInitComplete;

	private bool m_bNeedAttendanceAniTab;

	private bool m_bSetDataComplete;

	private bool m_bOpenNewsPopup;

	private List<int> m_lstNeedAttendanceKey = new List<int>();

	private Action m_NewsCallback;

	private bool m_bFirstOpen = true;

	public static NKCUIAttendance Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIAttendance>("ab_ui_nkm_ui_attendance", "NKM_UI_ATTENDANCE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIAttendance>();
				m_Instance.InitUI();
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

	public override string MenuName => NKCUtilString.GET_STRING_ATTENDANCE;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		NKCUtil.SetLabelText(m_lbTitle, MenuName);
		m_btnClose?.PointerDown.RemoveAllListeners();
		m_btnClose?.PointerDown.AddListener(delegate
		{
			Close();
		});
		NKCUtil.SetGameobjectActive(m_objBlockLayer, bValue: false);
		m_btnBG?.PointerUp.RemoveAllListeners();
		m_btnBG?.PointerUp.AddListener(base.Close);
		m_currentSubUI = null;
		m_bSetDataComplete = false;
		m_bInitComplete = true;
		NKCUtil.SetBindFunction(m_HELP_BUTTON, OnClickHelp);
	}

	private void OnClickHelp()
	{
		NKCUIPopUpGuide.Instance.Open("ARTICLE_SYSTEM_ATTENDANCE");
	}

	public override void Hide()
	{
		m_bHide = true;
		m_rtRoot.localScale = Vector3.zero;
	}

	public override void UnHide()
	{
		m_bHide = false;
		m_rtRoot.localScale = Vector3.one;
	}

	public override void CloseInternal()
	{
		StopAllCoroutines();
		NKCUtil.SetGameobjectActive(m_currentSubUI, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBlockLayer, bValue: false);
		if (m_bOpenNewsPopup)
		{
			NKCUINews.Instance.SetDataAndOpen(bForceRefresh: true);
			NKCUINews.Instance.SetCloseCallback(m_NewsCallback);
		}
		else
		{
			m_NewsCallback?.Invoke();
		}
		m_NewsCallback = null;
		m_bFirstOpen = true;
	}

	public override void OnCloseInstance()
	{
		for (int i = 0; i < m_listNKCAssetResourceData.Count; i++)
		{
			NKCAssetResourceManager.CloseInstance(m_listNKCAssetResourceData[i]);
		}
		m_listNKCAssetResourceData.Clear();
	}

	public void Open(List<int> lstNewAttendanceKey)
	{
		if (!m_bInitComplete)
		{
			InitUI();
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		m_tNextResetTime = NKMTime.GetNextResetTime(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Day).AddSeconds(1.0);
		NKMAttendanceManager.ResetNeedAttendanceKey();
		NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
		if (nKMUserData != null && nKMUserData.m_AttendanceData.AttList.Count == 0)
		{
			Debug.LogError("현재 날짜/시간에 맞는 출석체크 데이터가 없음");
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: true);
			NKCUtil.SetLabelText(m_lbAttendanceDuration, "");
			UIOpened();
			return;
		}
		if (lstNewAttendanceKey == null)
		{
			lstNewAttendanceKey = new List<int>();
		}
		m_lstNeedAttendanceKey = lstNewAttendanceKey;
		m_bNeedAttendanceAniTab = m_lstNeedAttendanceKey.Count > 0;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetData();
	}

	private void SetData()
	{
		NKMAttendanceData attendanceData = NKCScenManager.CurrentUserData().m_AttendanceData;
		m_currentSubUI = null;
		if (NKCNewsManager.CheckNeedNewsPopup(NKCSynchronizedTime.GetServerUTCTime()))
		{
			m_bOpenNewsPopup = true;
		}
		else
		{
			m_bOpenNewsPopup = false;
		}
		if (!m_bSetDataComplete || m_bNeedAttendanceAniTab)
		{
			ClearPrefabs();
			DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
			for (int i = 0; i < attendanceData.AttList.Count; i++)
			{
				NKMAttendanceTabTemplet attendanceTabTamplet = NKMAttendanceManager.GetAttendanceTabTamplet(attendanceData.AttList[i].IDX);
				if (attendanceTabTamplet == null)
				{
					Debug.LogWarning($"tabTemplet is null - key : {attendanceData.AttList[i].IDX}");
				}
				else
				{
					if ((NKMContentsVersionManager.HasDFChangeTagType(DataFormatChangeTagType.OPEN_TAG_ATTENDANCE) && !attendanceTabTamplet.EnableByTag) || attendanceData.AttList[i].EventEndDate < serverUTCTime || attendanceData.AttList[i].Count == 0 || m_dicTab.ContainsKey(attendanceTabTamplet.IDX))
					{
						continue;
					}
					NKCUIAttendanceTab component = UnityEngine.Object.Instantiate(m_pfbTab, m_trTabParent).GetComponent<NKCUIAttendanceTab>();
					if (component == null)
					{
						Debug.LogWarning("tabUI is null");
						continue;
					}
					component.SetData(attendanceTabTamplet, m_tglGroup, m_lstNeedAttendanceKey.Contains(attendanceTabTamplet.IDX), OnClickTabUI);
					m_dicTab.Add(component.GetTabIDX(), component);
					NKCUIAttendanceSubUI nKCUIAttendanceSubUI = OpenInstanceByAssetName<NKCUIAttendanceSubUI>("ab_ui_nkm_ui_attendance", attendanceTabTamplet.PrefabName, m_trSubUIParent);
					if (nKCUIAttendanceSubUI == null)
					{
						Debug.LogWarning("subUI is null");
						continue;
					}
					nKCUIAttendanceSubUI.SetData(attendanceTabTamplet);
					m_dicSubUI.Add(component.GetTabIDX(), nKCUIAttendanceSubUI);
				}
			}
			List<NKCUIAttendanceTab> list = m_dicTab.Values.ToList();
			list.Sort(CompTab);
			for (int j = 0; j < list.Count; j++)
			{
				list[j].transform.SetAsLastSibling();
			}
			m_bSetDataComplete = true;
		}
		if (m_bFirstOpen)
		{
			m_bFirstOpen = false;
			UIOpened();
		}
		OnClickTabUI(GetFirstTabID());
	}

	private int CompTab(NKCUIAttendanceTab rItem, NKCUIAttendanceTab lItem)
	{
		NKMAttendanceTabTemplet attendanceTabTamplet = NKMAttendanceManager.GetAttendanceTabTamplet(rItem.GetTabIDX());
		NKMAttendanceTabTemplet attendanceTabTamplet2 = NKMAttendanceManager.GetAttendanceTabTamplet(lItem.GetTabIDX());
		if (attendanceTabTamplet.TabID == attendanceTabTamplet2.TabID)
		{
			return rItem.GetTabIDX().CompareTo(lItem.GetTabIDX());
		}
		return attendanceTabTamplet.TabID.CompareTo(attendanceTabTamplet2.TabID);
	}

	private IEnumerator Process()
	{
		NKCUtil.SetGameobjectActive(m_objBlockLayer, bValue: true);
		if (m_bNeedAttendanceAniTab)
		{
			m_bNeedAttendanceAniTab = false;
			List<NKCUIAttendanceTab> lstTab = m_dicTab.Values.ToList();
			lstTab.Sort(CompTab);
			for (int i = 0; i < lstTab.Count; i++)
			{
				bool flag = m_lstNeedAttendanceKey.Contains(lstTab[i].GetTabIDX());
				if (flag)
				{
					NKCUtil.SetGameobjectActive(m_currentSubUI, bValue: false);
					lstTab[i].Select(bSelect: true);
					m_currentSubUI = m_dicSubUI[lstTab[i].GetTabIDX()];
					SetAttendanceDuration(lstTab[i].GetTabIDX());
					yield return m_currentSubUI.ProcessSubUI(flag);
					m_lstNeedAttendanceKey.Remove(lstTab[i].GetTabIDX());
				}
			}
		}
		else
		{
			yield return m_currentSubUI.ProcessSubUI(bShowSlotAnimation: false);
		}
		NKCUtil.SetGameobjectActive(m_objBlockLayer, bValue: false);
	}

	private void SetAttendanceDuration(int tabID)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null || !m_dicTab.ContainsKey(tabID) || !m_dicSubUI.ContainsKey(tabID))
		{
			NKCUtil.SetGameobjectActive(m_lbAttendanceDuration, bValue: false);
			return;
		}
		NKMAttendanceTabTemplet tabTemplet = NKMAttendanceManager.GetAttendanceTabTamplet(m_dicTab[tabID].GetTabIDX());
		DateTime utcTime = default(DateTime);
		DateTime eventEndDate = nKMUserData.m_AttendanceData.AttList.Find((NKMAttendance x) => x.IDX == tabTemplet.IDX).EventEndDate;
		switch (tabTemplet.EventType)
		{
		case NKM_ATTENDANCE_EVENT_TYPE.NEW:
			utcTime = new DateTime(Math.Max(tabTemplet.StartDateUtc.Ticks, nKMUserData.m_NKMUserDateData.m_RegisterTime.Ticks));
			break;
		case NKM_ATTENDANCE_EVENT_TYPE.RETURN:
			utcTime = eventEndDate.AddDays(-tabTemplet.LimitDayCount);
			break;
		case NKM_ATTENDANCE_EVENT_TYPE.NORMAL:
			utcTime = tabTemplet.StartDateUtc;
			break;
		}
		NKCUtil.SetLabelText(m_lbAttendanceDuration, NKCUtilString.GetTimeIntervalString(NKMTime.UTCtoLocal(utcTime), NKMTime.UTCtoLocal(eventEndDate), NKMTime.INTERVAL_FROM_UTC));
	}

	private void OnClickTabUI(int tabID)
	{
		if (m_dicSubUI.ContainsKey(tabID) && !(m_currentSubUI == m_dicSubUI[tabID]))
		{
			if (m_currentSubUI != null)
			{
				NKCUtil.SetGameobjectActive(m_currentSubUI, bValue: false);
			}
			m_dicTab[tabID].Select(bSelect: true);
			m_currentSubUI = m_dicSubUI[tabID];
			StopAllCoroutines();
			StartCoroutine(Process());
			SetAttendanceDuration(tabID);
		}
	}

	private void ClearPrefabs()
	{
		foreach (KeyValuePair<int, NKCUIAttendanceTab> item in m_dicTab)
		{
			UnityEngine.Object.Destroy(item.Value.gameObject);
		}
		m_dicTab.Clear();
		foreach (KeyValuePair<int, NKCUIAttendanceSubUI> item2 in m_dicSubUI)
		{
			UnityEngine.Object.Destroy(item2.Value.gameObject);
		}
		m_dicSubUI.Clear();
	}

	private int GetFirstTabID()
	{
		int num = int.MaxValue;
		foreach (KeyValuePair<int, NKCUIAttendanceTab> item in m_dicTab)
		{
			if (item.Value.GetTabIDX() < num)
			{
				num = item.Value.GetTabIDX();
			}
		}
		return num;
	}

	public static T OpenInstanceByAssetName<T>(string BundleName, string AssetName, Transform parent) where T : MonoBehaviour
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(BundleName, AssetName);
		if (nKCAssetInstanceData != null && nKCAssetInstanceData.m_Instant != null)
		{
			m_listNKCAssetResourceData.Add(nKCAssetInstanceData);
			return UnityEngine.Object.Instantiate(nKCAssetInstanceData.m_Instant, parent).GetComponent<T>();
		}
		Debug.LogWarning("prefab is null - " + BundleName + "/" + AssetName);
		return null;
	}

	public override void OnBackButton()
	{
		if (!m_objBlockLayer.activeInHierarchy)
		{
			base.OnBackButton();
		}
	}

	public void SetNewsCallback(Action action)
	{
		m_NewsCallback = action;
	}
}
