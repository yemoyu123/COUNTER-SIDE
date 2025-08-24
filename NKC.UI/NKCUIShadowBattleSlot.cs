using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Mode;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIShadowBattleSlot : MonoBehaviour
{
	public delegate void OnTouchBattle(NKMShadowBattleTemplet bttleTemplet);

	[Header("battle Info")]
	public Text m_txtNumber;

	public Text m_txtBestTime;

	[Header("enemy info")]
	public Text m_txtEnemyName;

	public Text m_txtEnemyLv;

	public Image m_imgEnemy;

	public GameObject m_objBoss;

	[Header("rewards")]
	public Transform m_trReward;

	[Header("clear data")]
	public GameObject m_objClear;

	public Text m_txtClearName;

	public Text m_txtClearTime;

	[Header("state")]
	public GameObject m_objCurrent;

	public GameObject m_objLock;

	[Header("button")]
	public NKCUIComStateButton m_btnBattle;

	public GameObject m_objButtonEnable;

	public GameObject m_objButtonDisable;

	private OnTouchBattle dOnTouchBattle;

	private NKMShadowBattleTemplet m_templet;

	private List<NKCUISlot> m_lstRewardSlot = new List<NKCUISlot>();

	private List<int> m_lstRewardGroupID = new List<int>();

	public void Init()
	{
		m_btnBattle?.PointerClick.RemoveAllListeners();
		m_btnBattle?.PointerClick.AddListener(TouchBattle);
	}

	public void SetData(NKMShadowBattleTemplet battleTemplet, NKMPalaceDungeonData dungeonData, int current_order, OnTouchBattle onTouchBattle)
	{
		m_templet = battleTemplet;
		dOnTouchBattle = onTouchBattle;
		NKCUtil.SetLabelText(m_txtNumber, battleTemplet.BATTLE_ORDER.ToString("D2"));
		string msg = "-:--:--";
		string hexRGB = "#FFFFFF";
		if (dungeonData != null && dungeonData.bestTime > 0)
		{
			msg = NKCUtilString.GetTimeSpanString(TimeSpan.FromSeconds(dungeonData.bestTime));
			hexRGB = "#FCCE3E";
		}
		NKCUtil.SetLabelTextColor(m_txtBestTime, NKCUtil.GetColor(hexRGB));
		NKCUtil.SetLabelText(m_txtBestTime, msg);
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(battleTemplet.DUNGEON_ID);
		if (dungeonTempletBase == null)
		{
			Debug.LogError($"dungeonTempletBase  is null - battleTemplet.DUNGEON_ID : {battleTemplet.DUNGEON_ID}");
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		if (dungeonTempletBase != null)
		{
			NKCUtil.SetLabelText(m_txtEnemyName, dungeonTempletBase.GetDungeonName());
			string text = ((battleTemplet.PALACE_BATTLE_TYPE == PALACE_BATTLE_TYPE.PBT_BOSS) ? "DA1515" : "FCCE3E");
			NKCUtil.SetLabelText(m_txtEnemyLv, NKCUtilString.GET_SHADOW_BATTLE_ENEMY_LEVEL, text, dungeonTempletBase.m_DungeonLevel);
		}
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UNIT_FACE_CARD", battleTemplet.PALACE_BATTLE_IMG);
		NKCUtil.SetImageSprite(m_imgEnemy, orLoadAssetResource, bDisableIfSpriteNull: true);
		NKCUtil.SetGameobjectActive(m_objBoss, battleTemplet.PALACE_BATTLE_TYPE == PALACE_BATTLE_TYPE.PBT_BOSS);
		NKCUtil.SetGameobjectActive(m_objLock, battleTemplet.BATTLE_ORDER > current_order);
		NKCUtil.SetGameobjectActive(m_objCurrent, battleTemplet.BATTLE_ORDER == current_order);
		bool flag = battleTemplet.BATTLE_ORDER < current_order;
		NKCUtil.SetGameobjectActive(m_objClear, flag);
		NKCUtil.SetGameobjectActive(m_objButtonEnable, !flag);
		NKCUtil.SetGameobjectActive(m_objButtonDisable, flag);
		if (flag)
		{
			NKCUtil.SetLabelText(m_txtClearName, NKCUtilString.GET_SHADOW_BATTLE_CLEAR_NUM, battleTemplet.BATTLE_ORDER);
			msg = NKCUtilString.GetTimeSpanString(TimeSpan.FromSeconds(dungeonData.recentTime));
			NKCUtil.SetLabelText(m_txtClearTime, msg);
		}
		List<int> lstRewardGroup = dungeonTempletBase.m_listDungeonReward.Select((DungeonReward v) => v.m_RewardGroupID).ToList();
		List<NKMRewardTemplet> lstOneRewardTemplet = new List<NKMRewardTemplet>();
		m_lstRewardGroupID.Clear();
		DivisionReward(lstRewardGroup, ref lstOneRewardTemplet, ref m_lstRewardGroupID);
		bool flag2 = NKCUtil.CheckExistRewardType(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_UNIT) || NKCUtil.CheckExistRewardType(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_OPERATOR);
		bool flag3 = NKCUtil.CheckExistRewardType(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_EQUIP);
		bool flag4 = NKCUtil.CheckExistRewardType(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_MOLD);
		bool flag5 = NKCUtil.CheckExistRewardType(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_MISC);
		int count = m_lstRewardSlot.Count;
		int num = lstOneRewardTemplet.Count;
		if (dungeonTempletBase.m_RewardUserEXP > 0)
		{
			num++;
		}
		if (dungeonTempletBase.m_RewardCredit_Min > 0)
		{
			num++;
		}
		if (dungeonTempletBase.m_RewardEternium_Min > 0)
		{
			num++;
		}
		if (dungeonTempletBase.m_RewardInformation_Min > 0)
		{
			num++;
		}
		if (flag2)
		{
			num++;
		}
		if (flag3)
		{
			num++;
		}
		if (flag4)
		{
			num++;
		}
		if (flag5)
		{
			num++;
		}
		for (int num2 = count; num2 < num; num2++)
		{
			NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_trReward);
			newInstance.Init();
			m_lstRewardSlot.Add(newInstance);
		}
		int num3 = 0;
		if (dungeonTempletBase.m_RewardUserEXP > 0)
		{
			SetMiscSlot(m_lstRewardSlot[num3++], 501, dungeonTempletBase.m_RewardUserEXP);
		}
		if (dungeonTempletBase.m_RewardCredit_Min > 0)
		{
			SetMiscSlot(m_lstRewardSlot[num3++], 1, dungeonTempletBase.m_RewardCredit_Min);
		}
		if (dungeonTempletBase.m_RewardEternium_Min > 0)
		{
			SetMiscSlot(m_lstRewardSlot[num3++], 2, dungeonTempletBase.m_RewardEternium_Min);
		}
		if (dungeonTempletBase.m_RewardInformation_Min > 0)
		{
			SetMiscSlot(m_lstRewardSlot[num3++], 3, dungeonTempletBase.m_RewardInformation_Min);
		}
		for (int num4 = 0; num4 < lstOneRewardTemplet.Count; num4++)
		{
			NKCUISlot nKCUISlot = m_lstRewardSlot[num3];
			NKMRewardTemplet nKMRewardTemplet = lstOneRewardTemplet[num4];
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(new NKMRewardInfo
			{
				rewardType = nKMRewardTemplet.m_eRewardType,
				ID = nKMRewardTemplet.m_RewardID,
				Count = nKMRewardTemplet.m_Quantity_Min
			});
			nKCUISlot.SetData(data);
			NKCUtil.SetGameobjectActive(nKCUISlot, bValue: true);
			num3++;
		}
		if (flag2)
		{
			int maxGradeInRewardGroups = NKCUtil.GetMaxGradeInRewardGroups(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_UNIT);
			int maxGradeInRewardGroups2 = NKCUtil.GetMaxGradeInRewardGroups(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_OPERATOR);
			SetRandomSlot(m_lstRewardSlot[num3++], 901, OnClickRandomUnitSlot, Mathf.Max(maxGradeInRewardGroups, maxGradeInRewardGroups2));
		}
		if (flag3)
		{
			int maxGradeInRewardGroups3 = NKCUtil.GetMaxGradeInRewardGroups(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_EQUIP);
			SetRandomSlot(m_lstRewardSlot[num3++], 902, OnClickRandomEquipSlot, maxGradeInRewardGroups3);
		}
		if (flag4)
		{
			int maxGradeInRewardGroups4 = NKCUtil.GetMaxGradeInRewardGroups(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_MOLD);
			SetRandomSlot(m_lstRewardSlot[num3++], 904, OnClickRandomMoldSlot, maxGradeInRewardGroups4);
		}
		if (flag5)
		{
			int maxGradeInRewardGroups5 = NKCUtil.GetMaxGradeInRewardGroups(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_MISC);
			SetRandomSlot(m_lstRewardSlot[num3++], 903, OnClickRandomMiscSlot, maxGradeInRewardGroups5);
		}
		for (; num3 < m_lstRewardSlot.Count; num3++)
		{
			NKCUtil.SetGameobjectActive(m_lstRewardSlot[num3], bValue: false);
		}
	}

	private void TouchBattle()
	{
		dOnTouchBattle?.Invoke(m_templet);
	}

	private void DivisionReward(List<int> lstRewardGroup, ref List<NKMRewardTemplet> lstOneRewardTemplet, ref List<int> lstRewardGroupID)
	{
		for (int i = 0; i < lstRewardGroup.Count; i++)
		{
			NKMRewardGroupTemplet rewardGroup = NKMRewardManager.GetRewardGroup(lstRewardGroup[i]);
			if (rewardGroup != null)
			{
				if (rewardGroup.List.Count == 1)
				{
					lstOneRewardTemplet.Add(rewardGroup.List[0]);
				}
				else if (!lstRewardGroupID.Contains(lstRewardGroup[i]))
				{
					lstRewardGroupID.Add(lstRewardGroup[i]);
				}
			}
		}
	}

	private void SetMiscSlot(NKCUISlot slot, int miscItemID, int count)
	{
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(new NKMItemMiscData(miscItemID, count, 0L));
		slot.SetData(data, bShowName: false, bShowNumber: false, bEnableLayoutElement: true, null);
		slot.SetOpenItemBoxOnClick();
		NKCUtil.SetGameobjectActive(slot, bValue: true);
	}

	private void SetRandomSlot(NKCUISlot slot, int miscItemID, NKCUISlot.OnClick onClick, int maxGrade)
	{
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(new NKMItemMiscData(miscItemID, 1L, 0L));
		slot.SetData(data, bShowName: false, bShowNumber: false, bEnableLayoutElement: true, onClick);
		slot.SetBackGround(maxGrade);
		NKCUtil.SetGameobjectActive(slot, bValue: true);
	}

	private void OnClickRandomUnitSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		HashSet<int> rewardIDs = NKCUtil.GetRewardIDs(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_UNIT);
		HashSet<int> rewardIDs2 = NKCUtil.GetRewardIDs(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_OPERATOR);
		List<NKMUnitTempletBase> list = new List<NKMUnitTempletBase>();
		foreach (int item in rewardIDs)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(item);
			if (unitTempletBase != null)
			{
				list.Add(unitTempletBase);
			}
		}
		foreach (int item2 in rewardIDs2)
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(item2);
			if (unitTempletBase2 != null)
			{
				list.Add(unitTempletBase2);
			}
		}
		list.Sort(new CompTemplet.CompNUTB());
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add(list[i].m_UnitID);
		}
		NKCUISlotListViewer.Instance.OpenRewardList(list2, NKM_REWARD_TYPE.RT_UNIT, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}

	private void OnClickRandomEquipSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		HashSet<int> rewardIDs = NKCUtil.GetRewardIDs(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_EQUIP);
		List<NKMEquipTemplet> list = new List<NKMEquipTemplet>();
		foreach (int item in rewardIDs)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(item);
			if (equipTemplet != null)
			{
				list.Add(equipTemplet);
			}
		}
		list.Sort(new CompTemplet.CompNET());
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add(list[i].m_ItemEquipID);
		}
		NKCUISlotListViewer.Instance.OpenRewardList(list2, NKM_REWARD_TYPE.RT_EQUIP, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}

	private void OnClickRandomMoldSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		HashSet<int> rewardIDs = NKCUtil.GetRewardIDs(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_MOLD);
		List<NKMItemMoldTemplet> list = new List<NKMItemMoldTemplet>();
		foreach (int item in rewardIDs)
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(item);
			if (itemMoldTempletByID != null)
			{
				list.Add(itemMoldTempletByID);
			}
		}
		list.Sort(new CompTemplet.CompNMT());
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add(list[i].m_MoldID);
		}
		NKCUISlotListViewer.Instance.OpenRewardList(list2, NKM_REWARD_TYPE.RT_MOLD, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}

	private void OnClickRandomMiscSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		HashSet<int> rewardIDs = NKCUtil.GetRewardIDs(m_lstRewardGroupID, NKM_REWARD_TYPE.RT_MISC);
		List<NKMItemMiscTemplet> list = new List<NKMItemMiscTemplet>();
		foreach (int item in rewardIDs)
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
			list2.Add(list[i].m_ItemMiscID);
		}
		NKCUISlotListViewer.Instance.OpenRewardList(list2, NKM_REWARD_TYPE.RT_MISC, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}
}
