using System.Collections.Generic;
using Cs.Logging;
using NKM;
using NKM.Event;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Module;

public class NKCUIModuleHome : NKCUIBase
{
	public abstract class EventModuleMessageDataBase
	{
		public int moduleID;

		public int eventID;
	}

	private NKMEventCollectionIndexTemplet m_eventCollectionIndexTemplet;

	private eMenutype m_eUIType;

	private string m_menuName = string.Empty;

	private Dictionary<int, NKCUIModuleSubUIBase> m_dicSubUI = new Dictionary<int, NKCUIModuleSubUIBase>();

	public List<NKCUIComToggle> m_lstTabButtons;

	public NKCUIComStateButton m_csbtnClose;

	public Text m_lbEventTimeLeft;

	[Header("Ani")]
	public Animator m_Ani;

	public override eMenutype eUIType => m_eUIType;

	public override string MenuName => m_menuName;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public NKMEventCollectionIndexTemplet EventCollectionIndexTemplet => m_eventCollectionIndexTemplet;

	public static NKCUIModuleHome MakeInstance(string bundleName, string assetName)
	{
		NKCUIModuleHome instance = NKCUIManager.OpenNewInstance<NKCUIModuleHome>(bundleName, assetName, NKCUIManager.eUIBaseRect.UIFrontCommon, null).GetInstance<NKCUIModuleHome>();
		instance.Init();
		return instance;
	}

