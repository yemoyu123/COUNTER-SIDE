using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUISlotListViewer : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_ITEM_LIST";

	private const float ENEMY_VIEWER_SPACING_Y = 0f;

	private const float COMMON_VIEWER_SPACING_Y = 67f;

	private static NKCUISlotListViewer m_Instance;

	public NKCUISlot m_pfbSlot;

	public ScrollRect m_srContents;

	public Transform m_trSlotRoot;

	public Text m_NKM_UI_POPUP_ITEM_LIST_TEXT;

	public Text m_NKM_UI_POPUP_ITEM_LIST_TOP_TEXT;

	public GameObject m_objPercentCutInfo;

	public NKCUIComButton m_csbtnOK;

	public GridLayoutGroup m_grid;

	public GameObject m_objEquipSlot;

	public NKCUIInvenEquipSlot m_EquipSlot;

	[Header("Tag List")]
	public GameObject m_objTagList;

	public Transform m_trTagParent;

	public NKCUIItemListTagSlot m_pfTagSlot;

	private List<NKCUIItemListTagSlot> m_lstTagListSlots = new List<NKCUIItemListTagSlot>();

	public GameObject m_objOperSkillList;

	public Transform m_trOperSkillList;

	private List<NKCUIOperatorPassiveSlot> m_lstPassiveSlots = new List<NKCUIOperatorPassiveSlot>();

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private List<NKCUISlot> m_lstSlot = new List<NKCUISlot>();

	private List<NKCDeckViewUnitSlot> m_lstDeckViewUnitSlot = new List<NKCDeckViewUnitSlot>();

	private int m_LastSelectedIndex = -1;

	private int m_CurSelectedIndex = -1;

	public static NKCUISlotListViewer Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUISlotListViewer>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_ITEM_LIST", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUISlotListViewer>();
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

	public override string MenuName => NKCUtilString.GET_STRING_SLOT_VIEWR;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static NKCUISlotListViewer GetNewInstance()
	{
		return NKCUIManager.OpenNewInstance<NKCUISlotListViewer>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_ITEM_LIST", NKCUIManager.eUIBaseRect.UIFrontPopup, null).GetInstance<NKCUISlotListViewer>();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		foreach (NKCUIItemListTagSlot lstTagListSlot in m_lstTagListSlots)
		{
			NKCUtil.SetGameobjectActive(lstTagListSlot.gameObject, bValue: false);
			Object.Destroy(lstTagListSlot);
		}
		m_lstTagListSlots.Clear();
		foreach (NKCUIOperatorPassiveSlot lstPassiveSlot in m_lstPassiveSlots)
		{
			NKCUtil.SetGameobjectActive(lstPassiveSlot.gameObject, bValue: false);
			lstPassiveSlot.DestoryInstance();
		}
		m_lstPassiveSlots.Clear();
	}

	private void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnOK, base.Close);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		NKCUtil.SetScrollHotKey(m_srContents);
	}

	private void SetSlotCount(int count)
	{
		while (m_lstSlot.Count < count)
		{
			NKCUISlot nKCUISlot = Object.Instantiate(m_pfbSlot);
			nKCUISlot.Init();
			nKCUISlot.transform.SetParent(m_trSlotRoot, worldPositionStays: false);
			m_lstSlot.Add(nKCUISlot);
		}
	}

	private void SetDeckViewUnitSlotCount(int count)
	{
		while (m_lstDeckViewUnitSlot.Count < count)
		{
			NKCDeckViewUnitSlot newInstance = NKCDeckViewUnitSlot.GetNewInstance(m_trSlotRoot);
			if (newInstance != null)
			{
				newInstance.Init(m_lstDeckViewUnitSlot.Count, bEnableDrag: false);
				newInstance.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				m_lstDeckViewUnitSlot.Add(newInstance);
			}
		}
	}

	private void TurnOffSlotList()
	{
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstSlot[i], bValue: false);
		}
	}

	private void TurnOffDeckViewUnitSlotList()
	{
		for (int i = 0; i < m_lstDeckViewUnitSlot.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstDeckViewUnitSlot[i], bValue: false);
		}
	}

	public void OpenDeckViewEnemySlotList(string stageBattleStrID, string title, string desc)
	{
		NKCUtil.SetGameobjectActive(m_objEquipSlot, bValue: false);
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(stageBattleStrID);
		if (nKMStageTempletV != null)
		{
			OpenDeckViewEnemySlotList(nKMStageTempletV, title, desc);
			return;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(stageBattleStrID);
		if (dungeonTempletBase != null)
		{
			OpenDeckViewEnemySlotList(dungeonTempletBase, title, desc);
		}
	}

	public void OpenDeckViewEnemySlotList(NKMStageTempletV2 stageTemplet, string title, string desc)
	{
		if (stageTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_objEquipSlot, bValue: false);
			Dictionary<string, NKCEnemyData> enemyUnits = NKMDungeonManager.GetEnemyUnits(stageTemplet);
			OpenDeckViewEnemySlotList(enemyUnits, title, desc);
		}
	}

	public void OpenDeckViewEnemySlotList(NKMDungeonTempletBase cNKMDungeonTempletBase, string title, string desc)
	{
		if (cNKMDungeonTempletBase != null)
		{
			NKCUtil.SetGameobjectActive(m_objEquipSlot, bValue: false);
			Dictionary<string, NKCEnemyData> enemyUnits = NKMDungeonManager.GetEnemyUnits(cNKMDungeonTempletBase);
			OpenDeckViewEnemySlotList(enemyUnits, title, desc);
		}
	}

	public void OpenDeckViewEnemySlotList(Dictionary<string, NKCEnemyData> dicEnemyUnitStrIDs, string title, string desc)
	{
		NKCUtil.SetGameobjectActive(m_objEquipSlot, bValue: false);
		m_NKM_UI_POPUP_ITEM_LIST_TEXT.text = desc;
		m_NKM_UI_POPUP_ITEM_LIST_TOP_TEXT.text = title;
		m_grid.spacing = new Vector2(m_grid.spacing.x, 0f);
		if (dicEnemyUnitStrIDs != null && dicEnemyUnitStrIDs.Count > 0)
		{
			TurnOffSlotList();
			SetDeckViewUnitSlotCount(dicEnemyUnitStrIDs.Count);
			List<NKCEnemyData> list = new List<NKCEnemyData>(dicEnemyUnitStrIDs.Values);
			list.Sort(new NKCEnemyData.CompNED());
			int num = 0;
			for (num = 0; num < list.Count; num++)
			{
				NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_lstDeckViewUnitSlot[num];
				nKCDeckViewUnitSlot.SetEnemyData(NKMUnitManager.GetUnitTempletBase(list[num].m_UnitStrID), list[num]);
				NKCUtil.SetGameobjectActive(nKCDeckViewUnitSlot.gameObject, bValue: true);
			}
			for (; num < m_lstDeckViewUnitSlot.Count; num++)
			{
				NKCUtil.SetGameobjectActive(m_lstDeckViewUnitSlot[num], bValue: false);
			}
			NKCUtil.SetGameobjectActive(m_objPercentCutInfo, bValue: false);
			OpenCommon();
		}
	}

	private void OpenCommon()
	{
		m_srContents.normalizedPosition = new Vector2(0f, 1f);
		UIOpened();
		if (m_NKCUIOpenAnimator == null)
		{
			m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		}
		m_NKCUIOpenAnimator.PlayOpenAni();
	}

	private void Update()
	{
		if (base.IsOpen && m_NKCUIOpenAnimator != null)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	private void SetSlotClickAction(ref NKCUISlot slot)
	{
		if (!(slot == null))
		{
			slot.SetOnClickAction(NKCUISlot.SlotClickType.RatioList, NKCUISlot.SlotClickType.MoldList, NKCUISlot.SlotClickType.ChoiceList, NKCUISlot.SlotClickType.Tooltip);
		}
	}

	private void SetSlotClickActionForEquip(ref NKCUISlot slot)
	{
		if (!(slot == null))
		{
			slot.SetOnClickAction(NKCUISlot.SlotClickType.None);
			slot.SetOnClick(OnClickSlot);
		}
	}

	private void OnClickSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (slotData == null)
		{
			return;
		}
		if (m_LastSelectedIndex >= 0)
		{
			NKCUISlot nKCUISlot = m_lstSlot[m_LastSelectedIndex];
			if (nKCUISlot != null)
			{
				nKCUISlot.SetSelected(bSelected: false);
			}
		}
		m_CurSelectedIndex = m_lstSlot.FindIndex((NKCUISlot x) => x.GetSlotData().ID == slotData.ID);
		if (m_CurSelectedIndex >= 0)
		{
			m_lstSlot[m_CurSelectedIndex].SetSelected(bSelected: true);
			m_LastSelectedIndex = m_CurSelectedIndex;
		}
		NKMEquipItemData cNKMEquipItemData = NKCEquipSortSystem.MakeTempEquipData(slotData.ID);
		m_EquipSlot.SetData(cNKMEquipItemData);
	}

	public void OpenEquipMoldRewardList(Dictionary<NKM_REWARD_TYPE, List<int>> dicData, string title, string desc)
	{
		NKCUtil.SetGameobjectActive(m_objEquipSlot, bValue: true);
		m_NKM_UI_POPUP_ITEM_LIST_TEXT.text = desc;
		m_NKM_UI_POPUP_ITEM_LIST_TOP_TEXT.text = title;
		m_grid.spacing = new Vector2(m_grid.spacing.x, 67f);
		if (dicData == null || dicData.Count <= 0)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		TurnOffDeckViewUnitSlotList();
		int num = 0;
		foreach (KeyValuePair<NKM_REWARD_TYPE, List<int>> dicDatum in dicData)
		{
			if (dicDatum.Value != null)
			{
				num += dicDatum.Value.Count;
			}
		}
		SetSlotCount(num);
		int num2 = 0;
		int num3 = -1;
		foreach (KeyValuePair<NKM_REWARD_TYPE, List<int>> dicDatum2 in dicData)
		{
			List<int> value = dicDatum2.Value;
			for (int i = 0; i < value.Count; i++)
			{
				NKCUISlot slot = m_lstSlot[num2];
				if (num2 < m_lstSlot.Count)
				{
					NKCUtil.SetGameobjectActive(slot, bValue: true);
					NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(dicDatum2.Key, value[i], 1);
					slot.SetData(data, bShowName: true, bShowNumber: false, bEnableLayoutElement: true, null);
					if (num3 < 0 && dicDatum2.Key == NKM_REWARD_TYPE.RT_EQUIP)
					{
						num3 = num2;
					}
					if (dicDatum2.Key == NKM_REWARD_TYPE.RT_EQUIP)
					{
						SetSlotClickActionForEquip(ref slot);
					}
					else
					{
						SetSlotClickAction(ref slot);
					}
					num2++;
				}
			}
		}
		for (int j = num2; j < m_lstSlot.Count; j++)
		{
			NKCUtil.SetGameobjectActive(m_lstSlot[j], bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objPercentCutInfo, bValue: false);
		if (dicData.ContainsKey(NKM_REWARD_TYPE.RT_EQUIP))
		{
			OnClickSlot(m_lstSlot[num3].GetSlotData(), bLocked: false);
		}
		OpenCommon();
	}

	public void OpenRewardList(List<int> listID, NKM_REWARD_TYPE type, string title, string desc)
	{
		NKCUtil.SetGameobjectActive(m_objEquipSlot, type == NKM_REWARD_TYPE.RT_EQUIP);
		m_NKM_UI_POPUP_ITEM_LIST_TEXT.text = desc;
		m_NKM_UI_POPUP_ITEM_LIST_TOP_TEXT.text = title;
		m_grid.spacing = new Vector2(m_grid.spacing.x, 67f);
		if (listID == null || listID.Count <= 0)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		TurnOffDeckViewUnitSlotList();
		SetSlotCount(listID.Count);
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			NKCUISlot slot = m_lstSlot[i];
			if (i < listID.Count)
			{
				NKCUtil.SetGameobjectActive(slot, bValue: true);
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(type, listID[i], 1);
				slot.SetData(data, bShowName: true, bShowNumber: false, bEnableLayoutElement: true, null);
				if (type == NKM_REWARD_TYPE.RT_EQUIP)
				{
					SetSlotClickActionForEquip(ref slot);
				}
				else
				{
					SetSlotClickAction(ref slot);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(slot, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_objPercentCutInfo, bValue: false);
		if (type == NKM_REWARD_TYPE.RT_EQUIP)
		{
			OnClickSlot(m_lstSlot[0].GetSlotData(), bLocked: false);
		}
		OpenCommon();
	}

	public void OpenRewardList(Dictionary<NKM_REWARD_TYPE, List<int>> dicRewardIdList, string title, string desc)
	{
		NKCUtil.SetGameobjectActive(m_objEquipSlot, bValue: false);
		m_NKM_UI_POPUP_ITEM_LIST_TEXT.text = desc;
		m_NKM_UI_POPUP_ITEM_LIST_TOP_TEXT.text = title;
		m_grid.spacing = new Vector2(m_grid.spacing.x, 67f);
		if (dicRewardIdList == null)
		{
			return;
		}
		TurnOffDeckViewUnitSlotList();
		int num = 0;
		foreach (KeyValuePair<NKM_REWARD_TYPE, List<int>> dicRewardId in dicRewardIdList)
		{
			if (dicRewardId.Value != null)
			{
				num += dicRewardId.Value.Count;
			}
		}
		SetSlotCount(num);
		int num2 = 0;
		foreach (KeyValuePair<NKM_REWARD_TYPE, List<int>> dicRewardId2 in dicRewardIdList)
		{
			if (dicRewardId2.Value == null)
			{
				continue;
			}
			int count = dicRewardId2.Value.Count;
			for (int i = 0; i < count; i++)
			{
				if (m_lstSlot.Count <= num2)
				{
					break;
				}
				NKCUISlot slot = m_lstSlot[num2];
				NKCUtil.SetGameobjectActive(slot, bValue: true);
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(dicRewardId2.Key, dicRewardId2.Value[i], 1);
				slot.SetData(data, bShowName: true, bShowNumber: false, bEnableLayoutElement: true, null);
				SetSlotClickAction(ref slot);
				num2++;
			}
		}
		for (int j = num2; j < m_lstSlot.Count; j++)
		{
			NKCUtil.SetGameobjectActive(m_lstSlot[j], bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objPercentCutInfo, bValue: false);
		OpenCommon();
	}

	public void OpenMissionRewardList(List<MissionReward> listReward)
	{
		NKCUtil.SetGameobjectActive(m_objEquipSlot, bValue: false);
		m_NKM_UI_POPUP_ITEM_LIST_TEXT.text = NKCUtilString.GET_STRING_SLOT_VIEWR_DESC;
		m_NKM_UI_POPUP_ITEM_LIST_TOP_TEXT.text = NKCUtilString.GET_STRING_SLOT_VIEWR;
		m_grid.spacing = new Vector2(m_grid.spacing.x, 67f);
		TurnOffDeckViewUnitSlotList();
		SetSlotCount(listReward.Count);
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			NKCUISlot slot = m_lstSlot[i];
			if (i < listReward.Count)
			{
				NKCUtil.SetGameobjectActive(slot, bValue: true);
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(listReward[i].reward_type, listReward[i].reward_id, listReward[i].reward_value);
				slot.SetData(data, bShowName: true, bShowNumber: true, bEnableLayoutElement: true, null);
				SetSlotClickAction(ref slot);
			}
			else
			{
				NKCUtil.SetGameobjectActive(slot, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_objPercentCutInfo, bValue: false);
		OpenCommon();
	}

	private List<NKMRandomBoxItemTemplet> GetRandomBoxItemTemplet(int itemID, bool bCheckIsRatioOpened = false)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
		if (itemMiscTempletByID == null)
		{
			Debug.LogError("itemTemplet null! ID : " + itemID);
			return null;
		}
		if (bCheckIsRatioOpened && !itemMiscTempletByID.IsRatioOpened())
		{
			Debug.LogError("Ratio probihited! : ID : " + itemID);
			return null;
		}
		if (itemMiscTempletByID.m_RewardGroupID == 0)
		{
			Debug.LogError("no rewardgroup! ID : " + itemID);
			return null;
		}
		UpdateSpacTagUI(itemMiscTempletByID);
		return NKCRandomBoxManager.GetRandomBoxItemTempletList(itemMiscTempletByID.m_RewardGroupID);
	}

	private void UpdateSpacTagUI(NKMItemMiscTemplet itemTemplet)
	{
		NKCUtil.SetGameobjectActive(m_objTagList, bValue: false);
		if (itemTemplet == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_objOperSkillList, bValue: false);
		if (itemTemplet.m_ItemMiscSubType == NKM_ITEM_MISC_SUBTYPE.IMST_EQUIP_CHOICE_OPTION_CUSTOM)
		{
			if (itemTemplet.ChangePotenOption)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.EQUIP_POTEN_OPTION);
			}
			if (itemTemplet.ChangePotenFirstOptionMax)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.EQUIP_POTEN_OPTION_MAX);
			}
			if (itemTemplet.ChangeSetOption)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.EQUIP_SET_OPTION);
			}
			if (itemTemplet.ChangeStat)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.EQUIP_STAT);
			}
			NKCUtil.SetGameobjectActive(m_objTagList, m_lstTagListSlots.Count > 0);
			return;
		}
		NKMCustomBoxTemplet nKMCustomBoxTemplet = NKMCustomBoxTemplet.Find(itemTemplet.m_CustomBoxId);
		if (nKMCustomBoxTemplet == null)
		{
			return;
		}
		switch (nKMCustomBoxTemplet.UnitType)
		{
		case NKM_REWARD_TYPE.RT_UNIT:
			if (nKMCustomBoxTemplet.Level > 1)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.UNIT_LEVEL, nKMCustomBoxTemplet.Level);
			}
			if (nKMCustomBoxTemplet.SkillLevel > 1)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.UNIT_SKILL_LEVEL, nKMCustomBoxTemplet.SkillLevel);
			}
			if (nKMCustomBoxTemplet.TacticUpdate > 0)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.UNIT_TACTIC, nKMCustomBoxTemplet.TacticUpdate);
			}
			if (nKMCustomBoxTemplet.ReactorLevel > 0)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.UNIT_REACTOR, nKMCustomBoxTemplet.ReactorLevel);
			}
			if (nKMCustomBoxTemplet.Loyalty / 100 > 0)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.UNIT_LOYALTY, nKMCustomBoxTemplet.Loyalty / 100);
			}
			break;
		case NKM_REWARD_TYPE.RT_SHIP:
			if (nKMCustomBoxTemplet.Level > 1)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.SHIP_LEVEL, nKMCustomBoxTemplet.Level);
			}
			break;
		case NKM_REWARD_TYPE.RT_OPERATOR:
			if (nKMCustomBoxTemplet.Level > 1)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.OPER_LEVEL, nKMCustomBoxTemplet.Level);
			}
			if (nKMCustomBoxTemplet.TacticUpdate > 1)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.OPER_MAIN_SKILL_LEVEL, nKMCustomBoxTemplet.TacticUpdate);
			}
			if (nKMCustomBoxTemplet.SkillLevel > 1)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.OPER_SUB_SKILL_LEVEL, nKMCustomBoxTemplet.SkillLevel);
			}
			if (nKMCustomBoxTemplet.CustomOperatorSkillIds.Count > 0)
			{
				AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE.OPER_SUB_SKILL_CUSTOM);
				SetOperCustomSkillList(nKMCustomBoxTemplet.CustomOperatorSkillIds);
			}
			break;
		}
		NKCUtil.SetGameobjectActive(m_objTagList, m_lstTagListSlots.Count > 0);
	}

	private void AddSpacTagSlot(NKCUIItemListTagSlot.SPAC_TAG_TYPE type, int val = 0)
	{
		GameObject orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<GameObject>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_ITEM_LIST_TAG");
		if (null == orLoadAssetResource)
		{
			return;
		}
		NKCUIItemListTagSlot component = orLoadAssetResource.GetComponent<NKCUIItemListTagSlot>();
		if (component == null)
		{
			return;
		}
		NKCUIItemListTagSlot nKCUIItemListTagSlot = Object.Instantiate(component, m_trTagParent);
		if (!(nKCUIItemListTagSlot == null))
		{
			nKCUIItemListTagSlot.gameObject.SetActive(value: true);
			nKCUIItemListTagSlot.SetData(type, val);
			nKCUIItemListTagSlot.transform.localScale = Vector3.one;
			if (type == NKCUIItemListTagSlot.SPAC_TAG_TYPE.OPER_SUB_SKILL_CUSTOM)
			{
				nKCUIItemListTagSlot.SetCallBack(OnEnableOperCustomSkillList);
			}
			m_lstTagListSlots.Add(nKCUIItemListTagSlot);
		}
	}

	private void SetOperCustomSkillList(List<int> lstOperCustomSkill)
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (int item in lstOperCustomSkill)
		{
			NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(item);
			if (skillTemplet != null && !hashSet.Contains(skillTemplet.m_OperSkillID))
			{
				NKCUIOperatorPassiveSlot slot = GetSlot();
				if (slot != null)
				{
					slot.SetData(NKCUtil.GetSkillIconSprite(skillTemplet), NKCStringTable.GetString(skillTemplet.m_OperSkillNameStrID), skillTemplet.m_OperSkillID, 1);
					hashSet.Add(skillTemplet.m_OperSkillID);
					slot.OnSelect(bActive: false);
					m_lstPassiveSlots.Add(slot);
				}
			}
		}
	}

	private NKCUIOperatorPassiveSlot GetSlot()
	{
		NKCUIOperatorPassiveSlot resource = NKCUIOperatorPassiveSlot.GetResource("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_OPR_SUB_SKILL_RATE_SLOT");
		if (resource != null)
		{
			NKCUtil.SetGameobjectActive(resource, bValue: true);
			resource.transform.SetParent(m_trOperSkillList);
			resource.transform.localScale = Vector3.one;
			resource.Init();
			return resource;
		}
		return null;
	}

	private void OnEnableOperCustomSkillList(bool bSelect)
	{
		if (m_lstPassiveSlots.Count != 0)
		{
			NKCUtil.SetGameobjectActive(m_objOperSkillList, bSelect);
		}
	}

	public void OpenPackageInfo(int BoxItemID, string top = "", string list = "")
	{
		NKCUtil.SetGameobjectActive(m_objEquipSlot, bValue: false);
		string msg = (string.IsNullOrEmpty(top) ? NKCUtilString.GET_STRING_SLOT_VIEWR : top);
		string msg2 = (string.IsNullOrEmpty(list) ? NKCUtilString.GET_STRING_SLOT_VIEWR_DESC : list);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_ITEM_LIST_TEXT, msg2);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_ITEM_LIST_TOP_TEXT, msg);
		m_grid.spacing = new Vector2(m_grid.spacing.x, 67f);
		List<NKMRandomBoxItemTemplet> randomBoxItemTemplet = GetRandomBoxItemTemplet(BoxItemID);
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(BoxItemID);
		if (randomBoxItemTemplet == null)
		{
			Debug.LogError("rewardgroup null! ID : " + itemMiscTempletByID.m_RewardGroupID);
			return;
		}
		TurnOffDeckViewUnitSlotList();
		SetSlotCount(randomBoxItemTemplet.Count);
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			NKCUISlot slot = m_lstSlot[i];
			if (i < randomBoxItemTemplet.Count)
			{
				NKCUtil.SetGameobjectActive(slot, bValue: true);
				NKMRandomBoxItemTemplet nKMRandomBoxItemTemplet = randomBoxItemTemplet[i];
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(nKMRandomBoxItemTemplet.m_reward_type, nKMRandomBoxItemTemplet.m_RewardID, nKMRandomBoxItemTemplet.TotalQuantity_Max);
				slot.SetData(data, bShowName: true, NKCUISlot.WillShowCount(data), bEnableLayoutElement: true, null);
				SetSlotClickAction(ref slot);
			}
			else
			{
				NKCUtil.SetGameobjectActive(slot, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_objPercentCutInfo, bValue: false);
		OpenCommon();
	}

	public void OpenItemBoxRatio(int BoxItemID)
	{
		NKCUtil.SetGameobjectActive(m_objEquipSlot, bValue: false);
		m_NKM_UI_POPUP_ITEM_LIST_TEXT.text = NKCStringTable.GetString("SI_DP_SLOT_RATIO_VIEWER_DESC");
		m_NKM_UI_POPUP_ITEM_LIST_TOP_TEXT.text = NKCUtilString.GET_STRING_SLOT_VIEWR;
		m_grid.spacing = new Vector2(m_grid.spacing.x, 67f);
		List<NKMRandomBoxItemTemplet> randomBoxItemTemplet = GetRandomBoxItemTemplet(BoxItemID, bCheckIsRatioOpened: true);
		if (randomBoxItemTemplet == null)
		{
			Debug.LogError("rewardgroup null! ID : " + BoxItemID);
			return;
		}
		int totalRatio = 0;
		randomBoxItemTemplet.ForEach(delegate(NKMRandomBoxItemTemplet x)
		{
			totalRatio += x.m_Ratio;
		});
		TurnOffDeckViewUnitSlotList();
		SetSlotCount(randomBoxItemTemplet.Count);
		for (int num = 0; num < m_lstSlot.Count; num++)
		{
			NKCUISlot slot = m_lstSlot[num];
			if (num < randomBoxItemTemplet.Count)
			{
				NKCUtil.SetGameobjectActive(slot, bValue: true);
				NKMRandomBoxItemTemplet nKMRandomBoxItemTemplet = randomBoxItemTemplet[num];
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(nKMRandomBoxItemTemplet.m_reward_type, nKMRandomBoxItemTemplet.m_RewardID, nKMRandomBoxItemTemplet.TotalQuantity_Max);
				slot.SetData(data, bShowName: true, NKCUISlot.WillShowCount(data), bEnableLayoutElement: true, null);
				float probability = (float)nKMRandomBoxItemTemplet.m_Ratio / (float)totalRatio;
				slot.AddProbabilityToName(probability);
				slot.SetCountRange(nKMRandomBoxItemTemplet.TotalQuantity_Min, nKMRandomBoxItemTemplet.TotalQuantity_Max);
				SetSlotClickAction(ref slot);
			}
			else
			{
				NKCUtil.SetGameobjectActive(slot, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_objPercentCutInfo, bValue: true);
		OpenCommon();
	}

	public void OpenChoiceInfo(int ChoiceItemID)
	{
		NKCUtil.SetGameobjectActive(m_objEquipSlot, bValue: false);
		m_NKM_UI_POPUP_ITEM_LIST_TEXT.text = NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_DESC");
		m_NKM_UI_POPUP_ITEM_LIST_TOP_TEXT.text = NKCUtilString.GET_STRING_SLOT_VIEWR;
		m_grid.spacing = new Vector2(m_grid.spacing.x, 67f);
		List<NKMRandomBoxItemTemplet> randomBoxItemTemplet = GetRandomBoxItemTemplet(ChoiceItemID);
		if (randomBoxItemTemplet == null)
		{
			Debug.LogError("rewardgroup null! ID : " + ChoiceItemID);
			return;
		}
		TurnOffDeckViewUnitSlotList();
		SetSlotCount(randomBoxItemTemplet.Count);
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			NKCUISlot slot = m_lstSlot[i];
			if (i < randomBoxItemTemplet.Count)
			{
				NKCUtil.SetGameobjectActive(slot, bValue: true);
				NKMRandomBoxItemTemplet nKMRandomBoxItemTemplet = randomBoxItemTemplet[i];
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(nKMRandomBoxItemTemplet.m_reward_type, nKMRandomBoxItemTemplet.m_RewardID, nKMRandomBoxItemTemplet.TotalQuantity_Max);
				slot.SetData(data, bShowName: true, NKCUISlot.WillShowCount(data), bEnableLayoutElement: true, null);
				SetSlotClickAction(ref slot);
			}
			else
			{
				NKCUtil.SetGameobjectActive(slot, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_objPercentCutInfo, bValue: false);
		OpenCommon();
	}

	public void OpenGenericUnitList(string title, string desc, IEnumerable<int> lstUnit, int unitLevel)
	{
		List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
		foreach (int item in lstUnit)
		{
			list.Add(NKCUISlot.SlotData.MakeUnitData(item, unitLevel));
		}
		list.Sort(GenericUnitSort);
		OpenGenericSlotList(title, desc, list);
	}

	private int GenericUnitSort(NKCUISlot.SlotData A, NKCUISlot.SlotData B)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(A.ID);
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(B.ID);
		if (unitTempletBase.m_bAwaken != unitTempletBase2.m_bAwaken)
		{
			return unitTempletBase2.m_bAwaken.CompareTo(unitTempletBase.m_bAwaken);
		}
		if (unitTempletBase.IsRearmUnit != unitTempletBase2.IsRearmUnit)
		{
			return unitTempletBase2.IsRearmUnit.CompareTo(unitTempletBase.IsRearmUnit);
		}
		if (unitTempletBase.m_NKM_UNIT_GRADE != unitTempletBase2.m_NKM_UNIT_GRADE)
		{
			return unitTempletBase2.m_NKM_UNIT_GRADE.CompareTo(unitTempletBase.m_NKM_UNIT_GRADE);
		}
		return A.ID.CompareTo(B.ID);
	}

	public void OpenGenericSlotList(string title, string desc, List<NKCUISlot.SlotData> lstSlotData)
	{
		NKCUtil.SetGameobjectActive(m_objEquipSlot, bValue: false);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_ITEM_LIST_TEXT, desc);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_ITEM_LIST_TOP_TEXT, title);
		m_grid.spacing = new Vector2(m_grid.spacing.x, 67f);
		TurnOffDeckViewUnitSlotList();
		SetSlotCount(lstSlotData.Count);
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			NKCUISlot slot = m_lstSlot[i];
			if (i < lstSlotData.Count)
			{
				NKCUtil.SetGameobjectActive(slot, bValue: true);
				slot.SetData(lstSlotData[i], bShowName: true, NKCUISlot.WillShowCount(lstSlotData[i]), bEnableLayoutElement: true, null);
				SetSlotClickAction(ref slot);
			}
			else
			{
				NKCUtil.SetGameobjectActive(slot, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_objPercentCutInfo, bValue: false);
		OpenCommon();
	}

	private void OnDestroy()
	{
		for (int i = 0; i < m_lstDeckViewUnitSlot.Count; i++)
		{
			m_lstDeckViewUnitSlot[i].CloseInstance();
		}
		m_lstDeckViewUnitSlot.Clear();
		m_Instance = null;
	}
}
