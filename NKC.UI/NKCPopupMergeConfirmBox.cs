using System;
using System.Collections.Generic;
using NKM;
using NKM.Event;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupMergeConfirmBox : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_eventmerge";

	private const string UI_ASSET_NAME = "EVENTMERGE_POPUP_MERGE_RESULT";

	private static NKCPopupMergeConfirmBox m_Instance;

	public List<NKCUISlot> m_lstUnitSlot;

	public Image m_imgResult;

	public NKCUIComStateButton m_csbtnResult;

	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnCancel;

	private List<long> m_TargetUids = new List<long>();

	private int m_groupID;

	private int m_mergeID;

	private Action m_OnClick;

	public static NKCPopupMergeConfirmBox Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupMergeConfirmBox>("ab_ui_eventmerge", "EVENTMERGE_POPUP_MERGE_RESULT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupMergeConfirmBox>();
				m_Instance?.Init();
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

	public override string MenuName => string.Empty;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => base.eUpsideMenuMode;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnOK, OnClickOK);
		NKCUtil.SetBindFunction(m_csbtnCancel, OnClickCancel);
		NKCUtil.SetBindFunction(m_csbtnResult, OnClickResult);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
	}

	public void Open(int mergeID, int groupID, List<long> lstConsumeUids, Action click)
	{
		m_mergeID = mergeID;
		m_groupID = groupID;
		m_TargetUids = lstConsumeUids;
		m_OnClick = click;
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		for (int i = 0; i < m_lstUnitSlot.Count; i++)
		{
			if (i < m_TargetUids.Count)
			{
				NKMUnitData trophyFromUID = armyData.GetTrophyFromUID(m_TargetUids[i]);
				if (trophyFromUID != null)
				{
					NKMUnitManager.GetUnitTempletBase(trophyFromUID);
					m_lstUnitSlot[i].SetData(NKCUISlot.SlotData.MakeUnitData(trophyFromUID));
					NKCUtil.SetGameobjectActive(m_lstUnitSlot[i], bValue: true);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstUnitSlot[i], bValue: false);
			}
		}
		UpdateResultIcon();
		UIOpened();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private void UpdateResultIcon()
	{
		NKMEventCollectionMergeTemplet nKMEventCollectionMergeTemplet = NKMTempletContainer<NKMEventCollectionMergeTemplet>.Find(m_mergeID);
		if (nKMEventCollectionMergeTemplet != null)
		{
			NKMEventCollectionMergeRecipeTemplet recipeTemplet = nKMEventCollectionMergeTemplet.GetRecipeTemplet(m_groupID);
			if (recipeTemplet != null)
			{
				Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(recipeTemplet.MergeOutputBundleName, recipeTemplet.MergeOutputAssetName);
				NKCUtil.SetImageSprite(m_imgResult, orLoadAssetResource);
			}
		}
	}

	private void OnClickResult()
	{
		NKMEventCollectionMergeTemplet nKMEventCollectionMergeTemplet = NKMTempletContainer<NKMEventCollectionMergeTemplet>.Find(m_mergeID);
		if (nKMEventCollectionMergeTemplet == null)
		{
			return;
		}
		NKMEventCollectionMergeRecipeTemplet recipeTemplet = nKMEventCollectionMergeTemplet.GetRecipeTemplet(m_groupID);
		if (recipeTemplet == null)
		{
			return;
		}
		List<int> list = new List<int>();
		foreach (NKMEventCollectionTemplet value in NKMTempletContainer<NKMEventCollectionTemplet>.Values)
		{
			if (nKMEventCollectionMergeTemplet.Key != value.CollectionMergeId)
			{
				continue;
			}
			foreach (NKMEventCollectionDetailTemplet detail in value.Details)
			{
				if (recipeTemplet.MergeOutputGradeGroupId == detail.CollectionGradeGroupId)
				{
					list.Add(detail.Key);
				}
			}
		}
		NKCUISlotListViewer.Instance.OpenRewardList(list, NKM_REWARD_TYPE.RT_UNIT, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}

	private void OnClickOK()
	{
		m_OnClick?.Invoke();
	}

	private void OnClickCancel()
	{
		Close();
	}
}
