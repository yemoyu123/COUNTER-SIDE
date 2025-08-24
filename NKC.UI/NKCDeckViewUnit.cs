using System.Collections.Generic;
using ClientPacket.Common;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI;

public class NKCDeckViewUnit : NKCUIInstantiatable
{
	public delegate void OnClickUnit(int index);

	public delegate void OnDragUnitEnd(int oldIndex, int newIndex);

	public RectTransform m_rectAnchor;

	public List<NKCDeckViewUnitSlot> m_listNKCDeckViewUnitSlot;

	public int m_iSelectedSlotIndex;

	public float m_fSelectedScale = 1f;

	public float m_fDeselectedScale = 0.7f;

	private NKMTrackingFloat m_ScaleX = new NKMTrackingFloat();

	private NKMTrackingFloat m_ScaleY = new NKMTrackingFloat();

	private OnClickUnit dOnClickUnit;

	private OnDragUnitEnd dOnDragUnitEnd;

	private bool m_bTrackingStarted;

	public List<NKCDeckViewUnitSlot> DeckViewUnitSlotList => m_listNKCDeckViewUnitSlot;

	public static NKCDeckViewUnit OpenInstance(string bundleName, string assetName, Transform trParent, OnClickUnit onClick, OnDragUnitEnd onDragUnitEnd)
	{
		NKCDeckViewUnit nKCDeckViewUnit = NKCUIInstantiatable.OpenInstance<NKCDeckViewUnit>(bundleName, assetName, trParent);
		if ((object)nKCDeckViewUnit != null)
		{
			nKCDeckViewUnit.Init(onClick, onDragUnitEnd);
			return nKCDeckViewUnit;
		}
		return nKCDeckViewUnit;
	}

	public void CloseResource(string bundleName, string assetName)
	{
		CloseInstance(bundleName, assetName);
	}

	public void Init(OnClickUnit onClick, OnDragUnitEnd onDragUnitEnd)
	{
		dOnClickUnit = onClick;
		dOnDragUnitEnd = onDragUnitEnd;
		for (int i = 0; i < m_listNKCDeckViewUnitSlot.Count; i++)
		{
			NKCDeckViewUnitSlot cNKCDeckListButton = m_listNKCDeckViewUnitSlot[i];
			cNKCDeckListButton.Init(i);
			if (cNKCDeckListButton.m_NKCUIComButton != null)
			{
				cNKCDeckListButton.m_NKCUIComButton.PointerClick.AddListener(delegate
				{
					OnClick(cNKCDeckListButton.m_Index);
				});
			}
			if (cNKCDeckListButton.m_NKCUIComDrag != null)
			{
				cNKCDeckListButton.m_NKCUIComDrag.BeginDrag.AddListener(delegate(PointerEventData eventData)
				{
					DeckViewUnitBeginDrag(cNKCDeckListButton.m_Index, eventData);
				});
				cNKCDeckListButton.m_NKCUIComDrag.Drag.AddListener(delegate(PointerEventData eventData)
				{
					DeckViewUnitDrag(cNKCDeckListButton.m_Index, eventData);
				});
				cNKCDeckListButton.m_NKCUIComDrag.EndDrag.AddListener(delegate(PointerEventData eventData)
				{
					DeckViewUnitEndDrag(cNKCDeckListButton.m_Index, eventData);
				});
			}
		}
		m_ScaleX.SetNowValue(m_fSelectedScale);
		m_ScaleY.SetNowValue(m_fSelectedScale);
		SlotResetPos();
	}

