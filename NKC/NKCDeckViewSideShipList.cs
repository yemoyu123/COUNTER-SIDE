using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCDeckViewSideShipList : MonoBehaviour
{
	private bool m_bOpen;

	private bool m_ShipListEnable = true;

	public RectTransform m_rectRoot;

	private float m_NKM_DECK_VIEW_SIDE_SHIP_LIST_OrgX;

	private NKMTrackingFloat m_NKM_DECK_VIEW_SIDE_SHIP_LIST_PosX = new NKMTrackingFloat();

	public RectTransform m_rectListContent;

	private NKCDeckViewShipListSlot m_SelectShipListSlot;

	private List<NKCDeckViewShipListSlot> m_NKCDeckViewShipListSlot = new List<NKCDeckViewShipListSlot>();

	public void Init(NKCDeckViewShipListSlot.OnShipChange dOnShipChange)
	{
		m_rectRoot = GetComponent<RectTransform>();
		base.gameObject.SetActive(value: false);
		m_NKM_DECK_VIEW_SIDE_SHIP_LIST_OrgX = m_rectRoot.anchoredPosition.x;
		m_NKM_DECK_VIEW_SIDE_SHIP_LIST_PosX.SetNowValue(m_NKM_DECK_VIEW_SIDE_SHIP_LIST_OrgX + 900f);
		for (int i = 0; i < 50; i++)
		{
			NKCDeckViewShipListSlot cNKCDeckViewShipListSlot = NKCDeckViewShipListSlot.GetNewInstance(i, m_rectListContent, dOnShipChange);
			if (!(cNKCDeckViewShipListSlot == null))
			{
				m_NKCDeckViewShipListSlot.Add(cNKCDeckViewShipListSlot);
				cNKCDeckViewShipListSlot.GetNKCUIComButton().PointerClick.RemoveAllListeners();
				cNKCDeckViewShipListSlot.GetNKCUIComButton().PointerClick.AddListener(delegate
				{
					DeckViewShipListSlotClick(cNKCDeckViewShipListSlot);
				});
			}
		}
	}

	public void Open(NKMArmyData armyData, NKMDeckIndex SelectedIndex)
	{
		if (!m_bOpen)
		{
			m_bOpen = true;
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			SetShipListData(armyData, SelectedIndex.m_eDeckType, SelectedIndex, bAnimate: true);
			Enable(bEnable: true);
		}
	}

	public void Close()
	{
		if (m_bOpen)
		{
			m_bOpen = false;
			Enable(bEnable: false);
		}
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public void Update()
	{
		m_NKM_DECK_VIEW_SIDE_SHIP_LIST_PosX.Update(Time.deltaTime);
		Update_PosX();
		if (!m_NKM_DECK_VIEW_SIDE_SHIP_LIST_PosX.IsTracking() && !m_ShipListEnable && base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void Update_PosX()
	{
		if (m_NKM_DECK_VIEW_SIDE_SHIP_LIST_PosX.IsTracking())
		{
			Vector2 anchoredPosition = m_rectRoot.anchoredPosition;
			anchoredPosition.x = m_NKM_DECK_VIEW_SIDE_SHIP_LIST_PosX.GetNowValue();
			m_rectRoot.anchoredPosition = anchoredPosition;
		}
	}

	public void SetShipListData(NKMArmyData armyData, NKM_DECK_TYPE eCurrentDeckType, NKMDeckIndex SelectedIndex, bool bAnimate)
	{
		int num = 0;
		Dictionary<long, NKMUnitData>.Enumerator enumerator = armyData.m_dicMyShip.GetEnumerator();
		NKMDeckData deckData = armyData.GetDeckData(SelectedIndex);
		while (enumerator.MoveNext())
		{
			NKMUnitData value = enumerator.Current.Value;
			m_NKCDeckViewShipListSlot[num].SetData(value, eCurrentDeckType, armyData.GetShipDeckIndex(SelectedIndex.m_eDeckType, value.m_UnitUID), bAnimate);
			if (deckData != null && deckData.m_ShipUID == value.m_UnitUID)
			{
				DeckViewShipListSlotClick(m_NKCDeckViewShipListSlot[num]);
			}
			num++;
		}
		Vector2 sizeDelta = m_rectListContent.sizeDelta;
		sizeDelta.y = 100 + num * 230;
		m_rectListContent.sizeDelta = sizeDelta;
		if (num < m_NKCDeckViewShipListSlot.Count)
		{
			for (int i = num; i < m_NKCDeckViewShipListSlot.Count; i++)
			{
				m_NKCDeckViewShipListSlot[num].SetData(null, eCurrentDeckType, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE), bAnimate: false);
			}
		}
	}

	public void DeckViewShipListSlotClick(NKCDeckViewShipListSlot cNKCDeckViewShipListSlot)
	{
		if (m_SelectShipListSlot != null)
		{
			m_SelectShipListSlot.DeSelect();
		}
		m_SelectShipListSlot = cNKCDeckViewShipListSlot;
		m_SelectShipListSlot.Select();
	}

	public void Enable(bool bEnable)
	{
		if (m_ShipListEnable == bEnable)
		{
			return;
		}
		if (bEnable)
		{
			m_NKM_DECK_VIEW_SIDE_SHIP_LIST_PosX.SetTracking(m_NKM_DECK_VIEW_SIDE_SHIP_LIST_OrgX, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			for (int i = 0; i < m_NKCDeckViewShipListSlot.Count && m_NKCDeckViewShipListSlot[i].FadeInMove(); i++)
			{
			}
			Vector2 anchoredPosition = m_rectListContent.anchoredPosition;
			anchoredPosition.y = 0f;
			m_rectListContent.anchoredPosition = anchoredPosition;
		}
		else
		{
			m_NKM_DECK_VIEW_SIDE_SHIP_LIST_PosX.SetTracking(m_NKM_DECK_VIEW_SIDE_SHIP_LIST_OrgX + 900f, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		m_ShipListEnable = bEnable;
	}
}
