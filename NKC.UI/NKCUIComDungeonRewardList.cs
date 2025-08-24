using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUIComDungeonRewardList : MonoBehaviour
{
	public GameObject m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content;

	private const int ITEM_SLOT_WIDTH_DIST = 150;

	private RectTransform m_rectListContent;

	private float m_ContentOrgPosX;

	private List<StageRewardData> m_listRewardData = new List<StageRewardData>();

	private List<NKCUISlot> m_listSlotToShow = new List<NKCUISlot>();

	private string m_strDungeonID;

	private float m_fElapsedTime;

	public void InitUI()
	{
		m_rectListContent = m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.GetComponent<RectTransform>();
		m_ContentOrgPosX = m_rectListContent.anchoredPosition.x;
		m_listRewardData.Clear();
		m_listRewardData.Add(new FirstClearData(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform));
		m_listRewardData.Add(new FirstAllClearData(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform));
		for (int i = 0; i < 3; i++)
		{
			m_listRewardData.Add(new OneTimeReward(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform, i));
		}
		m_listRewardData.Add(new MainReward(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform));
		int eventDropGroupCount = NKMStageTempletV2.EventDropGroupCount;
		for (int j = 0; j < eventDropGroupCount; j++)
		{
			m_listRewardData.Add(new EventDropReward(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform, j));
		}
		m_listRewardData.Add(new BuffDropReward(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform));
		m_listRewardData.Add(new RandomItemReward(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform, NKM_REWARD_TYPE.RT_UNIT, 901, OnClickUnitRewardSlot));
		m_listRewardData.Add(new RandomItemReward(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform, NKM_REWARD_TYPE.RT_EQUIP, 902, OnClickEquipRewardSlot));
		m_listRewardData.Add(new RandomItemReward(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform, NKM_REWARD_TYPE.RT_MOLD, 904, OnClickMoldRewardSlot));
		m_listRewardData.Add(new RandomItemReward(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform, NKM_REWARD_TYPE.RT_MISC, 903, OnClickMiscRewardSlot));
		m_listRewardData.Add(new MiscItemReward(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform, 501));
		m_listRewardData.Add(new MiscItemReward(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform, 1));
		m_listRewardData.Add(new MiscItemReward(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform, 2));
		m_listRewardData.Add(new MiscItemReward(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content.transform, 3));
	}

	public void ShowRewardListUpdate()
	{
		m_fElapsedTime += Time.deltaTime;
		if (m_fElapsedTime > 0.08f && m_listSlotToShow.Count > 0)
		{
			m_fElapsedTime = 0f;
			m_listSlotToShow[0].SetActive(bSet: true);
			m_listSlotToShow[0].PlaySmallToOrgSize();
			m_listSlotToShow.RemoveAt(0);
		}
	}

	public bool CreateRewardSlotDataList(NKMUserData cNKMUserData, NKMStageTempletV2 stageTemplet, string strDungeonID)
	{
		if (cNKMUserData == null || stageTemplet == null)
		{
			return false;
		}
		m_strDungeonID = strDungeonID;
		m_fElapsedTime = 0f;
		List<int> listNRGI = GetListNRGI(stageTemplet);
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content, bValue: true);
		ResetRewardSlotList();
		int count = m_listRewardData.Count;
		for (int i = 0; i < count; i++)
		{
			m_listRewardData[i].CreateSlotData(stageTemplet, cNKMUserData, listNRGI, m_listSlotToShow);
		}
		Vector2 anchoredPosition = m_rectListContent.anchoredPosition;
		anchoredPosition.x = m_ContentOrgPosX;
		m_rectListContent.anchoredPosition = anchoredPosition;
		Vector2 sizeDelta = m_rectListContent.sizeDelta;
		sizeDelta.x = m_listSlotToShow.Count * 150;
		m_rectListContent.sizeDelta = sizeDelta;
		return HasAnyReward(stageTemplet, listNRGI);
	}

	public bool CreateRewardSlotDataList(NKMUserData cNKMUserData, NKMDefenceTemplet defenceTemplet, string strDungeonID)
	{
		if (cNKMUserData == null || defenceTemplet == null)
		{
			return false;
		}
		m_strDungeonID = strDungeonID;
		m_fElapsedTime = 0f;
		List<int> listNRGI = GetListNRGI(defenceTemplet);
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content, bValue: true);
		ResetRewardSlotList();
		int count = m_listRewardData.Count;
		for (int i = 0; i < count; i++)
		{
			m_listRewardData[i].CreateSlotData(defenceTemplet, cNKMUserData, listNRGI, m_listSlotToShow);
		}
		Vector2 anchoredPosition = m_rectListContent.anchoredPosition;
		anchoredPosition.x = m_ContentOrgPosX;
		m_rectListContent.anchoredPosition = anchoredPosition;
		Vector2 sizeDelta = m_rectListContent.sizeDelta;
		sizeDelta.x = m_listSlotToShow.Count * 150;
		m_rectListContent.sizeDelta = sizeDelta;
		return HasAnyReward(defenceTemplet, listNRGI);
	}

	private List<int> GetListNRGI()
	{
		List<int> result = new List<int>();
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(m_strDungeonID);
		if (nKMStageTempletV == null)
		{
			return result;
		}
		return GetListNRGI(nKMStageTempletV);
	}

	private List<int> GetListNRGI(NKMStageTempletV2 stageTemplet)
	{
		List<int> list = new List<int>();
		switch (stageTemplet.m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_WARFARE:
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_strDungeonID);
			if (nKMWarfareTemplet == null)
			{
				break;
			}
			for (int l = 0; l < nKMWarfareTemplet.RewardList.Count; l++)
			{
				list.Add(nKMWarfareTemplet.RewardList[l].m_RewardGroupID);
			}
			NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
			if (mapTemplet == null)
			{
				break;
			}
			foreach (string dungeonStrID in mapTemplet.GetDungeonStrIDList())
			{
				NKMDungeonTempletBase dungeonTempletBase2 = NKMDungeonManager.GetDungeonTempletBase(dungeonStrID);
				if (dungeonTempletBase2 != null)
				{
					for (int m = 0; m < dungeonTempletBase2.m_listDungeonReward.Count; m++)
					{
						list.Add(dungeonTempletBase2.m_listDungeonReward[m].m_RewardGroupID);
					}
				}
			}
			if (nKMWarfareTemplet.ContainerRewardTemplet != null)
			{
				list.Add(nKMWarfareTemplet.ContainerRewardTemplet.GroupId);
			}
			break;
		}
		case STAGE_TYPE.ST_DUNGEON:
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_strDungeonID);
			if (dungeonTempletBase != null)
			{
				for (int k = 0; k < dungeonTempletBase.m_listDungeonReward.Count; k++)
				{
					list.Add(dungeonTempletBase.m_listDungeonReward[k].m_RewardGroupID);
				}
				if (dungeonTempletBase.m_RewardUnitExp1Tier > 0)
				{
					list.Add(1031);
				}
				if (dungeonTempletBase.m_RewardUnitExp2Tier > 0)
				{
					list.Add(1032);
				}
				if (dungeonTempletBase.m_RewardUnitExp3Tier > 0)
				{
					list.Add(1033);
				}
			}
			break;
		}
		case STAGE_TYPE.ST_PHASE:
		{
			NKMPhaseTemplet nKMPhaseTemplet = NKMPhaseTemplet.Find(m_strDungeonID);
			if (nKMPhaseTemplet == null)
			{
				break;
			}
			for (int i = 0; i < nKMPhaseTemplet.Rewards.Count; i++)
			{
				list.Add(nKMPhaseTemplet.Rewards[i].m_RewardGroupID);
			}
			if (nKMPhaseTemplet.PhaseList == null)
			{
				break;
			}
			foreach (NKMPhaseOrderTemplet item in nKMPhaseTemplet.PhaseList.List)
			{
				NKMDungeonTempletBase dungeon = item.Dungeon;
				if (dungeon != null)
				{
					for (int j = 0; j < dungeon.m_listDungeonReward.Count; j++)
					{
						list.Add(dungeon.m_listDungeonReward[j].m_RewardGroupID);
					}
				}
				if (dungeon.m_RewardUnitExp1Tier > 0 && !list.Contains(1031))
				{
					list.Add(1031);
				}
				if (dungeon.m_RewardUnitExp2Tier > 0 && !list.Contains(1032))
				{
					list.Add(1032);
				}
				if (dungeon.m_RewardUnitExp3Tier > 0 && !list.Contains(1033))
				{
					list.Add(1033);
				}
			}
			break;
		}
		}
		return list;
	}

	private List<int> GetListNRGI(NKMDefenceTemplet defenceTemplet)
	{
		List<int> list = new List<int>();
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_strDungeonID);
		if (dungeonTempletBase != null)
		{
			for (int i = 0; i < dungeonTempletBase.m_listDungeonReward.Count; i++)
			{
				list.Add(dungeonTempletBase.m_listDungeonReward[i].m_RewardGroupID);
			}
			if (dungeonTempletBase.m_RewardUnitExp1Tier > 0)
			{
				list.Add(1031);
			}
			if (dungeonTempletBase.m_RewardUnitExp2Tier > 0)
			{
				list.Add(1032);
			}
			if (dungeonTempletBase.m_RewardUnitExp3Tier > 0)
			{
				list.Add(1033);
			}
		}
		return list;
	}

	public static List<int> GetUnitRewardIdList(HashSet<int> hsUnits, HashSet<int> hsOperators)
	{
		List<NKMUnitTempletBase> list = new List<NKMUnitTempletBase>();
		foreach (int hsUnit in hsUnits)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(hsUnit);
			if (unitTempletBase != null)
			{
				list.Add(unitTempletBase);
			}
		}
		foreach (int hsOperator in hsOperators)
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(hsOperator);
			if (unitTempletBase2 != null)
			{
				list.Add(unitTempletBase2);
			}
		}
		list.Sort(new CompTemplet.CompNUTB());
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null)
			{
				list2.Add(list[i].m_UnitID);
			}
		}
		return list2;
	}

	public static List<int> GetEquipRewardIdList(HashSet<int> hsEquips)
	{
		List<NKMEquipTemplet> list = new List<NKMEquipTemplet>();
		foreach (int hsEquip in hsEquips)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(hsEquip);
			if (equipTemplet != null)
			{
				list.Add(equipTemplet);
			}
		}
		list.Sort(new CompTemplet.CompNET());
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null)
			{
				list2.Add(list[i].m_ItemEquipID);
			}
		}
		return list2;
	}

	public static List<int> GetMoldRewardIdList(HashSet<int> hsMolds)
	{
		List<NKMItemMoldTemplet> list = new List<NKMItemMoldTemplet>();
		foreach (int hsMold in hsMolds)
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(hsMold);
			if (itemMoldTempletByID != null)
			{
				list.Add(itemMoldTempletByID);
			}
		}
		list.Sort(new CompTemplet.CompNMT());
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null)
			{
				list2.Add(list[i].m_MoldID);
			}
		}
		return list2;
	}

	public static List<int> GetMiscRewardIdList(HashSet<int> hsMisc)
	{
		List<NKMItemMiscTemplet> list = new List<NKMItemMiscTemplet>();
		foreach (int item in hsMisc)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(item);
			if (itemMiscTempletByID != null)
			{
				list.Add(itemMiscTempletByID);
			}
		}
		list.Sort(new CompTemplet.CompNIMT());
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null)
			{
				list2.Add(list[i].m_ItemMiscID);
			}
		}
		return list2;
	}

	private void OnClickUnitRewardSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		List<int> listNRGI = GetListNRGI();
		HashSet<int> rewardIDs = NKCUtil.GetRewardIDs(listNRGI, NKM_REWARD_TYPE.RT_UNIT);
		HashSet<int> rewardIDs2 = NKCUtil.GetRewardIDs(listNRGI, NKM_REWARD_TYPE.RT_OPERATOR);
		List<int> unitRewardIdList = GetUnitRewardIdList(rewardIDs, rewardIDs2);
		NKCUISlotListViewer.Instance.OpenRewardList(unitRewardIdList, NKM_REWARD_TYPE.RT_UNIT, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}

	private void OnClickEquipRewardSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		List<int> equipRewardIdList = GetEquipRewardIdList(NKCUtil.GetRewardIDs(GetListNRGI(), NKM_REWARD_TYPE.RT_EQUIP));
		NKCUISlotListViewer.Instance.OpenRewardList(equipRewardIdList, NKM_REWARD_TYPE.RT_EQUIP, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}

	private void OnClickMoldRewardSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		List<int> moldRewardIdList = GetMoldRewardIdList(NKCUtil.GetRewardIDs(GetListNRGI(), NKM_REWARD_TYPE.RT_MOLD));
		NKCUISlotListViewer.Instance.OpenRewardList(moldRewardIdList, NKM_REWARD_TYPE.RT_MOLD, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}

	private void OnClickMiscRewardSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		List<int> miscRewardIdList = GetMiscRewardIdList(NKCUtil.GetRewardIDs(GetListNRGI(), NKM_REWARD_TYPE.RT_MISC));
		NKCUISlotListViewer.Instance.OpenRewardList(miscRewardIdList, NKM_REWARD_TYPE.RT_MISC, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}

	private void ResetRewardSlotList()
	{
		int count = m_listRewardData.Count;
		for (int i = 0; i < count; i++)
		{
			m_listRewardData[i].SetSlotActive(isActive: false);
		}
		m_listSlotToShow.Clear();
	}

	private bool HasAnyReward(NKMStageTempletV2 stageTemplet, List<int> lstID)
	{
		if (lstID.Count > 0)
		{
			return true;
		}
		if (stageTemplet.GetFirstRewardData() != null && stageTemplet.GetFirstRewardData().Type != NKM_REWARD_TYPE.RT_NONE)
		{
			return true;
		}
		if (stageTemplet.OnetimeRewards.Count > 0)
		{
			return true;
		}
		if (stageTemplet.MainRewardData != null && stageTemplet.MainRewardData.rewardType != NKM_REWARD_TYPE.RT_NONE)
		{
			return true;
		}
		return false;
	}

	private bool HasAnyReward(NKMDefenceTemplet defenceTemplet, List<int> lstID)
	{
		if (lstID.Count > 0)
		{
			return true;
		}
		if (defenceTemplet.GetFirstRewardData() != null && defenceTemplet.GetFirstRewardData().Type != NKM_REWARD_TYPE.RT_NONE)
		{
			return true;
		}
		if (defenceTemplet.m_MainRewardData != null && defenceTemplet.m_MainRewardData.rewardType != NKM_REWARD_TYPE.RT_NONE)
		{
			return true;
		}
		if (defenceTemplet.m_DungeonMissionReward != null && defenceTemplet.m_DungeonMissionReward.Count > 0)
		{
			return true;
		}
		return false;
	}

	private void OnDestroy()
	{
		m_NKM_UI_OPERATION_POPUP_DROP_ITEM_Content = null;
		m_rectListContent = null;
		for (int i = 0; i < m_listRewardData.Count; i++)
		{
			m_listRewardData[i].Release();
		}
		m_listRewardData.Clear();
		m_listRewardData = null;
		m_listSlotToShow.Clear();
		m_listSlotToShow = null;
		m_strDungeonID = null;
	}
}