	public void Open(NKMArmyData armyData, NKMDeckIndex deckIndex, NKCUIDeckViewer.DeckViewerOption deckViewerOption)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		for (int i = 0; i < m_listNKCDeckViewUnitSlot.Count; i++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_listNKCDeckViewUnitSlot[i];
			if (!(nKCDeckViewUnitSlot == null))
			{
				nKCDeckViewUnitSlot.SetEnableShowBan(NKCUtil.CheckPossibleShowBan(deckViewerOption.eDeckviewerMode));
				nKCDeckViewUnitSlot.SetEnableShowUpUnit(NKCUtil.CheckPossibleShowUpUnit(deckViewerOption.eDeckviewerMode));
			}
		}
		SetDeckListButton(armyData, deckIndex, deckViewerOption);
	}

	public void OpenDummy(NKMDummyUnitData[] dummyUnitList, int leaderIndex)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		for (int i = 0; i < m_listNKCDeckViewUnitSlot.Count; i++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_listNKCDeckViewUnitSlot[i];
			nKCDeckViewUnitSlot.SetEnableShowBan(bSet: false);
			NKMDummyUnitData nKMDummyUnitData = dummyUnitList[i];
			nKCDeckViewUnitSlot.EnableDrag(bEnalbe: false);
			if (nKMDummyUnitData == null)
			{
				nKCDeckViewUnitSlot.SetData(null, bEnableButton: false);
				continue;
			}
			NKMUnitData nKMUnitData = new NKMUnitData();
			nKMUnitData.FillDataFromDummy(nKMDummyUnitData);
			nKCDeckViewUnitSlot.SetData(nKMUnitData, bEnableButton: false);
			if (leaderIndex == i)
			{
				nKCDeckViewUnitSlot.SetLeader(bLeader: true, bEffect: false);
			}
			else
			{
				nKCDeckViewUnitSlot.SetLeader(bLeader: false, bEffect: false);
			}
		}
	}

	public void Close()
	{
		CancelAllDrag();
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void Update()
	{
		m_ScaleX.Update(Time.deltaTime);
		m_ScaleY.Update(Time.deltaTime);
		if (m_ScaleX.IsTracking())
		{
			SlotResetPos();
		}
		else if (m_bTrackingStarted)
		{
			m_bTrackingStarted = false;
			SlotResetPos(bImmediate: true);
		}
		Vector2 vector = m_rectAnchor.localScale;
		vector.Set(m_ScaleX.GetNowValue(), m_ScaleY.GetNowValue());
		m_rectAnchor.localScale = vector;
	}

	public void SetDeckListButton(NKMArmyData armyData, NKMDeckIndex deckIndex, NKCUIDeckViewer.DeckViewerOption deckViewerOption)
	{
		for (int i = 0; i < m_listNKCDeckViewUnitSlot.Count; i++)
		{
			SetUnitSlotData(armyData, deckIndex, i, bEffect: false, deckViewerOption);
		}
	}

	public void SetUnitSlotData(NKMArmyData armyData, NKMDeckIndex deckIndex, int unitSlotIndex, bool bEffect, NKCUIDeckViewer.DeckViewerOption deckViewerOption)
	{
		NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_listNKCDeckViewUnitSlot[unitSlotIndex];
		if (deckViewerOption.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.PrepareLocalDeck)
		{
			long localUnitData = NKCLocalDeckDataManager.GetLocalUnitData(deckIndex.m_iIndex, unitSlotIndex);
			NKMUnitData unitFromUID = armyData.GetUnitFromUID(localUnitData);
			nKCDeckViewUnitSlot.SetData(unitFromUID, deckViewerOption);
			nKCDeckViewUnitSlot.EnableDrag(bEnalbe: true);
			int localLeaderIndex = NKCLocalDeckDataManager.GetLocalLeaderIndex(deckIndex.m_iIndex);
			if (unitSlotIndex == localLeaderIndex)
			{
				nKCDeckViewUnitSlot.SetLeader(bLeader: true, bEffect: false);
			}
			else
			{
				nKCDeckViewUnitSlot.SetLeader(bLeader: false, bEffect: false);
			}
			if (bEffect)
			{
				nKCDeckViewUnitSlot.PlayEffect();
			}
			return;
		}
		NKMUnitData deckUnitByIndex = armyData.GetDeckUnitByIndex(deckIndex, unitSlotIndex);
		nKCDeckViewUnitSlot.SetData(deckUnitByIndex, deckViewerOption);
		nKCDeckViewUnitSlot.EnableDrag(bEnalbe: true);
		NKMDeckData deckData = armyData.GetDeckData(deckIndex);
		if (deckData != null && unitSlotIndex == deckData.m_LeaderIndex)
		{
			nKCDeckViewUnitSlot.SetLeader(bLeader: true, bEffect: false);
		}
		else
		{
			nKCDeckViewUnitSlot.SetLeader(bLeader: false, bEffect: false);
		}
		if (bEffect)
		{
			nKCDeckViewUnitSlot.PlayEffect();
		}
		if (deckViewerOption.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.TournamentApply)
		{
			nKCDeckViewUnitSlot.SetDataChanged(NKCTournamentManager.IsUnitChanged(unitSlotIndex, deckUnitByIndex));
		}
	}

	public void SetLeader(int leaderIndex, bool bEffect)
	{
		for (int i = 0; i < m_listNKCDeckViewUnitSlot.Count; i++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_listNKCDeckViewUnitSlot[i];
			if (i == leaderIndex)
			{
				nKCDeckViewUnitSlot.SetLeader(bLeader: true, bEffect);
			}
			else
			{
				nKCDeckViewUnitSlot.SetLeader(bLeader: false, bEffect);
			}
		}
	}

	public void OnClick(int index)
	{
		if (dOnClickUnit != null)
		{
			dOnClickUnit(index);
		}
		SelectDeckViewUnit(index);
	}

	public void SelectDeckViewUnit(int selectedIndex, bool bForce = false)
	{
		m_iSelectedSlotIndex = selectedIndex;
		for (int i = 0; i < m_listNKCDeckViewUnitSlot.Count; i++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_listNKCDeckViewUnitSlot[i];
			if (i != selectedIndex)
			{
				nKCDeckViewUnitSlot.ButtonDeSelect();
			}
			else
			{
				nKCDeckViewUnitSlot.ButtonSelect();
			}
		}
	}

	public void DeckViewUnitBeginDrag(int index, PointerEventData eventData)
	{
		CancelAllDrag();
		m_listNKCDeckViewUnitSlot[index].BeginDrag();
	}

	public void DeckViewUnitDrag(int index, PointerEventData eventData)
	{
		if (!m_listNKCDeckViewUnitSlot[index].GetInDrag())
		{
			return;
		}
		m_listNKCDeckViewUnitSlot[index].Drag(eventData);
		bool flag = false;
		for (int i = 0; i < m_listNKCDeckViewUnitSlot.Count; i++)
		{
			if (index != i)
			{
				NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_listNKCDeckViewUnitSlot[i];
				if (!flag && nKCDeckViewUnitSlot.IsEnter(m_listNKCDeckViewUnitSlot[index].m_rectMain.position))
				{
					flag = true;
					nKCDeckViewUnitSlot.Swap(m_listNKCDeckViewUnitSlot[index]);
				}
				else
				{
					nKCDeckViewUnitSlot.ReturnToOrg();
				}
			}
		}
	}

	public void DeckViewUnitEndDrag(int index, PointerEventData eventData)
	{
		if (m_listNKCDeckViewUnitSlot[index].GetInDrag())
		{
			bool flag = false;
			for (int i = 0; i < m_listNKCDeckViewUnitSlot.Count; i++)
			{
				if (index == i)
				{
					continue;
				}
				NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_listNKCDeckViewUnitSlot[i];
				if (!flag && nKCDeckViewUnitSlot.IsEnter(m_listNKCDeckViewUnitSlot[index].m_rectMain.position))
				{
					nKCDeckViewUnitSlot.Swap(m_listNKCDeckViewUnitSlot[index]);
					m_listNKCDeckViewUnitSlot[index].Swap(nKCDeckViewUnitSlot);
					m_listNKCDeckViewUnitSlot[index].ReturnToParent();
					if (dOnDragUnitEnd != null)
					{
						dOnDragUnitEnd(index, nKCDeckViewUnitSlot.m_Index);
					}
					flag = true;
				}
				else
				{
					nKCDeckViewUnitSlot.ReturnToOrg();
				}
			}
			if (flag)
			{
				return;
			}
		}
		m_listNKCDeckViewUnitSlot[index].EndDrag();
		for (int j = 0; j < m_listNKCDeckViewUnitSlot.Count; j++)
		{
			if (index != j)
			{
				m_listNKCDeckViewUnitSlot[j].ReturnToOrg();
			}
		}
	}

	public void CancelAllDrag()
	{
		for (int i = 0; i < m_listNKCDeckViewUnitSlot.Count; i++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_listNKCDeckViewUnitSlot[i];
			nKCDeckViewUnitSlot.EndDrag();
			nKCDeckViewUnitSlot.ButtonDeSelect(bForce: true, bImmediate: true);
		}
	}

	public void Enable()
	{
		m_ScaleX.SetTracking(m_fSelectedScale, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
		m_ScaleY.SetTracking(m_fSelectedScale, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
		m_bTrackingStarted = true;
	}

	public void Disable()
	{
		if (m_iSelectedSlotIndex >= 0)
		{
			m_listNKCDeckViewUnitSlot[m_iSelectedSlotIndex].ButtonDeSelect();
		}
		m_ScaleX.SetTracking(m_fDeselectedScale, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
		m_ScaleY.SetTracking(m_fDeselectedScale, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
		m_bTrackingStarted = true;
	}

	public void SlotResetPos(bool bImmediate = false)
	{
		for (int i = 0; i < m_listNKCDeckViewUnitSlot.Count; i++)
		{
			m_listNKCDeckViewUnitSlot[i].ResetPos(bImmediate);
		}
	}

	public void UpdateUnit(NKMUnitData unitData, NKCUIDeckViewer.DeckViewerOption deckViewerOption)
	{
		if (unitData == null)
		{
			return;
		}
		foreach (NKCDeckViewUnitSlot item in m_listNKCDeckViewUnitSlot)
		{
			if (item.m_NKMUnitData == null || item.m_NKMUnitData.m_UnitUID != unitData.m_UnitUID)
			{
				continue;
			}
			item.SetData(unitData, deckViewerOption);
			if (deckViewerOption.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.TournamentApply)
			{
				item.SetDataChanged(NKCTournamentManager.IsUnitChanged(NKCTournamentManager.m_TournamentApplyDeckData.units.Find((NKMAsyncUnitData x) => x != null && x.unitUid == unitData.m_UnitUID), unitData));
			}
		}
	}
}
