using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUISelectionSkin : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_UNIT_SELECTION";

	private const string UI_ASSET_NAME = "NKM_UI_SKIN_SELECTION";

	private static NKCUISelectionSkin m_Instance;

	public NKCUIComSafeArea m_SafeArea;

	public LoopScrollRect m_loopScrollRect;

	public Transform m_trContentParent;

	public Image m_imgBannerMisc;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public NKCUISkinSelectionSlot m_pfbSlot;

	private List<int> m_lstRewardId = new List<int>();

	private List<NKCUISkinSelectionSlot> m_lstVisibleSlot = new List<NKCUISkinSelectionSlot>();

	private Stack<NKCUISkinSelectionSlot> m_stkSlotPool = new Stack<NKCUISkinSelectionSlot>();

	private NKMItemMiscTemplet m_NKMItemMiscTemplet;

	public static NKCUISelectionSkin Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUISelectionSkin>("AB_UI_NKM_UI_UNIT_SELECTION", "NKM_UI_SKIN_SELECTION", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUISelectionSkin>();
				m_Instance.InitUI();
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

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_USE_CHOICE;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		m_loopScrollRect.dOnGetObject += GetObject;
		m_loopScrollRect.dOnReturnObject += ReturnObject;
		m_loopScrollRect.dOnProvideData += ProvideData;
		m_loopScrollRect.dOnRepopulate += CalculateContentRectSize;
		NKCUtil.SetScrollHotKey(m_loopScrollRect);
	}

	public override void CloseInternal()
	{
		m_lstRewardId.Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnCloseInstance()
	{
		m_NKMItemMiscTemplet = null;
	}

	public void Open(NKMItemMiscTemplet itemMiscTemplet)
	{
		m_NKMItemMiscTemplet = itemMiscTemplet;
		m_lstRewardId.Clear();
		List<NKMRandomBoxItemTemplet> randomBoxItemTempletList = NKCRandomBoxManager.GetRandomBoxItemTempletList(m_NKMItemMiscTemplet.m_RewardGroupID);
		if (randomBoxItemTempletList != null)
		{
			for (int i = 0; i < randomBoxItemTempletList.Count; i++)
			{
				m_lstRewardId.Add(randomBoxItemTempletList[i].m_RewardID);
			}
			base.gameObject.SetActive(value: true);
			CalculateContentRectSize();
			if (!string.IsNullOrEmpty(itemMiscTemplet.m_BannerImage))
			{
				NKCUtil.SetImageSprite(m_imgBannerMisc, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_UNIT_SELECTION_TEXTURE", itemMiscTemplet.m_BannerImage));
			}
			m_loopScrollRect.PrepareCells();
			m_loopScrollRect.TotalCount = m_lstRewardId.Count;
			m_loopScrollRect.RefreshCells(bForce: true);
			UIOpened();
		}
	}

	private void CalculateContentRectSize()
	{
		m_SafeArea?.SetSafeAreaBase();
		GridLayoutGroup component = m_trContentParent.GetComponent<GridLayoutGroup>();
		int constraintCount = component.constraintCount;
		Vector2 cellSize = component.cellSize;
		Vector2 spacing = component.spacing;
		NKCUtil.CalculateContentRectSizeHorizontal(m_loopScrollRect, m_trContentParent.GetComponent<GridLayoutGroup>(), constraintCount, cellSize, spacing);
	}

	public void OnSelectSkinSlot(int skinId)
	{
		NKCPopupSelectionConfirm.Instance.Open(m_NKMItemMiscTemplet, skinId, 1L);
	}

	private RectTransform GetObject(int index)
	{
		NKCUISkinSelectionSlot nKCUISkinSelectionSlot = null;
		if (m_stkSlotPool.Count > 0)
		{
			nKCUISkinSelectionSlot = m_stkSlotPool.Pop();
		}
		else
		{
			nKCUISkinSelectionSlot = Object.Instantiate(m_pfbSlot);
			nKCUISkinSelectionSlot.Init();
		}
		NKCUtil.SetGameobjectActive(nKCUISkinSelectionSlot, bValue: true);
		m_lstVisibleSlot.Add(nKCUISkinSelectionSlot);
		return nKCUISkinSelectionSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform go)
	{
		NKCUISkinSelectionSlot component = go.GetComponent<NKCUISkinSelectionSlot>();
		NKCUtil.SetGameobjectActive(component, bValue: false);
		go.SetParent(base.transform);
		if (component != null)
		{
			m_lstVisibleSlot.Remove(component);
			m_stkSlotPool.Push(component);
		}
	}

	private void ProvideData(Transform tr, int idx)
	{
		if (idx < 0 || idx >= m_lstRewardId.Count)
		{
			Debug.LogError("out of index");
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		NKCUISkinSelectionSlot component = tr.GetComponent<NKCUISkinSelectionSlot>();
		int skinID = m_lstRewardId[idx];
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
		bool haveSkin = NKCScenManager.CurrentUserData()?.m_InventoryData.HasItemSkin(skinID) ?? false;
		component.SetData(skinTemplet, haveSkin, OnSelectSkinSlot);
		NKCUtil.SetGameobjectActive(component.gameObject, bValue: true);
	}
}
