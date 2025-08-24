using System;
using NKM;
using NKM.Event;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Module;

public class NKCUIModuleLobby : MonoBehaviour
{
	public NKCUIComStateButton m_csbtnEventModule;

	public Image m_buttonIconImage;

	public Image m_buttonTitleImage;

	public Text m_lbRemainTime;

	public GameObject m_objRedDot;

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnEventModule, OnClickEventModuleHome);
		NKMEventCollectionIndexTemplet eventCollectionIndexTemplet = GetEventCollectionIndexTemplet();
		base.gameObject.SetActive(eventCollectionIndexTemplet != null);
		if (eventCollectionIndexTemplet != null)
		{
			Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("ab_ui_nkm_ui_lobby_texture", eventCollectionIndexTemplet.EventBannerStrId));
			NKCUtil.SetImageSprite(m_buttonIconImage, orLoadAssetResource);
		}
	}

	public void SetData()
	{
		NKMEventCollectionIndexTemplet eventCollectionIndexTemplet = GetEventCollectionIndexTemplet();
		if (eventCollectionIndexTemplet == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		SetEventRemainTime(NKMIntervalTemplet.Find(eventCollectionIndexTemplet.DateStrId));
		bool bValue = CheckMissionCompleteEnable();
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue);
	}

	public bool IsEventModuleOpen()
	{
		NKMEventCollectionIndexTemplet eventCollectionIndexTemplet = GetEventCollectionIndexTemplet();
		if (eventCollectionIndexTemplet == null)
		{
			return false;
		}
		if (!eventCollectionIndexTemplet.IsOpen)
		{
			return false;
		}
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(eventCollectionIndexTemplet.EventPrefabID_BundleName, eventCollectionIndexTemplet.EventPrefabID_AssetName);
		return NKCUIManager.GetInstance(nKMAssetName.m_AssetName, nKMAssetName.m_AssetName, OpenedUIOnly: true) != null;
	}

	private void SetEventRemainTime(NKMIntervalTemplet intervalTemplet)
	{
		if (intervalTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: true);
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(NKCSynchronizedTime.ToUtcTime(intervalTemplet.EndDate));
		string text = "";
		NKCUtil.SetLabelText(msg: (timeLeft.Days > 0) ? string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_DAYS"), timeLeft.Days) : ((timeLeft.Hours <= 0) ? string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_MINUTES"), timeLeft.Minutes) : string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_HOURS"), timeLeft.Hours)), label: m_lbRemainTime);
	}

	public static NKMEventCollectionIndexTemplet GetEventCollectionIndexTemplet()
	{
		foreach (NKMEventCollectionIndexTemplet value in NKMEventCollectionIndexTemplet.Values)
		{
			if (value != null && value.IsOpen)
			{
				NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(value.DateStrId);
				if (nKMIntervalTemplet != null && nKMIntervalTemplet.IsValidTime(NKCSynchronizedTime.ServiceTime))
				{
					return value;
				}
			}
		}
		return null;
	}

	private bool CheckMissionCompleteEnable()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		NKMUserMissionData missionData = myUserData.m_MissionData;
		if (missionData == null)
		{
			return false;
		}
		NKMEventCollectionIndexTemplet eventCollectionIndexTemplet = GetEventCollectionIndexTemplet();
		if (eventCollectionIndexTemplet == null)
		{
			return false;
		}
		foreach (int missionTabId in eventCollectionIndexTemplet.MissionTabIds)
		{
			if (missionData.CheckCompletableMission(myUserData, missionTabId))
			{
				return true;
			}
		}
		return false;
	}

	public void OpenEventModuleHome(int iKey)
	{
		NKMEventCollectionIndexTemplet eventCollectionIndexTemplet = NKMTempletContainer<NKMEventCollectionIndexTemplet>.Find(iKey);
		OpenEventModuleHome(eventCollectionIndexTemplet);
	}

	private void OnClickEventModuleHome()
	{
		NKMEventCollectionIndexTemplet eventCollectionIndexTemplet = GetEventCollectionIndexTemplet();
		OpenEventModuleHome(eventCollectionIndexTemplet);
	}

	private void OpenEventModuleHome(NKMEventCollectionIndexTemplet eventCollectionIndexTemplet)
	{
		if (eventCollectionIndexTemplet == null || !eventCollectionIndexTemplet.IsOpen)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_MENU_EXCEPTION_EVENT_EXPIRED_POPUP"), delegate
			{
				base.gameObject.SetActive(value: false);
			});
			return;
		}
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(eventCollectionIndexTemplet.EventPrefabID_BundleName, eventCollectionIndexTemplet.EventPrefabID_AssetName);
		NKCUIManager.LoadedUIData instance = NKCUIManager.GetInstance(nKMAssetName.m_AssetName.ToLower(), nKMAssetName.m_AssetName, OpenedUIOnly: false);
		NKCUIModuleHome nKCUIModuleHome = null;
		((instance != null) ? (instance.GetInstance() as NKCUIModuleHome) : NKCUIModuleHome.MakeInstance(nKMAssetName.m_AssetName, nKMAssetName.m_AssetName))?.Open(eventCollectionIndexTemplet);
	}
}
