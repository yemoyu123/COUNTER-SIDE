using System;
using System.Collections.Generic;
using ClientPacket.Raid;
using ClientPacket.WorldMap;
using Cs.Math;
using DG.Tweening;
using NKC.PacketHandler;
using NKC.UI.Component;
using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIRaidRightSide : NKCUIInstantiatable
{
	public delegate void onClickAttackBtn(long raidUID, List<int> _buffs, int reqItemID, int reqItemCount, bool bIsTryAssist, bool isPracticeMode);

	public enum NKC_RAID_SUB_MENU_TYPE
	{
		NRSMT_REMAIN_TIME,
		NRSMT_SUPPORT_EQUIP
	}

	public enum NKC_RAID_SUB_BUTTON_TYPE
	{
		NRSBT_READY,
		NRSBT_ATTACK,
		NRSBT_EXIT
	}

	private onClickAttackBtn m_dOnClickAttackBtn;

	[Header("기본 정보")]
	public Text m_lbLevel;

	public Text m_lbName;

	public NKCUIComStateButton m_btnGuide;

	public NKCUIComStateButton m_csbtnInfo;

	public NKCUIComStateButton m_csbtnEnemy;

	public Image m_imgBossHP;

	public Text m_lbRemainHP;

	public Text m_lbMyAccumDmg;

	public Text m_lbHeightDmg;

	public GameObject m_objTeamOnlyData;

	public GameObject m_objBossDesc;

	public Text m_lbBossDesc;

	public GameObject m_objAttendLimit;

	public Text m_lbAttendLimit;

	public Image m_imgEventPointColor;

	[Header("점수")]
	public GameObject m_objRaidPoint;

	public Text m_lbWinPoint;

	public GameObject m_objWinPoint_Entry;

	public Text m_lbWinPoint_Entry;

	public Text m_lbLosePoint;

	public GameObject m_objLosePoint_Entry;

	public Text m_lbLosePoint_Entry;

	[Header("보상")]
	public GameObject m_objReward;

	public LoopScrollRect m_lsrReward;

	public NKCUISlot m_pfbUISlot;

	[Header("지원 장비")]
	public GameObject m_objSupportEquip;

	public NKCUIComToggle m_tglEquip1;

	public NKCUIComToggle m_tglEquip2;

	public NKCUIComToggle m_tglEquip3;

	public NKCUIComToggle m_tglEquip4;

	public Text m_lbEquip1LV;

	public Text m_lbEquip2LV;

	public Text m_lbEquip3LV;

	public Text m_lbEquip4LV;

	public Text m_lbEquip1LVOff;

	public Text m_lbEquip2LVOff;

	public Text m_lbEquip3LVOff;

	public Text m_lbEquip4LVOff;

	public Text m_lbEquip1Cost;

	public Text m_lbEquip2Cost;

	public Text m_lbEquip3Cost;

	public Text m_lbEquip4Cost;

	public Image m_imgEquip1Cost;

	public Image m_imgEquip2Cost;

	public Image m_imgEquip3Cost;

	public Image m_imgEquip4Cost;

	[Header("남은 횟수")]
	public GameObject m_objRemainCount;

	public Text m_lbRemainCount;

	[Header("맨 아래 버튼 모음")]
	public NKCUIComStateButton m_csbtnSweep;

	public GameObject m_objSweepON;

	public GameObject m_objSweepOFF;

	public Image m_imgSweepCost;

	public NKCComTMPUIText m_lbSweepCost;

	public NKCUIComStateButton m_csbtnReady;

	public NKCUIComStateButton m_csbtnClear;

	public Image m_imgAttackCost;

	public Text m_lbAttackCost;

	public NKCUIComStateButton m_csbtnExit;

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_WORLD_MAP_RAID";

	private const string UI_ASSET_NAME = "NKM_UI_WORLD_MAP_RAID_RIGHT";

	private long m_RaidUID;

	private int m_RaidID;

	private List<int> m_listNRGI = new List<int>();

	private float m_fNextUpdateTime;

	private NKMRaidDetailData m_cNKMRaidDetailData;

	private bool m_bLockExitBtn;

	private bool m_bFirstOpen = true;

	public NKMRaidTemplet GetRaidTemplet()
	{
		return NKMRaidTemplet.Find(m_RaidID);
	}

	public static NKCUIRaidRightSide OpenInstance(Transform trParent, onClickAttackBtn _onClickAttackBtn = null)
	{
		NKCUIRaidRightSide nKCUIRaidRightSide = NKCUIInstantiatable.OpenInstance<NKCUIRaidRightSide>("AB_UI_NKM_UI_WORLD_MAP_RAID", "NKM_UI_WORLD_MAP_RAID_RIGHT", trParent);
		if (nKCUIRaidRightSide != null)
		{
			nKCUIRaidRightSide.Init(_onClickAttackBtn);
		}
		return nKCUIRaidRightSide;
	}

	public void CloseInstance()
	{
		m_lbAttackCost.DOKill();
		CloseInstance("AB_UI_NKM_UI_WORLD_MAP_RAID", "NKM_UI_WORLD_MAP_RAID_RIGHT");
	}

	public void Init(onClickAttackBtn _onClickAttackBtn = null)
	{
		m_dOnClickAttackBtn = _onClickAttackBtn;
		NKCUtil.SetBindFunction(m_csbtnSweep, OnClickSweep);
		NKCUtil.SetHotkey(m_csbtnSweep, HotkeyEventType.RotateLeft);
		NKCUtil.SetBindFunction(m_csbtnReady, OnClickReadyBtn);
		NKCUtil.SetHotkey(m_csbtnReady, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(m_csbtnClear, OnClickAttackBtn);
		NKCUtil.SetHotkey(m_csbtnClear, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(m_csbtnExit, OnClickExitBtn);
		if (null != m_lsrReward)
		{
			m_lsrReward.dOnGetObject += GetSlot;
			m_lsrReward.dOnReturnObject += ReturnSlot;
			m_lsrReward.dOnProvideData += ProvideSlotData;
		}
		NKCUtil.SetToggleValueChangedDelegate(m_tglEquip1, OnChangedEquips);
		NKCUtil.SetToggleValueChangedDelegate(m_tglEquip2, OnChangedEquips);
		NKCUtil.SetToggleValueChangedDelegate(m_tglEquip3, OnChangedEquips);
		NKCUtil.SetToggleValueChangedDelegate(m_tglEquip4, OnChangedEquips);
		NKCUtil.SetBindFunction(m_csbtnInfo, OnClickInfoBtn);
		NKCUtil.SetBindFunction(m_btnGuide, OnClickGuide);
		NKCUtil.SetBindFunction(m_csbtnEnemy, OnClickInfoBtn);
	}

	private void OnChangedEquips(bool bChanged)
	{
		UpdateAttackCostUI();
	}

	private void OnClickUnitRewardSlot(bool bIsFinder)
	{
		List<int> listNRGI = m_listNRGI;
		HashSet<int> rewardIDs = NKCUtil.GetRewardIDs(listNRGI, NKM_REWARD_TYPE.RT_UNIT);
		HashSet<int> rewardIDs2 = NKCUtil.GetRewardIDs(listNRGI, NKM_REWARD_TYPE.RT_OPERATOR);
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
			if (list[i] != null)
			{
				list2.Add(list[i].m_UnitID);
			}
		}
		NKCUISlotListViewer.Instance.OpenRewardList(list2, NKM_REWARD_TYPE.RT_UNIT, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}

	private void OnClickEquipRewardSlot(bool bIsFinder)
	{
		HashSet<int> rewardIDs = NKCUtil.GetRewardIDs(m_listNRGI, NKM_REWARD_TYPE.RT_EQUIP);
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
			if (list[i] != null)
			{
				list2.Add(list[i].m_ItemEquipID);
			}
		}
		NKCUISlotListViewer.Instance.OpenRewardList(list2, NKM_REWARD_TYPE.RT_EQUIP, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}

	private void OnClickMoldRewardSlot(bool bIsFinder)
	{
		HashSet<int> rewardIDs = NKCUtil.GetRewardIDs(m_listNRGI, NKM_REWARD_TYPE.RT_MOLD);
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
			if (list[i] != null)
			{
				list2.Add(list[i].m_MoldID);
			}
		}
		NKCUISlotListViewer.Instance.OpenRewardList(list2, NKM_REWARD_TYPE.RT_MOLD, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}

	private void OnClickMiscRewardSlot(bool bIsFinder)
	{
		HashSet<int> rewardIDs = NKCUtil.GetRewardIDs(m_listNRGI, NKM_REWARD_TYPE.RT_MISC);
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
			if (list[i] != null)
			{
				list2.Add(list[i].m_ItemMiscID);
			}
		}
		NKCUISlotListViewer.Instance.OpenRewardList(list2, NKM_REWARD_TYPE.RT_MISC, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_TITLE, NKCUtilString.GET_STRING_REWARD_LIST_POPUP_DESC);
	}

	private RectTransform GetSlot(int index)
	{
		NKCUISlot nKCUISlot = UnityEngine.Object.Instantiate(m_pfbUISlot);
		nKCUISlot.Init();
		return nKCUISlot.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform tr)
	{
		tr.SetParent(base.transform);
		UnityEngine.Object.Destroy(tr.gameObject);
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		NKCUISlot component = tr.GetComponent<NKCUISlot>();
		_ = NKCScenManager.CurrentUserData().m_UserUID;
		bool num = NKCUtil.CheckExistRewardType(m_listNRGI, NKM_REWARD_TYPE.RT_UNIT) || NKCUtil.CheckExistRewardType(m_listNRGI, NKM_REWARD_TYPE.RT_OPERATOR);
		bool flag = NKCUtil.CheckExistRewardType(m_listNRGI, NKM_REWARD_TYPE.RT_EQUIP);
		bool flag2 = NKCUtil.CheckExistRewardType(m_listNRGI, NKM_REWARD_TYPE.RT_MOLD);
		bool flag3 = NKCUtil.CheckExistRewardType(m_listNRGI, NKM_REWARD_TYPE.RT_MISC);
		int num2 = 0;
		if (num)
		{
			if (num2 == idx)
			{
				int maxGradeInRewardGroups = NKCUtil.GetMaxGradeInRewardGroups(m_listNRGI, NKM_REWARD_TYPE.RT_UNIT);
				int maxGradeInRewardGroups2 = NKCUtil.GetMaxGradeInRewardGroups(m_listNRGI, NKM_REWARD_TYPE.RT_OPERATOR);
				SetSlotDataByRandomItem(component, 901, delegate
				{
					OnClickUnitRewardSlot(bIsFinder: true);
				}, Mathf.Max(maxGradeInRewardGroups, maxGradeInRewardGroups2));
				return;
			}
			num2++;
		}
		if (flag)
		{
			if (num2 == idx)
			{
				int maxGradeInRewardGroups3 = NKCUtil.GetMaxGradeInRewardGroups(m_listNRGI, NKM_REWARD_TYPE.RT_EQUIP);
				SetSlotDataByRandomItem(component, 902, delegate
				{
					OnClickEquipRewardSlot(bIsFinder: true);
				}, maxGradeInRewardGroups3);
				return;
			}
			num2++;
		}
		if (flag2)
		{
			if (num2 == idx)
			{
				int maxGradeInRewardGroups4 = NKCUtil.GetMaxGradeInRewardGroups(m_listNRGI, NKM_REWARD_TYPE.RT_MOLD);
				SetSlotDataByRandomItem(component, 904, delegate
				{
					OnClickMoldRewardSlot(bIsFinder: true);
				}, maxGradeInRewardGroups4);
				return;
			}
			num2++;
		}
		if (!flag3)
		{
			return;
		}
		if (num2 == idx)
		{
			int maxGradeInRewardGroups5 = NKCUtil.GetMaxGradeInRewardGroups(m_listNRGI, NKM_REWARD_TYPE.RT_MISC);
			SetSlotDataByRandomItem(component, 903, delegate
			{
				OnClickMiscRewardSlot(bIsFinder: true);
			}, maxGradeInRewardGroups5);
		}
		else
		{
			num2++;
		}
	}

	private void SetSlotDataByRandomItem(NKCUISlot cSlot, int miscItemID, NKCUISlot.OnClick onClick, int maxGrade)
	{
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(new NKMItemMiscData(miscItemID, 1L, 0L));
		cSlot.SetData(data, bShowName: false, bShowNumber: false, bEnableLayoutElement: true, onClick);
		cSlot.SetBackGround(maxGrade);
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private int GetFinalBuffCost(NKMWorldMapCityData cityData, int orgCost)
	{
		if (cityData == null)
		{
			return orgCost;
		}
		int val = (int)cityData.CalcBuildStat(NKM_CITY_BUILDING_STAT.CBS_RAID_DEFENCE_COST_REDUCE_RATE, orgCost);
		val = Math.Min(orgCost, val);
		return orgCost - val;
	}

	private void SetEquipUI(NKMRaidTemplet cNKMRaidTemplet, NKMRaidDetailData cNKMRaidDetailData)
	{
		if (cNKMRaidDetailData != null)
		{
			int num = 1;
			NKMWorldMapCityData cityData = NKCScenManager.CurrentUserData().m_WorldmapData.GetCityData(cNKMRaidDetailData.cityID);
			if (cityData != null)
			{
				num = (int)cityData.CalcBuildStat(NKM_CITY_BUILDING_STAT.CBS_RAID_DEFENCE_LEVEL, 1f);
				num = Math.Min(5, num);
			}
			m_tglEquip1.Select(bSelect: false, bForce: true);
			m_tglEquip2.Select(bSelect: false, bForce: true);
			m_tglEquip3.Select(bSelect: false, bForce: true);
			m_tglEquip4.Select(bSelect: false, bForce: true);
			NKMRaidBuffTemplet nKMRaidBuffTemplet = NKMRaidBuffTemplet.Find(1, num);
			if (nKMRaidBuffTemplet != null)
			{
				m_lbEquip1Cost.text = GetFinalBuffCost(cityData, nKMRaidBuffTemplet.RaidBuffCost).ToString();
			}
			else
			{
				m_lbEquip1Cost.text = "???";
			}
			nKMRaidBuffTemplet = NKMRaidBuffTemplet.Find(2, num);
			if (nKMRaidBuffTemplet != null)
			{
				m_lbEquip2Cost.text = GetFinalBuffCost(cityData, nKMRaidBuffTemplet.RaidBuffCost).ToString();
			}
			else
			{
				m_lbEquip2Cost.text = "???";
			}
			nKMRaidBuffTemplet = NKMRaidBuffTemplet.Find(3, num);
			if (nKMRaidBuffTemplet != null)
			{
				m_lbEquip3Cost.text = GetFinalBuffCost(cityData, nKMRaidBuffTemplet.RaidBuffCost).ToString();
			}
			else
			{
				m_lbEquip3Cost.text = "???";
			}
			nKMRaidBuffTemplet = NKMRaidBuffTemplet.Find(4, num);
			if (nKMRaidBuffTemplet != null)
			{
				m_lbEquip4Cost.text = GetFinalBuffCost(cityData, nKMRaidBuffTemplet.RaidBuffCost).ToString();
			}
			else
			{
				m_lbEquip4Cost.text = "???";
			}
			string text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, num);
			m_lbEquip1LV.text = text;
			m_lbEquip2LV.text = text;
			m_lbEquip3LV.text = text;
			m_lbEquip4LV.text = text;
			m_lbEquip1LVOff.text = text;
			m_lbEquip2LVOff.text = text;
			m_lbEquip3LVOff.text = text;
			m_lbEquip4LVOff.text = text;
		}
	}

	public void SetUI(long raidUID, NKC_RAID_SUB_MENU_TYPE eNKC_RAID_SUB_MENU_TYPE = NKC_RAID_SUB_MENU_TYPE.NRSMT_REMAIN_TIME, NKC_RAID_SUB_BUTTON_TYPE eNKC_RAID_SUB_BUTTON_TYPE = NKC_RAID_SUB_BUTTON_TYPE.NRSBT_READY)
	{
		m_RaidUID = raidUID;
		m_bLockExitBtn = false;
		m_cNKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_RaidUID);
		if (m_cNKMRaidDetailData == null)
		{
			return;
		}
		NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(m_cNKMRaidDetailData.stageID);
		if (nKMRaidTemplet == null)
		{
			return;
		}
		m_RaidID = nKMRaidTemplet.Key;
		if (m_bFirstOpen)
		{
			m_bFirstOpen = false;
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			m_lsrReward.PrepareCells();
		}
		NKCUtil.SetRaidEventPoint(m_imgEventPointColor, nKMRaidTemplet);
		m_lbLevel.text = nKMRaidTemplet.RaidLevel.ToString();
		m_lbName.text = nKMRaidTemplet.DungeonTempletBase.GetDungeonName();
		NKCUtil.SetGameobjectActive(m_objAttendLimit, nKMRaidTemplet.AttendLimit > 0);
		if (nKMRaidTemplet.AttendLimit > 0)
		{
			NKCUtil.SetLabelText(m_lbAttendLimit, $"{m_cNKMRaidDetailData.raidJoinDataList.Count}/{nKMRaidTemplet.AttendLimit}");
		}
		NKCUtil.SetGameobjectActive(m_objTeamOnlyData, nKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_RAID);
		NKCUtil.SetGameobjectActive(m_objBossDesc, nKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID);
		if (nKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
		{
			NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(nKMRaidTemplet.DungeonTempletBase.m_DungeonID);
			if (dungeonTemplet != null)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(dungeonTemplet.m_BossUnitStrID);
				if (unitTempletBase != null)
				{
					m_lbBossDesc.text = unitTempletBase.GetUnitDesc();
				}
			}
		}
		float num = 0f;
		if (m_cNKMRaidDetailData.maxHP.IsNearlyZero())
		{
			m_imgBossHP.fillAmount = 0f;
		}
		else
		{
			num = m_cNKMRaidDetailData.curHP / m_cNKMRaidDetailData.maxHP;
			m_imgBossHP.fillAmount = num;
		}
		m_lbRemainHP.text = string.Format("{0} ({1:0.##}%)", ((int)m_cNKMRaidDetailData.curHP).ToString("N0"), num * 100f);
		NKMRaidJoinData nKMRaidJoinData = m_cNKMRaidDetailData.FindJoinData(NKCScenManager.CurrentUserData().m_UserUID);
		float num2 = 0f;
		if (nKMRaidJoinData != null)
		{
			num2 = nKMRaidJoinData.damage;
		}
		NKCUtil.SetLabelText(m_lbMyAccumDmg, string.Format("{0} ({1:0.##}%)", ((int)num2).ToString("N0"), GetDamageRate(num2) * 100f));
		NKCUtil.SetGameobjectActive(m_btnGuide, !string.IsNullOrEmpty(nKMRaidTemplet.GuideShortCut));
		NKCUtil.SetGameobjectActive(m_objWinPoint_Entry, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLosePoint_Entry, bValue: false);
		NKCUtil.SetLabelText(m_lbWinPoint, nKMRaidTemplet.RewardRaidPoint_Victory.ToString("N0"));
		NKCUtil.SetLabelText(m_lbLosePoint, nKMRaidTemplet.RewardRaidPoint_Fail.ToString("N0"));
		NKMRaidSeasonTemplet nowSeasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
		bool flag = nowSeasonTemplet != null && nowSeasonTemplet.RaidSeasonId == m_cNKMRaidDetailData.seasonID;
		int num3 = (flag ? ((int)NKCRaidSeasonManager.RaidSeason.highestDamage) : 0);
		NKCUtil.SetLabelText(m_lbHeightDmg, string.Format("{0} ({1:0.##}%)", num3.ToString("N0"), GetDamageRate(NKCRaidSeasonManager.RaidSeason.highestDamage) * 100f));
		NKCUtil.SetGameobjectActive(m_objRaidPoint, flag && eNKC_RAID_SUB_MENU_TYPE == NKC_RAID_SUB_MENU_TYPE.NRSMT_REMAIN_TIME);
		NKCUtil.SetGameobjectActive(m_objReward, bValue: true);
		NKCUtil.SetGameobjectActive(m_lsrReward.content, bValue: true);
		NKCUtil.SetGameobjectActive(m_objSupportEquip, eNKC_RAID_SUB_MENU_TYPE == NKC_RAID_SUB_MENU_TYPE.NRSMT_SUPPORT_EQUIP);
		if (eNKC_RAID_SUB_MENU_TYPE == NKC_RAID_SUB_MENU_TYPE.NRSMT_SUPPORT_EQUIP)
		{
			SetEquipUI(nKMRaidTemplet, m_cNKMRaidDetailData);
		}
		NKCUtil.SetGameobjectActive(m_csbtnReady.gameObject, eNKC_RAID_SUB_BUTTON_TYPE == NKC_RAID_SUB_BUTTON_TYPE.NRSBT_READY);
		NKCUtil.SetGameobjectActive(m_csbtnClear.gameObject, eNKC_RAID_SUB_BUTTON_TYPE == NKC_RAID_SUB_BUTTON_TYPE.NRSBT_ATTACK);
		NKCUtil.SetGameobjectActive(m_csbtnExit.gameObject, eNKC_RAID_SUB_BUTTON_TYPE == NKC_RAID_SUB_BUTTON_TYPE.NRSBT_EXIT);
		bool bValue = (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.RAID_SWEEP) && eNKC_RAID_SUB_BUTTON_TYPE == NKC_RAID_SUB_BUTTON_TYPE.NRSBT_READY) || eNKC_RAID_SUB_BUTTON_TYPE == NKC_RAID_SUB_BUTTON_TYPE.NRSBT_ATTACK;
		NKCUtil.SetGameobjectActive(m_objRemainCount, bValue);
		NKCUtil.SetGameobjectActive(m_csbtnSweep, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.RAID_SWEEP));
		UpdateSweepUI();
		int num4 = nKMRaidJoinData?.tryCount ?? 0;
		m_lbRemainCount.text = string.Format(NKCUtilString.GET_STRING_RAID_REMAIN_COUNT_ONE_PARAM, nKMRaidTemplet.RaidTryCount - num4);
		int num5 = 0;
		bool flag2 = false;
		flag2 = !(m_cNKMRaidDetailData.curHP > 0f) || !NKCSynchronizedTime.IsFinished(m_cNKMRaidDetailData.expireDate);
		m_listNRGI = GetListNRGI(flag2);
		bool num6 = NKCUtil.CheckExistRewardType(m_listNRGI, NKM_REWARD_TYPE.RT_UNIT) || NKCUtil.CheckExistRewardType(m_listNRGI, NKM_REWARD_TYPE.RT_OPERATOR);
		bool flag3 = NKCUtil.CheckExistRewardType(m_listNRGI, NKM_REWARD_TYPE.RT_EQUIP);
		bool flag4 = NKCUtil.CheckExistRewardType(m_listNRGI, NKM_REWARD_TYPE.RT_MOLD);
		bool flag5 = NKCUtil.CheckExistRewardType(m_listNRGI, NKM_REWARD_TYPE.RT_MISC);
		if (num6)
		{
			num5++;
		}
		if (flag3)
		{
			num5++;
		}
		if (flag4)
		{
			num5++;
		}
		if (flag5)
		{
			num5++;
		}
		m_lsrReward.TotalCount = num5;
		m_lsrReward.SetIndexPosition(0);
		Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(nKMRaidTemplet?.StageReqItemID ?? 3);
		NKCUtil.SetImageSprite(m_imgAttackCost, orLoadMiscItemSmallIcon);
		NKCUtil.SetImageSprite(m_imgEquip1Cost, orLoadMiscItemSmallIcon);
		NKCUtil.SetImageSprite(m_imgEquip2Cost, orLoadMiscItemSmallIcon);
		NKCUtil.SetImageSprite(m_imgEquip3Cost, orLoadMiscItemSmallIcon);
		NKCUtil.SetImageSprite(m_imgEquip4Cost, orLoadMiscItemSmallIcon);
		UpdateAttackCostUI();
	}

	private float GetDamageRate(float damage)
	{
		float num = 0f;
		if (m_cNKMRaidDetailData.maxHP.IsNearlyZero())
		{
			return 0f;
		}
		return damage / m_cNKMRaidDetailData.maxHP;
	}

	private void UpdateSweepUI()
	{
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.RAID_SWEEP))
		{
			return;
		}
		NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_RaidUID);
		if (nKMRaidDetailData != null)
		{
			bool flag = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().CheckCompletableRaid(m_RaidUID);
			NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(nKMRaidDetailData.stageID);
			if (nKMRaidTemplet == null)
			{
				Debug.Log("<color=red>NKCUIRaidRightSide::UpdateSweepUI - RaidTemplet null : {raidData.stageID}</color>");
				NKCUtil.SetGameobjectActive(m_objSweepON, bValue: false);
				NKCUtil.SetGameobjectActive(m_objSweepOFF, bValue: true);
				return;
			}
			_ = nKMRaidDetailData.userUID;
			_ = NKCScenManager.CurrentUserData().m_UserUID;
			int stageReqItemID = nKMRaidTemplet.StageReqItemID;
			int num = ((NKCRaidSeasonManager.RaidSeason.monthlyPoint >= NKMCommonConst.RaidPointReqItemDecline) ? nKMRaidTemplet.DeclineStageReqItemCount : nKMRaidTemplet.StageReqItemCount);
			_ = NKCScenManager.CurrentUserData().m_InventoryData.GetItemMisc(stageReqItemID).TotalCount;
			Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(stageReqItemID);
			NKCUtil.SetImageSprite(m_imgSweepCost, orLoadMiscItemSmallIcon);
			NKCUtil.SetLabelText(m_lbSweepCost, num.ToString());
			int num2 = m_cNKMRaidDetailData.FindJoinData(NKCScenManager.CurrentUserData().m_UserUID)?.tryCount ?? 0;
			_ = nKMRaidTemplet.RaidTryCount;
			NKMRaidSeasonTemplet nowSeasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
			bool flag2 = nowSeasonTemplet != null && nowSeasonTemplet.RaidSeasonId == m_cNKMRaidDetailData.seasonID;
			bool flag3 = NKCRaidSeasonManager.RaidSeason.highestDamage >= nKMRaidTemplet.RaidDamageBasis && flag2 && !flag;
			NKCUtil.SetGameobjectActive(m_objSweepON, flag3);
			NKCUtil.SetGameobjectActive(m_objSweepOFF, !flag3);
		}
	}

	private void UpdateBtns()
	{
		m_cNKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_RaidUID);
		if (m_cNKMRaidDetailData != null && (m_cNKMRaidDetailData.curHP <= 0f || NKCSynchronizedTime.IsFinished(m_cNKMRaidDetailData.expireDate)))
		{
			NKCUtil.SetGameobjectActive(m_csbtnReady.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnClear.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnExit.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_objRemainCount, bValue: false);
		}
	}

	public int GetCostByCurrSetting()
	{
		m_cNKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_RaidUID);
		if (m_cNKMRaidDetailData == null)
		{
			return 0;
		}
		NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(m_cNKMRaidDetailData.stageID);
		if (nKMRaidTemplet == null)
		{
			return 0;
		}
		int num = ((NKCRaidSeasonManager.RaidSeason.monthlyPoint >= NKMCommonConst.RaidPointReqItemDecline) ? nKMRaidTemplet.DeclineStageReqItemCount : nKMRaidTemplet.StageReqItemCount);
		if (m_tglEquip1.m_bChecked)
		{
			int result = 0;
			if (int.TryParse(m_lbEquip1Cost.text, out result))
			{
				num += result;
			}
		}
		if (m_tglEquip2.m_bChecked)
		{
			int result2 = 0;
			if (int.TryParse(m_lbEquip2Cost.text, out result2))
			{
				num += result2;
			}
		}
		if (m_tglEquip3.m_bChecked)
		{
			int result3 = 0;
			if (int.TryParse(m_lbEquip3Cost.text, out result3))
			{
				num += result3;
			}
		}
		if (m_tglEquip4.m_bChecked)
		{
			int result4 = 0;
			if (int.TryParse(m_lbEquip4Cost.text, out result4))
			{
				num += result4;
			}
		}
		return num;
	}

	private void UpdateAttackCostUI()
	{
		if (m_csbtnClear.gameObject.activeSelf)
		{
			int costByCurrSetting = GetCostByCurrSetting();
			m_lbAttackCost.DOText(costByCurrSetting.ToString(), 0.4f, richTextEnabled: true, ScrambleMode.Numerals);
		}
	}

	private void Update()
	{
		if (m_fNextUpdateTime + 1f < Time.time)
		{
			UpdateBtns();
			m_fNextUpdateTime = Time.time;
		}
	}

	private void OnClickSweep()
	{
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.RAID_SWEEP))
		{
			return;
		}
		NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_RaidUID);
		if (nKMRaidDetailData == null)
		{
			return;
		}
		NKMRaidSeasonTemplet nowSeasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
		if (nowSeasonTemplet == null || nowSeasonTemplet.RaidSeasonId != m_cNKMRaidDetailData.seasonID)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(NKM_ERROR_CODE.NEC_FAIL_RAID_SWEEP_RAID_SEASON_END));
			return;
		}
		bool bIsTryAssist = nKMRaidDetailData.userUID != NKCScenManager.CurrentUserData().m_UserUID;
		NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(nKMRaidDetailData.stageID);
		if (nKMRaidTemplet != null)
		{
			int stageReqItemID = nKMRaidTemplet.StageReqItemID;
			int num = (bIsTryAssist ? nKMRaidTemplet.HelpStageReqItemCount : nKMRaidTemplet.StageReqItemCount);
			NKMItemMiscData itemMisc = NKCScenManager.CurrentUserData().m_InventoryData.GetItemMisc(stageReqItemID);
			if (itemMisc != null && itemMisc.TotalCount < num)
			{
				NKCPopupItemLack.Instance.OpenItemMiscLackPopup(stageReqItemID, (int)(num - itemMisc.TotalCount));
				return;
			}
		}
		if (NKCRaidSeasonManager.RaidSeason.highestDamage < nKMRaidTemplet.RaidDamageBasis)
		{
			string content = string.Format(NKCPacketHandlers.GetErrorMessage(NKM_ERROR_CODE.NEC_FAIL_RAID_SWEEP_BELOW_BASIS_DAMAGE), ((int)nKMRaidTemplet.RaidDamageBasis).ToString("#,##0"));
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, content);
			return;
		}
		string content2 = string.Format(NKCUtilString.GET_STRING_WORLD_MAP_RAID_SWEEP_DESC, ((int)NKCRaidSeasonManager.RaidSeason.highestDamage).ToString("N0"));
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WORLD_MAP_RAID_SWEEP_TITLE, content2, delegate
		{
			NKCPacketSender.Send_NKMPacket_RAID_SWEEP_REQ(m_RaidUID, bIsTryAssist);
		});
	}

	private void OnClickReadyBtn()
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCUtil.CheckCommonStartCond(NKCScenManager.CurrentUserData());
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCUtil.OnExpandInventoryPopup(nKM_ERROR_CODE);
			return;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID_READY().SetRaidUID(m_RaidUID);
		NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID_READY().SetGuildRaid(bGuildRaid: false);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_RAID_READY);
	}

	private void OnClickAttackBtn()
	{
		if (m_dOnClickAttackBtn != null)
		{
			List<int> list = new List<int>();
			if (m_tglEquip1.m_bChecked)
			{
				list.Add(1);
			}
			if (m_tglEquip2.m_bChecked)
			{
				list.Add(2);
			}
			if (m_tglEquip3.m_bChecked)
			{
				list.Add(3);
			}
			if (m_tglEquip4.m_bChecked)
			{
				list.Add(4);
			}
			int reqItemID = 0;
			NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(m_RaidID);
			if (nKMRaidTemplet != null)
			{
				reqItemID = nKMRaidTemplet.StageReqItemID;
			}
			m_dOnClickAttackBtn(m_RaidUID, list, reqItemID, GetCostByCurrSetting(), bIsTryAssist: false, isPracticeMode: false);
		}
	}

	private void OnClickExitBtn()
	{
		if (!m_bLockExitBtn)
		{
			m_bLockExitBtn = true;
			NKCPacketSender.Send_NKMPacket_RAID_RESULT_ACCEPT_REQ(m_RaidUID);
		}
	}

	private void OnClickGuide()
	{
		m_cNKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_RaidUID);
		if (m_cNKMRaidDetailData != null)
		{
			NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(m_cNKMRaidDetailData.stageID);
			if (nKMRaidTemplet != null && !string.IsNullOrEmpty(nKMRaidTemplet.GuideShortCut))
			{
				NKCUIPopupTutorialImagePanel.Instance.Open(nKMRaidTemplet.GuideShortCut, null);
			}
		}
	}

	private void OnClickInfoBtn()
	{
		m_cNKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_RaidUID);
		if (m_cNKMRaidDetailData != null)
		{
			NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(m_cNKMRaidDetailData.stageID);
			if (nKMRaidTemplet != null)
			{
				NKCPopupEnemyList.Instance.Open(nKMRaidTemplet.DungeonTempletBase);
			}
		}
	}

	public List<int> GetListNRGI(bool bVictory)
	{
		List<int> list = new List<int>();
		if (m_cNKMRaidDetailData == null)
		{
			return list;
		}
		NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(m_cNKMRaidDetailData.stageID);
		if (nKMRaidTemplet == null)
		{
			return list;
		}
		if (bVictory)
		{
			foreach (NKMRewardGroupTemplet item in nKMRaidTemplet.RewardRaidGroupTemplets_Victory)
			{
				if (!list.Contains(item.GroupId))
				{
					list.Add(item.GroupId);
				}
			}
		}
		else
		{
			foreach (NKMRewardGroupTemplet item2 in nKMRaidTemplet.RewardRaidGroupTemplets_Fail)
			{
				if (!list.Contains(item2.GroupId))
				{
					list.Add(item2.GroupId);
				}
			}
		}
		return list;
	}
}
