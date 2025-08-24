using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientPacket.Event;
using NKM;
using NKM.Event;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEvent : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_event";

	private const string UI_ASSET_NAME = "NKM_UI_EVENT";

	private static NKCUIEvent m_Instance;

	private List<int> RESOURCE_LIST = new List<int>();

	public GameObject m_pfbMenuSlot;

	public LoopScrollRect m_loopScrollRect;

	public NKCUIComToggleGroup m_tglGroup;

	public Transform m_objSubUIParent;

	public NKCUIComStateButton m_csbtnClose;

	private List<NKMEventTabTemplet> m_lstEventTabTemplet = new List<NKMEventTabTemplet>();

	private Stack<NKCUIEventSlot> m_stkEventTabSlot = new Stack<NKCUIEventSlot>();

	private Dictionary<int, NKCUIEventSlot> m_dicEventTab = new Dictionary<int, NKCUIEventSlot>();

	private Dictionary<int, NKCUIEventSubUI> m_dicSubUI = new Dictionary<int, NKCUIEventSubUI>();

	private static List<NKCAssetInstanceData> m_listNKCAssetResourceData = new List<NKCAssetInstanceData>();

	private int m_SelectedTabID;

	public static NKCUIEvent Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIEvent>("ab_ui_nkm_ui_event", "NKM_UI_EVENT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIEvent>();
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

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_EVENT_MENU;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override List<int> UpsideMenuShowResourceList => RESOURCE_LIST;

	public int SelectedTabId => m_SelectedTabID;

	private static string LAST_VERSION_KEY
	{
		get
		{
			if (NKCScenManager.CurrentUserData() != null)
			{
				return "NKCUIEvent_LAST_VERSION_" + NKCScenManager.CurrentUserData().m_UserUID;
			}
			return "NKCUIEvent_LAST_VERSION";
		}
	}

	private static string LAST_EVENT_LIST_KEY
	{
		get
		{
			if (NKCScenManager.CurrentUserData() != null)
			{
				return "NKCUIEvent_LAST_EVENT_LIST_" + NKCScenManager.CurrentUserData().m_UserUID;
			}
			return "NKCUIEvent_LAST_EVENT_LIST";
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void OnCloseInstance()
	{
		base.OnCloseInstance();
		ClearPrefabs();
		m_Instance = null;
	}

	private void Init()
	{
		m_loopScrollRect.dOnGetObject += GetObject;
		m_loopScrollRect.dOnReturnObject += ReturnObject;
		m_loopScrollRect.dOnProvideData += ProvideData;
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
		m_loopScrollRect.PrepareCells();
	}

	private RectTransform GetObject(int index)
	{
		if (m_stkEventTabSlot.Count > 0)
		{
			NKCUIEventSlot nKCUIEventSlot = m_stkEventTabSlot.Pop();
			NKCUtil.SetGameobjectActive(nKCUIEventSlot, bValue: false);
			return nKCUIEventSlot.GetComponent<RectTransform>();
		}
		GameObject obj = Object.Instantiate(m_pfbMenuSlot);
		NKCUtil.SetGameobjectActive(obj, bValue: false);
		return obj.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform rt)
	{
		NKCUtil.SetGameobjectActive(rt, bValue: false);
		rt.SetParent(base.transform);
		m_stkEventTabSlot.Push(rt.GetComponent<NKCUIEventSlot>());
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUIEventSlot component = tr.GetComponent<NKCUIEventSlot>();
		if (!(component == null))
		{
			bool flag = false;
			if (!m_dicEventTab.ContainsKey(m_lstEventTabTemplet[idx].m_EventID))
			{
				m_dicEventTab.Add(m_lstEventTabTemplet[idx].m_EventID, component);
			}
			else
			{
				m_dicEventTab[m_lstEventTabTemplet[idx].m_EventID] = component;
			}
			flag = m_lstEventTabTemplet[idx].m_EventID == m_SelectedTabID;
			component.SetData(m_lstEventTabTemplet[idx], m_tglGroup, flag, OnSelectTab);
			component.CheckRedDot();
		}
	}

	public override void CloseInternal()
	{
		CloseSubUI();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(NKMEventTabTemplet reservedTabTemplet = null)
	{
		BuildTabs();
		if (m_lstEventTabTemplet.Count <= 0)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_EVENT_END, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			return;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loopScrollRect.TotalCount = m_lstEventTabTemplet.Count;
		m_loopScrollRect.RefreshCells();
		if (reservedTabTemplet == null || !m_lstEventTabTemplet.Contains(reservedTabTemplet))
		{
			reservedTabTemplet = m_lstEventTabTemplet[0];
		}
		OnSelectTab(reservedTabTemplet);
		SaveEventPopupOpenInfo();
		UIOpened();
	}

	private void BuildTabs()
	{
		m_lstEventTabTemplet = GetActiveEventTabList();
		m_lstEventTabTemplet.Sort(Comparer);
	}

	private int Comparer(NKMEventTabTemplet lItem, NKMEventTabTemplet rItem)
	{
		return lItem.m_OrderList.CompareTo(rItem.m_OrderList);
	}

	public void OnSelectTab(NKMEventTabTemplet tabTemplet)
	{
		m_SelectedTabID = tabTemplet.m_EventID;
		foreach (NKCUIEventSlot value in m_dicEventTab.Values)
		{
			value.SetToggle(value.EventID == tabTemplet.m_EventID, bForce: true, bImmediate: true);
		}
		if (m_dicSubUI.ContainsKey(tabTemplet.m_EventID) && m_dicSubUI[tabTemplet.m_EventID].gameObject.activeSelf)
		{
			m_dicSubUI[tabTemplet.m_EventID].Refresh();
			return;
		}
		foreach (NKCUIEventSubUI value2 in m_dicSubUI.Values)
		{
			NKCUtil.SetGameobjectActive(value2, bValue: false);
		}
		if (m_dicSubUI.ContainsKey(tabTemplet.m_EventID))
		{
			NKCUIEventSubUI nKCUIEventSubUI = m_dicSubUI[tabTemplet.m_EventID];
			NKCUtil.SetGameobjectActive(nKCUIEventSubUI.gameObject, bValue: true);
			nKCUIEventSubUI.Open(tabTemplet);
			return;
		}
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(GetBannerBundleName(tabTemplet), GetBannerAssetName(tabTemplet));
		NKCUIEventSubUI nKCUIEventSubUI2 = OpenInstanceByAssetName<NKCUIEventSubUI>(nKMAssetName.m_BundleName, nKMAssetName.m_AssetName, m_objSubUIParent);
		if (nKCUIEventSubUI2 != null)
		{
			m_dicSubUI.Add(tabTemplet.m_EventID, nKCUIEventSubUI2);
			nKCUIEventSubUI2.Init();
			nKCUIEventSubUI2.Open(tabTemplet);
		}
		RESOURCE_LIST = tabTemplet.m_lstResourceTypeID;
		NKCUIManager.UpdateUpsideMenu();
	}

	public static string GetBannerBundleName(NKMEventTabTemplet templet)
	{
		return templet.m_EventType switch
		{
			NKM_EVENT_TYPE.ONTIME => "AB_UI_NKM_UI_EVENT_PF_ONTIME", 
			NKM_EVENT_TYPE.CONTRACT => "AB_UI_NKM_UI_EVENT_PF_CONTRACT", 
			NKM_EVENT_TYPE.RACE => "AB_UI_NKM_UI_EVENT_PF_RACE", 
			_ => templet.m_EventBannerPrefabName, 
		};
	}

	public static string GetBannerAssetName(NKMEventTabTemplet templet)
	{
		return templet.m_EventType switch
		{
			NKM_EVENT_TYPE.CONTRACT => "NKM_UI_EVENT_CONTRACT", 
			_ => templet.m_EventBannerPrefabName, 
		};
	}

	public NKCUIEventSubUI GetEverOpenedEventSubUI(int eventID)
	{
		m_dicSubUI.TryGetValue(eventID, out var value);
		return value;
	}

	public void RefreshUI(int eventId = 0)
	{
		foreach (KeyValuePair<int, NKCUIEventSubUI> item in m_dicSubUI)
		{
			if (item.Value.gameObject.activeSelf)
			{
				item.Value.Refresh();
			}
		}
		foreach (KeyValuePair<int, NKCUIEventSlot> item2 in m_dicEventTab)
		{
			item2.Value.CheckRedDot();
		}
	}

	private void ClearPrefabs()
	{
		foreach (KeyValuePair<int, NKCUIEventSlot> item in m_dicEventTab)
		{
			Object.Destroy(item.Value.gameObject);
		}
		m_dicEventTab.Clear();
	}

	public override void Hide()
	{
		base.Hide();
		foreach (KeyValuePair<int, NKCUIEventSubUI> item in m_dicSubUI)
		{
			if (item.Value.gameObject.activeSelf)
			{
				item.Value.Hide();
			}
		}
	}

	public override void UnHide()
	{
		base.UnHide();
		foreach (KeyValuePair<int, NKCUIEventSubUI> item in m_dicSubUI)
		{
			if (item.Value.gameObject.activeSelf)
			{
				item.Value.UnHide();
			}
		}
	}

	public void CloseSubUI()
	{
		foreach (KeyValuePair<int, NKCUIEventSubUI> item in m_dicSubUI)
		{
			if (item.Value.gameObject.activeSelf)
			{
				item.Value.Close();
			}
		}
	}

	public override void OnBackButton()
	{
		foreach (KeyValuePair<int, NKCUIEventSubUI> item in m_dicSubUI)
		{
			if (item.Value.gameObject.activeSelf && item.Value.OnBackButton())
			{
				return;
			}
		}
		base.OnBackButton();
	}

	public static T OpenInstanceByAssetName<T>(string BundleName, string AssetName, Transform parent) where T : MonoBehaviour
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(BundleName, AssetName, bAsync: false, parent);
		if (nKCAssetInstanceData != null && nKCAssetInstanceData.m_Instant != null)
		{
			GameObject instant = nKCAssetInstanceData.m_Instant;
			T component = instant.GetComponent<T>();
			if (component == null)
			{
				Object.Destroy(instant);
				NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
				return null;
			}
			m_listNKCAssetResourceData.Add(nKCAssetInstanceData);
			return component;
		}
		Debug.LogWarning("prefab is null - " + BundleName + "/" + AssetName);
		return null;
	}

	private void OnDestroy()
	{
		for (int i = 0; i < m_listNKCAssetResourceData.Count; i++)
		{
			NKCAssetResourceManager.CloseInstance(m_listNKCAssetResourceData[i]);
		}
		m_listNKCAssetResourceData.Clear();
		m_Instance = null;
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		foreach (KeyValuePair<int, NKCUIEventSubUI> item in m_dicSubUI)
		{
			if (item.Value.gameObject.activeSelf)
			{
				item.Value.OnInventoryChange(itemData);
			}
		}
		foreach (KeyValuePair<int, NKCUIEventSlot> item2 in m_dicEventTab)
		{
			item2.Value.CheckRedDot();
		}
	}

	public void MarkBingo(int eventID, List<NKMBingoTile> bingoList, bool bRandom)
	{
		if (!m_dicSubUI.TryGetValue(eventID, out var value))
		{
			return;
		}
		foreach (NKCUIEventSubUIBingo subUI in value.GetSubUIs<NKCUIEventSubUIBingo>())
		{
			subUI.MarkBingo(bingoList, bRandom);
		}
	}

	private static List<NKMEventTabTemplet> GetActiveEventTabList()
	{
		List<NKMEventTabTemplet> list = new List<NKMEventTabTemplet>();
		foreach (NKMEventTabTemplet value in NKMTempletContainer<NKMEventTabTemplet>.Values)
		{
			if (!value.IsAvailable || !value.ShowEventBanner())
			{
				continue;
			}
			switch (value.m_EventType)
			{
			case NKM_EVENT_TYPE.BINGO:
				if (NKMEventManager.GetBingoData(value.m_EventID) != null)
				{
					list.Add(value);
				}
				break;
			case NKM_EVENT_TYPE.KAKAOEMOTE:
				if (NKCScenManager.CurrentUserData().IsKakaoMissionOngoing())
				{
					list.Add(value);
				}
				break;
			case NKM_EVENT_TYPE.MISSION:
			case NKM_EVENT_TYPE.ONTIME:
				list.Add(value);
				break;
			default:
				list.Add(value);
				break;
			}
		}
		return list;
	}

	private void SaveEventPopupOpenInfo()
	{
		PlayerPrefs.SetString(LAST_VERSION_KEY, NKMContentsVersionManager.CurrentVersion.Literal);
		PlayerPrefs.SetString(LAST_EVENT_LIST_KEY, MakeLastEventListString(m_lstEventTabTemplet));
		PlayerPrefs.Save();
	}

	private string MakeLastEventListString(IEnumerable<NKMEventTabTemplet> setEvent)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (NKMEventTabTemplet item in setEvent)
		{
			stringBuilder.Append(item.m_EventID);
			stringBuilder.Append(';');
		}
		return stringBuilder.ToString();
	}

	private static HashSet<int> LoadLastEventList()
	{
		string text = PlayerPrefs.GetString(LAST_EVENT_LIST_KEY, "");
		HashSet<int> hashSet = new HashSet<int>();
		string[] array = text.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			if (int.TryParse(array[i], out var result))
			{
				hashSet.Add(result);
			}
		}
		return hashSet;
	}

	public static NKMEventTabTemplet GetRequiredEventTemplet()
	{
		List<NKMEventTabTemplet> activeEventTabList = GetActiveEventTabList();
		HashSet<int> hashSet = new HashSet<int>(activeEventTabList.Select((NKMEventTabTemplet x) => x.m_EventID));
		HashSet<int> other = LoadLastEventList();
		hashSet.ExceptWith(other);
		if (hashSet.Count > 0)
		{
			return NKMEventTabTemplet.Find(hashSet.First());
		}
		if (NKMContentsVersionManager.CurrentVersion.Literal != PlayerPrefs.GetString(LAST_VERSION_KEY, "") && activeEventTabList.Count > 0)
		{
			return activeEventTabList[0];
		}
		return null;
	}
}
