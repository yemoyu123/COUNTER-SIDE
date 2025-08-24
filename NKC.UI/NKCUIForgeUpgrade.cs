using System.Collections;
using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForgeUpgrade : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_FACTORY";

	private const string UI_ASSET_NAME = "NKM_UI_FACTORY_UPGRADE";

	private static NKCUIForgeUpgrade m_Instance;

	private const string DEFAULT_TEXT_COLOR = "#582817";

	private const string LOCKED_TEXT_COLOR = "#222222";

	[Header("좌측 UI")]
	public NKCUISlotEquip m_pfbEquipSlot;

	public LoopScrollRect m_loop;

	public RectTransform m_NKM_UI_INVENTORY_LIST_ITEM_EQUIP;

	public NKCUIComSafeArea m_safeAreaEquip;

	public GridLayoutGroup m_GridLayoutGroupEquip;

	public RectTransform m_trSlotParent;

	public Transform m_trObjectPool;

	public GameObject m_objEmptyList;

	public bool m_bSetAutoResize;

	public NKCUIComEquipSortOptions m_SortUI;

	[Header("오른쪽 UI")]
	public GameObject m_objUpgradeInfo;

	public GameObject m_objEmpty;

	public Animator m_ani;

	public Text m_lbEquipName;

	public GameObject m_objEquipNeedSelect;

	public GameObject m_objEquipNotHave;

	public NKCUISlotEquip m_slotSourceEquip;

	public NKCUISlotEquip m_slotTargetEquip;

	public List<NKCUIForgeUpgradeStatSlot> m_lstStatSlot = new List<NKCUIForgeUpgradeStatSlot>();

	public List<NKCUIItemCostSlot> m_lstCostSlot = new List<NKCUIItemCostSlot>();

	public NKCUIComStateButton m_btnUpgrade;

	public Image m_imgUpgrade;

	public Text m_lbUpgrade;

	[Header("터치")]
	public GameObject m_objNoTouchPanel;

	[Header("슬롯 정보")]
	public Vector2 m_vEquipSlotSize;

	public Vector2 m_vEquipSlotSpacing;

	public float m_vEquipOffsetX;

	private Stack<NKCUISlotEquip> m_stkSlot = new Stack<NKCUISlotEquip>();

	private NKCEquipSortSystem m_ssActive;

	private HashSet<NKCEquipSortSystem.eFilterOption> m_LatestFilterOptionSet = new HashSet<NKCEquipSortSystem.eFilterOption>();

	private NKCUISlotEquip m_LatestSelectedSlot;

	private NKMItemEquipUpgradeTemplet m_LatestSelectedTemplet;

	private NKMEquipItemData m_sourceEquipItemData;

	private NKMEquipItemData m_UpgradedEquipData;

	private bool m_bWaitingPacket;

	private readonly HashSet<NKCEquipSortSystem.eFilterCategory> m_setEquipFilterCategory = new HashSet<NKCEquipSortSystem.eFilterCategory>
	{
		NKCEquipSortSystem.eFilterCategory.UnitType,
		NKCEquipSortSystem.eFilterCategory.EquipType,
		NKCEquipSortSystem.eFilterCategory.PrivateEquip,
		NKCEquipSortSystem.eFilterCategory.Tier
	};

	private bool bIsPlayingAni;

	private float m_fPlayTime;

	public static NKCUIForgeUpgrade Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIForgeUpgrade>("AB_UI_NKM_UI_FACTORY", "NKM_UI_FACTORY_UPGRADE", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIForgeUpgrade>();
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

	public static bool IsInstanceLoaded => m_Instance != null;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override string MenuName => NKCUtilString.GET_STRING_FACTORY_UPGRADE_TITLE;

	public override string GuideTempletID => "ARTICLE_EQUIP_UPGRADE";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public void InitUI()
	{
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.dOnRepopulate += CalculateContentRectSize;
		NKCUtil.SetScrollHotKey(m_loop);
		if (m_bSetAutoResize)
		{
			m_loop.SetAutoResize(2);
		}
		m_loop.PrepareCells();
		m_btnUpgrade.PointerClick.RemoveAllListeners();
		m_btnUpgrade.PointerClick.AddListener(OnClickUpgrade);
		m_btnUpgrade.m_bGetCallbackWhileLocked = true;
		for (int i = 0; i < m_lstStatSlot.Count; i++)
		{
			if (m_lstStatSlot[i] != null)
			{
				m_lstStatSlot[i].InitUI();
			}
		}
		if (m_SortUI != null)
		{
			m_SortUI.Init(OnSorted);
		}
	}

	private void CalculateContentRectSize()
	{
		int minColumn = 5;
		Vector2 vEquipSlotSize = m_vEquipSlotSize;
		Vector2 vEquipSlotSpacing = m_vEquipSlotSpacing;
		float vEquipOffsetX = m_vEquipOffsetX;
		m_NKM_UI_INVENTORY_LIST_ITEM_EQUIP.offsetMin = new Vector2(vEquipOffsetX, m_NKM_UI_INVENTORY_LIST_ITEM_EQUIP.offsetMin.y);
		m_NKM_UI_INVENTORY_LIST_ITEM_EQUIP.offsetMax = new Vector2(0f - vEquipOffsetX, m_NKM_UI_INVENTORY_LIST_ITEM_EQUIP.offsetMax.y);
		if (m_safeAreaEquip != null)
		{
			m_safeAreaEquip.SetSafeAreaBase();
		}
		NKCUtil.CalculateContentRectSize(m_loop, m_GridLayoutGroupEquip, minColumn, vEquipSlotSize, vEquipSlotSpacing);
		Debug.Log($"CellSize : {m_GridLayoutGroupEquip.cellSize}, rectContentWidth : {m_trSlotParent.GetWidth()}");
	}

	public override void OnBackButton()
	{
		if (!m_bWaitingPacket && !bIsPlayingAni)
		{
			base.OnBackButton();
		}
	}

	public override void UnHide()
	{
		base.UnHide();
		RefreshUI();
		NKCUtil.SetGameobjectActive(m_objNoTouchPanel, bValue: false);
	}

	public override void CloseInternal()
	{
		bIsPlayingAni = false;
		m_LatestSelectedSlot = null;
		m_LatestSelectedTemplet = null;
		m_sourceEquipItemData = null;
		m_UpgradedEquipData = null;
		NKCUtil.SetGameobjectActive(m_objNoTouchPanel, bValue: false);
		StopAllCoroutines();
		if (m_SortUI != null)
		{
			m_SortUI.ResetUI();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public RectTransform GetObject(int idx)
	{
		NKCUISlotEquip nKCUISlotEquip = null;
		if (m_stkSlot.Count > 0)
		{
			nKCUISlotEquip = m_stkSlot.Pop();
			nKCUISlotEquip.transform.SetParent(m_trSlotParent);
		}
		else
		{
			nKCUISlotEquip = Object.Instantiate(m_pfbEquipSlot, m_trSlotParent);
		}
		NKCUtil.SetGameobjectActive(nKCUISlotEquip, bValue: false);
		return nKCUISlotEquip.GetComponent<RectTransform>();
	}

	public void ReturnObject(Transform tr)
	{
		NKCUISlotEquip component = tr.GetComponent<NKCUISlotEquip>();
		component.transform.SetParent(m_trObjectPool);
		NKCUtil.SetGameobjectActive(component, bValue: false);
		m_stkSlot.Push(component);
	}

	public void ProvideData(Transform tr, int idx)
	{
		if (m_ssActive != null && idx < m_ssActive.SortedEquipList.Count)
		{
			NKCUISlotEquip component = tr.GetComponent<NKCUISlotEquip>();
			NKCUtil.SetGameobjectActive(component, bValue: true);
			bool flag = m_LatestSelectedTemplet != null && m_LatestSelectedTemplet.UpgradeEquipTemplet.m_ItemEquipID == m_ssActive.SortedEquipList[idx].m_ItemEquipID;
			component.SetSelected(flag);
			component.SetData(m_ssActive.SortedEquipList[idx], OnClickEquipSlot);
			if (flag)
			{
				OnClickUpgradeSlot(component, NKMItemManager.GetEquipUpgradableState(component.GetNKMEquipItemData()));
			}
		}
	}

	public void Open()
	{
		bIsPlayingAni = false;
		m_LatestFilterOptionSet = new HashSet<NKCEquipSortSystem.eFilterOption>();
		m_LatestSelectedSlot = null;
		m_LatestSelectedTemplet = null;
		m_sourceEquipItemData = null;
		m_bWaitingPacket = false;
		NKCUtil.SetGameobjectActive(m_objNoTouchPanel, bValue: false);
		SetUpgardeTempletData();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loop.TotalCount = m_ssActive.SortedEquipList.Count;
		m_loop.SetIndexPosition(0);
		NKCUtil.SetGameobjectActive(m_objEmptyList, m_ssActive.SortedEquipList.Count == 0);
		SetRightSide(null, NKC_EQUIP_UPGRADE_STATE.NOT_HAVE);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UIOpened();
		TutorialCheck();
	}

	private void RefreshUI(bool bResetScroll = true)
	{
		m_LatestSelectedSlot = null;
		m_sourceEquipItemData = null;
		SetUpgardeTempletData();
		SetRightSide(null, NKC_EQUIP_UPGRADE_STATE.NOT_HAVE);
		m_loop.TotalCount = m_ssActive.SortedEquipList.Count;
		int indexPosition = 0;
		if (m_LatestSelectedTemplet != null)
		{
			indexPosition = m_ssActive.SortedEquipList.FindIndex((NKMEquipItemData x) => x.m_ItemEquipID == m_LatestSelectedTemplet.UpgradeEquipTemplet.m_ItemEquipID);
		}
		m_loop.SetIndexPosition(indexPosition);
		NKCUtil.SetGameobjectActive(m_objEmptyList, m_ssActive.SortedEquipList.Count == 0);
	}

	private void OnSorted(bool bResetScroll = true)
	{
		m_LatestSelectedSlot = null;
		m_sourceEquipItemData = null;
		SetRightSide(null, NKC_EQUIP_UPGRADE_STATE.NOT_HAVE);
		m_loop.TotalCount = m_ssActive.SortedEquipList.Count;
		int indexPosition = 0;
		if (m_LatestSelectedTemplet != null)
		{
			indexPosition = m_ssActive.SortedEquipList.FindIndex((NKMEquipItemData x) => x.m_ItemEquipID == m_LatestSelectedTemplet.UpgradeEquipTemplet.m_ItemEquipID);
		}
		m_loop.SetIndexPosition(indexPosition);
		NKCUtil.SetGameobjectActive(m_objEmptyList, m_ssActive.SortedEquipList.Count == 0);
	}

	private void SetUpgardeTempletData()
	{
		List<NKMEquipItemData> list = new List<NKMEquipItemData>();
		foreach (KeyValuePair<long, NKMEquipItemData> equipItem in NKCScenManager.CurrentUserData().m_InventoryData.EquipItems)
		{
			if (NKMItemManager.CanUpgradeEquipByCoreID(equipItem.Value) == NKC_EQUIP_UPGRADE_STATE.UPGRADABLE)
			{
				list.Add(equipItem.Value);
			}
		}
		m_ssActive = new NKCEquipSortSystem(options: new NKCEquipSortSystem.EquipListOptions
		{
			setOnlyIncludeEquipID = new HashSet<int>(),
			lstSortOption = NKCEquipSortSystem.EQUIP_UPGRADE_SORT_LIST
		}, userData: NKCScenManager.CurrentUserData(), lstEquipData: list);
		if (m_LatestFilterOptionSet.Count > 0)
		{
			m_ssActive.FilterSet = m_LatestFilterOptionSet;
		}
		else
		{
			m_ssActive.FilterSet = new HashSet<NKCEquipSortSystem.eFilterOption>();
		}
		m_SortUI.RegisterCategories(m_setEquipFilterCategory, new HashSet<NKCEquipSortSystem.eSortCategory>());
		m_SortUI.RegisterEquipSort(m_ssActive);
		m_SortUI.ResetUI();
	}

	public void OnClickEquipSlot(NKCUISlotEquip slot, NKMEquipItemData data)
	{
		OnClickUpgradeSlot(slot, NKMItemManager.CanUpgradeEquipByCoreID(data));
	}

	public void OnClickUpgradeSlot(NKCUISlotEquip cSlot, NKC_EQUIP_UPGRADE_STATE state)
	{
		if (m_LatestSelectedSlot != null)
		{
			m_LatestSelectedSlot.SetSelected(bSelected: false);
		}
		m_LatestSelectedSlot = cSlot;
		m_LatestSelectedSlot.SetSelected(bSelected: true);
		m_LatestSelectedTemplet = GetEquipUpgradeTemplet(cSlot.GetNKMEquipItemData());
		m_sourceEquipItemData = cSlot.GetNKMEquipItemData();
		SetRightSide(m_LatestSelectedTemplet, NKMItemManager.GetEquipUpgradableState(m_LatestSelectedSlot.GetNKMEquipItemData()));
	}

	private void SetRightSide(NKMItemEquipUpgradeTemplet upgradeTemplet, NKC_EQUIP_UPGRADE_STATE state)
	{
		NKCUtil.SetGameobjectActive(m_objEmpty, upgradeTemplet == null);
		NKCUtil.SetGameobjectActive(m_objUpgradeInfo, upgradeTemplet != null);
		if (upgradeTemplet == null)
		{
			return;
		}
		m_ani?.Play("NKM_UI_FACTORY_UPGRADE_BASE");
		NKCUtil.SetLabelText(m_lbEquipName, m_LatestSelectedTemplet.UpgradeEquipTemplet.GetItemName());
		NKCUtil.SetGameobjectActive(m_objEquipNeedSelect, m_sourceEquipItemData == null && state != NKC_EQUIP_UPGRADE_STATE.NOT_HAVE);
		NKCUtil.SetGameobjectActive(m_objEquipNotHave, state == NKC_EQUIP_UPGRADE_STATE.NOT_HAVE);
		NKMEquipItemData nKMEquipItemData = NKCEquipSortSystem.MakeTempEquipData(m_LatestSelectedTemplet.CoreEquipTemplet.m_ItemEquipID, 0, bMaxValue: true);
		if (m_sourceEquipItemData != null)
		{
			nKMEquipItemData = m_sourceEquipItemData;
		}
		if (state == NKC_EQUIP_UPGRADE_STATE.NOT_HAVE)
		{
			m_slotSourceEquip.SetData(nKMEquipItemData);
		}
		else if (m_sourceEquipItemData == null)
		{
			m_slotSourceEquip.SetEmptyMaterial();
		}
		else
		{
			m_slotSourceEquip.SetData(m_sourceEquipItemData);
		}
		NKMEquipItemData nKMEquipItemData2 = NKCEquipSortSystem.MakeTempEquipData(m_LatestSelectedTemplet.UpgradeEquipTemplet.m_ItemEquipID);
		nKMEquipItemData2.m_Precision = 100;
		nKMEquipItemData2.m_Precision2 = 100;
		nKMEquipItemData2.m_SetOptionId = nKMEquipItemData.m_SetOptionId;
		nKMEquipItemData2.m_EnchantExp = NKMItemManager.GetMaxEquipEnchantExp(nKMEquipItemData.m_ItemEquipID);
		nKMEquipItemData2.m_EnchantLevel = GetEnchantLevel(nKMEquipItemData2.m_ItemEquipID, NKMItemManager.GetMaxEquipEnchantExp(nKMEquipItemData.m_ItemEquipID));
		nKMEquipItemData2.m_Stat[0].stat_value = nKMEquipItemData2.m_Stat[0].stat_value + (float)nKMEquipItemData2.m_EnchantLevel * nKMEquipItemData2.m_Stat[0].stat_level_value;
		m_slotTargetEquip.SetData(nKMEquipItemData2);
		for (int i = 0; i < m_lstStatSlot.Count; i++)
		{
			m_lstStatSlot[i].SetData(i, nKMEquipItemData, nKMEquipItemData2);
		}
		bool flag = true;
		for (int j = 0; j < m_lstCostSlot.Count; j++)
		{
			if (j < upgradeTemplet.MiscMaterials.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstCostSlot[j], bValue: true);
				int count = upgradeTemplet.MiscMaterials[j].Count32;
				long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(upgradeTemplet.MiscMaterials[j].ItemId);
				m_lstCostSlot[j].SetData(upgradeTemplet.MiscMaterials[j].ItemId, count, countMiscItem);
				if (count > countMiscItem)
				{
					flag = false;
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstCostSlot[j], bValue: false);
			}
		}
		if (flag && NKMItemManager.CanUpgradeEquipByCoreID(m_sourceEquipItemData) == NKC_EQUIP_UPGRADE_STATE.UPGRADABLE)
		{
			m_btnUpgrade.UnLock();
			NKCUtil.SetLabelTextColor(m_lbUpgrade, NKCUtil.GetColor("#582817"));
			NKCUtil.SetImageColor(m_imgUpgrade, NKCUtil.GetColor("#582817"));
		}
		else
		{
			m_btnUpgrade.Lock();
			NKCUtil.SetLabelTextColor(m_lbUpgrade, NKCUtil.GetColor("#222222"));
			NKCUtil.SetImageColor(m_imgUpgrade, NKCUtil.GetColor("#222222"));
		}
	}

	private int GetEnchantLevel(int equipID, int enchantExp)
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipID);
		if (equipTemplet == null)
		{
			return 0;
		}
		int num = 0;
		int enchantRequireExp = NKMItemManager.GetEnchantRequireExp(equipTemplet.m_NKM_ITEM_TIER, 0, equipTemplet.m_NKM_ITEM_GRADE);
		int num2 = enchantExp;
		while (enchantRequireExp <= num2 && num < NKMItemManager.GetMaxEquipEnchantLevel(equipTemplet.m_NKM_ITEM_TIER))
		{
			num++;
			num2 -= enchantRequireExp;
			enchantRequireExp = NKMItemManager.GetEnchantRequireExp(equipTemplet.m_NKM_ITEM_TIER, num, equipTemplet.m_NKM_ITEM_GRADE);
		}
		return num;
	}

	public void OnClickUpgrade()
	{
		if (!m_btnUpgrade.m_bLock)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FACTORY_UPGRADE_CONFIRM_POPUP, ProcessUpgrade);
		}
	}

	private void ProcessUpgrade()
	{
		NKCUtil.SetGameobjectActive(m_objNoTouchPanel, bValue: true);
		m_btnUpgrade.Lock();
		m_bWaitingPacket = true;
		NKCPacketSender.Send_NKMPacket_EQUIP_UPGRADE_REQ(m_sourceEquipItemData.m_ItemUid, new List<long>());
	}

	private NKMItemEquipUpgradeTemplet GetEquipUpgradeTemplet(NKMEquipItemData equipData)
	{
		return NKMTempletContainer<NKMItemEquipUpgradeTemplet>.Find((NKMItemEquipUpgradeTemplet x) => x.CoreEquipTemplet.m_ItemEquipID == equipData.m_ItemEquipID);
	}

	private void Update()
	{
		if (bIsPlayingAni)
		{
			m_fPlayTime += Time.deltaTime;
			if (NKCUtil.GetAnimationClip(m_ani, "NKM_UI_FACTORY_UPGRADE_START").length < m_fPlayTime)
			{
				m_fPlayTime = 0f;
				bIsPlayingAni = false;
				m_btnUpgrade.UnLock();
			}
		}
	}

	public void UpgradeFinished(NKMEquipItemData equipData)
	{
		m_bWaitingPacket = false;
		m_UpgradedEquipData = equipData;
		StartCoroutine(Process(equipData));
	}

	private IEnumerator Process(NKMEquipItemData equipData)
	{
		bIsPlayingAni = true;
		m_fPlayTime = 0f;
		m_ani?.Play("NKM_UI_FACTORY_UPGRADE_START");
		while (bIsPlayingAni)
		{
			yield return null;
		}
		ShowResultPopup();
	}

	private void ShowResultPopup()
	{
		StopAllCoroutines();
		RefreshUI();
		NKCPopupItemEquipBox.OpenForConfirm(m_UpgradedEquipData, null, bFierceInfo: false, bShowCancel: false, delegate
		{
			NKCUtil.SetGameobjectActive(m_objNoTouchPanel, bValue: false);
		});
		NKCPopupItemEquipBox.ShowTitle(NKCUtilString.GET_STRING_FACTORY_UPGRADE_COMPLETE);
		NKCPopupItemEquipBox.ShowUpgradeCompleteEffect();
		m_UpgradedEquipData = null;
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (base.gameObject.activeSelf)
		{
			base.OnInventoryChange(itemData);
			if (!m_bWaitingPacket)
			{
				RefreshUI();
			}
		}
	}

	public override void OnEquipChange(NKMUserData.eChangeNotifyType eType, long equipUID, NKMEquipItemData equipItem)
	{
		if (base.gameObject.activeSelf)
		{
			base.OnEquipChange(eType, equipUID, equipItem);
			if (!m_bWaitingPacket)
			{
				RefreshUI();
			}
		}
	}

	private void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.FactoryUpgrade);
	}
}
