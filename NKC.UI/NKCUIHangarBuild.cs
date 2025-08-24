using System.Collections.Generic;
using System.Linq;
using NKC.UI.Collection;
using NKC.UI.NPC;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIHangarBuild : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_hangar";

	private const string UI_ASSET_NAME = "NKM_UI_HANGAR_BUILD";

	private static NKCUIHangarBuild m_Instance;

	[Header("숏컷")]
	public NKCUIComStateButton m_NKM_UI_HANGAR_BUILD_SHORTCUT_MENU_BUTTON_SHIPCOLLECTION;

	[Header("토글")]
	public NKCUIComToggle m_NKM_UI_HANGAR_BUILD_TOGGLE;

	[Header("리스트")]
	public LoopHorizontalScrollRect m_LoopHorizontalScrollRect;

	public RectTransform m_NKM_UI_HANGAR_BUILD_SLOT_LIST_Viewport;

	public RectTransform m_NKM_UI_HANGAR_BUILD_SLOT_LIST_Content;

	public RectTransform m_rtShipBuildSlotPool;

	private Stack<RectTransform> m_stkShipBuildSlotPool = new Stack<RectTransform>();

	[Header("프리팹")]
	public NKCUIHangarBuildSlot m_pfNKM_UI_HANGAR_BUILD_SLOT_LIST;

	[Header("NPC")]
	public GameObject m_AB_NPC_NA_HEE_RIN;

	public GameObject m_NPCTouchArea;

	private NKCUINPCHangarNaHeeRin m_UINPC_NaHeeRin;

	private List<NKCAssetInstanceData> m_lstHangarSlotInstance = new List<NKCAssetInstanceData>();

	private bool m_bHideHasShip;

	private Dictionary<int, NKMShipBuildTemplet> m_dicBuildTemplet = new Dictionary<int, NKMShipBuildTemplet>();

	private Dictionary<int, NKCUIHangarBuildSlot> m_dicBuildSlots = new Dictionary<int, NKCUIHangarBuildSlot>();

	[Header("이벤트 버튼 오픈태그")]
	public string m_eventTabOpenTag = "TAG_HANGAR_BUILD_TYPE_EVENT";

	private List<NKMShipBuildTemplet> m_lstShipBuild = new List<NKMShipBuildTemplet>();

	private int m_iNewBuildCnt;

	public const string ShipBuildListChecked = "SHIP_BUILD_SLOT_CHECK";

	private NKMShipBuildTemplet.ShipUITabType m_CurShipTabType;

	public GameObject m_objNoneText;

	public NKCUIComToggle m_ctglNormalTab;

	public NKCUIComToggle m_ctglEventTab;

	public static NKCUIHangarBuild Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIHangarBuild>("ab_ui_nkm_ui_hangar", "NKM_UI_HANGAR_BUILD", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIHangarBuild>();
				m_Instance.Init();
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

	public override string MenuName => NKCUtilString.GET_STRING_HANGAR_BUILD;

	public override string GuideTempletID => "ARTICLE_SHIP_MAKE";

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

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(this, bValue: false);
		if (m_UINPC_NaHeeRin == null)
		{
			m_UINPC_NaHeeRin = null;
		}
	}

	public override void OnCloseInstance()
	{
		ClearBuildSlot();
	}

	public override void OnBackButton()
	{
		NKCUtil.SetGameobjectActive(this, bValue: false);
		base.OnBackButton();
	}

	public void Init()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (m_NKM_UI_HANGAR_BUILD_TOGGLE != null)
		{
			m_NKM_UI_HANGAR_BUILD_TOGGLE.OnValueChanged.RemoveAllListeners();
			m_NKM_UI_HANGAR_BUILD_TOGGLE.OnValueChanged.AddListener(OnToggleChange);
		}
		if (m_UINPC_NaHeeRin == null)
		{
			m_UINPC_NaHeeRin = m_NPCTouchArea.GetComponent<NKCUINPCHangarNaHeeRin>();
			m_UINPC_NaHeeRin.Init();
		}
		else
		{
			m_UINPC_NaHeeRin.PlayAni(NPC_ACTION_TYPE.START);
		}
		if (m_LoopHorizontalScrollRect != null)
		{
			m_LoopHorizontalScrollRect.dOnGetObject += MakeShipBuildSlot;
			m_LoopHorizontalScrollRect.dOnProvideData += ProvideShipBuildSlot;
			m_LoopHorizontalScrollRect.dOnReturnObject += ReturnShipBuildSlot;
			m_LoopHorizontalScrollRect.PrepareCells();
			NKCUtil.SetScrollHotKey(m_LoopHorizontalScrollRect);
		}
		NKCUtil.SetBindFunction(m_NKM_UI_HANGAR_BUILD_SHORTCUT_MENU_BUTTON_SHIPCOLLECTION, delegate
		{
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_SHIP, "");
		});
		InitShipBuildData();
	}

	private RectTransform MakeShipBuildSlot(int index)
	{
		if (m_stkShipBuildSlotPool.Count > 0)
		{
			RectTransform rectTransform = m_stkShipBuildSlotPool.Pop();
			NKCUtil.SetGameobjectActive(rectTransform, bValue: true);
			return rectTransform;
		}
		NKCUIHangarBuildSlot shipBuildSlot = GetShipBuildSlot();
		if (shipBuildSlot != null)
		{
			shipBuildSlot.transform.localPosition = Vector3.zero;
			shipBuildSlot.transform.localScale = Vector3.one;
			return shipBuildSlot.GetComponent<RectTransform>();
		}
		return null;
	}

	private NKCUIHangarBuildSlot GetShipBuildSlot()
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nkm_ui_hangar", "NKM_UI_HANGAR_BUILD_SLOT_LIST");
		if (nKCAssetInstanceData.m_Instant != null)
		{
			NKCUIHangarBuildSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIHangarBuildSlot>();
			component.InitUI(OpenConfirmPopup, CloseConfirmPopup, OpenShipInfo);
			m_lstHangarSlotInstance.Add(nKCAssetInstanceData);
			return component;
		}
		Debug.LogError("missing prefab path ab_ui_nkm_ui_hangar, NKM_UI_HANGAR_BUILD_SLOT_LIST");
		return null;
	}

	private void ProvideShipBuildSlot(Transform tr, int idx)
	{
		NKCUIHangarBuildSlot component = tr.GetComponent<NKCUIHangarBuildSlot>();
		if (!(component == null))
		{
			component.SetData(m_lstShipBuild[idx], idx < m_iNewBuildCnt);
		}
	}

	private void ReturnShipBuildSlot(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		go.SetParent(m_rtShipBuildSlotPool);
		m_stkShipBuildSlotPool.Push(go.GetComponent<RectTransform>());
	}

	private void OnToggleChange(bool bChange)
	{
		m_bHideHasShip = bChange;
		UpdateUI(bSlotUpdate: true);
	}

	private void ClearBuildSlot()
	{
		foreach (NKCAssetInstanceData item in m_lstHangarSlotInstance)
		{
			if (item != null)
			{
				NKCAssetResourceManager.CloseInstance(item);
			}
		}
		m_lstHangarSlotInstance.Clear();
		foreach (KeyValuePair<int, NKCUIHangarBuildSlot> dicBuildSlot in m_dicBuildSlots)
		{
			if (dicBuildSlot.Value.gameObject != null)
			{
				Object.Destroy(dicBuildSlot.Value.gameObject);
			}
		}
		m_dicBuildSlots.Clear();
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(this, bValue: true);
		if (m_NKM_UI_HANGAR_BUILD_SLOT_LIST_Content != null)
		{
			m_NKM_UI_HANGAR_BUILD_SLOT_LIST_Content.anchoredPosition = Vector2.zero;
		}
		m_CurShipTabType = NKMShipBuildTemplet.ShipUITabType.SHIP_NORMAL;
		m_ctglNormalTab?.Select(bSelect: true, bForce: true);
		UpdateUI(bSlotUpdate: true);
		UIOpened();
		CheckTutorial();
	}

	private void InitShipBuildData()
	{
		m_dicBuildTemplet.Clear();
		foreach (NKMShipBuildTemplet value in NKMTempletContainer<NKMShipBuildTemplet>.Values)
		{
			if (value.ShipBuildUnlockType != NKMShipBuildTemplet.BuildUnlockType.BUT_UNABLE)
			{
				m_dicBuildTemplet.Add(value.ShipID, value);
			}
		}
		if (null != m_ctglNormalTab)
		{
			m_ctglNormalTab.OnValueChanged.RemoveAllListeners();
			m_ctglNormalTab.OnValueChanged.AddListener(OnClickNormalTab);
		}
		if (null != m_ctglEventTab)
		{
			m_ctglEventTab.OnValueChanged.RemoveAllListeners();
			m_ctglEventTab.OnValueChanged.AddListener(OnClickEventTab);
		}
		NKCUtil.SetGameobjectActive(m_ctglEventTab.gameObject, NKMOpenTagManager.IsOpened(m_eventTabOpenTag));
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (itemData != null)
		{
			UpdateUI(bSlotUpdate: true);
		}
	}

	private void SrotingBuildList()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		m_iNewBuildCnt = 0;
		List<NKMShipBuildTemplet> list = new List<NKMShipBuildTemplet>();
		List<NKMShipBuildTemplet> list2 = new List<NKMShipBuildTemplet>();
		List<NKMShipBuildTemplet> list3 = new List<NKMShipBuildTemplet>();
		List<NKMShipBuildTemplet> list4 = new List<NKMShipBuildTemplet>();
		List<NKMShipBuildTemplet> list5 = new List<NKMShipBuildTemplet>();
		List<NKMShipBuildTemplet> list6 = new List<NKMShipBuildTemplet>();
		foreach (KeyValuePair<int, NKMShipBuildTemplet> item in m_dicBuildTemplet)
		{
			if (m_CurShipTabType != item.Value.ShipType)
			{
				continue;
			}
			bool flag = false;
			foreach (KeyValuePair<long, NKMUnitData> item2 in NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyShip)
			{
				if (NKMShipManager.IsSameKindShip(item2.Value.m_UnitID, item.Key))
				{
					flag = true;
					break;
				}
			}
			if (NKMShipManager.CanUnlockShip(nKMUserData, item.Value))
			{
				if (!flag && IsFirstCheck(nKMUserData.m_UserUID, item.Key))
				{
					list.Add(item.Value);
					continue;
				}
				List<BuildMaterial> buildMaterialList = item.Value.BuildMaterialList;
				bool flag2 = true;
				for (int i = 0; i < buildMaterialList.Count; i++)
				{
					if (nKMUserData.m_InventoryData.GetCountMiscItem(buildMaterialList[i].m_ShipBuildMaterialID) < buildMaterialList[i].m_ShipBuildMaterialCount)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					if (IsHasShip(item.Key))
					{
						if (!m_bHideHasShip)
						{
							list3.Add(item.Value);
						}
					}
					else
					{
						list2.Add(item.Value);
					}
				}
				else if (IsHasShip(item.Key))
				{
					if (!m_bHideHasShip)
					{
						list5.Add(item.Value);
					}
				}
				else
				{
					list4.Add(item.Value);
				}
			}
			else
			{
				list6.Add(item.Value);
			}
		}
		m_lstShipBuild.Clear();
		SortingBuildListDetail(list);
		SortingBuildListDetail(list2);
		SortingBuildListDetail(list3);
		SortingBuildListDetail(list4);
		SortingBuildListDetail(list5);
		SortingBuildListDetail(list6);
		m_iNewBuildCnt = list.Count;
		if (m_dicBuildTemplet.Count > 0)
		{
			List<KeyValuePair<int, NKMShipBuildTemplet>> list7 = m_dicBuildTemplet.ToList().FindAll((KeyValuePair<int, NKMShipBuildTemplet> e) => e.Value.ShipType == m_CurShipTabType);
			NKCUtil.SetGameobjectActive(m_objNoneText, list7.Count <= 0);
		}
	}

	private void SortingBuildListDetail(List<NKMShipBuildTemplet> targetList, int type = 0)
	{
		switch (type)
		{
		case 0:
		{
			if (targetList.Count < 1)
			{
				break;
			}
			Dictionary<NKM_UNIT_GRADE, List<NKMShipBuildTemplet>> dictionary2 = new Dictionary<NKM_UNIT_GRADE, List<NKMShipBuildTemplet>>();
			foreach (NKMShipBuildTemplet target in targetList)
			{
				if (target == null)
				{
					continue;
				}
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(target.ShipID);
				if (unitTempletBase2 != null && unitTempletBase2.CollectionEnableByTag)
				{
					if (!dictionary2.ContainsKey(unitTempletBase2.m_NKM_UNIT_GRADE))
					{
						dictionary2.Add(unitTempletBase2.m_NKM_UNIT_GRADE, new List<NKMShipBuildTemplet> { target });
					}
					else
					{
						dictionary2[unitTempletBase2.m_NKM_UNIT_GRADE].Add(target);
					}
				}
			}
			List<NKM_UNIT_GRADE> list2 = dictionary2.Keys.ToList();
			list2.Sort((NKM_UNIT_GRADE x, NKM_UNIT_GRADE y) => y.CompareTo(x));
			{
				foreach (NKM_UNIT_GRADE item in list2)
				{
					if (dictionary2.ContainsKey(item))
					{
						if (dictionary2[item].Count > 1)
						{
							SortingBuildListDetail(dictionary2[item], 1);
						}
						else
						{
							AddShipBuildList(dictionary2[item]);
						}
					}
				}
				break;
			}
		}
		case 1:
		{
			if (targetList.Count < 1)
			{
				break;
			}
			Dictionary<NKM_UNIT_STYLE_TYPE, List<NKMShipBuildTemplet>> dictionary = new Dictionary<NKM_UNIT_STYLE_TYPE, List<NKMShipBuildTemplet>>();
			foreach (NKMShipBuildTemplet target2 in targetList)
			{
				if (target2 == null)
				{
					continue;
				}
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(target2.ShipID);
				if (unitTempletBase != null && unitTempletBase.CollectionEnableByTag)
				{
					if (!dictionary.ContainsKey(unitTempletBase.m_NKM_UNIT_STYLE_TYPE))
					{
						dictionary.Add(unitTempletBase.m_NKM_UNIT_STYLE_TYPE, new List<NKMShipBuildTemplet> { target2 });
					}
					else
					{
						dictionary[unitTempletBase.m_NKM_UNIT_STYLE_TYPE].Add(target2);
					}
				}
			}
			List<NKM_UNIT_STYLE_TYPE> list = dictionary.Keys.ToList();
			list.Sort((NKM_UNIT_STYLE_TYPE x, NKM_UNIT_STYLE_TYPE y) => x.CompareTo(y));
			{
				foreach (NKM_UNIT_STYLE_TYPE item2 in list)
				{
					if (!dictionary.ContainsKey(item2))
					{
						continue;
					}
					if (dictionary[item2].Count > 1)
					{
						dictionary[item2].Sort((NKMShipBuildTemplet x, NKMShipBuildTemplet y) => x.ShipID.CompareTo(y.ShipID));
					}
					AddShipBuildList(dictionary[item2]);
				}
				break;
			}
		}
		}
	}

	private void AddShipBuildList(List<NKMShipBuildTemplet> addList)
	{
		if (addList.Count > 0)
		{
			m_lstShipBuild.AddRange(addList);
		}
	}

	private bool IsHasShip(int shipID)
	{
		bool result = false;
		foreach (KeyValuePair<long, NKMUnitData> item in NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyShip)
		{
			if (NKMShipManager.IsSameKindShip(item.Value.m_UnitID, shipID))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private bool IsFirstCheck(long userUID, int shipID)
	{
		string key = string.Format("{0}_{1}_{2}", "SHIP_BUILD_SLOT_CHECK", userUID, shipID);
		if (!PlayerPrefs.HasKey(key))
		{
			PlayerPrefs.SetInt(key, 0);
			return true;
		}
		return false;
	}

	public void UpdateUI(bool bSlotUpdate = false)
	{
		SrotingBuildList();
		if (bSlotUpdate)
		{
			m_LoopHorizontalScrollRect.TotalCount = m_lstShipBuild.Count;
			m_LoopHorizontalScrollRect.velocity = Vector2.zero;
			m_LoopHorizontalScrollRect.SetIndexPosition(0);
		}
	}

	public void OpenShipInfo(int shipID)
	{
		int num = 0;
		bool flag = true;
		List<NKMUnitData> list = new List<NKMUnitData>();
		list.Clear();
		foreach (KeyValuePair<int, NKCUIHangarBuildSlot> dicBuildSlot in m_dicBuildSlots)
		{
			if (m_bHideHasShip)
			{
				bool flag2 = false;
				Dictionary<long, NKMUnitData> dicMyShip = NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyShip;
				if (dicMyShip != null)
				{
					foreach (KeyValuePair<long, NKMUnitData> item in dicMyShip)
					{
						if (item.Value.m_UnitID == dicBuildSlot.Key)
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						continue;
					}
				}
			}
			if (flag)
			{
				if (dicBuildSlot.Key == shipID)
				{
					flag = false;
				}
				else
				{
					num++;
				}
			}
			list.Add(new NKMUnitData(dicBuildSlot.Key, 0L, islock: false, isPermanentContract: false, isSeized: false, fromContract: false));
		}
		NKCUICollectionShipInfo.CheckInstanceAndOpen(NKCUtil.MakeDummyUnit(NKMShipManager.GetMaxLevelShipID(shipID), 100, 3), NKMDeckIndex.None);
	}

	public void OpenConfirmPopup()
	{
		NKCUtil.SetGameobjectActive(m_AB_NPC_NA_HEE_RIN, bValue: false);
	}

	public void CloseConfirmPopup()
	{
		NKCUtil.SetGameobjectActive(m_AB_NPC_NA_HEE_RIN, bValue: true);
	}

	private void OnClickNormalTab(bool bVal)
	{
		if (m_CurShipTabType != NKMShipBuildTemplet.ShipUITabType.SHIP_NORMAL)
		{
			m_CurShipTabType = NKMShipBuildTemplet.ShipUITabType.SHIP_NORMAL;
			UpdateUI(bSlotUpdate: true);
		}
	}

	private void OnClickEventTab(bool bVal)
	{
		if (m_CurShipTabType != NKMShipBuildTemplet.ShipUITabType.SHIP_EVENT)
		{
			m_CurShipTabType = NKMShipBuildTemplet.ShipUITabType.SHIP_EVENT;
			UpdateUI(bSlotUpdate: true);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			OnClickNormalTab(bVal: true);
		}
		else if (Input.GetKeyDown(KeyCode.W))
		{
			OnClickEventTab(bVal: true);
		}
	}

	public void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.HangerBuild);
	}

	public NKCUIHangarBuildSlot GetSlot(int shipID)
	{
		int num = m_lstShipBuild.FindIndex((NKMShipBuildTemplet v) => v.ShipID == shipID);
		if (num >= 0)
		{
			m_LoopHorizontalScrollRect.SetIndexPosition(num);
			NKCUIHangarBuildSlot[] componentsInChildren = m_LoopHorizontalScrollRect.content.GetComponentsInChildren<NKCUIHangarBuildSlot>();
			for (int num2 = 0; num2 < componentsInChildren.Length; num2++)
			{
				if (componentsInChildren[num2].ShipID == shipID)
				{
					return componentsInChildren[num2];
				}
			}
		}
		return null;
	}
}
