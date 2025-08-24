using System;
using System.Collections.Generic;
using System.Text;
using ClientPacket.Contract;
using NKC.Templet;
using NKC.UI.Component;
using NKC.UI.Tooltip;
using NKM;
using NKM.Contract2;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Contract;

public class NKCUIContractV3 : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONTRACT_V2";

	public const string UI_ASSET_NAME = "NKM_UI_CONTRACT_V2";

	[Header("좌측 탭")]
	public NKCUIComFoldableList m_flTabs;

	public NKCUIContractListSlot m_pfbTab;

	public NKCUIContractListSlot m_pfbSubTab;

	[Header("채용 배너 최상단")]
	public Transform m_trBannerParent;

	[Header("좌상단 이벤트")]
	public GameObject m_objEvent;

	public GameObject m_objEventRemainCount;

	public Text m_lbEventRemainCount;

	public GameObject m_objEventDate;

	public Text m_lbEventDate;

	public GameObject m_objToolTip;

	public NKCUIComStateButton m_csbtnToolTip;

	public int m_iDisplayToolTipTargetContractID = 4002;

	[Header("우상단 채용확률")]
	public NKCUIComStateButton m_btnContractPool;

	public NKCUIComStateButton m_btnOperatorSubSkillPool;

	[Header("우상단 개발자권한 오브젝트")]
	public GameObject m_objDevTest;

	[Header("우하단 확률업 관련")]
	public GameObject m_objSubTitle;

	public Text m_lbSubTitle;

	[Header("우하단 채용안내")]
	public GameObject m_objContractInfo;

	public Text m_lbContractInfo;

	public GameObject m_objRemainTime;

	public Text m_lbRemainTime;

	[Header("상점 바로가기")]
	public NKCUIComStateButton m_btnShopShortcut;

	[Header("채용버튼 왼쪽")]
	public NKCUIComResourceButton m_btnContractLeft;

	public Image m_imgContractLeftBG;

	public Image m_imgContractLeftIcon;

	public Text m_lbContractLeftDesc;

	public Text m_lbContractLeftCount;

	public GameObject m_objFreeLeft;

	public Text m_lbFreeLeft;

	[Header("채용버튼 오른쪽")]
	public NKCUIComResourceButton m_btnContractRight;

	public Image m_imgContractRightBG;

	public Image m_imgContractRightIcon;

	public Text m_lbContractRightDesc;

	public Text m_lbContractRightCount;

	public GameObject m_objFreeRight;

	public Text m_lbFreeRight;

	[Space]
	public GameObject m_objInfo;

	public NKCComTMPUIText m_lbInfoTitle;

	public NKCUIComStarRank m_InfoStar;

	public NKCComTMPUIText m_lbInfoUnitLv;

	public NKCComTMPUIText m_lbInfoLimitBreakLv;

	public NKCComTMPUIText m_lbInfoSkillLv;

	[Header("선별채용 채용버튼")]
	public NKCUIComResourceButton m_btnSelectContract;

	public Text m_lbSelectContractRemainCount;

	[Header("확정채용 채용버튼")]
	public NKCUIComResourceButton m_btnConfirmContract;

	public Text m_lbConfirmContractDesc;

	[Header("확정채용")]
	public NKCUIComStateButton m_btnConfirmation;

	public Text m_lbConfirmationCount;

	public NKCUIComStateButton m_btnConfirmationInfo;

	public RectTransform m_Content;

	public List<ContractConfirmationSDFace> m_lstSDFace = new List<ContractConfirmationSDFace>(5);

	[Header("기밀채용 패키지")]
	public NKCUIComShopItem m_ShopItem;

	[Header("기간한정 채용")]
	public GameObject m_objTimeLimitContract;

	public NKCComTMPUIText m_lbTimeLimitContract;

	[Header("커스텀 픽업 채용")]
	public NKCUIComStateButton m_csbtnCustomPickUpSelectUnit;

	public Text m_lbCustomPickUpSelectCnt;

	public NKCUIContractUnitSelectList m_CustomPickUpUnitSelectList;

	private const string m_ShopPackageActiveTag = "CLASSIFIED_CONTRACT_BUY_BUTTON";

	private static List<NKCAssetInstanceData> m_listNKCAssetResourceData = new List<NKCAssetInstanceData>();

	private Dictionary<int, List<int>> m_dicTemplet = new Dictionary<int, List<int>>();

	private Dictionary<int, NKCUIContractBanner> m_dicSubUI = new Dictionary<int, NKCUIContractBanner>();

	private int m_SelectedContractID;

	private bool m_bUpdateTime;

	private DateTime m_CurDateEndTime;

	private bool m_bUpdateLimitCount;

	private int m_iCurRemainFreeContractCount;

	private DateTime m_LastUIOpenTimeUTC;

	private DateTime m_ShopPackageNextResetTime;

	private ContractCostType m_costTypeLeft;

	private ContractCostType m_costTypeRight;

	private bool m_bLimitContractUpateTime;

	private DateTime m_CurLimitContractEndTimeUTC;

	private string m_strReservedOpenID;

	private int m_iCurContractLimitCount = -1;

	private int m_iCurMultiContractTryCnt;

	private float m_fDeltaTime;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_CONTRACT;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public override string GuideTempletID => "ARTICLE_SYSTEM_CONTRACT";

	public override List<int> UpsideMenuShowResourceList => BuildUpsideMenuResources();

	private List<int> BuildUpsideMenuResources()
	{
		HashSet<int> other = new HashSet<int> { 401 };
		HashSet<int> hashSet = new HashSet<int>();
		ContractTempletBase contractTempletBase = ContractTempletBase.FindBase(m_SelectedContractID);
		if (contractTempletBase != null)
		{
			hashSet.UnionWith(contractTempletBase.GetPriceItemIDSet());
		}
		bool flag = hashSet.Remove(102);
		hashSet.ExceptWith(other);
		hashSet.Remove(0);
		List<int> list = new List<int>();
		if (contractTempletBase is ContractTempletV2 contractTempletV)
		{
			foreach (RewardUnit resultReward in contractTempletV.m_ResultRewards)
			{
				if (resultReward.RewardType == NKM_REWARD_TYPE.RT_MISC && resultReward.Count > 0)
				{
					list.Add(resultReward.ItemID);
				}
			}
		}
		List<int> list2 = new List<int>(hashSet);
		list2.Sort((int x, int y) => y.CompareTo(x));
		List<int> list3 = new List<int>();
		list3.AddRange(list);
		list3.AddRange(list2);
		if (flag)
		{
			list3.Add(102);
		}
		if (list3.Count == 0)
		{
			list3.Add(401);
			list3.Add(1001);
			list3.Add(1034);
			list3.Add(101);
		}
		return list3;
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private bool IsCurContractCustomPickUp()
	{
		return CustomPickupContractTemplet.Find(m_SelectedContractID) != null;
	}

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_btnContractPool, OnClickPool);
		NKCUtil.SetButtonClickDelegate(m_btnShopShortcut, OnClickShortcut);
		NKCUtil.SetButtonClickDelegate(m_btnOperatorSubSkillPool, OnClickOperatorSubSkillPool);
		m_btnContractLeft?.Init();
		m_btnContractRight?.Init();
		m_btnSelectContract?.Init();
		NKCUtil.SetButtonClickDelegate(m_btnSelectContract, OnClickSelectContract);
		m_btnSelectContract?.Init();
		NKCUtil.SetButtonClickDelegate(m_btnConfirmation, OnClickConfirmation);
		if (m_btnConfirmationInfo != null)
		{
			m_btnConfirmationInfo.PointerDown.RemoveAllListeners();
			m_btnConfirmationInfo.PointerDown.AddListener(OnClickConfirmationTooltip);
			m_btnConfirmationInfo.PointerUp.RemoveAllListeners();
			m_btnConfirmationInfo.PointerUp.AddListener(delegate
			{
				NKCUITooltip.Instance.Close();
			});
		}
		if (m_flTabs != null)
		{
			m_flTabs.m_pfbMajor = m_pfbTab;
			m_flTabs.m_pfbMinor = m_pfbSubTab;
			m_flTabs.m_bCanSelectMajor = false;
		}
		NKCUtil.SetBindFunction(m_csbtnToolTip, delegate
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_CONTRACT_COUNT_CLOSE_TOOLTIP_TITLE, NKCUtilString.GET_STRING_CONTRACT_COUNT_CLOSE_TOOLTIP_DESC);
		});
		NKCUtil.SetBindFunction(m_csbtnCustomPickUpSelectUnit, OnClickChangeCustomPickUpUnit);
		m_iCurRemainFreeContractCount = 0;
		m_CustomPickUpUnitSelectList.Init(OnPickUpUnitClicked, OnPickUpUnitConfirm, OnClosePickUpList);
	}

	private void OnDestroy()
	{
		for (int i = 0; i < m_listNKCAssetResourceData.Count; i++)
		{
			NKCAssetResourceManager.CloseInstance(m_listNKCAssetResourceData[i]);
		}
		m_listNKCAssetResourceData.Clear();
	}

	public override void OnBackButton()
	{
		if (m_CustomPickUpUnitSelectList.IsOpen)
		{
			m_CustomPickUpUnitSelectList.Close();
			return;
		}
		base.OnBackButton();
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
	}

	public void Open(string reservedOpenID = "")
	{
		BuildFoldableList();
		if (SelectRecruitBanner(reservedOpenID))
		{
			m_strReservedOpenID = reservedOpenID;
		}
		else
		{
			SelectFirstBanner();
		}
		CheckInstantContractList();
		UpdateTabRedDot();
		m_LastUIOpenTimeUTC = NKCSynchronizedTime.GetServerUTCTime();
		UIOpened();
		TutorialCheck();
	}

	private void CheckInstantContractList()
	{
		NKCPacketSender.NKMPacket_INSTANT_CONTRACT_LIST_REQ();
	}

	private void BuildFoldableList()
	{
		m_dicTemplet.Clear();
		NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		List<NKCUIComFoldableList.Element> list = new List<NKCUIComFoldableList.Element>();
		foreach (ContractTempletBase value in NKMTempletContainer<ContractTempletBase>.Values)
		{
			if (!NKCSynchronizedTime.IsEventTime(value.EventIntervalTemplet) || (nKCContractDataMgr != null && !nKCContractDataMgr.CheckOpenCond(value)))
			{
				continue;
			}
			NKCUIComFoldableList.Element item = default(NKCUIComFoldableList.Element);
			if (!m_dicTemplet.ContainsKey(value.Category))
			{
				NKCContractCategoryTemplet nKCContractCategoryTemplet = NKCContractCategoryTemplet.Find(value.Category);
				if (nKCContractCategoryTemplet == null)
				{
					Debug.LogError($"ContractCategoryTemplet is null - id : {value.Category}");
					continue;
				}
				if (nKCContractCategoryTemplet.m_Type == NKCContractCategoryTemplet.TabType.Hidden)
				{
					continue;
				}
				item.MajorKey = nKCContractCategoryTemplet.m_CategoryID;
				item.MinorKey = nKCContractCategoryTemplet.IDX;
				item.isMajor = true;
				item.MinorSortKey = nKCContractCategoryTemplet.IDX;
				list.Add(item);
				m_dicTemplet.Add(value.Category, new List<int>());
			}
			item.MajorKey = value.Category;
			item.MinorKey = value.Key;
			item.isMajor = false;
			item.MinorSortKey = value.Order;
			list.Add(item);
			m_dicTemplet[value.Category].Add(value.Key);
		}
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.CONTRACT_CUSTOM_PICKUP))
		{
			foreach (CustomPickupContractTemplet value2 in CustomPickupContractTemplet.Values)
			{
				if (!NKCSynchronizedTime.IsEventTime(value2.IntervalTemplet) || !value2.EnableByTag)
				{
					continue;
				}
				if (value2.CheckReturningUser)
				{
					NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
					if (myUserData == null || !myUserData.IsReturnUser(value2.ReturningUserType))
					{
						continue;
					}
				}
				if (value2.TriggeredContractTimeLimit > 0 && NKCSynchronizedTime.IsFinished(NKCSynchronizedTime.ToUtcTime(nKCContractDataMgr.GetInstantContractEndDateTime(value2.Key))))
				{
					continue;
				}
				NKCUIComFoldableList.Element item2 = default(NKCUIComFoldableList.Element);
				if (!m_dicTemplet.ContainsKey(value2.Category))
				{
					NKCContractCategoryTemplet nKCContractCategoryTemplet2 = NKCContractCategoryTemplet.Find(value2.Category);
					if (nKCContractCategoryTemplet2 == null)
					{
						Debug.LogError($"ContractCategoryTemplet is null - id : {value2.Category}");
						continue;
					}
					if (nKCContractCategoryTemplet2.m_Type == NKCContractCategoryTemplet.TabType.Hidden)
					{
						continue;
					}
					item2.MajorKey = nKCContractCategoryTemplet2.m_CategoryID;
					item2.MinorKey = nKCContractCategoryTemplet2.IDX;
					item2.isMajor = true;
					item2.MinorSortKey = nKCContractCategoryTemplet2.IDX;
					list.Add(item2);
					m_dicTemplet.Add(value2.Category, new List<int>());
				}
				item2.MajorKey = value2.Category;
				item2.MinorKey = value2.Key;
				item2.isMajor = false;
				item2.MinorSortKey = value2.GetOrder();
				list.Add(item2);
				m_dicTemplet[value2.Category].Add(value2.Key);
			}
		}
		m_flTabs.BuildList(list, OnSelectTab);
	}

	public void ResetContractUI(bool bForce = false)
	{
		RefreshBanner(bForce);
		UpdateChildUI();
		UpdateUpsideMenu();
		UpdateTabRedDot();
	}

	private void UpdateTabRedDot()
	{
		NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		foreach (KeyValuePair<int, List<int>> item in m_dicTemplet)
		{
			(m_flTabs.GetMajorSlot(item.Key) as NKCUIContractListSlot)?.SetActiveRedDot(bValue: false);
			for (int i = 0; i < item.Value.Count; i++)
			{
				bool flag = nKCContractDataMgr.CheckOpenCond(item.Value[i]) && nKCContractDataMgr.GetRemainFreeChangeCnt(item.Value[i]) > 0;
				(m_flTabs.GetMinorSlot(item.Key, item.Value[i]) as NKCUIContractListSlot)?.SetActiveRedDot(flag);
				if (flag)
				{
					(m_flTabs.GetMajorSlot(item.Key) as NKCUIContractListSlot)?.SetActiveRedDot(flag);
				}
			}
		}
	}

	private void UpdateShopPackageItem()
	{
		if (m_ShopItem == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_ShopItem, bValue: false);
		bool flag = NKMOpenTagManager.IsOpened("CLASSIFIED_CONTRACT_BUY_BUTTON");
		CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(m_SelectedContractID);
		if (customPickupContractTemplet != null && customPickupContractTemplet.CustomPickUpType == CustomPickupContractTemplet.CUSTOM_PICK_UP_TYPE.AWAKEN)
		{
			NKCUtil.SetGameobjectActive(m_ShopItem, flag);
			if (flag)
			{
				m_ShopItem.SetData();
			}
			return;
		}
		ContractTempletV2 contractTempletV = ContractTempletV2.Find(m_SelectedContractID);
		if (contractTempletV == null)
		{
			return;
		}
		NKCContractCategoryTemplet nKCContractCategoryTemplet = NKCContractCategoryTemplet.Find(contractTempletV.Category);
		if (nKCContractCategoryTemplet == null)
		{
			return;
		}
		switch (nKCContractCategoryTemplet.m_Type)
		{
		case NKCContractCategoryTemplet.TabType.Confirm:
			return;
		case NKCContractCategoryTemplet.TabType.Basic:
			return;
		case NKCContractCategoryTemplet.TabType.FollowTarget:
		{
			bool flag2 = false;
			foreach (RandomUnitTempletV2 unitTemplet in contractTempletV.UnitPoolTemplet.UnitTemplets)
			{
				if (unitTemplet.PickUpTarget && unitTemplet.UnitTemplet.m_bAwaken)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				return;
			}
			break;
		}
		}
		NKCUtil.SetGameobjectActive(m_ShopItem, flag);
		if (flag)
		{
			m_ShopItem.SetData();
		}
	}

	protected void RefreshBanner(bool bForce = false)
	{
		if (CheckContractEnd() || bForce)
		{
			if (NKCPopupResourceConfirmBox.IsInstanceOpen)
			{
				NKCPopupResourceConfirmBox.Instance.Close();
			}
			BuildFoldableList();
			if (bForce && SelectRecruitBanner(m_strReservedOpenID))
			{
				m_strReservedOpenID = "";
			}
			else
			{
				SelectFirstBanner();
			}
			UpdateUpsideMenu();
		}
	}

	private bool CheckContractEnd()
	{
		if (IsCurContractCustomPickUp())
		{
			return false;
		}
		NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		foreach (List<int> value in m_dicTemplet.Values)
		{
			for (int i = 0; i < value.Count; i++)
			{
				if (nKCContractDataMgr != null && !nKCContractDataMgr.CheckOpenCond(value[i]) && value[i] == m_SelectedContractID)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void OnContractCompleteAck()
	{
		RefreshBanner();
		UpdateChildUI();
		UpdateTabRedDot();
	}

	public void UpdateChildUI()
	{
		ContractTempletBase contractTempletBase = ContractTempletBase.FindBase(m_SelectedContractID);
		if (contractTempletBase is SelectableContractTemplet)
		{
			UpdateChildUI(contractTempletBase as SelectableContractTemplet);
			return;
		}
		if (contractTempletBase is ContractTempletV2)
		{
			UpdateChildUI(contractTempletBase as ContractTempletV2);
			return;
		}
		if (m_CustomPickUpUnitSelectList.IsOpen)
		{
			m_CustomPickUpUnitSelectList.Close();
		}
		CustomPickupContractTemplet customPickUpTemplet = CustomPickupContractTemplet.Find(m_SelectedContractID);
		UpdateChildUI(customPickUpTemplet);
	}

	private void UpdateChildUI(SelectableContractTemplet templet)
	{
		if (templet != null)
		{
			NKCUtil.SetGameobjectActive(m_btnContractLeft, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnContractRight, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnSelectContract, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnConfirmContract, bValue: false);
			NKCUtil.SetGameobjectActive(m_objEvent, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnConfirmation, bValue: false);
			int selectableContractChangeCnt = NKCScenManager.GetScenManager().GetNKCContractDataMgr().GetSelectableContractChangeCnt(templet.Key);
			NKCUtil.SetLabelText(m_lbSelectContractRemainCount, $"{selectableContractChangeCnt}/{templet.m_UnitPoolChangeCount}");
			NKCUtil.SetGameobjectActive(m_btnShopShortcut, bValue: false);
		}
	}

	private void UpdateChildUI(ContractTempletV2 contractTemplet)
	{
		if (contractTemplet == null)
		{
			return;
		}
		NKCContractCategoryTemplet categoryTemplet = NKCContractCategoryTemplet.Find(contractTemplet.Category);
		if (!UpdateChildUIConfimrUI(categoryTemplet, contractTemplet.m_SingleTryRequireItems))
		{
			NKCUtil.SetGameobjectActive(m_btnContractLeft, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnContractRight, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnSelectContract, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnConfirmContract, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnShopShortcut, contractTemplet.m_ResultRewards.Find((RewardUnit x) => x.ItemID == 401) != null);
			UpdateContractLimitUI(contractTemplet);
			UpdateBonusMileage(contractTemplet);
			UpdateFreeContractUI(contractTemplet, out var remainFreeChance, out var bSkipUpdateContractUI);
			if (remainFreeChance <= 0 && !bSkipUpdateContractUI)
			{
				UpdateContractUI(contractTemplet);
			}
		}
	}

	private void UpdateChildUI(CustomPickupContractTemplet customPickUpTemplet)
	{
		if (customPickUpTemplet == null)
		{
			return;
		}
		NKCContractCategoryTemplet nKCContractCategoryTemplet = NKCContractCategoryTemplet.Find(customPickUpTemplet.Category);
		if (nKCContractCategoryTemplet != null && !UpdateChildUIConfimrUI(nKCContractCategoryTemplet, customPickUpTemplet.SingleTryRequireItems, bIsCustomPickUp: true, !customPickUpTemplet.IsSelectedPickUpUnit()))
		{
			NKCUtil.SetGameobjectActive(m_btnContractLeft, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnContractRight, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnSelectContract, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnConfirmContract, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnShopShortcut, customPickUpTemplet.ResultRewards.Find((RewardUnit x) => x.ItemID == 401) != null);
			m_iCurContractLimitCount = -1;
			NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
			if (nKCContractDataMgr.hasContractLimit(m_SelectedContractID))
			{
				m_iCurContractLimitCount = nKCContractDataMgr.GetContractLimitCnt(m_SelectedContractID);
			}
			UpdatePickupSubTitle(customPickUpTemplet);
			UpdateCustomPickUpSelectUI(customPickUpTemplet);
			UpdateBonusMileage(customPickUpTemplet);
			UpdateContractUI(customPickUpTemplet);
		}
	}

	private bool UpdateChildUIConfimrUI(NKCContractCategoryTemplet categoryTemplet, MiscItemUnit[] SingleTryRequireItems, bool bIsCustomPickUp = false, bool bBlockCustomPickUp = false)
	{
		if (categoryTemplet == null)
		{
			return false;
		}
		if (categoryTemplet.m_Type == NKCContractCategoryTemplet.TabType.Confirm)
		{
			NKCUtil.SetGameobjectActive(m_btnContractLeft, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnContractRight, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnSelectContract, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnConfirmContract, bValue: true);
			NKCUtil.SetGameobjectActive(m_objEvent, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnConfirmation, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnShopShortcut, bValue: false);
			for (int i = 0; i < SingleTryRequireItems.Length; i++)
			{
				MiscItemUnit reqItem = SingleTryRequireItems[i];
				if (reqItem == null)
				{
					continue;
				}
				ContractCostType costTypeLeft = ((i == 0) ? ContractCostType.Ticket : ContractCostType.Money);
				NKCUtil.SetLabelText(m_lbConfirmContractDesc, string.Format(NKCUtilString.GET_STRING_CONTRACT_BUTTON_DESC, 1));
				m_btnConfirmContract.PointerClick.RemoveAllListeners();
				m_btnConfirmContract.SetData(reqItem.ItemId, reqItem.Count32);
				if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(reqItem.ItemId) >= reqItem.Count)
				{
					NKMItemMiscTemplet miscTemplet = NKMItemMiscTemplet.Find(reqItem.ItemId);
					if (miscTemplet == null)
					{
						continue;
					}
					m_costTypeLeft = costTypeLeft;
					m_btnConfirmContract.PointerClick.AddListener(delegate
					{
						if (bIsCustomPickUp && bBlockCustomPickUp)
						{
							NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_CONTRACT_CUSTOM_CONTRACT_PICK_UP_WARNING_DESC);
						}
						else
						{
							NKCPopupResourceConfirmBox.Instance.OpenForContract(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCUtilString.GET_STRING_CONTRACT_REQ_DESC_03, miscTemplet.GetItemName(), reqItem.Count, 1), reqItem.ItemId, reqItem.Count32, delegate
							{
								if (bIsCustomPickUp)
								{
									NKCPacketSender.Send_NKMPacket_CUSTOM_PICKUP_REQ(m_SelectedContractID, m_costTypeLeft, 1);
								}
								else
								{
									NKCPacketSender.Send_NKMPacket_CONTRACT_REQ(m_SelectedContractID, m_costTypeLeft, 1);
								}
							}, null, m_SelectedContractID, 1);
						}
					});
					break;
				}
				m_btnConfirmContract.PointerClick.AddListener(delegate
				{
					NKCShopManager.OpenItemLackPopup(reqItem.ItemId, reqItem.Count32);
				});
			}
			return true;
		}
		return false;
	}

	private void UpdateContractLimitUI(ContractTempletV2 templet)
	{
		if (templet != null)
		{
			NKCUtil.SetGameobjectActive(m_objEvent, bValue: true);
			m_iCurContractLimitCount = -1;
			NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
			if (nKCContractDataMgr.hasContractLimit(m_SelectedContractID))
			{
				m_iCurContractLimitCount = nKCContractDataMgr.GetContractLimitCnt(m_SelectedContractID);
			}
			NKCUtil.SetGameobjectActive(m_objEventRemainCount, m_iCurContractLimitCount >= 0);
			NKCUtil.SetGameobjectActive(m_objToolTip, m_SelectedContractID == m_iDisplayToolTipTargetContractID);
			if (m_objEventRemainCount.activeSelf)
			{
				NKCUtil.SetLabelText(m_lbEventRemainCount, string.Format(NKCUtilString.GET_STRING_CONTRACT_REMAIN_COUNT_DESC, m_iCurContractLimitCount));
			}
			NKCUtil.SetGameobjectActive(m_objEventDate, !string.IsNullOrEmpty(templet.ContractDescId));
			if (m_objEventDate.activeSelf)
			{
				NKCUtil.SetLabelText(m_lbEventDate, NKCStringTable.GetString(templet.ContractDescId));
			}
			m_bUpdateLimitCount = nKCContractDataMgr.IsDailyContractLimit(templet);
		}
	}

	private void UpdateCustomPickUpSelectUI(CustomPickupContractTemplet customPickUpTemplet)
	{
		NKCUtil.SetLabelText(m_lbCustomPickUpSelectCnt, $"{customPickUpTemplet.MaxSelectCount - customPickUpTemplet.CurrentSelectCount}/{customPickUpTemplet.MaxSelectCount}");
	}

	private void UpdateButtonInfoUI()
	{
		ContractTempletBase contractTempletBase = ContractTempletBase.FindBase(m_SelectedContractID);
		NKCUtil.SetLabelText(m_lbInfoTitle, contractTempletBase.GetContractName());
		bool bValue = false;
		if (contractTempletBase is ContractTempletV2)
		{
			ContractTempletV2 contractTempletV = contractTempletBase as ContractTempletV2;
			m_InfoStar.SetStarRank(contractTempletV.m_pickUnitLimits, 6);
			switch (contractTempletV.m_pickUnitLevel)
			{
			default:
				NKCUtil.SetLabelText(m_lbInfoUnitLv, "");
				NKCUtil.SetLabelText(m_lbInfoLimitBreakLv, "");
				break;
			case 100:
				NKCUtil.SetLabelText(m_lbInfoUnitLv, NKCStringTable.GetString("SI_PF_CONTRACT_GT_100LV"));
				NKCUtil.SetLabelText(m_lbInfoLimitBreakLv, NKCStringTable.GetString("SI_PF_CONTRACT_GT_LIMITBREAK_3"));
				break;
			case 110:
				NKCUtil.SetLabelText(m_lbInfoUnitLv, NKCStringTable.GetString("SI_PF_CONTRACT_GT_110LV"));
				NKCUtil.SetLabelText(m_lbInfoLimitBreakLv, NKCStringTable.GetString("SI_PF_CONTRACT_GT_LIMITFUSION_5"));
				break;
			}
			if (contractTempletV.m_isMaxSkillLevelUnits)
			{
				NKCUtil.SetLabelText(m_lbInfoSkillLv, NKCStringTable.GetString("SI_PF_CONTRACT_GT_SKILL_MAX"));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbInfoSkillLv, "");
			}
			bValue = contractTempletV.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL;
		}
		NKCUtil.SetGameobjectActive(m_objInfo, bValue);
	}

	private void UpdateBonusMileage(ContractTempletV2 templet)
	{
		NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		bool flag = false;
		if (nKCContractDataMgr != null && templet.m_ContractBonusCountGroupID != 0)
		{
			flag = nKCContractDataMgr.IsActiveContrctConfirmation(templet.Key);
		}
		if (flag)
		{
			int contractBonusCnt = NKCScenManager.GetScenManager().GetNKCContractDataMgr().GetContractBonusCnt(templet.m_ContractBonusCountGroupID);
			int contractBounsItemReqireCount = templet.m_ContractBounsItemReqireCount;
			if (contractBounsItemReqireCount - contractBonusCnt <= 1)
			{
				NKCUtil.SetLabelText(m_lbConfirmationCount, NKCUtilString.GET_STRING_CONTRACT_CONFIRMATION_DESC);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbConfirmationCount, $"<color=#FFCF3B>{contractBonusCnt}</color>/{contractBounsItemReqireCount}");
			}
			NKCUtil.SetGameobjectActive(m_btnConfirmation, bValue: true);
			List<NKMUnitTempletBase> lstGetableUnit = new List<NKMUnitTempletBase>();
			foreach (RandomUnitTempletV2 unitTemplet in templet.UnitPoolTemplet.UnitTemplets)
			{
				if (unitTemplet.PickUpTarget && unitTemplet.UnitTemplet.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR)
				{
					lstGetableUnit.Add(unitTemplet.UnitTemplet);
				}
			}
			List<int> curSelectableUnitList = NKCScenManager.GetScenManager().GetNKCContractDataMgr().GetCurSelectableUnitList(templet.Key);
			bool flag2 = true;
			int cnt = 0;
			while (cnt < m_lstSDFace.Count)
			{
				if (cnt < lstGetableUnit.Count)
				{
					NKCUtil.SetGameobjectActive(m_lstSDFace[cnt].m_objRoot, bValue: true);
					if (curSelectableUnitList.Count > 0)
					{
						flag2 = curSelectableUnitList.Find((int e) => e == lstGetableUnit[cnt].m_UnitID) > 0;
					}
					NKCUtil.SetImageSprite(m_lstSDFace[cnt].m_imgFace, NKCResourceUtility.GetOrLoadMinimapFaceIcon(lstGetableUnit[cnt]));
					NKCUtil.SetGameobjectActive(m_lstSDFace[cnt].m_imgFace, bValue: true);
					NKCUtil.SetGameobjectActive(m_lstSDFace[cnt].m_objCheck, !flag2);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstSDFace[cnt].m_objRoot, bValue: false);
				}
				int num = cnt + 1;
				cnt = num;
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_btnConfirmation, bValue: false);
		}
	}

	private void UpdateBonusMileage(CustomPickupContractTemplet templet)
	{
		int contractBonusCnt = NKCScenManager.GetScenManager().GetNKCContractDataMgr().GetContractBonusCnt(templet.ContractBonusCountGroupID);
		int maxBonusReqoreCount = templet.MaxBonusReqoreCount;
		if (maxBonusReqoreCount - contractBonusCnt <= 1)
		{
			NKCUtil.SetLabelText(m_lbConfirmationCount, NKCUtilString.GET_STRING_CONTRACT_CONFIRMATION_DESC);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbConfirmationCount, $"<color=#FFCF3B>{contractBonusCnt}</color>/{maxBonusReqoreCount}");
		}
		NKCUtil.SetGameobjectActive(m_btnConfirmation, bValue: true);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(templet.PickUpTargetUnitID);
		if (unitTempletBase == null)
		{
			for (int i = 0; i < m_lstSDFace.Count; i++)
			{
				if (i == 0)
				{
					NKCUtil.SetGameobjectActive(m_lstSDFace[i].m_objRoot, bValue: true);
					NKCUtil.SetGameobjectActive(m_lstSDFace[i].m_imgFace, bValue: false);
					NKCUtil.SetGameobjectActive(m_lstSDFace[i].m_objCheck, bValue: false);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstSDFace[i].m_objRoot, bValue: false);
				}
			}
			return;
		}
		List<NKMUnitTempletBase> list = new List<NKMUnitTempletBase> { unitTempletBase };
		for (int j = 0; j < m_lstSDFace.Count; j++)
		{
			if (j < list.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstSDFace[j].m_objRoot, bValue: true);
				NKCUtil.SetImageSprite(m_lstSDFace[j].m_imgFace, NKCResourceUtility.GetOrLoadMinimapFaceIcon(list[j]), bDisableIfSpriteNull: true);
				NKCUtil.SetGameobjectActive(m_lstSDFace[j].m_imgFace, bValue: true);
				NKCUtil.SetGameobjectActive(m_lstSDFace[j].m_objCheck, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstSDFace[j].m_objRoot, bValue: false);
			}
		}
	}

	private void UpdateFreeContractUI(ContractTempletV2 templet, out int remainFreeChance, out bool bSkipUpdateContractUI)
	{
		bSkipUpdateContractUI = false;
		remainFreeChance = 0;
		m_iCurRemainFreeContractCount = 0;
		m_btnContractLeft.PointerClick.RemoveAllListeners();
		m_btnContractRight.PointerClick.RemoveAllListeners();
		NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		if (nKCContractDataMgr == null)
		{
			return;
		}
		if (templet.m_FreeTryCnt > 0)
		{
			if (nKCContractDataMgr.IsHasContractStateData(templet.Key) && nKCContractDataMgr.IsActiveNextFreeChance(templet.Key))
			{
				NKCPacketSender.Send_NKMPacket_CONTRACT_STATE_LIST_REQ();
				return;
			}
			remainFreeChance = nKCContractDataMgr.GetRemainFreeChangeCnt(templet.Key);
			if (nKCContractDataMgr.hasContractLimit(m_SelectedContractID) && m_iCurContractLimitCount >= 0)
			{
				remainFreeChance = Mathf.Min(remainFreeChance, m_iCurContractLimitCount);
			}
			if (!templet.m_resetFreeCount)
			{
				bool flag = NKCSynchronizedTime.IsFinished(templet.EventIntervalTemplet.GetStartDateUtc().AddDays(templet.m_freeCountDays));
				NKCUtil.SetGameobjectActive(m_objFreeLeft, !flag);
				NKCUtil.SetGameobjectActive(m_objFreeRight, !flag);
				if (!flag)
				{
					NKCUtil.SetLabelText(m_lbFreeLeft, string.Format(NKCStringTable.GetString("SI_CONTRACT_RESET_LIMIT"), templet.m_FreeTryCnt));
					NKCUtil.SetLabelText(m_lbFreeRight, string.Format(NKCStringTable.GetString("SI_CONTRACT_RESET_LIMIT"), templet.m_FreeTryCnt));
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objFreeLeft, remainFreeChance > 0);
				NKCUtil.SetGameobjectActive(m_objFreeRight, remainFreeChance > 0);
				NKCUtil.SetLabelText(m_lbFreeLeft, NKCStringTable.GetString("SI_DP_CONTRACT_RESET_TIME_DESC"));
				NKCUtil.SetLabelText(m_lbFreeRight, NKCStringTable.GetString("SI_DP_CONTRACT_RESET_TIME_DESC"));
			}
			if (remainFreeChance >= 1)
			{
				m_btnContractLeft.PointerClick.AddListener(OnClickFreeContract);
				m_btnContractRight.PointerClick.AddListener(OnClickFreeContractMulti);
				NKCUtil.SetGameobjectActive(m_imgContractLeftIcon, bValue: false);
				NKCUtil.SetGameobjectActive(m_imgContractRightIcon, bValue: false);
				NKCUtil.SetImageSprite(m_imgContractLeftBG, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW));
				NKCUtil.SetLabelText(m_lbContractLeftDesc, string.Format(NKCUtilString.GET_STRING_CONTRACT_FREE_BUTTON_DESC_01, 1));
				NKCUtil.SetLabelTextColor(m_lbContractLeftDesc, (remainFreeChance >= 1) ? NKCUtil.GetColor("#582817") : NKCUtil.GetColor("#212122"));
				NKCUtil.SetLabelText(m_lbContractLeftCount, $"1/{remainFreeChance}");
				NKCUtil.SetLabelTextColor(m_lbContractLeftCount, NKCUtil.GetColor("#FFCF3B"));
				if (remainFreeChance > 1)
				{
					m_iCurRemainFreeContractCount = Math.Min(remainFreeChance, 10);
					NKCUtil.SetLabelText(m_lbContractRightDesc, string.Format(NKCUtilString.GET_STRING_CONTRACT_FREE_BUTTON_DESC_01, m_iCurRemainFreeContractCount));
					NKCUtil.SetLabelText(m_lbContractRightCount, $"{m_iCurRemainFreeContractCount}/{remainFreeChance}");
				}
				else
				{
					NKCUtil.SetLabelText(m_lbContractRightDesc, NKCUtilString.GET_STRING_CONTRACT_FREE_BUTTON_DESC);
					NKCUtil.SetLabelText(m_lbContractRightCount, "-");
				}
				Sprite sp = ((remainFreeChance > 1) ? NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW) : NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
				NKCUtil.SetImageSprite(m_imgContractRightBG, sp);
				NKCUtil.SetLabelTextColor(m_lbContractRightDesc, (remainFreeChance > 1) ? NKCUtil.GetColor("#582817") : NKCUtil.GetColor("#212122"));
				NKCUtil.SetLabelTextColor(m_lbContractRightCount, (remainFreeChance > 1) ? NKCUtil.GetColor("#FFCF3B") : NKCUtil.GetColor("#212122"));
			}
			else if (templet.IsFreeOnlyContract)
			{
				bSkipUpdateContractUI = true;
				NKCUtil.SetBindFunction(m_btnContractLeft, delegate
				{
					NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_CONTRACT_FREE_TRY_EXIT_DESC);
				});
				NKCUtil.SetBindFunction(m_btnContractRight, delegate
				{
					NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_CONTRACT_FREE_TRY_EXIT_DESC);
				});
				NKCUtil.SetGameobjectActive(m_imgContractLeftIcon, bValue: false);
				NKCUtil.SetGameobjectActive(m_imgContractRightIcon, bValue: false);
				NKCUtil.SetLabelTextColor(m_lbContractLeftDesc, NKCUtil.GetColor("#212122"));
				NKCUtil.SetLabelTextColor(m_lbContractRightDesc, NKCUtil.GetColor("#212122"));
				NKCUtil.SetImageSprite(m_imgContractLeftBG, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
				NKCUtil.SetImageSprite(m_imgContractRightBG, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
				NKCUtil.SetLabelText(m_lbContractLeftDesc, string.Format(NKCUtilString.GET_STRING_CONTRACT_FREE_BUTTON_DESC_01, 1));
				NKCUtil.SetLabelText(m_lbContractRightDesc, string.Format(NKCUtilString.GET_STRING_CONTRACT_FREE_BUTTON_DESC_01, 10));
				NKCUtil.SetLabelText(m_lbContractLeftCount, "-");
				NKCUtil.SetLabelText(m_lbContractRightCount, "-");
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objFreeLeft, bValue: false);
			NKCUtil.SetGameobjectActive(m_objFreeRight, bValue: false);
		}
	}

	private void UpdateContractUI(MiscItemUnit[] m_SingleTryRequireItems, int iLeftCountUntilConfirmation, bool bIsCustomPickUp = false, bool bBlockCustomPickUp = false)
	{
		m_iCurMultiContractTryCnt = 0;
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		m_btnContractLeft.PointerClick.RemoveAllListeners();
		m_btnContractRight.PointerClick.RemoveAllListeners();
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < m_SingleTryRequireItems.Length; i++)
		{
			MiscItemUnit reqItem = m_SingleTryRequireItems[i];
			if (reqItem == null)
			{
				continue;
			}
			if (flag)
			{
				break;
			}
			ContractCostType contractCostType = ((i == 0) ? ContractCostType.Ticket : ContractCostType.Money);
			int num = (int)inventoryData.GetCountMiscItem(reqItem.ItemId);
			int val = num / reqItem.Count32;
			if (m_iCurContractLimitCount >= 0)
			{
				val = Math.Min(val, m_iCurContractLimitCount);
			}
			if (iLeftCountUntilConfirmation > 0)
			{
				val = Math.Min(val, iLeftCountUntilConfirmation);
			}
			m_iCurMultiContractTryCnt = Math.Min(val, 10);
			if (reqItem.ItemId > 0)
			{
				NKCUtil.SetGameobjectActive(m_imgContractLeftIcon, bValue: true);
				NKCUtil.SetGameobjectActive(m_imgContractRightIcon, bValue: true);
			}
			NKMItemMiscTemplet miscTemplet = NKMItemManager.GetItemMiscTempletByID(reqItem.ItemId);
			if (!flag)
			{
				m_btnContractLeft.PointerClick.RemoveAllListeners();
				m_btnContractLeft.SetData(reqItem.ItemId, reqItem.Count32);
				if (num >= reqItem.Count)
				{
					flag = true;
					m_costTypeLeft = contractCostType;
					m_btnContractLeft.PointerClick.AddListener(delegate
					{
						if (bBlockCustomPickUp && bIsCustomPickUp)
						{
							NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_CONTRACT_CUSTOM_CONTRACT_PICK_UP_WARNING_DESC);
						}
						else
						{
							NKCPopupResourceConfirmBox.Instance.OpenForContract(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCUtilString.GET_STRING_CONTRACT_REQ_DESC_03, miscTemplet.GetItemName(), reqItem.Count, 1), reqItem.ItemId, reqItem.Count32, delegate
							{
								if (bIsCustomPickUp)
								{
									NKCPacketSender.Send_NKMPacket_CUSTOM_PICKUP_REQ(m_SelectedContractID, m_costTypeLeft, 1);
								}
								else
								{
									NKCPacketSender.Send_NKMPacket_CONTRACT_REQ(m_SelectedContractID, m_costTypeLeft, 1);
								}
							}, null, m_SelectedContractID, 1);
						}
					});
				}
				else
				{
					m_btnContractLeft.PointerClick.AddListener(delegate
					{
						NKCShopManager.OpenItemLackPopup(reqItem.ItemId, reqItem.Count32);
					});
				}
			}
			if (flag2)
			{
				continue;
			}
			m_btnContractRight.PointerClick.RemoveAllListeners();
			m_btnContractRight.SetData(reqItem.ItemId, reqItem.Count32 * m_iCurMultiContractTryCnt);
			if (m_iCurMultiContractTryCnt > 0)
			{
				if (num >= reqItem.Count32 * m_iCurMultiContractTryCnt)
				{
					flag2 = true;
					m_costTypeRight = contractCostType;
					m_btnContractRight.PointerClick.AddListener(delegate
					{
						if (bBlockCustomPickUp && bIsCustomPickUp)
						{
							NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_CONTRACT_CUSTOM_CONTRACT_PICK_UP_WARNING_DESC);
						}
						else
						{
							NKCPopupResourceConfirmBox.Instance.OpenForContract(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCUtilString.GET_STRING_CONTRACT_REQ_DESC_03, miscTemplet.GetItemName(), reqItem.Count * m_iCurMultiContractTryCnt, m_iCurMultiContractTryCnt), reqItem.ItemId, reqItem.Count32 * m_iCurMultiContractTryCnt, delegate
							{
								if (bIsCustomPickUp)
								{
									NKCPacketSender.Send_NKMPacket_CUSTOM_PICKUP_REQ(m_SelectedContractID, m_costTypeRight, m_iCurMultiContractTryCnt);
								}
								else
								{
									NKCPacketSender.Send_NKMPacket_CONTRACT_REQ(m_SelectedContractID, m_costTypeRight, m_iCurMultiContractTryCnt);
								}
							}, null, m_SelectedContractID, m_iCurMultiContractTryCnt);
						}
					});
					NKCUtil.SetLabelText(m_lbContractRightDesc, string.Format(NKCUtilString.GET_STRING_CONTRACT_BUTTON_DESC, m_iCurMultiContractTryCnt));
					NKCUtil.SetLabelText(m_lbContractRightCount, (m_iCurMultiContractTryCnt * reqItem.Count).ToString("#,##0"));
				}
				else
				{
					m_btnContractRight.PointerClick.AddListener(delegate
					{
						NKCShopManager.OpenItemLackPopup(reqItem.ItemId, reqItem.Count32 * m_iCurMultiContractTryCnt);
					});
				}
			}
			else
			{
				m_btnContractRight.SetText("-");
				NKCUtil.SetLabelText(m_lbContractRightDesc, string.Format(NKCUtilString.GET_STRING_CONTRACT_BUTTON_DESC, 1));
				NKCUtil.SetLabelText(m_lbContractRightCount, "-");
			}
			if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(reqItem.ItemId) >= reqItem.Count32 * m_iCurMultiContractTryCnt)
			{
				NKCUtil.SetLabelTextColor(m_lbContractRightCount, NKCUtil.GetColor("#FFFFFF"));
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_lbContractRightCount, NKCUtil.GetColor("#CD2121"));
			}
		}
		NKCUtil.SetLabelText(m_lbContractLeftDesc, string.Format(NKCUtilString.GET_STRING_CONTRACT_BUTTON_DESC, 1));
		Color col = ((m_iCurMultiContractTryCnt > 0) ? NKCUtil.GetColor("#582817") : NKCUtil.GetColor("#212122"));
		Sprite sp = ((m_iCurMultiContractTryCnt > 0) ? NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW) : NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
		NKCUtil.SetImageSprite(m_imgContractRightBG, sp);
		NKCUtil.SetLabelTextColor(m_lbContractRightDesc, col);
		if (m_bUpdateLimitCount)
		{
			NKCUtil.SetLabelTextColor(m_lbContractLeftDesc, NKCUtil.GetColor("#212122"));
			NKCUtil.SetImageSprite(m_imgContractLeftBG, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
			NKCUtil.SetImageSprite(m_imgContractRightBG, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
			m_btnContractLeft.PointerClick.RemoveAllListeners();
			m_btnContractRight.PointerClick.RemoveAllListeners();
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbContractLeftDesc, NKCUtil.GetColor("#582817"));
			NKCUtil.SetImageSprite(m_imgContractLeftBG, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW));
		}
	}

	private void UpdateContractUI(ContractTempletV2 templet)
	{
		int iLeftCountUntilConfirmation = 0;
		if (templet.m_ContractBonusCountGroupID != 0)
		{
			NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
			if (nKCContractDataMgr != null && nKCContractDataMgr.IsActiveContrctConfirmation(m_SelectedContractID))
			{
				int contractBonusCnt = NKCScenManager.GetScenManager().GetNKCContractDataMgr().GetContractBonusCnt(templet.m_ContractBonusCountGroupID);
				iLeftCountUntilConfirmation = templet.m_ContractBounsItemReqireCount - contractBonusCnt;
			}
		}
		UpdateContractUI(templet.m_SingleTryRequireItems, iLeftCountUntilConfirmation);
	}

	private void UpdateContractUI(CustomPickupContractTemplet templet)
	{
		UpdateContractUI(templet.SingleTryRequireItems, templet.LeftCountUntilConfirm, bIsCustomPickUp: true, !templet.IsSelectedPickUpUnit());
		NKCUtil.SetGameobjectActive(m_objFreeLeft, bValue: false);
		NKCUtil.SetGameobjectActive(m_objFreeRight, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEventRemainCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEvent, bValue: false);
	}

	private void UpdateContractRemainTime()
	{
		NKCUtil.SetGameobjectActive(m_objRemainTime, bValue: true);
		NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: true);
		NKCUtil.SetLabelText(m_lbRemainTime, NKCUtilString.GetRemainTimeStringEx(m_CurDateEndTime));
		if (NKCSynchronizedTime.IsFinished(m_CurDateEndTime))
		{
			m_bUpdateTime = false;
		}
	}

	private bool UpdateLimitContractRemainTime()
	{
		if (!NKCSynchronizedTime.IsFinished(m_CurLimitContractEndTimeUTC))
		{
			TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(m_CurLimitContractEndTimeUTC);
			string text = null;
			NKCUtil.SetLabelText(msg: (timeLeft.Days <= 0) ? string.Format(NKCUtilString.GET_STRING_CONTRACT_LIMIT_TIME_HOUR, timeLeft.Hours, timeLeft.Minutes) : string.Format(NKCUtilString.GET_STRING_CONTRACT_LIMIT_TIME_DAY, timeLeft.Days, timeLeft.Hours), label: m_lbTimeLimitContract);
			return true;
		}
		return false;
	}

	public void SelectFirstBanner()
	{
		int num = 0;
		int num2 = 0;
		foreach (List<int> value in m_dicTemplet.Values)
		{
			for (int i = 0; i < value.Count; i++)
			{
				int num3 = 0;
				int num4 = 0;
				ContractTempletBase contractTempletBase = ContractTempletBase.FindBase(value[i]);
				if (contractTempletBase != null)
				{
					num3 = contractTempletBase.Priority;
					num4 = contractTempletBase.Key;
				}
				else
				{
					CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(value[i]);
					if (customPickupContractTemplet != null)
					{
						num3 = customPickupContractTemplet.Priority;
						num4 = customPickupContractTemplet.Key;
					}
				}
				if (num4 != 0)
				{
					if (num == 0)
					{
						num = value[i];
					}
					if (num2 < num3)
					{
						num2 = num3;
						num = value[i];
					}
					else if (num2 == num3 && num < num4)
					{
						num = num4;
					}
				}
			}
		}
		if (num != 0)
		{
			SelectRecruitBanner(num);
		}
	}

	public bool SelectRecruitBanner(string contractStrID)
	{
		if (string.IsNullOrEmpty(contractStrID))
		{
			return false;
		}
		ContractTempletBase contractTempletBase = ContractTempletBase.Find(contractStrID);
		if (contractTempletBase != null && NKCScenManager.GetScenManager().GetNKCContractDataMgr().CheckOpenCond(contractTempletBase))
		{
			SelectRecruitBanner(contractTempletBase, bForceUpdate: true);
			return true;
		}
		CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(contractStrID);
		if (customPickupContractTemplet == null)
		{
			return false;
		}
		if (!customPickupContractTemplet.EnableByTag)
		{
			return false;
		}
		DateTime currentServiceTime = NKMTime.UTCtoLocal(NKCSynchronizedTime.GetServerUTCTime());
		if (!customPickupContractTemplet.IsAvailableTime(currentServiceTime))
		{
			return false;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		if (customPickupContractTemplet.CheckReturningUser && !myUserData.IsReturnUser(customPickupContractTemplet.ReturningUserType))
		{
			return false;
		}
		SelectRecruitBanner(customPickupContractTemplet, bForceUpdate: true);
		return true;
	}

	public void SelectRecruitBanner(int contractID)
	{
		ContractTempletBase contractTempletBase = ContractTempletBase.FindBase(contractID);
		if (contractTempletBase != null)
		{
			SelectRecruitBanner(contractTempletBase, bForceUpdate: true);
			return;
		}
		CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(contractID);
		if (customPickupContractTemplet != null)
		{
			SelectRecruitBanner(customPickupContractTemplet);
		}
		else
		{
			Debug.Log($"<color=red>NKCUIContractV3::SelectRecruitBanner - Can not find Contract id : {contractID}</color>");
		}
	}

	public void SelectRecruitBanner(ContractTempletBase templet, bool bForceUpdate = false)
	{
		if (templet == null || (templet.Key == m_SelectedContractID && !bForceUpdate))
		{
			return;
		}
		m_flTabs.SelectMinorSlot(templet.Category, templet.Key);
		m_bUpdateTime = false;
		if (templet.IsPickUp())
		{
			DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime(-1.0);
			if (NKCSynchronizedTime.GetTimeLeft(templet.GetDateEndUtc()).TotalDays > (double)NKCSynchronizedTime.UNLIMITD_REMAIN_DAYS)
			{
				m_bUpdateTime = false;
			}
			else if (!templet.IsAvailableTime(NKMTime.UTCtoLocal(serverUTCTime)))
			{
				NKCUtil.SetLabelText(m_lbRemainTime, NKCUtilString.GET_STRING_CONTRACT_END_RECRUIT_TIME);
				m_bUpdateTime = false;
			}
			else
			{
				m_CurDateEndTime = templet.GetDateEndUtc();
				m_bUpdateTime = true;
				UpdateContractRemainTime();
				m_fDeltaTime = 0f;
			}
		}
		NKCUtil.SetGameobjectActive(m_objContractInfo, m_bUpdateTime || !string.IsNullOrEmpty(templet.ContractBannerDescID));
		NKCUtil.SetGameobjectActive(m_lbContractInfo, !string.IsNullOrEmpty(templet.ContractBannerDescID) && NKCSynchronizedTime.IsEventTime(templet.BannerDescIntervalTemplet));
		NKCUtil.SetLabelText(m_lbContractInfo, NKCStringTable.GetString(templet.ContractBannerDescID));
		NKCUtil.SetGameobjectActive(m_objRemainTime, m_bUpdateTime);
		NKCUtil.SetGameobjectActive(m_objSubTitle, !string.IsNullOrEmpty(templet.ContractBannerNameID));
		NKCUtil.SetLabelText(m_lbSubTitle, GetSubTitleDesc(templet));
		NKCUtil.SetGameobjectActive(m_btnContractPool.gameObject, templet is ContractTempletV2);
		m_SelectedContractID = templet.Key;
		(m_flTabs.GetMajorSlot(templet.Category) as NKCUIContractListSlot).SetImage(templet.ImageName);
		UpdateChildUI();
		NKCUtil.SetGameobjectActive(m_objDevTest, bValue: false);
		bool bValue = templet is ContractTempletV2 { m_NKM_UNIT_TYPE: NKM_UNIT_TYPE.NUT_OPERATOR } && NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.OPEN_TAG_RATE_INFO);
		NKCUtil.SetGameobjectActive(m_btnOperatorSubSkillPool, bValue);
		if (m_dicSubUI.ContainsKey(m_SelectedContractID) && m_dicSubUI[m_SelectedContractID].gameObject.activeSelf)
		{
			return;
		}
		foreach (NKCUIContractBanner value in m_dicSubUI.Values)
		{
			NKCUtil.SetGameobjectActive(value, bValue: false);
		}
		if (m_dicSubUI.ContainsKey(m_SelectedContractID))
		{
			NKCUtil.SetGameobjectActive(m_dicSubUI[m_SelectedContractID].gameObject, bValue: true);
		}
		else
		{
			NKCUIContractBanner nKCUIContractBanner = OpenInstanceByAssetName<NKCUIContractBanner>(templet.GetMainBannerName(), templet.GetMainBannerName(), m_trBannerParent);
			if (nKCUIContractBanner != null)
			{
				m_dicSubUI.Add(m_SelectedContractID, nKCUIContractBanner);
				nKCUIContractBanner.transform.localPosition = Vector3.zero;
				NKCUtil.SetGameobjectActive(nKCUIContractBanner, bValue: true);
				nKCUIContractBanner.SetActiveEventTag(bValue: false);
				nKCUIContractBanner.SetEnableAnimator(bValue: true);
			}
		}
		NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		if (nKCContractDataMgr != null)
		{
			m_CurLimitContractEndTimeUTC = nKCContractDataMgr.GetInstantContractEndDateTime(templet.Key);
			m_CurLimitContractEndTimeUTC = NKCSynchronizedTime.ToUtcTime(m_CurLimitContractEndTimeUTC);
			TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(m_CurLimitContractEndTimeUTC);
			if (!NKCSynchronizedTime.IsFinished(m_CurLimitContractEndTimeUTC) && timeLeft.Days < 365)
			{
				m_bLimitContractUpateTime = true;
				NKCUtil.SetGameobjectActive(m_objTimeLimitContract, bValue: true);
			}
			else
			{
				m_bLimitContractUpateTime = false;
				NKCUtil.SetGameobjectActive(m_objTimeLimitContract, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_csbtnCustomPickUpSelectUnit.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_CustomPickUpUnitSelectList.gameObject, bValue: false);
		UpdateShopPackageItem();
		UpdateUpsideMenu();
		UpdateButtonInfoUI();
	}

	public void SelectRecruitBanner(CustomPickupContractTemplet templet, bool bForceUpdate = false)
	{
		NKCUtil.SetGameobjectActive(m_csbtnCustomPickUpSelectUnit.gameObject, bValue: true);
		if (templet == null || (templet.Key == m_SelectedContractID && !bForceUpdate))
		{
			return;
		}
		m_flTabs.SelectMinorSlot(templet.Category, templet.Key);
		UpdateCustomPickUpSelectUI(templet);
		NKCSynchronizedTime.GetServerUTCTime(-1.0);
		if (NKCSynchronizedTime.GetTimeLeft(templet.EndDateUtc).TotalDays > (double)NKCSynchronizedTime.UNLIMITD_REMAIN_DAYS)
		{
			m_bUpdateTime = false;
		}
		else
		{
			if (templet.TriggeredContractTimeLimit > 0)
			{
				DateTime curDateEndTime = NKCSynchronizedTime.ToUtcTime(NKCScenManager.GetScenManager().GetNKCContractDataMgr().GetInstantContractEndDateTime(templet.Key));
				m_CurDateEndTime = curDateEndTime;
			}
			else
			{
				m_CurDateEndTime = templet.EndDateUtc;
			}
			if (templet.CheckReturningUser)
			{
				NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
				if (myUserData != null)
				{
					if (templet.ReturningUserStatePeriod == 0)
					{
						m_CurDateEndTime = myUserData.GetReturnEndDate(templet.ReturningUserType);
					}
					else
					{
						m_CurDateEndTime = myUserData.GetReturnStartDate(templet.ReturningUserType);
						m_CurDateEndTime = m_CurDateEndTime.AddDays(templet.ReturningUserStatePeriod);
					}
				}
			}
			m_bUpdateTime = true;
			UpdateContractRemainTime();
			m_fDeltaTime = 0f;
		}
		NKCUtil.SetGameobjectActive(m_objContractInfo, m_bUpdateTime || !string.IsNullOrEmpty(templet.m_ContractBannerDesc));
		NKCUtil.SetGameobjectActive(m_lbContractInfo, !string.IsNullOrEmpty(templet.m_ContractBannerDesc));
		NKCUtil.SetLabelText(m_lbContractInfo, NKCStringTable.GetString(templet.m_ContractBannerDesc));
		NKCUtil.SetGameobjectActive(m_objRemainTime, m_bUpdateTime);
		NKCUtil.SetGameobjectActive(m_objSubTitle, bValue: true);
		NKCUtil.SetGameobjectActive(m_objTimeLimitContract, bValue: false);
		UpdatePickupSubTitle(templet);
		NKCUtil.SetGameobjectActive(m_btnContractPool.gameObject, bValue: true);
		m_SelectedContractID = templet.Key;
		(m_flTabs.GetMajorSlot(templet.Category) as NKCUIContractListSlot).SetImage(templet.ImageName);
		UpdateChildUI();
		NKCUtil.SetGameobjectActive(m_objDevTest, bValue: false);
		NKCUtil.SetGameobjectActive(m_ShopItem, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnOperatorSubSkillPool, bValue: false);
		foreach (NKCUIContractBanner value in m_dicSubUI.Values)
		{
			NKCUtil.SetGameobjectActive(value, bValue: false);
		}
		if (m_dicSubUI.ContainsKey(m_SelectedContractID))
		{
			NKCUtil.SetGameobjectActive(m_dicSubUI[m_SelectedContractID].gameObject, bValue: true);
			m_dicSubUI[m_SelectedContractID].SetUnitData(templet.PickUpTargetUnitID);
		}
		else
		{
			NKCUIContractBanner nKCUIContractBanner = OpenInstanceByAssetName<NKCUIContractBanner>(templet.GetMainBannerName(), templet.GetMainBannerName(), m_trBannerParent);
			if (nKCUIContractBanner != null)
			{
				m_dicSubUI.Add(m_SelectedContractID, nKCUIContractBanner);
				nKCUIContractBanner.transform.localPosition = Vector3.zero;
				NKCUtil.SetGameobjectActive(nKCUIContractBanner, bValue: true);
				nKCUIContractBanner.SetActiveEventTag(bValue: false);
				nKCUIContractBanner.SetEnableAnimator(bValue: true);
				nKCUIContractBanner.SetUnitData(templet.PickUpTargetUnitID);
			}
		}
		UpdateShopPackageItem();
		UpdateUpsideMenu();
	}

	private string GetSubTitleDesc(ContractTempletBase templet)
	{
		if (!string.IsNullOrEmpty(templet.ContractBannerNameID))
		{
			StringBuilder stringBuilder = new StringBuilder();
			_ = string.Empty;
			if (templet is ContractTempletV2)
			{
				int key = 0;
				foreach (RandomUnitTempletV2 unitTemplet in (templet as ContractTempletV2).UnitPoolTemplet.UnitTemplets)
				{
					if (unitTemplet.PickUpTarget)
					{
						key = unitTemplet.UnitTemplet.m_UnitID;
						break;
					}
				}
				NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(key);
				if (nKMUnitTempletBase != null)
				{
					stringBuilder.Append(nKMUnitTempletBase.GetUnitName());
				}
			}
			return string.Format(NKCStringTable.GetString(templet.ContractBannerNameID), stringBuilder.ToString());
		}
		return "";
	}

	private void UpdatePickupSubTitle(CustomPickupContractTemplet templet)
	{
		string text = "";
		if (templet.IsSelectedPickUpUnit())
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(templet.PickUpTargetUnitID);
			text = string.Format(NKCUtilString.GET_STRING_CONTRACT_SUBTITLE_PERCENTAGE_TITLE_01, unitTempletBase?.GetUnitName());
		}
		else
		{
			text = string.Format(NKCUtilString.GET_STRING_CONTRACT_SUBTITLE_PERCENTAGE_TITLE_01, NKCUtilString.GET_STRING_CONTRACT_CUSTOM_CONTRACT_NON_SELECT_TARGET_UNIT_DESC);
		}
		NKCUtil.SetLabelText(m_lbSubTitle, text);
	}

	private void Update()
	{
		m_fDeltaTime += Time.deltaTime;
		if (m_fDeltaTime > 1f)
		{
			m_fDeltaTime -= 1f;
			if (m_bUpdateTime)
			{
				UpdateContractRemainTime();
			}
			if (m_bLimitContractUpateTime && !UpdateLimitContractRemainTime())
			{
				NKCUtil.SetGameobjectActive(m_objTimeLimitContract, bValue: false);
				m_bLimitContractUpateTime = false;
				ResetContractUI();
			}
			if (NKCSynchronizedTime.IsFinished(NKMTime.GetNextResetTime(m_LastUIOpenTimeUTC, NKM_MISSION_RESET_INTERVAL.DAILY)))
			{
				m_LastUIOpenTimeUTC = NKMTime.GetNextResetTime(m_LastUIOpenTimeUTC, NKM_MISSION_RESET_INTERVAL.DAILY);
				ResetContractUI();
			}
		}
	}

	private void OnSelectTab(int major, int minor)
	{
		SelectRecruitBanner(minor);
	}

	public void OnClickPool()
	{
		ContractTempletV2 contractTempletV = ContractTempletV2.Find(m_SelectedContractID);
		if (contractTempletV != null)
		{
			if (NKCSynchronizedTime.IsFinished(contractTempletV.GetDateEndUtc()) || !contractTempletV.IsAvailableTime(NKMTime.UTCtoLocal(NKCSynchronizedTime.GetServerUTCTime())))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCUtilString.GET_STRING_CONTRACT_POPUP_RATE_EVENT_TIME_OVER_01, contractTempletV.GetDateEnd()));
				return;
			}
			NKCUIContractPopupRateV2.Instance.Open(contractTempletV);
		}
		if (!IsCurContractCustomPickUp())
		{
			return;
		}
		CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(m_SelectedContractID);
		if (customPickupContractTemplet != null)
		{
			if (NKCSynchronizedTime.IsFinished(customPickupContractTemplet.EndDateUtc) || !customPickupContractTemplet.IsAvailableTime(NKMTime.UTCtoLocal(NKCSynchronizedTime.GetServerUTCTime())))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCUtilString.GET_STRING_CONTRACT_POPUP_RATE_EVENT_TIME_OVER_01, customPickupContractTemplet.EndDateUtc));
			}
			else
			{
				NKCUIContractPopupRateV2.Instance.Open(customPickupContractTemplet);
			}
		}
	}

	public void OnClickOperatorSubSkillPool()
	{
		NKCPopupOperatorSubSkillList.Instance.Open();
	}

	public void OnClickShortcut()
	{
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_SHOP, "TAB_EXCHANGE_TASK_PLANET");
	}

	private void OnClickFreeContract()
	{
		NKCPopupResourceConfirmBox.Instance.OpenForContract(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_CONTRACT_FREE_TRY_DESC, 0, 0, delegate
		{
			NKCPacketSender.Send_NKMPacket_CONTRACT_REQ(m_SelectedContractID, ContractCostType.FreeChance, 1);
		}, null, m_SelectedContractID, 1);
	}

	private void OnClickFreeContractMulti()
	{
		NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		if (m_iCurRemainFreeContractCount > 1)
		{
			NKCPopupResourceConfirmBox.Instance.OpenForContract(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCUtilString.GET_STRING_CONTRACT_FREE_02_TRY_DESC_01, m_iCurRemainFreeContractCount), 0, 0, delegate
			{
				NKCPacketSender.Send_NKMPacket_CONTRACT_REQ(m_SelectedContractID, ContractCostType.FreeChance, m_iCurRemainFreeContractCount);
			}, null, m_SelectedContractID, m_iCurRemainFreeContractCount);
		}
	}

	private void OnClickSelectContract()
	{
		ContractTempletBase contractTempletBase = ContractTempletBase.FindBase(m_SelectedContractID);
		if (contractTempletBase == null || !(contractTempletBase is SelectableContractTemplet))
		{
			return;
		}
		SelectableContractTemplet selectableTemplet = contractTempletBase as SelectableContractTemplet;
		if (selectableTemplet == null)
		{
			return;
		}
		bool flag = false;
		NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		if (nKCContractDataMgr != null && nKCContractDataMgr.GetSelectableContractChangeCnt(selectableTemplet.Key) > 0)
		{
			flag = true;
			NKCUIContractSelection.Instance.Open(nKCContractDataMgr.GetSelectableContractState());
		}
		if (!flag)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCUtilString.GET_STRING_SELECTABLE_CONTRACT_DESC, selectableTemplet.GetContractName(), selectableTemplet.m_UnitPoolChangeCount), delegate
			{
				NKCPacketSender.Send_NKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_REQ(selectableTemplet.Key);
			});
		}
	}

	private void OnClickConfirmation()
	{
		ContractTempletV2 contractTempletV = ContractTempletV2.Find(m_SelectedContractID);
		if (contractTempletV != null)
		{
			NKCPopupContractConfirmation.Instance.Open(contractTempletV);
		}
		CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(m_SelectedContractID);
		if (customPickupContractTemplet != null)
		{
			if (!customPickupContractTemplet.IsSelectedPickUpUnit())
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_CONTRACT_CUSTOM_CONTRACT_PICK_UP_WARNING_DESC);
			}
			else
			{
				NKCPopupContractConfirmation.Instance.Open(customPickupContractTemplet);
			}
		}
	}

	private void OnClickConfirmationTooltip(PointerEventData e)
	{
		string title = NKCUtilString.GET_STRING_CONTRACT_CONFIRM_TOOLTIP_TITLE;
		string desc = NKCUtilString.GET_STRING_CONTRACT_CONFIRM_TOOLTIP_DESC;
		CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(m_SelectedContractID);
		if (customPickupContractTemplet != null)
		{
			if (customPickupContractTemplet.CustomPickUpType == CustomPickupContractTemplet.CUSTOM_PICK_UP_TYPE.AWAKEN)
			{
				title = NKCUtilString.GET_STRING_CUSTOM_CONTRACT_CONFIRM_TOOLTIP_TITLE;
				desc = NKCUtilString.GET_STRING_CUSTOM_CONTRACT_AWAKEN_CONFIRM_TOOLTIP_DESC;
			}
			else
			{
				title = NKCUtilString.GET_STRING_CONTRACT_CUSTOM_CONTRACT_CONFIRM_TOOLTIP_TITLE;
				desc = NKCUtilString.GET_STRING_CONTRACT_CUSTOM_CONTRACT_CONFIRM_TOOLTIP_DESC;
			}
		}
		NKCUITooltip.Instance.Open(NKCUISlot.eSlotMode.Etc, title, desc, e.position);
	}

	private void OnClickChangeCustomPickUpUnit()
	{
		CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(m_SelectedContractID);
		if (customPickupContractTemplet != null)
		{
			if (!customPickupContractTemplet.CanSelectUnit())
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_CONTRACT_CUSTOM_CONTRACT_SELECT_BLOCK_DESC);
			}
			else if (customPickupContractTemplet.CurrentBonusCount > 0 && customPickupContractTemplet.CustomPickUpType != CustomPickupContractTemplet.CUSTOM_PICK_UP_TYPE.AWAKEN)
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_CONTRACT_CUSTOM_CONTRACT_RESET_WARNING_DESC, OpenUnitSelectList);
			}
			else
			{
				OpenUnitSelectList();
			}
		}
	}

	private void OpenUnitSelectList()
	{
		CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(m_SelectedContractID);
		if (customPickupContractTemplet == null)
		{
			return;
		}
		List<NKMUnitData> list = new List<NKMUnitData>();
		List<NKMOperator> list2 = new List<NKMOperator>();
		customPickupContractTemplet.UnitPoolTemplet.RecalculateUnitTemplets();
		foreach (RandomUnitTempletV2 unitTemplet in customPickupContractTemplet.UnitPoolTemplet.UnitTemplets)
		{
			if (unitTemplet.UnitTemplet != null && unitTemplet.CustomPickupTarget)
			{
				if (customPickupContractTemplet.CustomPickUpType == CustomPickupContractTemplet.CUSTOM_PICK_UP_TYPE.OPERATOR)
				{
					NKMOperator dummyOperator = NKCOperatorUtil.GetDummyOperator(NKMUnitManager.GetUnitTempletBase(unitTemplet.UnitTemplet.m_UnitID));
					list2.Add(dummyOperator);
				}
				else
				{
					NKMUnitData nKMUnitData = NKCUnitSortSystem.MakeTempUnitData(unitTemplet.UnitTemplet.m_UnitID, 1, 0);
					nKMUnitData.m_UnitUID = unitTemplet.UnitTemplet.Key;
					list.Add(nKMUnitData);
				}
			}
		}
		if (customPickupContractTemplet.CustomPickUpType == CustomPickupContractTemplet.CUSTOM_PICK_UP_TYPE.OPERATOR)
		{
			NKCOperatorSortSystem.OperatorListOptions operatorListOptions = new NKCOperatorSortSystem.OperatorListOptions
			{
				eDeckType = NKM_DECK_TYPE.NDT_NONE,
				bHideFilter = true,
				lstSortOption = new List<NKCOperatorSortSystem.eSortOption>
				{
					NKCOperatorSortSystem.eSortOption.Rarity_High,
					NKCOperatorSortSystem.eSortOption.UID_First
				}
			};
			NKCOperatorSortSystem operSortSystem = new NKCGenericOperatorSort(NKCScenManager.CurrentUserData(), operatorListOptions, list2);
			m_CustomPickUpUnitSelectList.Open(operSortSystem, operatorListOptions, customPickupContractTemplet.PickUpTargetUnitID);
		}
		else
		{
			NKCUnitSortSystem.UnitListOptions options = new NKCUnitSortSystem.UnitListOptions
			{
				eDeckType = NKM_DECK_TYPE.NDT_NONE,
				lstSortOption = new List<NKCUnitSortSystem.eSortOption>
				{
					NKCUnitSortSystem.eSortOption.Rarity_High,
					NKCUnitSortSystem.eSortOption.ID_First
				},
				bDescending = true,
				bIncludeUndeckableUnit = true
			};
			NKCUnitSortSystem sortSystem = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), options, list);
			m_CustomPickUpUnitSelectList.Open(sortSystem, customPickupContractTemplet.PickUpTargetUnitID);
		}
	}

	public void OnPickUpUnitClicked(int unitID, NKM_UNIT_TYPE eType)
	{
		if (m_dicSubUI.ContainsKey(m_SelectedContractID))
		{
			m_dicSubUI[m_SelectedContractID].SetUnitData(unitID);
		}
	}

	public void OnPickUpUnitConfirm(int unitID, NKM_UNIT_TYPE eType)
	{
		NKCPacketSender.Send_NKMPacket_CUSTOM_PICUP_SELECT_TARGET_REQ(m_SelectedContractID, unitID);
	}

	public void OnClosePickUpList()
	{
		if (m_CustomPickUpUnitSelectList.IsOpen)
		{
			m_CustomPickUpUnitSelectList.Close();
		}
		CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(m_SelectedContractID);
		if (customPickupContractTemplet == null)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(customPickupContractTemplet.PickUpTargetUnitID);
		if (m_dicSubUI.ContainsKey(m_SelectedContractID))
		{
			NKCUIContractBanner nKCUIContractBanner = m_dicSubUI[m_SelectedContractID];
			if (unitTempletBase == null)
			{
				nKCUIContractBanner.SetUnitData(0);
			}
			else
			{
				nKCUIContractBanner.SetUnitData(unitTempletBase.m_UnitID);
			}
		}
	}

	public static T OpenInstanceByAssetName<T>(string BundleName, string AssetName, Transform parent) where T : MonoBehaviour
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(BundleName, AssetName, bAsync: false, parent);
		if (nKCAssetInstanceData != null && nKCAssetInstanceData.m_Instant != null)
		{
			GameObject instant = nKCAssetInstanceData.m_Instant;
			T val = instant.GetComponent<T>();
			if (val == null)
			{
				val = instant.AddComponent<T>();
			}
			m_listNKCAssetResourceData.Add(nKCAssetInstanceData);
			return val;
		}
		Debug.LogWarning("prefab is null - " + BundleName + "/" + AssetName);
		return null;
	}

	public void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Contract);
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		base.OnInventoryChange(itemData);
		ResetContractUI();
		UpdateShopPackageItem();
	}
}
