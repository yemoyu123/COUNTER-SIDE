using System.Collections.Generic;
using Cs.Logging;
using NKC.UI.Collection;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLobbyEventDeckData : MonoBehaviour
{
	public GameObject m_objDeckRoot;

	public GameObject m_objShip;

	public NKCUIComStateButton m_csbtnShip;

	public Image m_shipImage;

	public NKCUISlot m_slotShip;

	public NKCUIOperatorDeckSlot m_operatorSlot;

	public NKCUISlot m_randomSlot;

	public NKCUISlot[] m_unitSlot;

	public GameObject m_objFreeDeck;

	public Text m_lbEventDeckDesc;

	public Text m_lbUnitSlotCondition;

	private int m_shipId;

	private int m_operatorId;

	public void Init()
	{
		if (m_unitSlot != null)
		{
			int num = m_unitSlot.Length;
			for (int i = 0; i < num; i++)
			{
				m_unitSlot[i].Init();
			}
		}
		if (m_randomSlot != null)
		{
			m_randomSlot.Init();
		}
		m_slotShip.Init();
		m_operatorSlot.Init(OnSelectOperator);
		NKCUtil.SetButtonClickDelegate(m_csbtnShip, OnClickShip);
	}

	public void SetData(NKMDungeonEventDeckTemplet eventDeckData)
	{
		if (eventDeckData == null)
		{
			SetActive(active: false);
			Log.Error("EventDeckData not exist", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLobbyEventDeckData.cs", 54);
			return;
		}
		bool flag = true;
		m_shipId = 0;
		m_operatorId = 0;
		if (NKCEventPvpMgr.IsDeteminedSlotType(eventDeckData.ShipSlot.m_eType))
		{
			m_shipId = eventDeckData.ShipSlot.m_ID;
			if (NKMUnitManager.GetUnitTempletBase(m_shipId).IsShip())
			{
				NKCUtil.SetGameobjectActive(m_objShip, bValue: true);
				NKCUtil.SetGameobjectActive(m_slotShip, bValue: false);
				Sprite sp = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, eventDeckData.ShipSlot.m_ID, eventDeckData.ShipSlot.m_SkinID);
				NKCUtil.SetImageSprite(m_shipImage, sp);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objShip, bValue: false);
				NKCUtil.SetGameobjectActive(m_slotShip, bValue: true);
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeUnitData(eventDeckData.ShipSlot.m_ID, eventDeckData.ShipSlot.m_Level, eventDeckData.ShipSlot.m_SkinID);
				m_slotShip.SetData(data, bEnableLayoutElement: true, OnClickShipSlot);
			}
			flag = false;
		}
		else if (eventDeckData.ShipSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
		{
			NKCUtil.SetGameobjectActive(m_objShip, bValue: true);
			NKCUtil.SetGameobjectActive(m_slotShip, bValue: false);
			NKCUtil.SetImageSprite(m_shipImage, NKCResourceUtility.GetShipRandomFaceCard(), bDisableIfSpriteNull: true);
			flag = false;
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objShip, bValue: false);
			NKCUtil.SetGameobjectActive(m_slotShip, bValue: false);
		}
		if (NKCEventPvpMgr.IsDeteminedSlotType(eventDeckData.OperatorSlot.m_eType))
		{
			m_operatorId = eventDeckData.OperatorSlot.m_ID;
			NKCUtil.SetGameobjectActive(m_operatorSlot, bValue: true);
			NKMOperator dummyOperator = NKCOperatorUtil.GetDummyOperator(NKMUnitManager.GetUnitTempletBase(eventDeckData.OperatorSlot.m_ID).m_UnitID, bSetMaximum: true);
			dummyOperator.level = eventDeckData.OperatorSlot.m_Level;
			NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(eventDeckData.OperatorSubSkillID);
			if (skillTemplet != null)
			{
				dummyOperator.subSkill.id = skillTemplet.m_OperSkillID;
				dummyOperator.subSkill.level = (byte)skillTemplet.m_MaxSkillLevel;
			}
			m_operatorSlot?.SetData(dummyOperator);
			flag = false;
		}
		else if (eventDeckData.OperatorSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
		{
			NKCUtil.SetGameobjectActive(m_operatorSlot, bValue: true);
			if (m_operatorSlot != null)
			{
				m_operatorSlot.SetRandom();
			}
			flag = false;
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_operatorSlot, bValue: false);
		}
		int num = 0;
		int num2 = 0;
		if (eventDeckData.HasRandomUnitSlot() && m_randomSlot != null)
		{
			NKCUtil.SetGameobjectActive(m_randomSlot, bValue: true);
			NKCUISlot.SlotData data2 = NKCUISlot.SlotData.MakeMiscItemData(901, 1L);
			m_randomSlot.SetData(data2, bEnableLayoutElement: true, OnClickRandomUnitSlot);
			HashSet<int> allRandomUnits = GetAllRandomUnits(eventDeckData);
			int num3 = -1;
			foreach (int item in allRandomUnits)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(item);
				if (unitTempletBase != null)
				{
					num3 = Mathf.Max((int)unitTempletBase.m_NKM_UNIT_GRADE, num3);
				}
			}
			m_randomSlot.SetBackGround(num3);
			flag = false;
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_randomSlot, bValue: false);
		}
		if (m_unitSlot != null)
		{
			num2 = m_unitSlot.Length;
			for (int i = 0; i < num2; i++)
			{
				NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = eventDeckData.GetUnitSlot(i);
				if (unitSlot.m_eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED)
				{
					num++;
				}
				if (!(m_unitSlot[i] == null))
				{
					if (NKCEventPvpMgr.IsDeteminedSlotType(unitSlot.m_eType))
					{
						m_unitSlot[i].SetActive(bSet: true);
						NKCUISlot.SlotData data3 = NKCUISlot.SlotData.MakeUnitData(unitSlot.m_ID, unitSlot.m_Level, unitSlot.m_SkinID);
						m_unitSlot[i].SetData(data3, bEnableLayoutElement: true, OnClickSlot);
						flag = false;
					}
					else
					{
						m_unitSlot[i].SetActive(bSet: false);
					}
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_objDeckRoot, !flag);
		NKCUtil.SetGameobjectActive(m_objFreeDeck, flag && num == num2);
		NKCUtil.SetGameobjectActive(m_lbEventDeckDesc, num2 > num);
		NKCUtil.SetGameobjectActive(m_lbUnitSlotCondition, num2 > num);
		if (num2 > num)
		{
			NKCUtil.SetLabelText(m_lbEventDeckDesc, string.Format(NKCUtilString.GET_STRING_GAUNTLET_EVENTMATCH_EVENTDECK_DESC, num));
			if (flag)
			{
				NKCUtil.SetLabelText(m_lbUnitSlotCondition, NKCUtilString.GET_STRING_GAUNTLET_EVENT_SLOT_CLOSED);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbUnitSlotCondition, NKCUtilString.GET_STRING_GAUNTLET_EVENT_SLOT_UNIT_DETERMINED);
			}
		}
	}

	public void SetActive(bool active)
	{
		base.gameObject.SetActive(active);
	}

	private void OnClickSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKCUICollectionUnitInfo.CheckInstanceAndOpen(NKCUtil.MakeDummyUnit(slotData.ID, 100, 3), null, null, NKCUICollectionUnitInfo.eCollectionState.CS_PROFILE, isGauntlet: false, NKCUIUpsideMenu.eMode.BackButtonOnly);
	}

	private void OnClickShipSlot(NKCUISlot.SlotData slotData, bool bLock)
	{
		OnClickShip();
	}

	private void OnClickShip()
	{
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet?.EventDeckTemplet != null && eventPvpSeasonTemplet.EventDeckTemplet.ShipSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
		{
			OpenRandomSlotUnitList(eventPvpSeasonTemplet.EventDeckTemplet.ShipSlot);
		}
		else
		{
			NKCUICollectionShipInfo.CheckInstanceAndOpen(NKCUtil.MakeDummyUnit(m_shipId, 100, 3), NKMDeckIndex.None);
		}
	}

	private void OnSelectOperator(long unitUID)
	{
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
		NKMOperator dummyOperator = NKCOperatorUtil.GetDummyOperator(m_operatorId, bSetMaximum: true);
		if (eventPvpSeasonTemplet?.EventDeckTemplet != null)
		{
			if (eventPvpSeasonTemplet.EventDeckTemplet.OperatorSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
			{
				OpenRandomSlotUnitList(eventPvpSeasonTemplet.EventDeckTemplet.OperatorSlot);
				return;
			}
			if (dummyOperator != null)
			{
				dummyOperator.level = eventPvpSeasonTemplet.EventDeckTemplet.OperatorSlot.m_Level;
				NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(eventPvpSeasonTemplet.EventDeckTemplet.OperatorSubSkillID);
				if (skillTemplet != null)
				{
					dummyOperator.subSkill.id = skillTemplet.m_OperSkillID;
					dummyOperator.subSkill.level = (byte)skillTemplet.m_MaxSkillLevel;
				}
			}
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_operatorId);
		if (dummyOperator == null || unitTempletBase == null || unitTempletBase.IsUnitDescNullOrEmplty())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_EVENT_UNIT_DETAIL_INFO_NOT_POSSIBLE);
		}
		else
		{
			NKCUICollectionOperatorInfoV2.CheckInstanceAndOpen(dummyOperator, null, NKCUICollectionOperatorInfoV2.eCollectionState.CS_PROFILE, NKCUIUpsideMenu.eMode.BackButtonOnly);
		}
	}

	private HashSet<int> GetAllRandomUnits(NKMDungeonEventDeckTemplet eventDeckTemplet)
	{
		if (eventDeckTemplet == null)
		{
			return new HashSet<int>();
		}
		return eventDeckTemplet.GetAllRandomUnits();
	}

	private void OpenRandomSlotUnitList(NKMDungeonEventDeckTemplet.EventDeckSlot slot)
	{
		HashSet<int> hashSet = new HashSet<int>();
		List<int> connectedUnitList = slot.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_A1);
		List<int> connectedUnitList2 = slot.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_B1);
		if (connectedUnitList != null)
		{
			hashSet.UnionWith(connectedUnitList);
		}
		if (connectedUnitList2 != null)
		{
			hashSet.UnionWith(connectedUnitList2);
		}
		NKCUISlotListViewer.Instance.OpenGenericUnitList(NKCStringTable.GetString("SI_DP_SLOT_RANDOM_UNIT_VIEWR"), NKCStringTable.GetString("SI_DP_SLOT_RANDOM_UNIT_VIEWR_DESC"), hashSet, slot.m_Level);
	}

	private void OnClickRandomUnitSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet?.EventDeckTemplet == null)
		{
			return;
		}
		HashSet<int> allRandomUnits = GetAllRandomUnits(eventPvpSeasonTemplet.EventDeckTemplet);
		int unitLevel = 100;
		foreach (NKMDungeonEventDeckTemplet.EventDeckSlot item in eventPvpSeasonTemplet.EventDeckTemplet.m_lstUnitSlot)
		{
			if (item.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
			{
				unitLevel = item.m_Level;
				break;
			}
		}
		NKCUISlotListViewer.Instance.OpenGenericUnitList(NKCStringTable.GetString("SI_DP_SLOT_RANDOM_UNIT_VIEWR"), NKCStringTable.GetString("SI_DP_SLOT_RANDOM_UNIT_VIEWR_DESC"), allRandomUnits, unitLevel);
	}
}
