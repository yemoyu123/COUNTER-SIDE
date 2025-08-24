using System;
using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Component;

public class NKCUIComBattleEnvironmentList : MonoBehaviour
{
	[Serializable]
	public class BuffProgress
	{
		public GameObject m_objSetupBuffProgressRoot;

		public GameObject m_objSetupBuffProgressLow;

		public GameObject m_objSetupBuffProgressMid;

		public GameObject m_objSetupBuffProgressHigh;

		public void SetProgress(int progressCount, int totalCount)
		{
			if (totalCount == 0)
			{
				NKCUtil.SetGameobjectActive(m_objSetupBuffProgressRoot, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(m_objSetupBuffProgressRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSetupBuffProgressLow, progressCount == 0);
			NKCUtil.SetGameobjectActive(m_objSetupBuffProgressMid, progressCount > 0 && progressCount < totalCount);
			NKCUtil.SetGameobjectActive(m_objSetupBuffProgressHigh, progressCount == totalCount);
		}
	}

	private enum Tab
	{
		Environment,
		DeckCondition
	}

	[Header("Ÿ\ufffd\ufffdƲ")]
	public GameObject m_objBattleConditionTitle;

	public GameObject m_objDeckConditionTitle;

	[Header("\ufffd\ufffdư")]
	public NKCUIComToggle m_tglEnvironment;

	public GameObject m_objRootEnvironment;

	public NKCUIComToggle m_tglDeckSetupCondition;

	public GameObject m_objRootDeckSetupCondition;

	public NKCUIComToggle m_tglClose;

	[Header("\ufffd\ufffdƲ\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public RectTransform m_rtBCSlotParents;

	public NKCUIBattleConditionDetailSlot m_pfbBCSlots;

	private List<NKCUIBattleConditionDetailSlot> m_lstBCSlots = new List<NKCUIBattleConditionDetailSlot>();

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objRootDeckCondition;

	public RectTransform m_rtConditionSlotParent;

	public NKCUIDeckConditionSlot m_pfbDeckConditionSlot;

	private List<NKCUIDeckConditionSlot> m_lstDeckConditionSlots = new List<NKCUIDeckConditionSlot>();

	private Dictionary<NKMDeckCondition.ALL_DECK_CONDITION, int> m_dicAllDeckValueCache = new Dictionary<NKMDeckCondition.ALL_DECK_CONDITION, int>();

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objRootSetupBuff;

	public RectTransform m_rtSetupBuffSlotParent;

	public NKCUIBattlePreConditionDetailSlot m_pfbPreConditionSlot;

	private List<NKCUIBattlePreConditionDetailSlot> m_lstPreConditionSlots = new List<NKCUIBattlePreConditionDetailSlot>();

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd ǥ\ufffd\ufffd")]
	public List<BuffProgress> m_lstBuffProgress;

	private NKMDeckCondition m_DeckCondition;

	private List<NKMBattleConditionTemplet> m_lstBC;

	private List<int> m_lstPreConditionalBC;

	private List<GameUnitData> m_lstGameUnit;

	private NKMUnitData m_ShipData;

	private NKMOperator m_Operator;

	private long m_LeaderUID;

	private Tab m_Tab = Tab.DeckCondition;

	private void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_tglEnvironment, OnTglEnvironment);
		NKCUtil.SetButtonClickDelegate(m_tglDeckSetupCondition, OnTglDeckCondition);
		NKCUtil.SetButtonClickDelegate(m_tglClose, OnTglOpenClose);
	}

	private void ClearAllSlots()
	{
		foreach (NKCUIBattleConditionDetailSlot lstBCSlot in m_lstBCSlots)
		{
			UnityEngine.Object.Destroy(lstBCSlot.gameObject);
		}
		m_lstBCSlots.Clear();
		foreach (NKCUIDeckConditionSlot lstDeckConditionSlot in m_lstDeckConditionSlots)
		{
			UnityEngine.Object.Destroy(lstDeckConditionSlot.gameObject);
		}
		m_lstDeckConditionSlots.Clear();
	}

	public bool InitData(NKMDeckCondition deckCondition, List<NKMBattleConditionTemplet> lstBattleCondition, List<int> lstPreconditionBC)
	{
		Init();
		ClearAllSlots();
		m_DeckCondition = deckCondition;
		m_lstBC = lstBattleCondition?.FindAll((NKMBattleConditionTemplet x) => !x.m_bHide);
		m_lstPreConditionalBC = ((lstPreconditionBC != null) ? new List<int>(lstPreconditionBC) : null);
		bool flag = m_lstBC != null && m_lstBC.Count > 0;
		bool flag2 = m_DeckCondition != null || (m_lstPreConditionalBC != null && m_lstPreConditionalBC.Count > 0);
		bool flag3 = flag || flag2;
		NKCUtil.SetButtonLock(m_tglEnvironment, !flag);
		NKCUtil.SetButtonLock(m_tglDeckSetupCondition, !flag2);
		NKCUtil.SetButtonLock(m_tglClose, !flag3);
		if (flag2)
		{
			m_Tab = Tab.DeckCondition;
		}
		else if (flag)
		{
			m_Tab = Tab.Environment;
		}
		return flag3;
	}

	public void Open()
	{
		base.gameObject.SetActive(value: true);
		if (m_tglClose != null)
		{
			m_tglClose.Select(bSelect: true);
		}
		SelectTab(m_Tab);
	}

	public void Close()
	{
		base.gameObject.SetActive(value: false);
		if (m_tglClose != null)
		{
			m_tglClose.Select(bSelect: false);
		}
	}

	public void ClearData()
	{
		if (m_lstBC != null)
		{
			m_lstBC.Clear();
			m_lstBC = null;
		}
		if (m_lstPreConditionalBC != null)
		{
			m_lstPreConditionalBC.Clear();
			m_lstPreConditionalBC = null;
		}
		m_DeckCondition = null;
		m_LeaderUID = 0L;
		m_Operator = null;
		m_lstGameUnit = null;
		m_ShipData = null;
		ClearAllSlots();
	}

	private void SelectTab(Tab tab)
	{
		m_Tab = tab;
		NKCUtil.SetGameobjectActive(m_objRootEnvironment, m_Tab == Tab.Environment);
		NKCUtil.SetGameobjectActive(m_objRootDeckSetupCondition, m_Tab == Tab.DeckCondition);
		NKCUtil.SetGameobjectActive(m_objBattleConditionTitle, m_Tab == Tab.Environment);
		NKCUtil.SetGameobjectActive(m_objDeckConditionTitle, m_Tab == Tab.DeckCondition);
		if (m_tglEnvironment != null)
		{
			m_tglEnvironment.Select(m_Tab == Tab.Environment, bForce: true);
		}
		if (m_tglDeckSetupCondition != null)
		{
			m_tglDeckSetupCondition.Select(m_Tab == Tab.DeckCondition, bForce: true);
		}
		UpdateData(bReset: true);
	}

	public void UpdateData(NKMEventDeckData eventDeckData, NKMDungeonEventDeckTemplet eventDeckTemplet)
	{
		List<GameUnitData> list = NKMDungeonManager.MakeEventDeckUnitDataList(NKCScenManager.CurrentArmyData(), eventDeckTemplet, m_DeckCondition, eventDeckData, NKCScenManager.CurrentUserData().m_InventoryData);
		NKMUnitData shipData = NKMDungeonManager.MakeEventDeckShipData(NKCScenManager.CurrentArmyData(), eventDeckTemplet, m_DeckCondition, eventDeckData);
		NKMOperator operatorData = NKMDungeonManager.MakeEventDeckOperatorData(NKCScenManager.CurrentArmyData(), eventDeckTemplet, m_DeckCondition, eventDeckData);
		UpdateData(list, shipData, operatorData, eventDeckData.GetLeaderUID(list, eventDeckTemplet));
	}

	public void UpdateData(NKMDeckData deckData)
	{
		List<GameUnitData> lstUnit = NKMDungeonManager.MakeDeckUnitDataList(NKCScenManager.CurrentArmyData(), deckData, NKCScenManager.CurrentUserData().m_InventoryData);
		NKMUnitData shipFromUID = NKCScenManager.CurrentArmyData().GetShipFromUID(deckData.m_ShipUID);
		NKMOperator operatorFromUId = NKCScenManager.CurrentArmyData().GetOperatorFromUId(deckData.m_OperatorUID);
		UpdateData(lstUnit, shipFromUID, operatorFromUId, deckData.GetLeaderUnitUID());
	}

	public void UpdateData(List<GameUnitData> lstUnit, NKMUnitData shipData, NKMOperator operatorData, long leaderUID)
	{
		m_lstGameUnit = lstUnit;
		m_ShipData = shipData;
		m_Operator = operatorData;
		m_LeaderUID = leaderUID;
		UpdateData(bReset: false);
	}

	private void UpdateData(bool bReset)
	{
		switch (m_Tab)
		{
		case Tab.Environment:
			SetNonConditionalBattleCondition(m_lstBC);
			break;
		case Tab.DeckCondition:
			SetConditionalDeckCondition(m_lstPreConditionalBC, m_lstGameUnit, m_ShipData, m_Operator, m_LeaderUID, bReset);
			SetDeckCondition(m_lstGameUnit, m_DeckCondition);
			break;
		}
		UpdateConditionalBCProgress(m_lstPreConditionalBC, m_lstGameUnit, m_ShipData, m_Operator, m_LeaderUID);
	}

	private void SetNonConditionalBattleCondition(List<NKMBattleConditionTemplet> lstBattleCondition)
	{
		if (lstBattleCondition == null)
		{
			return;
		}
		int count = lstBattleCondition.Count;
		while (m_lstBCSlots.Count < count)
		{
			NKCUIBattleConditionDetailSlot item = UnityEngine.Object.Instantiate(m_pfbBCSlots, m_rtBCSlotParents);
			m_lstBCSlots.Add(item);
		}
		for (int i = 0; i < m_lstBCSlots.Count; i++)
		{
			NKCUIBattleConditionDetailSlot nKCUIBattleConditionDetailSlot = m_lstBCSlots[i];
			if (!(nKCUIBattleConditionDetailSlot == null))
			{
				if (i < lstBattleCondition.Count)
				{
					NKCUtil.SetGameobjectActive(nKCUIBattleConditionDetailSlot, bValue: true);
					nKCUIBattleConditionDetailSlot.SetData(i, lstBattleCondition[i]);
				}
				else
				{
					NKCUtil.SetGameobjectActive(nKCUIBattleConditionDetailSlot, bValue: false);
				}
			}
		}
	}

	private void SetConditionalDeckCondition(List<int> lstPreconditionBC, List<GameUnitData> lstUnit, NKMUnitData shipData, NKMOperator operatorData, long leaderUID, bool bReset)
	{
		if (lstPreconditionBC == null || lstPreconditionBC.Count == 0)
		{
			NKCUtil.SetGameobjectActive(m_objRootSetupBuff, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objRootSetupBuff, bValue: true);
		int count = lstPreconditionBC.Count;
		while (m_lstPreConditionSlots.Count < count)
		{
			NKCUIBattlePreConditionDetailSlot item = UnityEngine.Object.Instantiate(m_pfbPreConditionSlot, m_rtSetupBuffSlotParent);
			m_lstPreConditionSlots.Add(item);
		}
		for (int i = 0; i < m_lstPreConditionSlots.Count; i++)
		{
			NKCUIBattlePreConditionDetailSlot nKCUIBattlePreConditionDetailSlot = m_lstPreConditionSlots[i];
			if (i < lstPreconditionBC.Count)
			{
				int groupID = lstPreconditionBC[i];
				NKCUtil.SetGameobjectActive(nKCUIBattlePreConditionDetailSlot, bValue: true);
				nKCUIBattlePreConditionDetailSlot.SetData(groupID, lstUnit, shipData, operatorData, leaderUID, bReset);
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUIBattlePreConditionDetailSlot, bValue: false);
			}
		}
	}

	private void UpdateConditionalBCProgress(List<int> lstPreconditionBC, List<GameUnitData> lstUnit, NKMUnitData shipData, NKMOperator operatorData, long leaderUID)
	{
		if (lstPreconditionBC == null || lstPreconditionBC.Count == 0)
		{
			foreach (BuffProgress item in m_lstBuffProgress)
			{
				item?.SetProgress(0, 0);
			}
			return;
		}
		int num = 0;
		int num2 = 0;
		foreach (int item2 in lstPreconditionBC)
		{
			List<NKMPreconditionBCGroupTemplet> preConditionTypeByGroupId = NKMBattleConditionManager.GetPreConditionTypeByGroupId(item2);
			if (preConditionTypeByGroupId == null)
			{
				continue;
			}
			num += preConditionTypeByGroupId.Count;
			NKMBattleConditionTemplet EnabledBCTemplet = NKMBattleConditionManager.GetEnabledBCByPreCondition(item2, NKCScenManager.CurrentUserData().m_InventoryData, lstUnit, shipData, operatorData, leaderUID);
			if (EnabledBCTemplet != null)
			{
				int num3 = preConditionTypeByGroupId.FindIndex((NKMPreconditionBCGroupTemplet x) => x.m_BCondID == EnabledBCTemplet.BattleCondID);
				num2 += num3 + 1;
			}
		}
		foreach (BuffProgress item3 in m_lstBuffProgress)
		{
			item3?.SetProgress(num2, num);
		}
	}

	private void SetDeckCondition(List<GameUnitData> lstUnitData, NKMDeckCondition deckCondition)
	{
		if (deckCondition == null)
		{
			NKCUtil.SetGameobjectActive(m_objRootDeckCondition, bValue: false);
			return;
		}
		CalculateDeckAllCondition(lstUnitData, deckCondition);
		NKCUtil.SetGameobjectActive(m_objRootDeckCondition, bValue: true);
		int num = deckCondition.ConditionCount + deckCondition.m_dicGameCondition.Count;
		while (m_lstDeckConditionSlots.Count < num)
		{
			NKCUIDeckConditionSlot item = UnityEngine.Object.Instantiate(m_pfbDeckConditionSlot, m_rtConditionSlotParent);
			m_lstDeckConditionSlots.Add(item);
		}
		List<NKMDeckCondition.EventDeckCondition> list = new List<NKMDeckCondition.EventDeckCondition>(deckCondition.AllConditionEnumerator());
		List<NKMDeckCondition.GameCondition> list2 = new List<NKMDeckCondition.GameCondition>(deckCondition.m_dicGameCondition.Values);
		for (int i = 0; i < m_lstDeckConditionSlots.Count; i++)
		{
			if (i < list.Count)
			{
				if (list[i] is NKMDeckCondition.AllDeckCondition allDeckCondition && m_dicAllDeckValueCache.TryGetValue(allDeckCondition.eCondition, out var value))
				{
					m_lstDeckConditionSlots[i].SetCondition(list[i], value);
				}
				else
				{
					m_lstDeckConditionSlots[i].SetCondition(list[i]);
				}
				NKCUtil.SetGameobjectActive(m_lstDeckConditionSlots[i].gameObject, bValue: true);
			}
			else if (list.Count <= i && i < list.Count + list2.Count)
			{
				m_lstDeckConditionSlots[i].SetCondition(list2[i - list.Count]);
				NKCUtil.SetGameobjectActive(m_lstDeckConditionSlots[i].gameObject, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstDeckConditionSlots[i].gameObject, bValue: false);
			}
		}
	}

	private void CalculateDeckAllCondition(List<GameUnitData> lstUnitData, NKMDeckCondition deckCondition)
	{
		Debug.Log("RecalculateDeckAllCondition");
		m_dicAllDeckValueCache.Clear();
		if (deckCondition.m_dicAllDeckCondition == null)
		{
			return;
		}
		NKCScenManager.CurrentUserData();
		foreach (NKMDeckCondition.AllDeckCondition value in deckCondition.m_dicAllDeckCondition.Values)
		{
			int num = 0;
			foreach (GameUnitData lstUnitDatum in lstUnitData)
			{
				NKMUnitData nKMUnitData = lstUnitDatum?.unit;
				if (nKMUnitData != null)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(nKMUnitData);
					num += value.GetAllDeckConditionValue(unitTempletBase);
				}
			}
			m_dicAllDeckValueCache[value.eCondition] = num;
		}
	}

	private void OnTglEnvironment(bool value)
	{
		SelectTab(Tab.Environment);
	}

	private void OnTglDeckCondition(bool value)
	{
		SelectTab(Tab.DeckCondition);
	}

	private void OnTglOpenClose(bool value)
	{
		if (value)
		{
			Open();
		}
		else
		{
			Close();
		}
	}
}
