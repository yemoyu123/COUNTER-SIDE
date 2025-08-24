using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitSelectListEventDeckSlot : NKCUIUnitSelectListSlot, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IDropHandler
{
	public delegate void OnSelectEventDeckSlot(int index);

	public delegate void OnUnitDetail(NKMUnitData unitData);

	public delegate void DragHandler(PointerEventData eventData, int index);

	[Header("이벤트 덱 관련")]
	public Image m_ImgTargetUnit;

	public GameObject m_objTouchToEntry;

	public GameObject m_objGuestPlus;

	public GameObject m_objGuest;

	public GameObject m_objCounterOnly;

	public GameObject m_objSoldierOnly;

	public GameObject m_objMechOnly;

	public GameObject m_objNPC;

	public GameObject m_objRandom;

	[Header("리더 선택")]
	public GameObject m_objLeaderSelectFx;

	public GameObject m_objLeaderMark;

	[Space]
	public NKCUIComStateButton m_csbtnDetail;

	private NKMDungeonEventDeckTemplet.SLOT_TYPE m_eSlotType;

	private int m_index;

	private OnSelectEventDeckSlot dOnSelectEventDeckSlot;

	private OnUnitDetail dOnUnitDetail;

	private int m_targetUnit;

	private DragHandler dOnBeginDrag;

	private DragHandler dOnDrag;

	private DragHandler dOnEndDrag;

	private DragHandler dOnDrop;

	public void SetData(NKMUnitData unitData, int index, bool bEnableLayoutElement, OnSelectEventDeckSlot onSelectEventDeckSlot, bool bShowFierceInfo, bool equipEnabled)
	{
		SetData(unitData, NKMDeckIndex.None, bEnableLayoutElement, null);
		m_index = index;
		dOnSelectEventDeckSlot = onSelectEventDeckSlot;
		SetEnableEquipListData(equipEnabled);
		SetEquipListData(m_NKMUnitData);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_FIERCE_BATTLE, bValue: false);
	}

	public void InitEventSlot(NKMDungeonEventDeckTemplet.EventDeckSlot eventSlotData, int index, bool bEnableLayoutElement, bool equipEnabled, OnSelectEventDeckSlot onSelectEventDeckSlot, OnUnitDetail onUnitDetail)
	{
		m_index = index;
		dOnSelectEventDeckSlot = onSelectEventDeckSlot;
		dOnUnitDetail = onUnitDetail;
		m_eSlotType = eventSlotData.m_eType;
		NKCUtil.SetButtonClickDelegate(m_csbtnDetail, OnBtnDetail);
		NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
		switch (eventSlotData.m_eType)
		{
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
			SetTargetUnit(eventSlotData.m_ID);
			break;
		default:
			SetTargetUnit(0);
			break;
		}
		SetSlotTypeLabel(eventSlotData.m_eType);
		switch (eventSlotData.m_eType)
		{
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
			SetRandom(bEnableLayoutElement, null);
			break;
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED:
			SetClosed(bEnableLayoutElement, null);
			break;
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
		{
			NKMUnitData cNKMUnitData = NKMDungeonManager.MakeUnitDataFromID(eventSlotData.m_ID, -1L, eventSlotData.m_Level, -1, eventSlotData.m_SkinID, eventSlotData.m_TacticLevel);
			SetData(cNKMUnitData, NKMDeckIndex.None, bEnableLayoutElement, null);
			break;
		}
		default:
			SetEmpty(bEnableLayoutElement, null);
			break;
		}
		SetEquipListData(m_NKMUnitData);
		SetEnableEquipListData(equipEnabled);
	}

	public void SetSlotTypeLabel(NKMDungeonEventDeckTemplet.SLOT_TYPE slotType)
	{
		NKCUtil.SetGameobjectActive(m_objGuest, slotType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST);
		NKCUtil.SetGameobjectActive(m_objCounterOnly, slotType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_COUNTER);
		NKCUtil.SetGameobjectActive(m_objSoldierOnly, slotType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_SOLDIER);
		NKCUtil.SetGameobjectActive(m_objMechOnly, slotType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_MECHANIC);
		NKCUtil.SetGameobjectActive(m_objNPC, slotType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC);
		NKCUtil.SetGameobjectActive(m_objRandom, slotType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM);
	}

	public void SetTargetUnit(int unitID)
	{
		m_targetUnit = unitID;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		m_ImgTargetUnit.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase);
	}

	protected override void SetMode(eUnitSlotMode mode)
	{
		base.SetMode(mode);
		bool bValue = ((m_eSlotType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED || mode != eUnitSlotMode.Character) ? (m_targetUnit != 0 && mode == eUnitSlotMode.Empty) : (m_targetUnit != 0 && !HasUnit()));
		NKCUtil.SetGameobjectActive(m_objGuestPlus, m_eSlotType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST && !HasUnit());
		NKCUtil.SetGameobjectActive(m_ImgTargetUnit, bValue);
		NKCUtil.SetGameobjectActive(m_objTouchToEntry, mode == eUnitSlotMode.Empty);
		NKCUtil.SetGameobjectActive(m_csbtnDetail, HasUnit());
	}

	protected override void OnClick()
	{
		if (dOnSelectEventDeckSlot != null)
		{
			dOnSelectEventDeckSlot(m_index);
		}
	}

	private void OnBtnDetail()
	{
		dOnUnitDetail?.Invoke(m_NKMUnitData);
	}

	private bool HasUnit()
	{
		if (m_NKMUnitData != null)
		{
			return m_NKMUnitData.m_UnitUID > 0;
		}
		return false;
	}

	public bool ConfirmLeader(int leaderIndex)
	{
		if (!CanBecomeLeader())
		{
			NKCUtil.SetGameobjectActive(m_objLeaderSelectFx, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLeaderMark, bValue: false);
			return false;
		}
		bool flag = m_index == leaderIndex;
		NKCUtil.SetGameobjectActive(m_objLeaderSelectFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLeaderMark, flag);
		if (m_NKMUnitTempletBase != null)
		{
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(m_NKMUnitTempletBase.m_UnitID);
			if (unitStatTemplet != null)
			{
				if (m_bEnableShowBan && NKCBanManager.IsBanUnit(m_NKMUnitTempletBase.m_UnitID))
				{
					int respawnCost = unitStatTemplet.GetRespawnCost(bPVP: true, flag, NKCBanManager.GetBanData(), null);
					NKCUtil.SetLabelText(m_lbSummonCost, string.Format(NKCUtilString.GET_STRING_UNIT_BAN_COST, respawnCost.ToString()));
				}
				else if (m_bEnableShowUpUnit && NKCBanManager.IsUpUnit(m_NKMUnitTempletBase.m_UnitID))
				{
					int respawnCost2 = unitStatTemplet.GetRespawnCost(bPVP: true, flag, null, NKCBanManager.m_dicNKMUpData);
					NKCUtil.SetLabelText(m_lbSummonCost, string.Format(NKCUtilString.GET_STRING_UNIT_UP_COST, respawnCost2.ToString()));
				}
				else
				{
					int respawnCost3 = unitStatTemplet.GetRespawnCost(flag, null);
					if (flag)
					{
						NKCUtil.SetLabelText(m_lbSummonCost, $"<color=#FFCD07>{respawnCost3}</color>");
					}
					else
					{
						NKCUtil.SetLabelText(m_lbSummonCost, $"{respawnCost3}");
					}
				}
			}
		}
		return flag;
	}

	public bool CanBecomeLeader()
	{
		return m_eSlotType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED;
	}

	public void LeaderSelectState(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objLeaderSelectFx, m_eSlotType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED && value);
	}

	public void SetActiveUpBan(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objBan, value);
	}

	public void SetDragHandler(DragHandler onBeginDrag, DragHandler onDrag, DragHandler onEndDrag, DragHandler onDrop)
	{
		dOnBeginDrag = onBeginDrag;
		dOnDrag = onDrag;
		dOnEndDrag = onEndDrag;
		dOnDrop = onDrop;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		dOnBeginDrag?.Invoke(eventData, m_index);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		dOnEndDrag?.Invoke(eventData, m_index);
	}

	public void OnDrag(PointerEventData eventData)
	{
		dOnDrag?.Invoke(eventData, m_index);
	}

	public void OnDrop(PointerEventData eventData)
	{
		dOnDrop?.Invoke(eventData, m_index);
	}
}
