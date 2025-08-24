using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUIPopupCollectionSkinSelect : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_collection";

	private const string UI_ASSET_NAME = "AB_UI_COLLECTION_POPUP_SKIN_SELECT";

	private static NKCUIPopupCollectionSkinSelect m_Instance;

	public NKCUIPopupCollectionSkinSlot m_SkinSlotPrefab;

	public ScrollRect m_ScrollRect;

	public Transform m_Content;

	public NKCUIComStateButton m_OKButton;

	public NKCUIComStateButton m_CancelButton;

	private List<NKCUIPopupCollectionSkinSlot> m_skinSlotList = new List<NKCUIPopupCollectionSkinSlot>();

	private NKMUnitData m_unitData;

	private int m_selectedSkinId;

	public static NKCUIPopupCollectionSkinSelect Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupCollectionSkinSelect>("ab_ui_collection", "AB_UI_COLLECTION_POPUP_SKIN_SELECT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupCollectionSkinSelect>();
				m_Instance?.Init();
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

	public override string MenuName => "Skin Select for Permanent Contract Replay";

	private static void CleanupInstance()
	{
		m_Instance?.Clear();
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void Init()
	{
		m_skinSlotList.Clear();
		if (m_Content != null)
		{
			int childCount = m_Content.childCount;
			for (int i = 0; i < childCount; i++)
			{
				NKCUIPopupCollectionSkinSlot component = m_Content.GetChild(i).GetComponent<NKCUIPopupCollectionSkinSlot>();
				if (!(component == null))
				{
					component.Init(OnClickSkinSlot);
					m_skinSlotList.Add(component);
				}
			}
		}
		NKCUtil.SetButtonClickDelegate(m_OKButton, OnOK);
		NKCUtil.SetButtonClickDelegate(m_CancelButton, OnCancel);
	}

	public override void CloseInternal()
	{
		m_unitData = null;
		base.gameObject.SetActive(value: false);
	}

	public void Open(NKMUnitData unitData)
	{
		m_unitData = unitData;
		m_selectedSkinId = 0;
		List<NKMSkinTemplet> skinlistForCharacter = NKMSkinManager.GetSkinlistForCharacter(unitData.m_UnitID, NKCScenManager.CurrentUserData().m_InventoryData);
		int num = skinlistForCharacter.Count - m_skinSlotList.Count + 1;
		for (int i = 0; i < num; i++)
		{
			NKCUIPopupCollectionSkinSlot nKCUIPopupCollectionSkinSlot = Object.Instantiate(m_SkinSlotPrefab, m_Content);
			if (!(nKCUIPopupCollectionSkinSlot == null))
			{
				nKCUIPopupCollectionSkinSlot.Init(OnClickSkinSlot);
				m_skinSlotList.Add(nKCUIPopupCollectionSkinSlot);
			}
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		int count = m_skinSlotList.Count;
		for (int j = 0; j < count; j++)
		{
			NKCUIPopupCollectionSkinSlot nKCUIPopupCollectionSkinSlot2 = m_skinSlotList[j];
			if (j == 0)
			{
				nKCUIPopupCollectionSkinSlot2.SetData(NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID), unitData.m_SkinID == 0);
				nKCUIPopupCollectionSkinSlot2.SetSelected(unitData.m_SkinID == 0);
				NKCUtil.SetGameobjectActive(nKCUIPopupCollectionSkinSlot2, bValue: true);
				continue;
			}
			if (j - 1 >= skinlistForCharacter.Count)
			{
				NKCUtil.SetGameobjectActive(nKCUIPopupCollectionSkinSlot2, bValue: false);
				continue;
			}
			NKMSkinTemplet nKMSkinTemplet = skinlistForCharacter[j - 1];
			bool flag = unitData.m_SkinID == nKMSkinTemplet.m_SkinID;
			nKCUIPopupCollectionSkinSlot2.SetData(nKMSkinTemplet, myUserData.m_InventoryData.HasItemSkin(nKMSkinTemplet.m_SkinID), flag);
			nKCUIPopupCollectionSkinSlot2.SetSelected(flag);
			if (flag)
			{
				m_selectedSkinId = unitData.m_SkinID;
			}
			NKCUtil.SetGameobjectActive(nKCUIPopupCollectionSkinSlot2, bValue: true);
		}
		UIOpened();
	}

	private void OnClickSkinSlot(int skinId)
	{
		m_selectedSkinId = skinId;
		m_skinSlotList.ForEach(delegate(NKCUIPopupCollectionSkinSlot e)
		{
			if (e.gameObject.activeSelf)
			{
				e.SetSelected(e.SkinID == skinId);
			}
		});
	}

	private void OnOK()
	{
		NKMUnitData nKMUnitData = m_unitData.DeepCopy();
		nKMUnitData.m_SkinID = m_selectedSkinId;
		NKCUILifetime.Instance.Open(nKMUnitData, replay: true);
	}

	private void OnCancel()
	{
		Close();
	}

	private void Clear()
	{
		if (m_Content != null)
		{
			int childCount = m_Content.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Object.Destroy(m_Content.GetChild(i));
			}
		}
		m_skinSlotList?.Clear();
		m_skinSlotList = null;
	}
}
