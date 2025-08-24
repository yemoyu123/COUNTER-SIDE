using System.Collections.Generic;
using ClientPacket.Mode;
using Cs.Logging;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUICounterCaseNormal : NKCUIBase
{
	public class Comp : IComparer<NKCUICCNormalSlot>
	{
		public int Compare(NKCUICCNormalSlot x, NKCUICCNormalSlot y)
		{
			if (!x.IsActive())
			{
				return 1;
			}
			if (!y.IsActive())
			{
				return -1;
			}
			if (y.GetStageIndex() <= x.GetStageIndex())
			{
				return 1;
			}
			return -1;
		}
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_COUNTER_CASE";

	private const string UI_ASSET_NAME = "NKM_UI_COUNTER_CASE_NORMAL";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public NKCUIEpisodeActSlotCC m_NKCUIEpisodeActSlotCC;

	public Transform m_NKM_UI_COUNTER_CASE_NORMAL_LIST_Content;

	public GameObject m_NKM_UI_COUNTER_CASE_NORMAL_UNIT;

	public ScrollRect m_NKM_UI_COUNTER_CASE_NORMAL_LIST_ScrollView;

	private List<NKCUICCNormalSlot> m_listItemSlot = new List<NKCUICCNormalSlot>();

	private int m_EpisodeID = 50;

	private int m_ActID;

	public static bool IsInstanceOpen
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsUIOpen;
			}
			return false;
		}
	}

	public static bool IsInstanceLoaded
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsLoadComplete;
			}
			return false;
		}
	}

	public override string MenuName => NKCUtilString.GET_STRING_MENU_NAME_CC;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override List<int> UpsideMenuShowResourceList => new List<int> { 3 };

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUICounterCaseNormal>("AB_UI_NKM_UI_COUNTER_CASE", "NKM_UI_COUNTER_CASE_NORMAL", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUICounterCaseNormal GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUICounterCaseNormal>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public void InitUI()
	{
		NKCUtil.SetScrollHotKey(m_NKM_UI_COUNTER_CASE_NORMAL_LIST_ScrollView);
	}

	public void SetActID(int actID)
	{
		if (actID > 0)
		{
			m_ActID = actID;
		}
	}

	private void OnSelectedSlot(int stageIndex, string stageBattleStrID)
	{
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(stageBattleStrID);
		if (nKMStageTempletV == null)
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		if (!myUserData.CheckUnlockedCounterCase(stageBattleStrID))
		{
			if (myUserData.CheckPrice(nKMStageTempletV.UnlockReqItem.Count32, nKMStageTempletV.UnlockReqItem.ItemId))
			{
				int dungeonID = NKMDungeonManager.GetDungeonID(stageBattleStrID);
				NKMPacket_COUNTERCASE_UNLOCK_REQ nKMPacket_COUNTERCASE_UNLOCK_REQ = new NKMPacket_COUNTERCASE_UNLOCK_REQ();
				nKMPacket_COUNTERCASE_UNLOCK_REQ.dungeonID = dungeonID;
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_COUNTERCASE_UNLOCK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			}
			else
			{
				NKCShopManager.OpenItemLackPopup(nKMStageTempletV.UnlockReqItem.ItemId, nKMStageTempletV.UnlockReqItem.Count32);
			}
			return;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(stageBattleStrID);
		if (dungeonTempletBase == null)
		{
			return;
		}
		if (dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
		{
			if (dungeonTempletBase.m_CutScenStrIDBefore != "" || dungeonTempletBase.m_CutScenStrIDAfter != "")
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().SetReservedDungeonType(dungeonTempletBase.m_DungeonID);
				NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetCounterCaseNormalActID(m_ActID);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON);
			}
		}
		else
		{
			NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SetDungeonInfo(nKMStageTempletV);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DUNGEON_ATK_READY);
		}
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
		if (nKMEpisodeTempletV == null)
		{
			UIOpened();
			return;
		}
		UpdateLeftslot();
		m_NKM_UI_COUNTER_CASE_NORMAL_UNIT.transform.DOLocalMove(new Vector3(-500f, 150f, 0f), 0.35f).From(isRelative: true).SetEase(Ease.OutCubic);
		if (!nKMEpisodeTempletV.m_DicStage.ContainsKey(m_ActID))
		{
			Log.Info($"Act ID not Found : {m_ActID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUICounterCaseNormal.cs", 204);
			UIOpened();
		}
		else
		{
			UpdateRightSlots(bSlotAni: true);
			UIOpened();
			CheckTutorial();
		}
	}

	public void UpdateLeftslot()
	{
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
		if (nKMEpisodeTempletV != null)
		{
			m_NKCUIEpisodeActSlotCC.SetData(nKMEpisodeTempletV, m_ActID);
		}
	}

	public void UpdateRightSlots(bool bSlotAni = false, int dungeonIDForBtnAni = -1)
	{
		if (m_ActID <= 0)
		{
			return;
		}
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
		if (nKMEpisodeTempletV == null)
		{
			return;
		}
		int num = 0;
		int count = nKMEpisodeTempletV.m_DicStage[m_ActID].Count;
		if (m_listItemSlot.Count < count)
		{
			int count2 = m_listItemSlot.Count;
			for (num = 0; num < count - count2; num++)
			{
				NKCUICCNormalSlot newInstance = NKCUICCNormalSlot.GetNewInstance(m_NKM_UI_COUNTER_CASE_NORMAL_LIST_Content, OnSelectedSlot);
				if (newInstance != null)
				{
					newInstance.gameObject.GetComponent<RectTransform>().localScale = Vector2.one;
				}
				m_listItemSlot.Add(newInstance);
			}
		}
		int num2 = 0;
		for (num = 0; num < m_listItemSlot.Count; num++)
		{
			NKCUICCNormalSlot nKCUICCNormalSlot = m_listItemSlot[num];
			if (num < count)
			{
				NKMStageTempletV2 nKMStageTempletV = nKMEpisodeTempletV.m_DicStage[m_ActID][num];
				if (nKMStageTempletV == null)
				{
					continue;
				}
				if (nKMStageTempletV.m_StageBasicUnlockType == STAGE_BASIC_UNLOCK_TYPE.SBUT_LOCK)
				{
					num2++;
					nKCUICCNormalSlot.SetData(nKMEpisodeTempletV.m_DicStage[m_ActID][num], dungeonIDForBtnAni);
					if (!nKCUICCNormalSlot.IsActive())
					{
						nKCUICCNormalSlot.SetActive(bSet: true);
					}
				}
				else if (nKCUICCNormalSlot.IsActive())
				{
					nKCUICCNormalSlot.SetActive(bSet: false);
				}
			}
			else if (nKCUICCNormalSlot.IsActive())
			{
				nKCUICCNormalSlot.SetActive(bSet: false);
			}
		}
		Sort();
		if (!bSlotAni)
		{
			return;
		}
		for (num = 0; num < m_listItemSlot.Count; num++)
		{
			NKCUICCNormalSlot nKCUICCNormalSlot2 = m_listItemSlot[num];
			if (nKCUICCNormalSlot2.IsActive())
			{
				nKCUICCNormalSlot2.SetAlphaAni(num);
			}
		}
	}

	private void Sort()
	{
		m_listItemSlot.Sort(new Comp());
		int num = 0;
		for (num = 0; num < m_listItemSlot.Count; num++)
		{
			NKCUICCNormalSlot nKCUICCNormalSlot = m_listItemSlot[num];
			if (nKCUICCNormalSlot.IsActive())
			{
				nKCUICCNormalSlot.transform.SetSiblingIndex(num);
			}
		}
	}

	public override void CloseInternal()
	{
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetCounterCaseNormalActID(0);
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.CounterCaseList);
	}

	public NKCUICCNormalSlot GetItemByStageIdx(int stageIndex)
	{
		NKCUICCNormalSlot nKCUICCNormalSlot = m_listItemSlot.Find((NKCUICCNormalSlot v) => v.GetStageIndex() == stageIndex);
		if (nKCUICCNormalSlot != null)
		{
			m_NKM_UI_COUNTER_CASE_NORMAL_LIST_ScrollView.normalizedPosition = new Vector2(0f, 1f);
			return nKCUICCNormalSlot;
		}
		return null;
	}
}