	public static NKCUIModuleHome OpenEventModule(NKMEventCollectionIndexTemplet eventCollectionIndexTemplet)
	{
		if (eventCollectionIndexTemplet == null || !eventCollectionIndexTemplet.IsOpen)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_MENU_EXCEPTION_EVENT_EXPIRED_POPUP"));
			return null;
		}
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(eventCollectionIndexTemplet.EventPrefabID_BundleName, eventCollectionIndexTemplet.EventPrefabID_AssetName);
		NKCUIManager.LoadedUIData instance = NKCUIManager.GetInstance(nKMAssetName.m_BundleName.ToLower(), nKMAssetName.m_AssetName, OpenedUIOnly: false);
		NKCUIModuleHome nKCUIModuleHome = null;
		nKCUIModuleHome = ((instance != null) ? (instance.GetInstance() as NKCUIModuleHome) : MakeInstance(nKMAssetName.m_BundleName, nKMAssetName.m_AssetName));
		if (nKCUIModuleHome == null)
		{
			instance?.CloseInstance();
			return null;
		}
		nKCUIModuleHome.Open(eventCollectionIndexTemplet);
		return nKCUIModuleHome;
	}

	public override void CloseInternal()
	{
		foreach (KeyValuePair<int, NKCUIModuleSubUIBase> item in m_dicSubUI)
		{
			if (null != item.Value)
			{
				item.Value.OnClose();
			}
		}
		base.gameObject.SetActive(value: false);
	}

	public override void OnBackButton()
	{
		bool flag = false;
		foreach (KeyValuePair<int, NKCUIModuleSubUIBase> item in m_dicSubUI)
		{
			if (null != item.Value)
			{
				flag |= item.Value.OnBackButton();
			}
		}
		if (!flag)
		{
			Close();
		}
	}

	public override void UnHide()
	{
		base.UnHide();
		foreach (KeyValuePair<int, NKCUIModuleSubUIBase> item in m_dicSubUI)
		{
			item.Value.UnHide();
		}
		PlayEventBGM();
	}

	public override void Hide()
	{
		base.Hide();
		foreach (KeyValuePair<int, NKCUIModuleSubUIBase> item in m_dicSubUI)
		{
			item.Value.Hide();
		}
	}

	public override void Initialize()
	{
		Init();
	}

	private void Init()
	{
		NKCUIModuleSubUIBase[] componentsInChildren = GetComponentsInChildren<NKCUIModuleSubUIBase>(includeInactive: true);
		foreach (NKCUIModuleSubUIBase nKCUIModuleSubUIBase in componentsInChildren)
		{
			if (m_dicSubUI.ContainsKey(nKCUIModuleSubUIBase.ModuleID))
			{
				NKCUtil.SetGameobjectActive(nKCUIModuleSubUIBase, bValue: false);
				Log.Error($"Module of same ID {nKCUIModuleSubUIBase.ModuleID} exist!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Module/NKCUIModuleHome.cs", 143);
			}
			else
			{
				nKCUIModuleSubUIBase.Init();
				m_dicSubUI.Add(nKCUIModuleSubUIBase.ModuleID, nKCUIModuleSubUIBase);
			}
		}
		foreach (NKCUIComToggle lstTabButton in m_lstTabButtons)
		{
			if (!(lstTabButton == null))
			{
				lstTabButton.OnValueChangedWithData = OnSelectTab;
			}
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
	}

	private void OnSelectTab(bool value, int tabID)
	{
		if (value)
		{
			SelectTab(tabID);
		}
	}

	private void SelectTab(int tabID)
	{
		foreach (KeyValuePair<int, NKCUIModuleSubUIBase> item in m_dicSubUI)
		{
			bool flag = item.Key == tabID;
			NKCUIModuleSubUIBase value = item.Value;
			if (!flag && value.gameObject.activeInHierarchy)
			{
				value.OnClose();
			}
			NKCUtil.SetGameobjectActive(value, flag);
			if (flag)
			{
				value.OnOpen(m_eventCollectionIndexTemplet);
			}
		}
	}

	public override void OpenByShortcut(Dictionary<string, string> dicParam)
	{
		int intValue = NKCUtil.GetIntValue(dicParam, "ID");
		NKMEventCollectionIndexTemplet templet = ((intValue <= 0) ? NKMEventCollectionIndexTemplet.GetEventCollectionIndexTemplet(NKCSynchronizedTime.ServiceTime) : NKMEventCollectionIndexTemplet.Find(intValue));
		int intValue2 = NKCUtil.GetIntValue(dicParam, "TabID", 0);
		Open(templet, intValue2);
	}

	public void Open(NKMEventCollectionIndexTemplet templet, int tabID = 0)
	{
		m_eventCollectionIndexTemplet = templet;
		(m_lstTabButtons?.Find((NKCUIComToggle e) => e.m_DataInt == tabID))?.Select(bSelect: true);
		PlayEventBGM();
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		if (m_Ani != null)
		{
			m_Ani.SetTrigger("INTRO");
		}
		SelectTab(tabID);
		NKMIntervalTemplet nKMIntervalTemplet = null;
		if (templet != null)
		{
			nKMIntervalTemplet = NKMIntervalTemplet.Find(templet.DateStrId);
		}
		if (nKMIntervalTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_lbEventTimeLeft, bValue: true);
			if (NKCSynchronizedTime.GetTimeLeft(NKMTime.LocalToUTC(nKMIntervalTemplet.EndDate)).TotalDays > (double)NKCSynchronizedTime.UNLIMITD_REMAIN_DAYS)
			{
				NKCUtil.SetLabelText(m_lbEventTimeLeft, NKCUtilString.GET_STRING_EVENT_DATE_UNLIMITED_TEXT);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbEventTimeLeft, NKCUtilString.GetTimeIntervalString(nKMIntervalTemplet.StartDate, nKMIntervalTemplet.EndDate, NKMTime.INTERVAL_FROM_UTC));
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbEventTimeLeft, bValue: false);
		}
		UIOpened();
	}

	public void PlayEventBGM()
	{
		if (m_eventCollectionIndexTemplet != null && !string.IsNullOrEmpty(m_eventCollectionIndexTemplet.BgmAssetId))
		{
			NKCSoundManager.PlayMusic(m_eventCollectionIndexTemplet.BgmAssetId, bLoop: true, (float)m_eventCollectionIndexTemplet.BgmVolume * 0.01f);
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (itemData != null)
		{
			UpdateUI();
		}
	}

	public void UpdateUI()
	{
		foreach (KeyValuePair<int, NKCUIModuleSubUIBase> item in m_dicSubUI)
		{
			item.Value.Refresh();
		}
	}

	public IEnumerable<T> GetSubUIs<T>() where T : NKCUIModuleSubUIBase
	{
		foreach (KeyValuePair<int, NKCUIModuleSubUIBase> item in m_dicSubUI)
		{
			if (item.Value is T)
			{
				yield return item.Value as T;
			}
		}
	}

	public static void PlayBGMMusic()
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_HOME)
		{
			return;
		}
		using List<NKCUIModuleHome>.Enumerator enumerator = NKCUIManager.GetOpenedUIsByType<NKCUIModuleHome>().GetEnumerator();
		if (enumerator.MoveNext())
		{
			enumerator.Current.PlayEventBGM();
		}
	}

	private void _SendMessage(EventModuleMessageDataBase data)
	{
		if (data == null)
		{
			return;
		}
		foreach (KeyValuePair<int, NKCUIModuleSubUIBase> item in m_dicSubUI)
		{
			if (item.Value != null && item.Value.gameObject.activeInHierarchy)
			{
				item.Value.PassData(data);
			}
		}
	}

	public static bool IsAnyInstanceOpen()
	{
		return NKCUIManager.GetOpenedUIsByType<NKCUIModuleHome>().Count > 0;
	}

	public static void UpdateAllModule(bool bOpenOnly = true)
	{
		foreach (NKCUIModuleHome item in NKCUIManager.GetOpenedUIsByType<NKCUIModuleHome>())
		{
			if (!(!item.IsOpen && bOpenOnly))
			{
				item.UpdateUI();
			}
		}
	}

	public static void SendMessage(EventModuleMessageDataBase message, bool bOpenOnly = true)
	{
		foreach (NKCUIModuleHome item in NKCUIManager.GetOpenedUIsByType<NKCUIModuleHome>())
		{
			if (!(!item.IsOpen && bOpenOnly))
			{
				item._SendMessage(message);
			}
		}
	}
}
