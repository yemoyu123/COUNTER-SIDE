using System;
using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIChangeLobbyFace : MonoBehaviour
{
	public delegate void OnSelectFace(int id);

	public NKCUIChangeLobbyFaceSlot m_pfbSlot;

	public LoopScrollRect m_srFace;

	public NKCUIComToggleGroup m_tglgrpSlot;

	public NKCUIComUnitLoyalty m_UnitLoyalty;

	public GameObject m_objLoyaltyDesc;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnOutsideTouch;

	private OnSelectFace dOnSelectFace;

	private List<NKMLobbyFaceTemplet> m_lstLobbyFace;

	private NKM_UNIT_TYPE m_eUnitType;

	private NKMUnitData m_currentUnitData;

	private NKMOperator m_currentOperator;

	private int m_currentFaceID;

	private bool m_bSlotInit;

	public bool IsOpen { get; private set; }

	public void Init(OnSelectFace onSelectFace)
	{
		if (m_srFace != null)
		{
			m_srFace.dOnGetObject += GetObject;
			m_srFace.dOnReturnObject += ReturnObject;
			m_srFace.dOnProvideData += ProvideData;
			m_srFace.SetAutoResize(2);
		}
		dOnSelectFace = onSelectFace;
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnOutsideTouch, Close);
	}

	public void Open(NKMUnitData unitData, int currentFaceID, NKCASUIUnitIllust targetIllust)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_currentUnitData = unitData;
		m_currentOperator = null;
		m_currentFaceID = currentFaceID;
		if (unitData != null)
		{
			_ = unitData.loyalty / 100;
		}
		bool flag = NKMUnitManager.GetUnitTempletBase(unitData)?.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_TRAINER) ?? false;
		NKCUtil.SetGameobjectActive(m_UnitLoyalty, !flag);
		NKCUtil.SetGameobjectActive(m_objLoyaltyDesc, !flag);
		if (m_UnitLoyalty != null)
		{
			m_UnitLoyalty.SetLoyalty(unitData);
		}
		m_eUnitType = NKM_UNIT_TYPE.NUT_NORMAL;
		m_lstLobbyFace = MakeFaceList(targetIllust);
		if (!m_bSlotInit)
		{
			m_bSlotInit = true;
			m_srFace.TotalCount = m_lstLobbyFace.Count;
			m_srFace.PrepareCells();
			m_srFace.SetIndexPosition(0);
		}
		else
		{
			m_srFace.TotalCount = m_lstLobbyFace.Count;
			m_srFace.RefreshCells(bForce: true);
		}
		IsOpen = true;
	}

	public void Open(NKMOperator operatorData, int currentFaceID, NKCASUIUnitIllust targetIllust)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_currentUnitData = null;
		m_currentOperator = operatorData;
		m_currentFaceID = currentFaceID;
		NKCUtil.SetGameobjectActive(m_UnitLoyalty, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLoyaltyDesc, bValue: false);
		m_eUnitType = NKM_UNIT_TYPE.NUT_OPERATOR;
		m_lstLobbyFace = MakeFaceList(targetIllust);
		if (!m_bSlotInit)
		{
			m_bSlotInit = true;
			m_srFace.TotalCount = m_lstLobbyFace.Count;
			m_srFace.PrepareCells();
			m_srFace.SetIndexPosition(0);
		}
		else
		{
			m_srFace.TotalCount = m_lstLobbyFace.Count;
			m_srFace.RefreshCells(bForce: true);
		}
		IsOpen = true;
	}

	public void Close()
	{
		base.gameObject.SetActive(value: false);
		IsOpen = false;
	}

	private List<NKMLobbyFaceTemplet> MakeFaceList(NKCASUIUnitIllust illust)
	{
		List<NKMLobbyFaceTemplet> list = new List<NKMLobbyFaceTemplet>();
		if (illust == null)
		{
			return list;
		}
		foreach (NKMLobbyFaceTemplet value in NKMTempletContainer<NKMLobbyFaceTemplet>.Values)
		{
			if (Enum.TryParse<NKCASUIUnitIllust.eAnimation>(value.AnimationName, out var result) && illust.HasAnimation(result))
			{
				list.Add(value);
			}
		}
		return list;
	}

	private void OnSelectSlot(bool value, int data)
	{
		if (value)
		{
			m_currentFaceID = data;
			dOnSelectFace?.Invoke(data);
		}
	}

	private RectTransform GetObject(int index)
	{
		NKCUIChangeLobbyFaceSlot nKCUIChangeLobbyFaceSlot = UnityEngine.Object.Instantiate(m_pfbSlot);
		nKCUIChangeLobbyFaceSlot.Init(OnSelectSlot, m_tglgrpSlot);
		return nKCUIChangeLobbyFaceSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		tr.gameObject.SetActive(value: false);
		tr.SetParent(null);
		UnityEngine.Object.Destroy(tr.gameObject);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUIChangeLobbyFaceSlot component = tr.GetComponent<NKCUIChangeLobbyFaceSlot>();
		if (!(component == null))
		{
			switch (m_eUnitType)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
				component.SetData(m_currentUnitData, m_lstLobbyFace[idx]);
				break;
			case NKM_UNIT_TYPE.NUT_OPERATOR:
				component.SetData(m_currentOperator, m_lstLobbyFace[idx]);
				break;
			}
			component.SetSelected(m_lstLobbyFace[idx].Key == m_currentFaceID);
		}
	}
}
