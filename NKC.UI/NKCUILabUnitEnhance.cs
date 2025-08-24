using System;
using System.Collections.Generic;
using ClientPacket.Unit;
using NKC.UI.NPC;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUILabUnitEnhance : MonoBehaviour
{
	[Serializable]
	public class ConsumeUnitSlot
	{
		public NKCUISlot m_Slot;

		public GameObject m_objEffect;

		public GameObject m_objExpBonus;

		public void Init()
		{
			m_Slot.Init();
			NKCUtil.SetGameobjectActive(m_objEffect, bValue: false);
			NKCUtil.SetGameobjectActive(m_objExpBonus, bValue: false);
		}

		public void SetEnhanceEffect(bool value)
		{
			NKCUtil.SetGameobjectActive(m_objEffect, value);
		}

		public void OffEnhanceEffect()
		{
			NKCUtil.SetGameobjectActive(m_objEffect, bValue: false);
		}

		public bool HasUnitdata()
		{
			NKCUISlot.SlotData slotData = m_Slot.GetSlotData();
			if (slotData != null)
			{
				return slotData.UID != 0;
			}
			return false;
		}
	}

	public delegate List<long> GetUnitList(ref int selectIdx);

	public delegate void OnAutoSelectPlayVoice(NPC_ACTION_TYPE actionType, bool bMute);

	[Header("오른쪽 스탯 정보")]
	public NKCUIRectMove m_rmLabUnitStat;

	public NKCUIRectMove m_rmLabUnitSlot;

	public NKCUIComStatEnhanceBar m_UIExpBarHP;

	public NKCUIComStatEnhanceBar m_UIExpBarAttack;

	public NKCUIComStatEnhanceBar m_UIExpBarDefense;

	public NKCUIComStatEnhanceBar m_UIExpBarCritical;

	public NKCUIComStatEnhanceBar m_UIExpBarHit;

	public NKCUIComStatEnhanceBar m_UIExpBarEvade;

	public Text m_txtPower;

	public NKCUIComStateButton m_csbtnDetail;

	[Header("먹잇감 유닛 정보")]
	public List<ConsumeUnitSlot> m_lstObjUnitSlot;

	[Header("가격")]
	public NKCUIItemCostSlot m_costSlot;

	[Header("버튼들")]
	public NKCUIComStateButton m_sbtnAutoSelect;

	public NKCUIComStateButton m_sbtnClear;

	public NKCUIComStateButton m_sbtnEnhance;

	private NKMUnitData m_targetUnitData;

	private GameObject m_NKM_UI_LAB_UNIT_INFO_RESET_BG_DISABLE;

	private GameObject m_NKM_UI_LAB_UNIT_INFO_ENTER_BG_DISABLE;

	private GameObject m_NKM_UI_LAB_UNIT_INFO_ENTER_LIGHT;

	private Text m_NKM_UI_LAB_UNIT_INFO_RESET_TEXT;

	public Text m_NKM_UI_LAB_UNIT_INFO_ENTER_TEXT;

	public Image m_NKM_UI_LAB_UNIT_INFO_ENTER_ICON;

	private List<long> m_currentSlotUIDList = new List<long>();

	private GetUnitList dGetUnitList;

	private OnAutoSelectPlayVoice dOnAutoSelectPlayVoice;

	private void OnDisable()
	{
		for (int i = 0; i < m_lstObjUnitSlot.Count; i++)
		{
			m_lstObjUnitSlot[i].OffEnhanceEffect();
		}
	}

	public void Init(NKCUINPCProfessorOlivia npcProfessorOlivia, GetUnitList GetUnitList = null)
	{
		if (m_sbtnAutoSelect != null)
		{
			m_sbtnAutoSelect.PointerClick.RemoveAllListeners();
			m_sbtnAutoSelect.PointerClick.AddListener(OnBtnAutoSelect);
		}
		if (m_sbtnClear != null)
		{
			m_sbtnClear.PointerClick.RemoveAllListeners();
			m_sbtnClear.PointerClick.AddListener(ClearAllFeedUnitSlots);
		}
		if (m_sbtnEnhance != null)
		{
			m_sbtnEnhance.PointerClick.RemoveAllListeners();
			m_sbtnEnhance.PointerClick.AddListener(Enhance);
		}
		if (m_csbtnDetail != null)
		{
			m_csbtnDetail.PointerClick.RemoveAllListeners();
			m_csbtnDetail.PointerClick.AddListener(OnBtnDetail);
		}
		foreach (ConsumeUnitSlot item in m_lstObjUnitSlot)
		{
			item.Init();
		}
		InitChildObject();
		if (GetUnitList != null)
		{
			dGetUnitList = GetUnitList;
		}
		if (npcProfessorOlivia != null)
		{
			dOnAutoSelectPlayVoice = npcProfessorOlivia.PlayAni;
		}
	}

	private void InitChildObject()
	{
		m_NKM_UI_LAB_UNIT_INFO_RESET_BG_DISABLE = GameObject.Find("NKM_UI_LAB_UNIT_INFO_RESET_BG_DISABLE");
		m_NKM_UI_LAB_UNIT_INFO_ENTER_BG_DISABLE = GameObject.Find("NKM_UI_LAB_UNIT_INFO_ENTER_BG_DISABLE");
		m_NKM_UI_LAB_UNIT_INFO_ENTER_LIGHT = GameObject.Find("NKM_UI_LAB_UNIT_INFO_ENTER_LIGHT");
		m_NKM_UI_LAB_UNIT_INFO_RESET_TEXT = GameObject.Find("NKM_UI_LAB_UNIT_INFO_RESET_TEXT").GetComponent<Text>();
	}

	public void Cleanup()
	{
		m_targetUnitData = null;
		NKCPopupUnitInfoDetail.CheckInstanceAndClose();
	}

	public void SetData(NKMUnitData unitData)
	{
		m_currentSlotUIDList.Clear();
		NKCPopupUnitInfoDetail.CheckInstanceAndClose();
		m_targetUnitData = unitData;
		for (int i = 0; i < m_lstObjUnitSlot.Count; i++)
		{
			m_lstObjUnitSlot[i].m_Slot.SetEmptyMaterial(OnClickSlot);
			NKCUtil.SetGameobjectActive(m_lstObjUnitSlot[i].m_objExpBonus, bValue: false);
		}
		UpdateStatInfo();
		UpdateRequiredCredit();
		SwitchEtcObject(CanEnhance(unitData));
		NKCUtil.SetGameobjectActive(m_sbtnAutoSelect.gameObject, m_targetUnitData != null);
		NKCUtil.SetGameobjectActive(m_csbtnDetail.gameObject, m_targetUnitData != null);
	}

	private void SwitchEtcObject(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_LAB_UNIT_INFO_ENTER_LIGHT, bActive);
		NKCUtil.SetGameobjectActive(m_NKM_UI_LAB_UNIT_INFO_RESET_BG_DISABLE, !bActive);
		NKCUtil.SetGameobjectActive(m_NKM_UI_LAB_UNIT_INFO_ENTER_BG_DISABLE, !bActive);
		m_NKM_UI_LAB_UNIT_INFO_RESET_TEXT.color = (bActive ? Color.white : NKCUtil.GetButtonUIColor(Active: false));
		m_NKM_UI_LAB_UNIT_INFO_ENTER_TEXT.color = NKCUtil.GetButtonUIColor(bActive);
		m_NKM_UI_LAB_UNIT_INFO_ENTER_ICON.color = NKCUtil.GetButtonUIColor(bActive);
	}

	public void SetDataWithoutReset(NKMUnitData unitData)
	{
		m_targetUnitData = unitData;
		for (int i = 0; i < m_lstObjUnitSlot.Count; i++)
		{
			m_lstObjUnitSlot[i].SetEnhanceEffect(!m_lstObjUnitSlot[i].m_Slot.IsEmpty());
		}
		UpdateRequiredCredit();
	}

	public void Enhance()
	{
		if (m_targetUnitData == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ENHANCE_NEED_SET_TARGET_UNIT);
			return;
		}
		if (m_targetUnitData.IsSeized)
		{
			NKCPopupMessageManager.AddPopupMessage(NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED);
			return;
		}
		switch (NKCScenManager.CurrentUserData().m_ArmyData.GetUnitDeckState(m_targetUnitData))
		{
		case NKM_DECK_STATE.DECK_STATE_WARFARE:
			NKCPopupMessageManager.AddPopupMessage(NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING);
			return;
		case NKM_DECK_STATE.DECK_STATE_DIVE:
			NKCPopupMessageManager.AddPopupMessage(NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING);
			return;
		}
		if (m_currentSlotUIDList == null)
		{
			return;
		}
		m_currentSlotUIDList.RemoveAll((long x) => x == 0);
		int num = NKMEnhanceManager.CalculateCreditCost(m_currentSlotUIDList);
		if (NKCScenManager.CurrentUserData().GetCredit() < num)
		{
			NKCShopManager.OpenItemLackPopup(1, num);
		}
		else if (m_currentSlotUIDList.Count > 0)
		{
			foreach (long currentSlotUID in m_currentSlotUIDList)
			{
				if (!IsDismissibleUnit(currentSlotUID))
				{
					NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_DP_ENHANCE_INCLUDE_HIGH_RARITY_AND_LEVEL"), delegate
					{
						Send_ENHANCE_UNIT_REQ();
					}, delegate
					{
						m_currentSlotUIDList.Clear();
						NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_DP_ENHANCE_CANCELED"), UpdateUnitSlots);
					});
					return;
				}
			}
			Send_ENHANCE_UNIT_REQ();
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ENHANCE_NEED_SET_CONSUME_UNIT);
		}
	}

	private void Send_ENHANCE_UNIT_REQ()
	{
		NKMPacket_ENHANCE_UNIT_REQ nKMPacket_ENHANCE_UNIT_REQ = new NKMPacket_ENHANCE_UNIT_REQ();
		nKMPacket_ENHANCE_UNIT_REQ.unitUID = m_targetUnitData.m_UnitUID;
		nKMPacket_ENHANCE_UNIT_REQ.consumeUnitUIDList = m_currentSlotUIDList;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_ENHANCE_UNIT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void RemoveUnDismissibleUnit()
	{
		for (int i = 0; i < m_currentSlotUIDList.Count && i >= 0; i++)
		{
			if (!IsDismissibleUnit(m_currentSlotUIDList[i]))
			{
				m_currentSlotUIDList.RemoveAt(i);
				i--;
			}
		}
	}

	private void UpdateStatInfo()
	{
		NKMUnitData targetUnitData = m_targetUnitData;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(targetUnitData);
		if (unitTempletBase != null)
		{
			Dictionary<NKM_STAT_TYPE, int> dictionary = NKMEnhanceManager.CalculateExpGain(NKCScenManager.CurrentUserData().m_ArmyData, m_currentSlotUIDList, unitTempletBase.m_NKM_UNIT_ROLE_TYPE);
			m_UIExpBarHP.SetData(targetUnitData, NKM_STAT_TYPE.NST_HP, dictionary[NKM_STAT_TYPE.NST_HP]);
			m_UIExpBarAttack.SetData(targetUnitData, NKM_STAT_TYPE.NST_ATK, dictionary[NKM_STAT_TYPE.NST_ATK]);
			m_UIExpBarDefense.SetData(targetUnitData, NKM_STAT_TYPE.NST_DEF, dictionary[NKM_STAT_TYPE.NST_DEF]);
			m_UIExpBarCritical.SetData(targetUnitData, NKM_STAT_TYPE.NST_CRITICAL, dictionary[NKM_STAT_TYPE.NST_CRITICAL]);
			m_UIExpBarHit.SetData(targetUnitData, NKM_STAT_TYPE.NST_HIT, dictionary[NKM_STAT_TYPE.NST_HIT]);
			m_UIExpBarEvade.SetData(targetUnitData, NKM_STAT_TYPE.NST_EVADE, dictionary[NKM_STAT_TYPE.NST_EVADE]);
			m_txtPower.text = targetUnitData.CalculateOperationPower(NKCScenManager.CurrentUserData().m_InventoryData).ToString("N0");
		}
		else
		{
			m_UIExpBarHP.SetData(null, NKM_STAT_TYPE.NST_HP, 0);
			m_UIExpBarAttack.SetData(null, NKM_STAT_TYPE.NST_ATK, 0);
			m_UIExpBarDefense.SetData(null, NKM_STAT_TYPE.NST_DEF, 0);
			m_UIExpBarCritical.SetData(null, NKM_STAT_TYPE.NST_CRITICAL, 0);
			m_UIExpBarHit.SetData(null, NKM_STAT_TYPE.NST_HIT, 0);
			m_UIExpBarEvade.SetData(null, NKM_STAT_TYPE.NST_EVADE, 0);
			m_txtPower.text = "";
		}
	}

	private int GetMaxPlusStatValue(float fPlusEXP, float fEXPPerStatUp, float fRemainStat)
	{
		float num = 0f;
		num = fPlusEXP / fEXPPerStatUp;
		if (num >= fRemainStat)
		{
			num = fRemainStat;
		}
		return (int)num;
	}

	private void OnClickSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (m_targetUnitData == null)
		{
			return;
		}
		if (slotData == null || slotData.UID == 0L)
		{
			if (NKMEnhanceManager.CheckUnitFullEnhance(m_targetUnitData))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ALREADY_ENHANCE_MAX);
				return;
			}
			NKCUIUnitSelectList.UnitSelectListOptions options = MakeUnitSelectListOption();
			options.dOnSelectedUnitWarning = CheckSelectedUnit;
			NKCUIUnitSelectList.Instance.Open(options, UpdateUnitSlots);
		}
		else
		{
			m_currentSlotUIDList.Remove(slotData.UID);
			UpdateUnitSlots();
		}
	}

	private bool CheckSelectedUnit(long selectUnitUID, List<long> selectedUnitList, out string msg)
	{
		msg = string.Empty;
		if (!IsDismissibleUnit(selectUnitUID))
		{
			msg = NKCStringTable.GetString("SI_DP_ENHANCE_SELECT_HIGH_RARITY_AND_LEVEL");
			return true;
		}
		List<long> list = new List<long>();
		if (selectedUnitList != null && selectedUnitList.Count > 0)
		{
			list.AddRange(selectedUnitList);
		}
		if (WillBecomeFullExp(m_targetUnitData, new HashSet<long>(list)))
		{
			list.RemoveAt(list.Count - 1);
			if (!WillBecomeFullExp(m_targetUnitData, new HashSet<long>(list)))
			{
				msg = NKCStringTable.GetString("SI_DP_ENHANCE_ALREADY_MAX_CONSUME_UNIT");
				return true;
			}
		}
		return false;
	}

	private bool IsDismissibleUnit(long unitUID)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(unitUID);
			if (unitFromUID.m_UnitLevel > 1)
			{
				return false;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID);
			if (unitTempletBase != null && unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
			{
				return true;
			}
			if (unitFromUID.GetUnitGrade() >= NKM_UNIT_GRADE.NUG_SR)
			{
				return false;
			}
		}
		return true;
	}

	public void ClearFeedUnitSlot(int index)
	{
		if (index >= 0 && index < m_lstObjUnitSlot.Count)
		{
			m_lstObjUnitSlot[index].m_Slot.SetEmptyMaterial(OnClickSlot);
			NKCUtil.SetGameobjectActive(m_lstObjUnitSlot[index].m_objExpBonus, bValue: false);
		}
	}

	public void ClearAllFeedUnitSlots()
	{
		if (m_currentSlotUIDList.Count > 0)
		{
			m_currentSlotUIDList.Clear();
		}
		UpdateUnitSlots();
	}

	private void UpdateUnitSlots(List<long> unitUID)
	{
		NKCUIUnitSelectList.CheckInstanceAndClose();
		bool flag = false;
		if (unitUID.Count == m_currentSlotUIDList.Count)
		{
			for (int i = 0; i < unitUID.Count; i++)
			{
				if (!m_currentSlotUIDList.Contains(unitUID[i]))
				{
					flag = true;
					break;
				}
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			m_currentSlotUIDList.Clear();
			m_currentSlotUIDList.AddRange(unitUID);
			UpdateUnitSlots();
		}
	}

	private void UpdateUnitSlots()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKM_UNIT_ROLE_TYPE nKM_UNIT_ROLE_TYPE = NKMUnitManager.GetUnitTempletBase(m_targetUnitData)?.m_NKM_UNIT_ROLE_TYPE ?? NKM_UNIT_ROLE_TYPE.NURT_INVALID;
		for (int i = 0; i < m_lstObjUnitSlot.Count; i++)
		{
			if (i < m_currentSlotUIDList.Count)
			{
				NKMUnitData unitFromUID = myUserData.m_ArmyData.GetUnitFromUID(m_currentSlotUIDList[i]);
				if (unitFromUID != null)
				{
					m_lstObjUnitSlot[i].m_Slot.SetUnitData(unitFromUID, bShowName: false, bShowLevel: false, bEnableLayoutElement: true, OnClickSlot);
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID);
					NKCUtil.SetGameobjectActive(m_lstObjUnitSlot[i].m_objExpBonus, nKM_UNIT_ROLE_TYPE == unitTempletBase.m_NKM_UNIT_ROLE_TYPE);
				}
				else
				{
					m_lstObjUnitSlot[i].m_Slot.SetEmptyMaterial(OnClickSlot);
					NKCUtil.SetGameobjectActive(m_lstObjUnitSlot[i].m_objExpBonus, bValue: false);
				}
			}
			else
			{
				m_lstObjUnitSlot[i].m_Slot.SetEmptyMaterial(OnClickSlot);
				NKCUtil.SetGameobjectActive(m_lstObjUnitSlot[i].m_objExpBonus, bValue: false);
			}
		}
		UpdateStatInfo();
		UpdateRequiredCredit();
		SwitchEtcObject(CanEnhance(m_targetUnitData));
	}

	public void UpdateRequiredCredit()
	{
		m_currentSlotUIDList.RemoveAll((long x) => x == 0);
		int reqCnt = NKMEnhanceManager.CalculateCreditCost(m_currentSlotUIDList);
		m_costSlot.SetData(1, reqCnt, NKCScenManager.CurrentUserData().GetCredit());
	}

	private void OnBtnAutoSelect()
	{
		AutoSelect();
	}

	private void AutoSelect()
	{
		if (m_targetUnitData == null || NKMEnhanceManager.CheckUnitFullEnhance(m_targetUnitData))
		{
			return;
		}
		NKCUnitSort nKCUnitSort = new NKCUnitSort(NKCScenManager.CurrentUserData(), MakeSortOptions(bIsAutoSelect: true));
		HashSet<long> hashSet = new HashSet<long>();
		foreach (long currentSlotUID in m_currentSlotUIDList)
		{
			hashSet.Add(currentSlotUID);
		}
		if (WillBecomeFullExp(m_targetUnitData, hashSet))
		{
			return;
		}
		for (int i = m_currentSlotUIDList.Count; i < m_lstObjUnitSlot.Count; i++)
		{
			NKMUnitData nKMUnitData = nKCUnitSort.AutoSelect(hashSet, AutoSelectFilterSameType);
			if (nKMUnitData == null)
			{
				nKMUnitData = nKCUnitSort.AutoSelect(hashSet, AutoSelectFilterAnyType);
			}
			if (nKMUnitData == null)
			{
				break;
			}
			hashSet.Add(nKMUnitData.m_UnitUID);
			if (WillBecomeFullExp(m_targetUnitData, hashSet))
			{
				break;
			}
		}
		List<long> list = new List<long>();
		list.AddRange(hashSet);
		UpdateUnitSlots(list);
		if (dOnAutoSelectPlayVoice != null)
		{
			dOnAutoSelectPlayVoice(NPC_ACTION_TYPE.MATERIAL_AUTO_SELECT, bMute: false);
		}
	}

	private bool WillBecomeFullExp(NKMUnitData targetUnit, HashSet<long> hsTargetUnits)
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_targetUnitData);
		if (unitTempletBase == null)
		{
			return true;
		}
		Dictionary<NKM_STAT_TYPE, int> dictionary = NKMEnhanceManager.CalculateExpGain(armyData, new List<long>(hsTargetUnits), unitTempletBase.m_NKM_UNIT_ROLE_TYPE);
		foreach (NKM_STAT_TYPE item in NKMEnhanceManager.s_lstEnhancebleStat)
		{
			int num = NKMEnhanceManager.CalculateMaxEXP(targetUnit, item);
			if (dictionary[item] + targetUnit.m_listStatEXP[(int)item] < num)
			{
				return false;
			}
		}
		return true;
	}

	private bool AutoSelectFilterSameType(NKMUnitData unitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_targetUnitData);
		if (unitTempletBase == null)
		{
			return false;
		}
		NKM_UNIT_ROLE_TYPE nKM_UNIT_ROLE_TYPE = unitTempletBase.m_NKM_UNIT_ROLE_TYPE;
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase2 != null)
		{
			if (unitData.m_UnitLevel > 1)
			{
				return false;
			}
			if (nKM_UNIT_ROLE_TYPE != unitTempletBase2.m_NKM_UNIT_ROLE_TYPE)
			{
				return false;
			}
			if (unitTempletBase2.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
			{
				return true;
			}
			return unitTempletBase2.m_NKM_UNIT_GRADE switch
			{
				NKM_UNIT_GRADE.NUG_N => true, 
				_ => false, 
			};
		}
		return false;
	}

	private bool AutoSelectFilterAnyType(NKMUnitData unitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase != null)
		{
			if (unitData.m_UnitLevel > 1)
			{
				return false;
			}
			if (unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
			{
				return true;
			}
			switch (unitTempletBase.m_NKM_UNIT_GRADE)
			{
			case NKM_UNIT_GRADE.NUG_N:
				return true;
			case NKM_UNIT_GRADE.NUG_R:
			case NKM_UNIT_GRADE.NUG_SR:
			case NKM_UNIT_GRADE.NUG_SSR:
				return false;
			default:
				return true;
			}
		}
		return false;
	}

	private void OnBtnDetail()
	{
		if (m_targetUnitData != null)
		{
			if (NKCPopupUnitInfoDetail.IsInstanceOpen)
			{
				NKCPopupUnitInfoDetail.CheckInstanceAndClose();
			}
			else
			{
				NKCPopupUnitInfoDetail.InstanceOpen(m_targetUnitData, NKCPopupUnitInfoDetail.UnitInfoDetailType.lab);
			}
		}
	}

	private NKCUIUnitSelectList.UnitSelectListOptions MakeUnitSelectListOption()
	{
		NKCUIUnitSelectList.UnitSelectListOptions result = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL);
		result.m_SortOptions = MakeSortOptions(bIsAutoSelect: false);
		result.bShowRemoveSlot = false;
		result.bShowHideDeckedUnitMenu = false;
		result.strUpsideMenuName = NKCUtilString.GET_STRING_ENHANCE_SELECT_CONSUM_UNIT;
		result.iMaxMultipleSelect = m_lstObjUnitSlot.Count;
		result.dOnAutoSelectFilter = AutoSelectFilterAnyType;
		result.dOnSlotSetData = OnSlotSetData;
		result.strEmptyMessage = NKCUtilString.GET_STRING_ENHANCE_NO_EXIST_CONSUME_UNIT;
		result.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
		result.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
		result.setSelectedUnitUID = new HashSet<long>(m_currentSlotUIDList);
		if (NKCGameEventManager.IsEventPlaying())
		{
			result.m_SortOptions.PreemptiveSortFunc = TutorialSort;
		}
		return result;
	}

	private int TutorialSort(NKMUnitData lhs, NKMUnitData rhs)
	{
		bool flag = lhs.m_UnitID == 1008;
		bool flag2 = rhs.m_UnitID == 1008;
		if (flag != flag2)
		{
			return flag2.CompareTo(flag);
		}
		if (lhs.m_UnitLevel != rhs.m_UnitLevel)
		{
			return lhs.m_UnitLevel.CompareTo(rhs.m_UnitLevel);
		}
		return lhs.m_UnitUID.CompareTo(rhs.m_UnitUID);
	}

	private void OnSlotSetData(NKCUIUnitSelectListSlotBase cUnitSlot, NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex)
	{
		if (cNKMUnitData == null)
		{
			return;
		}
		NKCUIUnitSelectListSlot nKCUIUnitSelectListSlot = cUnitSlot as NKCUIUnitSelectListSlot;
		if (nKCUIUnitSelectListSlot != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_targetUnitData);
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(cNKMUnitData);
			if (unitTempletBase != null && unitTempletBase2 != null && unitTempletBase.m_NKM_UNIT_ROLE_TYPE == unitTempletBase2.m_NKM_UNIT_ROLE_TYPE)
			{
				nKCUIUnitSelectListSlot.SetExpBonusMark(value: true);
			}
			else
			{
				nKCUIUnitSelectListSlot.SetExpBonusMark(value: false);
			}
		}
	}

	private NKCUnitSortSystem.UnitListOptions MakeSortOptions(bool bIsAutoSelect)
	{
		NKCUnitSortSystem.UnitListOptions result = new NKCUnitSortSystem.UnitListOptions
		{
			bDescending = false,
			setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>(),
			lstSortOption = new List<NKCUnitSortSystem.eSortOption>
			{
				NKCUnitSortSystem.eSortOption.Rarity_Low,
				NKCUnitSortSystem.eSortOption.Level_Low
			},
			bExcludeDeckedUnit = false,
			bExcludeLockedUnit = false,
			bHideDeckedUnit = false,
			bUseDeckedState = true,
			bUseLockedState = true,
			bPushBackUnselectable = true,
			eDeckType = NKM_DECK_TYPE.NDT_NORMAL,
			bIncludeUndeckableUnit = true,
			setExcludeUnitID = NKCUnitSortSystem.GetDefaultExcludeUnitIDs(),
			AdditionalExcludeFilterFunc = IsUnitHaveEnhanceExp
		};
		HashSet<long> hashSet = new HashSet<long>();
		hashSet.Add(m_targetUnitData.m_UnitUID);
		result.setExcludeUnitUID = hashSet;
		if (bIsAutoSelect)
		{
			result.PreemptiveSortFunc = CompareByRoleForAutoSelect;
		}
		else
		{
			result.PreemptiveSortFunc = CompareByRole;
		}
		return result;
	}

	private bool IsUnitHaveEnhanceExp(NKMUnitData unitData)
	{
		if (unitData != null)
		{
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
			if (unitStatTemplet != null)
			{
				for (int i = 0; i < NKMEnhanceManager.s_lstEnhancebleStat.Count; i++)
				{
					int statType = (int)NKMEnhanceManager.s_lstEnhancebleStat[i];
					if (unitStatTemplet.m_StatData.GetStatEnhanceFeedEXP(statType) > 0f)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void UnitUpdated(long uid, NKMUnitData unitData)
	{
		if (m_targetUnitData != null && m_targetUnitData.m_UnitUID == uid)
		{
			SetDataWithoutReset(unitData);
		}
	}

	private int Compare(NKMUnitData lhs, NKMUnitData rhs, bool TrophyFirst)
	{
		if (m_targetUnitData == null)
		{
			return 0;
		}
		int num = lhs.m_bLock.CompareTo(rhs.m_bLock);
		if (num != 0)
		{
			return num;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_targetUnitData.m_UnitID);
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(lhs.m_UnitID);
		NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(rhs.m_UnitID);
		if (unitTempletBase == null || unitTempletBase2 == null || unitTempletBase3 == null)
		{
			return 0;
		}
		bool flag = unitTempletBase.m_NKM_UNIT_ROLE_TYPE == unitTempletBase2.m_NKM_UNIT_ROLE_TYPE;
		bool flag2 = unitTempletBase.m_NKM_UNIT_ROLE_TYPE == unitTempletBase3.m_NKM_UNIT_ROLE_TYPE;
		if (flag != flag2)
		{
			return flag2.CompareTo(flag);
		}
		bool flag3 = unitTempletBase2.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER;
		bool flag4 = unitTempletBase3.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER;
		if (flag3 != flag4)
		{
			if (TrophyFirst)
			{
				return flag4.CompareTo(flag3);
			}
			return flag3.CompareTo(flag4);
		}
		return 0;
	}

	private int CompareByRole(NKMUnitData lhs, NKMUnitData rhs)
	{
		return Compare(lhs, rhs, TrophyFirst: true);
	}

	private int CompareByRoleForAutoSelect(NKMUnitData lhs, NKMUnitData rhs)
	{
		return Compare(lhs, rhs, TrophyFirst: true);
	}

	private bool CanEnhance(NKMUnitData targetUnit)
	{
		if (targetUnit == null)
		{
			return false;
		}
		NKM_DECK_STATE unitDeckState = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitDeckState(targetUnit);
		if ((uint)(unitDeckState - 2) <= 1u)
		{
			return false;
		}
		return m_currentSlotUIDList.Count > 0;
	}
}
