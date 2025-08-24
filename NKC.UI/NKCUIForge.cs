using System.Collections.Generic;
using ClientPacket.Common;
using NKM;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForge : NKCUIBase
{
	public enum NKC_FORGE_TAB
	{
		NFT_ENCHANT,
		NFT_TUNING,
		NFT_HIDDEN_OPTION
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_factory";

	private const string UI_ASSET_NAME = "NKM_UI_FACTORY";

	private static NKCUIForge m_Instance;

	private readonly List<int> RESOURCE_LIST_FORGE = new List<int> { 1013, 1035, 1, 101 };

	private readonly List<int> RESOURCE_LIST_HIDDEN_OPTION = new List<int> { 1073, 1, 101 };

	private List<int> RESOURCE_LIST;

	private GameObject m_NUM_FORGE;

	public NKCUIInvenEquipSlot m_NKCUIInvenEquipSlot;

	private long m_LeftEquipUID;

	public NKCUIForgeEnchant m_NKCUIForgeEnchant;

	public NKCUIForgeTuning m_NKCUIForgeTuning;

	public NKCUIForgeHiddenOption m_NKCUIForgeHiddenOption;

	public Animator m_Animator;

	public Text m_NKM_UI_FACTORY_SUMMARY_TITLE;

	public Text m_NKM_UI_FACTORY_SUMMARY_NAME;

	public NKCUIComButton m_cbtn_NKM_UI_FACTORY_ENCHANT_BUTTON_CHANGE;

	public NKCUIFactoryShortCutMenu m_NKM_UI_FACTORY_SHORTCUT_MENU;

	private SkeletonGraphic m_SkeletonGraphic;

	private NKC_FORGE_TAB m_NKC_FORGE_TAB;

	[Space]
	public NKCUIComToggle m_tglLock;

	[Header("아이템 변경")]
	public NKCUIComStateButton m_NKM_UI_FACTORY_ARROW_RIGHT;

	public NKCUIComStateButton m_NKM_UI_FACTORY_ARROW_LEFT;

	public RectTransform m_rtNKM_UI_FACTORY_BACK;

	public GameObject m_NKM_UI_FACTORY_ENCHANT_CARD_BACK;

	public GameObject m_NKM_UI_FACTORY_LEFT;

	public GameObject m_NKM_UI_FACTORY_ENCHANT_BUTTON_CHANGE;

	public EventTrigger m_etNoTouchPanel;

	private static NKCAssetInstanceData m_AssetInstanceData;

	private const string ASSET_SELECT_BUNDLE_NAME = "ab_ui_nuf_base";

	private const string UI_SELECT_ASSET_NAME = "NKM_UI_BASE_UNIT_SELECT";

	private NKCUIUnitSelect m_NKCUIUnitSelect;

	private NKCUIInventory m_UIInventory;

	private HashSet<NKCEquipSortSystem.eFilterOption> m_setFilterOptions = new HashSet<NKCEquipSortSystem.eFilterOption>();

	private int m_iFilterSetOptionID;

	private bool m_bPlayingAni;

	private int enchantEffectSoundId = -1;

	private List<long> m_lstSelectedItem = new List<long>();

	public static NKCUIForge Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIForge>("ab_ui_nkm_ui_factory", "NKM_UI_FACTORY", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIForge>();
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

	public override string MenuName => m_NKC_FORGE_TAB switch
	{
		NKC_FORGE_TAB.NFT_ENCHANT => NKCUtilString.GET_STRING_FORGE, 
		NKC_FORGE_TAB.NFT_TUNING => NKCUtilString.GET_STRING_FORGE_TUNNING, 
		NKC_FORGE_TAB.NFT_HIDDEN_OPTION => NKCUtilString.GET_STRING_FACTORY_HIDDEN_OPTION_TITLE, 
		_ => "", 
	};

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string GuideTempletID
	{
		get
		{
			switch (m_NKC_FORGE_TAB)
			{
			case NKC_FORGE_TAB.NFT_ENCHANT:
				return "ARTICLE_EQUIP_ENCHANT";
			case NKC_FORGE_TAB.NFT_TUNING:
				if (m_NKCUIForgeTuning != null && m_NKCUIForgeTuning.GetCurTuningTab() == NKCUIForgeTuning.NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE)
				{
					return "ARTICLE_EQUIP_SET_CHANGE";
				}
				return "ARTICLE_EQUIP_TUNING";
			case NKC_FORGE_TAB.NFT_HIDDEN_OPTION:
				return "ARTICLE_EQUIP_HIDDEN_OPTION";
			default:
				return "";
			}
		}
	}

	public override List<int> UpsideMenuShowResourceList => RESOURCE_LIST;

	public NKCUIInventory Inventory => m_UIInventory;

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

	public NKC_FORGE_TAB GetCurTab()
	{
		return m_NKC_FORGE_TAB;
	}

	public void InitUI()
	{
		m_NUM_FORGE = NKCUIManager.OpenUI("NUM_FACTORY");
		m_NKCUIForgeEnchant.InitUI();
		m_NKCUIForgeTuning.InitUI(SelectEquipslot);
		m_NKCUIForgeHiddenOption.InitUI();
		m_SkeletonGraphic = GetComponentInChildren<SkeletonGraphic>();
		if (m_cbtn_NKM_UI_FACTORY_ENCHANT_BUTTON_CHANGE != null)
		{
			m_cbtn_NKM_UI_FACTORY_ENCHANT_BUTTON_CHANGE.PointerClick.RemoveAllListeners();
			m_cbtn_NKM_UI_FACTORY_ENCHANT_BUTTON_CHANGE.PointerClick.AddListener(OnChangeTargetEquip);
		}
		if (m_tglLock != null)
		{
			m_tglLock.OnValueChanged.RemoveAllListeners();
			m_tglLock.OnValueChanged.AddListener(OnChangedLock);
		}
		NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_ARROW_RIGHT, delegate
		{
			ChangeItem(bNext: true);
		});
		NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_ARROW_LEFT, delegate
		{
			ChangeItem(bNext: false);
		});
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate(BaseEventData eventData)
		{
			SkipAnimation(eventData);
		});
		m_etNoTouchPanel.triggers.Add(entry);
	}

	private void OpenSelectInstance()
	{
		m_AssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nuf_base", "NKM_UI_BASE_UNIT_SELECT");
		if (m_AssetInstanceData.m_Instant != null)
		{
			m_NKCUIUnitSelect = m_AssetInstanceData.m_Instant.GetComponent<NKCUIUnitSelect>();
			m_NKCUIUnitSelect.Init(ChangeTargetEquip);
			m_NKCUIUnitSelect.transform.SetParent(m_rtNKM_UI_FACTORY_BACK.transform, worldPositionStays: false);
			m_NKCUIUnitSelect.Open();
		}
	}

	public void CloseSelectInstance()
	{
		if (m_AssetInstanceData != null)
		{
			m_AssetInstanceData.Unload();
		}
	}

	public void Open(NKC_FORGE_TAB eNKC_FORGE_TAB, long equipUID = 0L, HashSet<NKCEquipSortSystem.eFilterOption> setFilterOptions = null, List<NKMEquipItemData> lstItemSortedList = null, int filterSetOptionID = 0)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_NUM_FORGE, bValue: true);
		if (setFilterOptions == null)
		{
			setFilterOptions = new HashSet<NKCEquipSortSystem.eFilterOption>();
		}
		m_setFilterOptions = setFilterOptions;
		m_iFilterSetOptionID = filterSetOptionID;
		if (NKCScenManager.CurrentUserData().hasReservedEquipCandidate())
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_TUNING_HAS_RESERVED_EQUIP_TUNING, MoveToReservedEquipTuning);
			return;
		}
		if (NKCScenManager.CurrentUserData().hasReservedHiddenOptionRerollData())
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_RELIC_REROLL_HAS_RESERVED_EQUIP_TUNING, MoveToReservedHiddenOptionReroll);
			return;
		}
		if (equipUID == 0L)
		{
			OpenSelectInstance();
			m_NKC_FORGE_TAB = eNKC_FORGE_TAB;
		}
		SetLeftEquip(equipUID);
		SetTab(eNKC_FORGE_TAB);
		NKCUtil.SetGameobjectActive(m_NKCUIInvenEquipSlot.gameObject, equipUID != 0);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_LEFT, equipUID != 0);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_ENCHANT_BUTTON_CHANGE, equipUID != 0);
		m_NKM_UI_FACTORY_SHORTCUT_MENU.OnConfirmBeforeChangeToCraft(ConfirmExitTuning);
		m_NKM_UI_FACTORY_SHORTCUT_MENU.SetData(eNKC_FORGE_TAB);
		SetUpsideResource(eNKC_FORGE_TAB);
		if (lstItemSortedList != null && lstItemSortedList.Count > 0)
		{
			OnGetItemListAfterSelected(lstItemSortedList);
		}
		else
		{
			UpdateSelectedItemListToUnitEquip();
		}
		SetActiveChangeItemArrow(m_lstSelectedItem.Count > 1);
		UIOpened();
	}

	public void SetUpsideResource(NKC_FORGE_TAB forgeTab)
	{
		switch (forgeTab)
		{
		case NKC_FORGE_TAB.NFT_ENCHANT:
			RESOURCE_LIST = base.UpsideMenuShowResourceList;
			break;
		case NKC_FORGE_TAB.NFT_TUNING:
			RESOURCE_LIST = RESOURCE_LIST_FORGE;
			break;
		case NKC_FORGE_TAB.NFT_HIDDEN_OPTION:
			RESOURCE_LIST = RESOURCE_LIST_HIDDEN_OPTION;
			break;
		}
		NKCUIManager.UpdateUpsideMenu();
	}

	private void MoveToReservedEquipTuning()
	{
		NKMEquipTuningCandidate tuiningData = NKCScenManager.CurrentUserData().GetTuiningData();
		SetTab(NKC_FORGE_TAB.NFT_TUNING);
		SetLeftEquip(tuiningData.equipUid, bForce: false, bMoveToTabBeingTuned: true);
		NKCUtil.SetGameobjectActive(m_NKCUIInvenEquipSlot.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_LEFT, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_ENCHANT_BUTTON_CHANGE, bValue: true);
		m_NKM_UI_FACTORY_SHORTCUT_MENU.OnConfirmBeforeChangeToCraft(ConfirmExitTuning);
		m_NKM_UI_FACTORY_SHORTCUT_MENU.SetData(NKC_FORGE_TAB.NFT_TUNING);
		SetUpsideResource(NKC_FORGE_TAB.NFT_TUNING);
		UpdateSelectedItemListToUnitEquip();
		SetActiveChangeItemArrow(m_lstSelectedItem.Count > 1);
		UIOpened();
	}

	private void MoveToReservedHiddenOptionReroll()
	{
		NKMPotentialOptionChangeCandidate potentialData = NKCScenManager.CurrentUserData().GetPotentialData();
		SetTab(NKC_FORGE_TAB.NFT_HIDDEN_OPTION);
		m_NKCUIForgeHiddenOption.SetCurTab(NKCUIForgeHiddenOption.HIDDEN_OPTION_UI_TYPE.REROLL);
		SetLeftEquip(potentialData.equipUid, bForce: false, bMoveToTabBeingTuned: true);
		NKCUtil.SetGameobjectActive(m_NKCUIInvenEquipSlot.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_LEFT, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_ENCHANT_BUTTON_CHANGE, bValue: true);
		m_NKM_UI_FACTORY_SHORTCUT_MENU.OnConfirmBeforeChangeToCraft(ConfirmExitTuning);
		m_NKM_UI_FACTORY_SHORTCUT_MENU.SetData(NKC_FORGE_TAB.NFT_HIDDEN_OPTION);
		SetUpsideResource(NKC_FORGE_TAB.NFT_HIDDEN_OPTION);
		UpdateSelectedItemListToUnitEquip();
		SetActiveChangeItemArrow(m_lstSelectedItem.Count > 1);
		UIOpened();
	}

	public void SetTab(NKC_FORGE_TAB eNKC_FORGE_TAB)
	{
		switch (eNKC_FORGE_TAB)
		{
		case NKC_FORGE_TAB.NFT_ENCHANT:
			m_NKCUIForgeTuning.SetOut();
			m_NKCUIForgeEnchant.AnimateOutToIn();
			m_NKCUIForgeHiddenOption.SetOut();
			m_NKCUIInvenEquipSlot.SetHighlightOnlyOneStatColor(0);
			m_NKCUIForgeEnchant.ResetMaterialEquipSlotsToEnhance();
			break;
		case NKC_FORGE_TAB.NFT_TUNING:
			m_NKCUIForgeTuning.AnimateOutToIn();
			m_NKCUIForgeEnchant.SetOut();
			m_NKCUIForgeHiddenOption.SetOut();
			m_NKCUIForgeTuning.ResetUI();
			m_NKCUIInvenEquipSlot.SetHighlightOnlyOneStatColor(m_NKCUIForgeTuning.GetSelectOption());
			break;
		case NKC_FORGE_TAB.NFT_HIDDEN_OPTION:
			m_NKCUIForgeTuning.SetOut();
			m_NKCUIForgeEnchant.SetOut();
			m_NKCUIForgeHiddenOption.AnimateOutToIn();
			m_NKCUIForgeHiddenOption.SetEnchantCard(m_NKM_UI_FACTORY_ENCHANT_CARD_BACK);
			m_NKCUIForgeHiddenOption.SetUI();
			m_NKCUIInvenEquipSlot.SetHighlightOnlyOneStatColor(3);
			break;
		}
		m_NKC_FORGE_TAB = eNKC_FORGE_TAB;
		SetUpsideResource(m_NKC_FORGE_TAB);
		if (m_LeftEquipUID == 0L)
		{
			ClearEquip();
		}
		PlaySpineAni();
		TutorialCheck();
	}

	private void SelectEquipslot(int idx)
	{
		m_NKCUIInvenEquipSlot.SetHighlightOnlyOneStatColor(idx);
	}

	public override void Hide()
	{
		base.Hide();
		NKCUtil.SetGameobjectActive(m_NUM_FORGE, bValue: false);
	}

	public override void OnBackButton()
	{
		if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedEquipTuningData())
		{
			CheckTuningExitConfirm(delegate
			{
				base.OnBackButton();
			});
		}
		else if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedSetOptionData())
		{
			CheckSetOptionExitConfirm(delegate
			{
				base.OnBackButton();
			});
		}
		else
		{
			if (!IsHiddenOptionEffectStopped())
			{
				return;
			}
			if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedHiddenOptionRerollData())
			{
				CheckHiddenOptionExitConfirm(delegate
				{
					base.OnBackButton();
				});
			}
			else
			{
				base.OnBackButton();
			}
		}
	}

	public override bool OnHomeButton()
	{
		if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedEquipTuningData())
		{
			CheckTuningExitConfirm(delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			return false;
		}
		if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedSetOptionData())
		{
			CheckSetOptionExitConfirm(delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			return false;
		}
		if (!IsHiddenOptionEffectStopped())
		{
			return false;
		}
		if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedHiddenOptionRerollData())
		{
			CheckHiddenOptionExitConfirm(delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			return false;
		}
		return base.OnHomeButton();
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_TUNING && itemData != null && m_NKCUIForgeTuning != null && itemData.GetTemplet().m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_MISC && m_NKCUIForgeTuning.GetCurTuningTab() != NKCUIForgeTuning.NKC_TUNING_TAB.NTT_PRECISION)
		{
			m_NKCUIForgeTuning.UpdateRequireItemUI();
		}
	}

	private void ConfirmExitTuning()
	{
		if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedEquipTuningData())
		{
			CheckTuningExitConfirm(delegate
			{
				m_NKM_UI_FACTORY_SHORTCUT_MENU.MoveToCraft();
			}, delegate
			{
				m_NKM_UI_FACTORY_SHORTCUT_MENU.SetData(m_NKC_FORGE_TAB);
			});
		}
		else if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedSetOptionData())
		{
			CheckSetOptionExitConfirm(delegate
			{
				m_NKM_UI_FACTORY_SHORTCUT_MENU.MoveToCraft();
			}, delegate
			{
				m_NKM_UI_FACTORY_SHORTCUT_MENU.SetData(m_NKC_FORGE_TAB);
			});
		}
		else if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedHiddenOptionRerollData())
		{
			CheckHiddenOptionExitConfirm(delegate
			{
				m_NKM_UI_FACTORY_SHORTCUT_MENU.MoveToCraft();
			}, delegate
			{
				m_NKM_UI_FACTORY_SHORTCUT_MENU.SetData(m_NKC_FORGE_TAB);
			});
		}
		else
		{
			m_NKM_UI_FACTORY_SHORTCUT_MENU.MoveToCraft();
		}
	}

	private void CheckTuningExitConfirm(UnityAction OK, UnityAction CANCEL = null)
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_TUNING_EXIT_CONFIRM, delegate
		{
			NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
			OK?.Invoke();
		}, delegate
		{
			CANCEL?.Invoke();
		});
	}

	private void CheckSetOptionExitConfirm(UnityAction OK, UnityAction CANCEL = null)
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_SET_OPTION_TUNING_EXIT_CONFIRM, delegate
		{
			NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
			OK?.Invoke();
		}, delegate
		{
			CANCEL?.Invoke();
		});
	}

	private void CheckHiddenOptionExitConfirm(UnityAction OK, UnityAction CANCEL = null)
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_RELIC_REROLL_EXIT_CONFIRM, delegate
		{
			NKCScenManager.CurrentUserData().SetEquipPotentialData(new NKMPotentialOptionChangeCandidate());
			NKCPacketSender.Send_NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CANCLE_REQ();
			OK?.Invoke();
		}, delegate
		{
			CANCEL?.Invoke();
		});
	}

	public override void UnHide()
	{
		base.UnHide();
		NKCUtil.SetGameobjectActive(m_NUM_FORGE, bValue: true);
		PlaySpineAni();
		if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_ENCHANT)
		{
			m_NKCUIForgeEnchant?.AnimateOutToIn();
		}
		else if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_TUNING)
		{
			m_NKCUIForgeTuning?.AnimateOutToIn();
			if (m_NKCUIForgeTuning != null && m_NKCUIForgeTuning.GetCurTuningTab() == NKCUIForgeTuning.NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE)
			{
				m_NKCUIForgeTuning.ResetUI();
			}
		}
		else if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_HIDDEN_OPTION && m_NKCUIForgeHiddenOption != null)
		{
			m_NKCUIForgeHiddenOption.AnimateOutToIn();
			m_NKCUIForgeHiddenOption.SetEnchantCard(m_NKM_UI_FACTORY_ENCHANT_CARD_BACK);
			m_NKCUIForgeHiddenOption.SetUI();
		}
	}

	public void ResetEquipEnhanceUI()
	{
		SetLeftEquip(m_LeftEquipUID);
	}

	public void ResetUI()
	{
		SetLeftEquip(m_LeftEquipUID, bForce: false);
	}

	public void DoAfterRefine(NKMEquipItemData orgData, int changedSlotNum)
	{
		ResetUI();
		if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_TUNING)
		{
			m_NKCUIForgeTuning.DoAfterRefine(orgData, changedSlotNum);
		}
	}

	public void DoAfterOptionChanged(int selectedSlot)
	{
		if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_TUNING && m_NKCUIForgeTuning.GetCurTuningTab() == NKCUIForgeTuning.NKC_TUNING_TAB.NTT_OPTION_CHANGE)
		{
			m_NKCUIForgeTuning.SetEffect(1, selectedSlot);
		}
	}

	public void DoAfterOptionChangedConfirm()
	{
		if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_TUNING && m_NKCUIForgeTuning.GetCurTuningTab() == NKCUIForgeTuning.NKC_TUNING_TAB.NTT_OPTION_CHANGE)
		{
			m_NKCUIForgeTuning.SetEffect(3);
		}
	}

	public void DoAfterSetOptionChanged()
	{
		if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_TUNING && m_NKCUIForgeTuning.GetCurTuningTab() == NKCUIForgeTuning.NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE)
		{
			m_NKCUIForgeTuning.SetEffect(2);
		}
	}

	public void DoAfterSetOptionChangeConfirm()
	{
		if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_TUNING && m_NKCUIForgeTuning.GetCurTuningTab() == NKCUIForgeTuning.NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE)
		{
			m_NKCUIForgeTuning.SetEffect(4);
		}
	}

	public void DoAfterPotentialChanged()
	{
		if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_HIDDEN_OPTION && m_NKCUIForgeHiddenOption.GetCurTab() == NKCUIForgeHiddenOption.HIDDEN_OPTION_UI_TYPE.REROLL)
		{
			m_NKCUIForgeHiddenOption.SetEffect(NKCUIForgeHiddenOption.HIDDEN_OPTION_CHANGE_EFFECT_TYPE.CHANGE);
		}
	}

	public void DoAfterPotentialChangedConfirm()
	{
		if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_HIDDEN_OPTION && m_NKCUIForgeHiddenOption.GetCurTab() == NKCUIForgeHiddenOption.HIDDEN_OPTION_UI_TYPE.REROLL)
		{
			m_NKCUIForgeHiddenOption.SetEffect(NKCUIForgeHiddenOption.HIDDEN_OPTION_CHANGE_EFFECT_TYPE.CHANGE_CONFIRM);
		}
	}

	public void PlayEnhanceEffect()
	{
		m_NKCUIForgeEnchant.PlayEnhanceEffect();
		if (m_Animator != null)
		{
			m_bPlayingAni = true;
			enchantEffectSoundId = NKCSoundManager.PlaySound("FX_UI_UNIT_ENCHANT_START", 1f, 0f, 0f);
			m_Animator.Play("ENHANCE");
		}
	}

	public void SkipAnimation(BaseEventData cBaseEventData)
	{
		m_NKCUIForgeEnchant.ClearEnhanceEffect();
		if (m_Animator != null)
		{
			if (enchantEffectSoundId >= 0)
			{
				NKCSoundManager.StopSound(enchantEffectSoundId);
				enchantEffectSoundId = -1;
			}
			ResetEquipEnhanceUI();
			m_Animator.Play("BASE");
			m_bPlayingAni = false;
		}
	}

	private void ClearEquip()
	{
		PlaySpineAni();
		m_NKCUIInvenEquipSlot.SetEmpty();
		if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_ENCHANT)
		{
			m_NKCUIForgeEnchant.ClearAllUI();
		}
		else
		{
			m_NKCUIForgeTuning.ClearAllUI();
		}
	}

	private void SetLeftEquip(long equipUID, bool bForce = true, bool bMoveToTabBeingTuned = false)
	{
		m_LeftEquipUID = equipUID;
		if (equipUID == 0L)
		{
			ClearEquip();
			if (m_tglLock != null)
			{
				m_tglLock.Lock();
			}
			return;
		}
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip != null)
		{
			m_NKCUIForgeTuning.SetLeftEquipUID(equipUID);
			m_NKCUIForgeEnchant.SetLeftEquipUID(equipUID);
			m_NKCUIForgeHiddenOption.SetLeftEquipUID(equipUID);
			m_NKCUIInvenEquipSlot.SetData(itemEquip);
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet != null)
			{
				m_NKM_UI_FACTORY_SUMMARY_TITLE.text = NKCUtilString.GetEquipPositionStringByUnitStyle(equipTemplet);
				m_NKM_UI_FACTORY_SUMMARY_NAME.text = NKCUtilString.GetItemEquipNameWithTier(equipTemplet);
			}
			if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_ENCHANT)
			{
				m_NKCUIForgeEnchant.ResetMaterialEquipSlotsToEnhance();
			}
			else if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_HIDDEN_OPTION)
			{
				m_NKCUIForgeHiddenOption.SetUI();
			}
			else
			{
				m_NKCUIForgeTuning.ResetUI(bForce, bMoveToTabBeingTuned);
			}
			if (m_tglLock != null)
			{
				m_tglLock.UnLock();
				m_tglLock.Select(itemEquip.m_bLock, bForce: true);
			}
		}
	}

	public override void CloseInternal()
	{
		m_bPlayingAni = false;
		m_LeftEquipUID = 0L;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_FORGE, bValue: false);
		if (m_UIInventory != null && m_UIInventory.IsOpen)
		{
			m_UIInventory.Close();
		}
		m_NKCUIForgeTuning.Close();
		m_NKCUIForgeEnchant.Close();
		m_NKCUIForgeHiddenOption.Close();
		m_UIInventory = null;
		CloseSelectInstance();
	}

	public override void OnCloseInstance()
	{
		base.OnCloseInstance();
		m_NKCUIForgeEnchant?.OnCloseInstance();
		if (m_UIInventory != null && m_UIInventory.IsOpen)
		{
			m_UIInventory.Close();
		}
		m_UIInventory = null;
	}

	public bool IsInventoryInstanceOpen()
	{
		if (m_UIInventory != null)
		{
			return m_UIInventory.IsOpen;
		}
		return false;
	}

	private void OnChangeTargetEquip()
	{
		if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedSetOptionData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_SET_OPTION_TUNING_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
				ChangeTargetEquip();
			});
		}
		else if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedEquipTuningData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_TUNING_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
				ChangeTargetEquip();
			});
		}
		else if (NKCScenManager.CurrentUserData().hasReservedHiddenOptionRerollData())
		{
			CheckHiddenOptionExitConfirm(delegate
			{
				ChangeTargetEquip();
			});
		}
		else
		{
			ChangeTargetEquip();
		}
	}

	public void ChangeTargetEquip()
	{
		if (!IsHiddenOptionEffectStopped())
		{
			return;
		}
		NKCUIInventory.EquipSelectListOptions options = new NKCUIInventory.EquipSelectListOptions(NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL, _bMultipleSelect: false);
		options.lstSortOption = NKCEquipSortSystem.FORGE_TARGET_SORT_LIST;
		options.m_NKC_INVENTORY_OPEN_TYPE = NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_SELECT;
		options.m_dOnSelectedEquipSlot = OnSelectedEquipSlotForTarget;
		options.m_dOnGetItemListAfterSelected = OnGetItemListAfterSelected;
		if (m_UIInventory == null)
		{
			m_UIInventory = NKCUIInventory.OpenNewInstance();
		}
		if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_ENCHANT)
		{
			options.strEmptyMessage = NKCUtilString.GET_STRING_NO_EXIST_ENCHANT_EQUIP;
			options.bLockMaxItem = true;
			options.bHideMaxLvItem = true;
		}
		else if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_TUNING)
		{
			if (m_NKCUIForgeTuning.GetCurTuningTab() == NKCUIForgeTuning.NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE)
			{
				options.m_EquipListOptions.bHideNotPossibleSetOptionItem = true;
			}
			options.strEmptyMessage = NKCUtilString.GET_STRING_NO_EXIST_TUNING_EQUIP;
			options.bLockMaxItem = false;
			options.bHideMaxLvItem = false;
		}
		else if (m_NKC_FORGE_TAB == NKC_FORGE_TAB.NFT_HIDDEN_OPTION)
		{
			options.strEmptyMessage = NKCUtilString.GET_STRING_NO_EXIST_HIDDEN_OPTION_EQUIP;
			if (m_NKCUIForgeHiddenOption.GetCurTab() == NKCUIForgeHiddenOption.HIDDEN_OPTION_UI_TYPE.LIST)
			{
				options.m_EquipListOptions.AdditionalExcludeFilterFunc = IsRelicOpenTarget;
			}
			else if (m_NKCUIForgeHiddenOption.GetCurTab() == NKCUIForgeHiddenOption.HIDDEN_OPTION_UI_TYPE.REROLL)
			{
				options.m_EquipListOptions.AdditionalExcludeFilterFunc = NKMItemManager.IsRelicRerollTarget;
			}
			options.bLockMaxItem = false;
			options.bHideMaxLvItem = false;
		}
		else
		{
			options.strEmptyMessage = "";
		}
		options.bHideLockItem = false;
		options.m_EquipListOptions.setExcludeFilterOption = new HashSet<NKCEquipSortSystem.eFilterOption> { NKCEquipSortSystem.eFilterOption.Equip_Enchant };
		options.m_ButtonMenuType = NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_OK;
		options.setFilterOption = m_setFilterOptions;
		options.m_EquipListOptions.FilterSetOptionID = m_iFilterSetOptionID;
		m_UIInventory?.Open(options, RESOURCE_LIST, 0L);
		m_UIInventory?.SetOptionChangeNotifyFunc(OnChangeFilterOption);
	}

	public bool IsHiddenOptionEffectStopped()
	{
		if (m_NKCUIForgeHiddenOption == null)
		{
			return true;
		}
		return m_NKCUIForgeHiddenOption.IsEffectStopped();
	}

	private bool IsRelicOpenTarget(NKMEquipItemData equipData)
	{
		if (NKMItemManager.GetEquipTemplet(equipData.m_ItemEquipID).IsRelic())
		{
			if (equipData.potentialOptions.Count <= 0 || equipData.potentialOptions[0] == null)
			{
				return true;
			}
			bool flag = false;
			int num = equipData.potentialOptions[0].sockets.Length;
			for (int i = 0; i < num; i++)
			{
				if (equipData.potentialOptions[0].sockets[i] == null)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	private void OnChangedLock(bool bValue)
	{
		if (m_LeftEquipUID > 0)
		{
			NKCPacketSender.Send_NKMPacket_LOCK_ITEM_REQ(m_LeftEquipUID, bValue);
		}
	}

	private void OnSelectedEquipSlotForTarget(NKCUISlotEquip slot, NKMEquipItemData equipData)
	{
		if (equipData == null)
		{
			return;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = NKMItemManager.CanEnchantItem(NKCScenManager.GetScenManager().GetMyUserData(), equipData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
			return;
		}
		if (m_UIInventory != null && m_UIInventory.IsOpen)
		{
			m_setFilterOptions = m_UIInventory.GetNKCUIInventoryOption().setFilterOption;
			m_UIInventory.Close();
		}
		SetLeftEquip(equipData.m_ItemUid);
		if (m_NKCUIUnitSelect != null)
		{
			m_NKCUIUnitSelect.Close();
		}
		NKCUtil.SetGameobjectActive(m_NKCUIInvenEquipSlot.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_LEFT, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_ENCHANT_BUTTON_CHANGE, bValue: true);
		SetActiveChangeItemArrow(m_lstSelectedItem.Count > 1);
		PlaySpineAni();
	}

	private void OnChangeFilterOption(HashSet<NKCEquipSortSystem.eFilterOption> setFilterOptions = null, int filterSetOptionID = 0)
	{
		m_setFilterOptions = setFilterOptions;
		m_iFilterSetOptionID = filterSetOptionID;
	}

	private void UpdateSelectedItemListToUnitEquip()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKMEquipItemData itemEquip = nKMUserData.m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip == null || itemEquip.m_OwnerUnitUID <= 0)
		{
			return;
		}
		NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(itemEquip.m_OwnerUnitUID);
		if (unitFromUID != null)
		{
			m_lstSelectedItem.Clear();
			if (unitFromUID.GetEquipItemWeaponUid() > 0)
			{
				m_lstSelectedItem.Add(unitFromUID.GetEquipItemWeaponUid());
			}
			if (unitFromUID.GetEquipItemDefenceUid() > 0)
			{
				m_lstSelectedItem.Add(unitFromUID.GetEquipItemDefenceUid());
			}
			if (unitFromUID.GetEquipItemAccessoryUid() > 0)
			{
				m_lstSelectedItem.Add(unitFromUID.GetEquipItemAccessoryUid());
			}
			if (unitFromUID.GetEquipItemAccessory2Uid() > 0)
			{
				m_lstSelectedItem.Add(unitFromUID.GetEquipItemAccessory2Uid());
			}
		}
	}

	private void OnGetItemListAfterSelected(List<NKMEquipItemData> lstItemData)
	{
		m_lstSelectedItem.Clear();
		foreach (NKMEquipItemData lstItemDatum in lstItemData)
		{
			m_lstSelectedItem.Add(lstItemDatum.m_ItemUid);
		}
	}

	private void PlaySpineAni()
	{
		NKCSoundManager.PlaySound("FX_UI_FACTORY_WORK_START", 1f, 0f, 0f);
		m_SkeletonGraphic.AnimationState.ClearTrack(0);
		SetSpineAnimation(0, "BASE", bLoop: false, bSet: true);
		SetSpineAnimation(0, "BASE_LOOP", bLoop: true, bSet: false);
	}

	private void SetSpineAnimation(int trackID, string animName, bool bLoop, bool bSet)
	{
		if (m_SkeletonGraphic == null || m_SkeletonGraphic.SkeletonData == null || m_SkeletonGraphic.SkeletonData.FindAnimation(animName) == null)
		{
			return;
		}
		if (bSet)
		{
			m_SkeletonGraphic.AnimationState.SetAnimation(trackID, animName, bLoop);
			return;
		}
		Spine.Animation animation = m_SkeletonGraphic.SkeletonData.FindAnimation("BASE");
		if (animation != null)
		{
			m_SkeletonGraphic.AnimationState.AddAnimation(trackID, animName, bLoop, animation.Duration);
		}
	}

	public void OnSelectPotentialSlot(int socketIndex)
	{
		if (m_NKC_FORGE_TAB != NKC_FORGE_TAB.NFT_HIDDEN_OPTION || m_NKCUIForgeHiddenOption.GetCurTab() != NKCUIForgeHiddenOption.HIDDEN_OPTION_UI_TYPE.REROLL)
		{
			return;
		}
		if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedHiddenOptionRerollData())
		{
			CheckHiddenOptionExitConfirm(delegate
			{
				m_NKCUIForgeHiddenOption.SetSelectedSocketIndex(socketIndex);
				m_NKCUIForgeHiddenOption.SetUI();
			});
		}
		else
		{
			m_NKCUIForgeHiddenOption.SelectRerollSlot(socketIndex);
		}
	}

	private void ChangeItem(bool bNext)
	{
		if (m_LeftEquipUID == 0L || !IsHiddenOptionEffectStopped())
		{
			return;
		}
		int num = m_lstSelectedItem.FindIndex((long x) => x == m_LeftEquipUID);
		if (num < 0 || num > m_lstSelectedItem.Count)
		{
			return;
		}
		int targetIndex = -1;
		if (bNext)
		{
			if (m_lstSelectedItem.Count <= num + 1)
			{
				targetIndex = 0;
			}
			else
			{
				targetIndex = num + 1;
			}
		}
		else if (num == 0)
		{
			targetIndex = m_lstSelectedItem.Count - 1;
		}
		else
		{
			targetIndex = num - 1;
		}
		if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedEquipTuningData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_TUNING_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
				SetLeftEquip(m_lstSelectedItem[targetIndex]);
			});
		}
		else if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedSetOptionData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_SET_OPTION_TUNING_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
				SetLeftEquip(m_lstSelectedItem[targetIndex]);
			});
		}
		else if (NKCScenManager.CurrentUserData().hasReservedHiddenOptionRerollData())
		{
			CheckHiddenOptionExitConfirm(delegate
			{
				SetLeftEquip(m_lstSelectedItem[targetIndex]);
			});
		}
		else
		{
			SetLeftEquip(m_lstSelectedItem[targetIndex]);
		}
	}

	private void SetActiveChangeItemArrow(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_ARROW_RIGHT.gameObject, bActive);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_ARROW_LEFT.gameObject, bActive);
	}

	private void TutorialCheck()
	{
		switch (m_NKC_FORGE_TAB)
		{
		case NKC_FORGE_TAB.NFT_ENCHANT:
			NKCTutorialManager.TutorialRequired(TutorialPoint.FactoryEnchant);
			break;
		case NKC_FORGE_TAB.NFT_TUNING:
			NKCTutorialManager.TutorialRequired(TutorialPoint.FactoryTuning);
			break;
		case NKC_FORGE_TAB.NFT_HIDDEN_OPTION:
			NKCTutorialManager.TutorialRequired(TutorialPoint.FactoryHiddenOption);
			break;
		}
	}

	private void Update()
	{
		if (m_bPlayingAni && Input.anyKeyDown)
		{
			m_bPlayingAni = false;
			SkipAnimation(null);
		}
	}
}
