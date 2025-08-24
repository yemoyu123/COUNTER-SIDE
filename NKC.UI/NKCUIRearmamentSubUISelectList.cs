using System.Collections.Generic;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIRearmamentSubUISelectList : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
	public delegate void OnSelectedUnitID(int unitID);

	[Header("슬롯 이동 속도")]
	public float m_fSlotMoveSpeed = 0.1f;

	public float m_fSlotDelaySpeed = 0.05f;

	[Header("선택된 재무장 유닛 이름&타입")]
	public Text m_lbRearmName;

	public RectTransform m_rtLayoutGroup;

	private OnSelectedUnitID dSelected;

	[Header("재무장 화살표")]
	public NKCUIComStateButton m_csbtnLeftArrow;

	public NKCUIComStateButton m_csbtnRightArrow;

	[Header("재무장 유닛 슬롯(0번이 메인 슬롯(Facecard_Focus))")]
	public List<NKCUIUnitSelectListSlot> m_lstRearmUnitSlots;

	private List<NKMUnitRearmamentTemplet> m_lstRearmTargetTemplets;

	public List<RectTransform> m_rtMoveTarget = new List<RectTransform>();

	private int m_iCurSelectedUnitID;

	private bool bMoving;

	public float DRAG_THRESHOLD = 100f;

	private bool m_bDrag;

	private float m_fDragOffset;

	public void Init(OnSelectedUnitID func)
	{
		foreach (NKCUIUnitSelectListSlot lstRearmUnitSlot in m_lstRearmUnitSlots)
		{
			lstRearmUnitSlot.Init(resetLocalScale: true);
		}
		NKCUtil.SetBindFunction(m_csbtnLeftArrow, OnClickRearmLeftArrow);
		NKCUtil.SetBindFunction(m_csbtnRightArrow, OnClickRearmRightArrow);
		dSelected = func;
		m_bDrag = false;
	}

	public void SetData(int baseUnitID, int iSelectedRearmUnitID = 0, long iSelectedRearmBaseUnitUID = 0L)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(baseUnitID);
		if (unitTempletBase == null || !NKCRearmamentUtil.IsCanRearmamentUnit(unitTempletBase))
		{
			return;
		}
		m_lstRearmTargetTemplets = NKCRearmamentUtil.GetRearmamentTargetTemplets(unitTempletBase);
		if (m_lstRearmTargetTemplets.Count <= 0)
		{
			return;
		}
		if (iSelectedRearmUnitID != 0)
		{
			foreach (NKMUnitRearmamentTemplet lstRearmTargetTemplet in m_lstRearmTargetTemplets)
			{
				if (lstRearmTargetTemplet.Key == iSelectedRearmUnitID)
				{
					UpdateRearmTargetSlotUI(iSelectedRearmUnitID, bInit: true, iSelectedRearmBaseUnitUID);
					return;
				}
			}
		}
		UpdateRearmTargetSlotUI(m_lstRearmTargetTemplets[0].Key, bInit: true, 0L);
	}

	public void UpdateReamUnitSlotData(long iSelectedRearmBaseUnitUID = 0L)
	{
		int tacticLevel = 0;
		if (iSelectedRearmBaseUnitUID != 0L)
		{
			NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(iSelectedRearmBaseUnitUID);
			if (unitFromUID != null)
			{
				tacticLevel = unitFromUID.tacticLevel;
			}
		}
		for (int i = 0; i < m_lstRearmUnitSlots.Count; i++)
		{
			if (m_lstRearmTargetTemplets.Count <= i)
			{
				NKCUtil.SetGameobjectActive(m_lstRearmUnitSlots[i], bValue: false);
				continue;
			}
			NKMUnitData nKMUnitData = new NKMUnitData();
			nKMUnitData.m_UnitID = m_lstRearmTargetTemplets[i].Key;
			nKMUnitData.m_SkinID = 0;
			nKMUnitData.m_UnitLevel = 1;
			nKMUnitData.tacticLevel = tacticLevel;
			m_lstRearmUnitSlots[i].SetData(nKMUnitData, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE), bEnableLayoutElement: false, OnSlotClicked);
			m_lstRearmUnitSlots[i].SetSlotDisable(m_lstRearmTargetTemplets[i].Key != m_iCurSelectedUnitID);
		}
	}

	private void InitRearmSlot(int targetRearmID, long iSelectedRearmBaseUnitUID = 0L)
	{
		bMoving = false;
		m_iCurSelectedUnitID = GetTargetID(targetRearmID);
		int tacticLevel = 0;
		if (iSelectedRearmBaseUnitUID != 0L)
		{
			NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(iSelectedRearmBaseUnitUID);
			if (unitFromUID != null)
			{
				tacticLevel = unitFromUID.tacticLevel;
			}
		}
		for (int i = 0; i < m_lstRearmUnitSlots.Count; i++)
		{
			if (m_lstRearmTargetTemplets.Count <= i)
			{
				NKCUtil.SetGameobjectActive(m_lstRearmUnitSlots[i], bValue: false);
				continue;
			}
			NKMUnitData nKMUnitData = new NKMUnitData();
			nKMUnitData.m_UnitID = m_lstRearmTargetTemplets[i].Key;
			nKMUnitData.m_SkinID = 0;
			nKMUnitData.m_UnitLevel = 1;
			nKMUnitData.tacticLevel = tacticLevel;
			m_lstRearmUnitSlots[i].SetSlotDisable(m_lstRearmTargetTemplets[i].Key != m_iCurSelectedUnitID);
			m_lstRearmUnitSlots[i].SetDataForRearm(nKMUnitData, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE), bEnableLayoutElement: false, OnSlotClicked, bShowEqup: false, bShowLevel: false, i != 0);
			NKCUtil.SetGameobjectActive(m_lstRearmUnitSlots[i], bValue: true);
		}
		if (null != m_rtLayoutGroup)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(m_rtLayoutGroup);
		}
	}

	private void UpdateRearmTargetSlotUI(int targetRearmID, bool bInit = false, long iSelectedRearmBaseUnitUID = 0L)
	{
		if (m_lstRearmTargetTemplets.Count > 0)
		{
			m_iCurSelectedUnitID = GetTargetID(targetRearmID);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_iCurSelectedUnitID);
			if (bInit)
			{
				InitRearmSlot(targetRearmID, iSelectedRearmBaseUnitUID);
			}
			NKCUtil.SetLabelText(m_lbRearmName, unitTempletBase.GetUnitTitle());
			MoveSlot();
			dSelected?.Invoke(m_iCurSelectedUnitID);
			Debug.Log($"<color=red> Selected UnitID {m_iCurSelectedUnitID}</color>");
		}
	}

	private void MoveSlot()
	{
		if (bMoving)
		{
			return;
		}
		bMoving = true;
		int num = 2;
		for (int i = 0; i < m_lstRearmTargetTemplets.Count; i++)
		{
			if (m_lstRearmTargetTemplets[i].Key == m_iCurSelectedUnitID)
			{
				switch (i)
				{
				case 0:
					num = 2;
					break;
				case 1:
					num = 1;
					break;
				case 2:
					num = 0;
					break;
				}
			}
		}
		for (int j = 0; j < m_lstRearmTargetTemplets.Count; j++)
		{
			if (m_lstRearmTargetTemplets[j].Key == m_iCurSelectedUnitID)
			{
				if (m_lstRearmTargetTemplets.Count == 1)
				{
					m_lstRearmUnitSlots[j].transform.DOMove(m_rtMoveTarget[num + j].position, m_fSlotMoveSpeed).OnComplete(EndMove);
				}
				else
				{
					m_lstRearmUnitSlots[j].transform.DOMove(m_rtMoveTarget[num + j].position, m_fSlotMoveSpeed);
				}
			}
			else
			{
				m_lstRearmUnitSlots[j].transform.DOMove(m_rtMoveTarget[num + j].position, m_fSlotMoveSpeed).SetDelay(m_fSlotDelaySpeed).OnComplete(EndMove);
			}
			m_lstRearmUnitSlots[j].SetSlotDisable(m_lstRearmTargetTemplets[j].Key != m_iCurSelectedUnitID);
		}
	}

	private int GetTargetID(int targetRearmID)
	{
		int num = 0;
		foreach (NKMUnitRearmamentTemplet lstRearmTargetTemplet in m_lstRearmTargetTemplets)
		{
			if (lstRearmTargetTemplet.Key == targetRearmID)
			{
				num = targetRearmID;
				break;
			}
		}
		if (num == 0)
		{
			return m_lstRearmTargetTemplets[0].Key;
		}
		return num;
	}

	private void EndMove()
	{
		bMoving = false;
		int num = 2;
		for (int i = 0; i < m_lstRearmTargetTemplets.Count; i++)
		{
			if (m_lstRearmTargetTemplets[i].Key == m_iCurSelectedUnitID)
			{
				switch (i)
				{
				case 0:
					num = 2;
					break;
				case 1:
					num = 1;
					break;
				case 2:
					num = 0;
					break;
				}
			}
		}
		for (int j = 0; j < m_lstRearmTargetTemplets.Count; j++)
		{
			m_lstRearmUnitSlots[j].transform.SetParent(m_rtMoveTarget[j + num]);
			m_lstRearmUnitSlots[j].transform.localPosition = Vector3.zero;
		}
	}

	public void OnSlotClicked(NKMUnitData unitData, NKMUnitTempletBase unitTempletBase, NKMDeckIndex deckIndex, NKCUnitSortSystem.eUnitState slotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		if (m_iCurSelectedUnitID != unitData.m_UnitID)
		{
			UpdateRearmTargetSlotUI(unitData.m_UnitID, bInit: false, 0L);
		}
	}

	private void OnClickRearmLeftArrow()
	{
		if (m_lstRearmTargetTemplets.Count <= 1)
		{
			return;
		}
		for (int i = 0; i < m_lstRearmTargetTemplets.Count; i++)
		{
			if (i > 0 && m_lstRearmTargetTemplets[i].Key == m_iCurSelectedUnitID)
			{
				UpdateRearmTargetSlotUI(m_lstRearmTargetTemplets[i - 1].Key, bInit: false, 0L);
			}
		}
	}

	private void OnClickRearmRightArrow()
	{
		if (m_lstRearmTargetTemplets.Count <= 1)
		{
			return;
		}
		for (int i = 0; i < m_lstRearmTargetTemplets.Count; i++)
		{
			if (i < m_lstRearmTargetTemplets.Count - 1 && m_lstRearmTargetTemplets[i].Key == m_iCurSelectedUnitID)
			{
				UpdateRearmTargetSlotUI(m_lstRearmTargetTemplets[i + 1].Key, bInit: false, 0L);
			}
		}
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (eventData.scrollDelta.y > 0f)
		{
			OnClickRearmLeftArrow();
		}
		else if (eventData.scrollDelta.y < 0f)
		{
			OnClickRearmRightArrow();
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		m_bDrag = true;
		m_fDragOffset = 0f;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (m_bDrag)
		{
			m_fDragOffset += eventData.delta.x;
			if (m_fDragOffset > DRAG_THRESHOLD * 2f)
			{
				OnClickRearmLeftArrow();
				m_bDrag = false;
			}
			if (m_fDragOffset < (0f - DRAG_THRESHOLD) * 2f)
			{
				OnClickRearmRightArrow();
				m_bDrag = false;
			}
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (m_bDrag)
		{
			m_bDrag = false;
			if (m_fDragOffset > DRAG_THRESHOLD)
			{
				OnClickRearmLeftArrow();
			}
			if (m_fDragOffset < 0f - DRAG_THRESHOLD)
			{
				OnClickRearmRightArrow();
			}
		}
	}
}
