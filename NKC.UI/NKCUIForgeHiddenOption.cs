using System.Collections;
using System.Collections.Generic;
using ClientPacket.Common;
using NKC.FX;
using NKM;
using NKM.Contract2;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForgeHiddenOption : MonoBehaviour
{
	public enum HIDDEN_OPTION_UI_TYPE
	{
		LIST,
		REROLL
	}

	public enum HIDDEN_OPTION_CHANGE_EFFECT_TYPE
	{
		CHANGE,
		CHANGE_CONFIRM
	}

	public delegate void OnExplosionFxActivated();

	public NKCUIComToggle m_tglList;

	public NKCUIComToggle m_tglReroll;

	public NKCUIRectMove m_rectMove;

	public GameObject m_objOptionList;

	public GameObject m_objOptionListOnly;

	public GameObject m_objOptionRerollOnly;

	public GameObject m_objDisable;

	public Text m_lbDisable;

	public GameObject m_objEmpty;

	public Text m_lbEmpty;

	public GameObject m_objResource;

	public GameObject m_objRerollButtons;

	public NKCUIComStateButton m_btnReroll;

	public NKCUIComStateButton m_btnRerollConfirm;

	public NKCUIComStateButton m_csbtnUnlockOption;

	public NKCUIComStateButton m_csbtnOptionInfo;

	public NKCUIComStateButton m_csbtnRerollInfo;

	public Text m_lbResult;

	public Text m_lbResultDetail;

	[Header("개방 옵션 슬롯")]
	public NKCUIForgeHiddenOptionSlot[] m_HiddenOptionSlotArray;

	public int[] m_socketEnchantLv;

	[Header("개방 비용 슬롯")]
	public Text m_lbCostTitle;

	public NKCUIItemCostSlot[] m_itemCostSlotArray;

	[Header("개방 이펙트")]
	public GameObject m_objFxRoot;

	public Transform m_explosionFx;

	public GameObject[] m_objSocketUnlockFx;

	[Header("이펙트 활성화 체크")]
	public NKC_FXM_PLAYER[] m_fxActiveCheck;

	[Header("리롤 결과")]
	public NKCUIForgeHiddenOptionSlot m_slotRerollResult;

	private long m_LeftEquipUID;

	private int m_iOpenedSocketCount;

	private GameObject m_objEnchantCard;

	private NKC_FXM_PLAYER m_fxExplosionPlayer;

	private Coroutine m_fxExplosionCoroutine;

	private bool m_bUnlockingSocket;

	private OnExplosionFxActivated m_dOnExplosionFxActivated;

	private HIDDEN_OPTION_UI_TYPE m_curUIType;

	private int m_SelectedSocketIndex = -1;

	public bool UnlockingSocket
	{
		set
		{
			m_bUnlockingSocket = value;
		}
	}

	public HIDDEN_OPTION_UI_TYPE GetCurTab()
	{
		return m_curUIType;
	}

	public void SetCurTab(HIDDEN_OPTION_UI_TYPE tabType)
	{
		m_curUIType = tabType;
	}

	public void SetSelectedSocketIndex(int socketIndex)
	{
		m_SelectedSocketIndex = socketIndex;
	}

	public void InitUI()
	{
		if (m_tglList != null)
		{
			m_tglList.OnValueChanged.RemoveAllListeners();
			m_tglList.OnValueChanged.AddListener(OnTglList);
		}
		if (m_tglReroll != null)
		{
			m_tglReroll.OnValueChanged.RemoveAllListeners();
			m_tglReroll.OnValueChanged.AddListener(OnTglReroll);
			if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.EQUIP_POTENTIAL_PRECISION_CHANGE))
			{
				m_tglReroll.Lock();
			}
			else
			{
				m_tglReroll.UnLock();
			}
		}
		NKCUtil.SetButtonClickDelegate(m_btnReroll, OnClickReroll);
		m_btnReroll.m_bGetCallbackWhileLocked = true;
		NKCUtil.SetButtonClickDelegate(m_btnRerollConfirm, OnClickRerollConfirm);
		NKCUtil.SetButtonClickDelegate(m_csbtnUnlockOption, OnClickUnlockOption);
		NKCUtil.SetHotkey(m_csbtnUnlockOption, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_csbtnOptionInfo, OnClickOptionInfo);
		NKCUtil.SetButtonClickDelegate(m_csbtnRerollInfo, OnClickRerollInfo);
		if (m_HiddenOptionSlotArray != null)
		{
			int num = m_HiddenOptionSlotArray.Length;
			for (int i = 0; i < num; i++)
			{
				m_HiddenOptionSlotArray[i].Init();
			}
		}
		m_bUnlockingSocket = false;
		m_curUIType = HIDDEN_OPTION_UI_TYPE.LIST;
	}

	public void SetOut()
	{
		m_rectMove.Set("Out");
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		DeactivateAllFx();
	}

	public void AnimateOutToIn()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_rectMove.Set("Out");
		m_rectMove.Transit("In");
	}

	public void SetEnchantCard(GameObject enchantCard)
	{
		m_objEnchantCard = enchantCard;
	}

	public void SetLeftEquipUID(long uid)
	{
		if (m_LeftEquipUID != uid)
		{
			m_SelectedSocketIndex = -1;
			SelectRerollSlot(m_SelectedSocketIndex);
		}
		m_LeftEquipUID = uid;
	}

	public void SetUI()
	{
		m_tglList.Select(m_curUIType == HIDDEN_OPTION_UI_TYPE.LIST, bForce: true);
		m_tglReroll.Select(m_curUIType == HIDDEN_OPTION_UI_TYPE.REROLL, bForce: true);
		if (NKCScenManager.CurrentUserData().GetPotentialData() != null && NKCScenManager.CurrentUserData().GetPotentialData().equipUid > 0)
		{
			m_SelectedSocketIndex = NKCScenManager.CurrentUserData().GetPotentialData().socketIndex;
		}
		EnableUI(m_LeftEquipUID != 0);
		if (m_curUIType == HIDDEN_OPTION_UI_TYPE.REROLL)
		{
			SelectRerollSlot(m_SelectedSocketIndex);
		}
	}

	public void ActivateUnlockFx(int socketIndex, OnExplosionFxActivated dOnEffectActivated = null)
	{
		if (m_objSocketUnlockFx != null && m_objSocketUnlockFx.Length > socketIndex)
		{
			if (m_explosionFx != null && m_objEnchantCard != null)
			{
				m_explosionFx.position = m_objEnchantCard.transform.position;
			}
			NKCUtil.SetGameobjectActive(m_objFxRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSocketUnlockFx[socketIndex], bValue: true);
			m_dOnExplosionFxActivated = dOnEffectActivated;
			StopFxCoroutine();
			m_fxExplosionCoroutine = StartCoroutine(IExplosionFxActivationCheck());
		}
	}

	public bool IsEffectStopped()
	{
		if (m_fxActiveCheck == null)
		{
			return true;
		}
		int num = m_fxActiveCheck.Length;
		for (int i = 0; i < num; i++)
		{
			if (!(m_fxActiveCheck[i] == null) && m_fxActiveCheck[i].gameObject.activeInHierarchy && !m_fxActiveCheck[i].IsStopped)
			{
				return false;
			}
		}
		return true;
	}

	public void Close()
	{
		m_LeftEquipUID = 0L;
		m_objEnchantCard = null;
		DeactivateAllFx();
		StopFxCoroutine();
		m_dOnExplosionFxActivated = null;
		m_fxExplosionPlayer = null;
		m_bUnlockingSocket = false;
	}

	private IEnumerator IExplosionFxActivationCheck()
	{
		bool skip = false;
		m_csbtnUnlockOption.SetLock(value: true);
		if (m_fxExplosionPlayer != null)
		{
			while (!m_fxExplosionPlayer.gameObject.activeInHierarchy || m_fxExplosionPlayer.IsStopped)
			{
				if (Input.GetMouseButtonDown(0))
				{
					if (m_fxActiveCheck != null)
					{
						int num = m_fxActiveCheck.Length;
						for (int i = 0; i < num; i++)
						{
							m_fxActiveCheck[i].Stop();
						}
					}
					skip = true;
					break;
				}
				yield return null;
			}
		}
		m_csbtnUnlockOption.SetLock(value: false);
		if (m_dOnExplosionFxActivated != null)
		{
			m_dOnExplosionFxActivated();
		}
		if (skip)
		{
			m_fxExplosionPlayer.Restart();
		}
	}

	private void EnableUI(bool bActive)
	{
		m_iOpenedSocketCount = 0;
		m_fxExplosionPlayer = m_explosionFx?.GetComponent<NKC_FXM_PLAYER>();
		NKCUtil.SetGameobjectActive(m_objOptionList, bActive);
		NKCUtil.SetGameobjectActive(m_objOptionListOnly, bActive && m_curUIType == HIDDEN_OPTION_UI_TYPE.LIST);
		NKCUtil.SetGameobjectActive(m_objOptionRerollOnly, bActive && m_curUIType == HIDDEN_OPTION_UI_TYPE.REROLL);
		NKCUtil.SetGameobjectActive(m_csbtnUnlockOption, m_curUIType == HIDDEN_OPTION_UI_TYPE.LIST);
		NKCUtil.SetGameobjectActive(m_objRerollButtons, m_curUIType == HIDDEN_OPTION_UI_TYPE.REROLL);
		NKCUtil.SetGameobjectActive(m_objDisable, bActive);
		NKCUtil.SetGameobjectActive(m_objEmpty, !bActive);
		NKCUtil.SetGameobjectActive(m_csbtnOptionInfo, bActive);
		NKCUtil.SetGameobjectActive(m_csbtnRerollInfo, bActive && NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.OPEN_TAG_RATE_INFO));
		m_csbtnUnlockOption.SetLock(value: true);
		m_btnReroll.SetLock(value: true);
		m_btnRerollConfirm.SetLock(value: true);
		if (!m_bUnlockingSocket)
		{
			DeactivateAllFx();
		}
		m_bUnlockingSocket = false;
		StopFxCoroutine();
		if (m_curUIType == HIDDEN_OPTION_UI_TYPE.LIST)
		{
			NKCUtil.SetLabelText(m_lbCostTitle, NKCUtilString.GET_STRING_FACTORY_HIDDEN_OPTION_COST);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbCostTitle, NKCUtilString.GET_STRING_FACTORY_HIDDEN_OPTION_REROLL_COST);
		}
		if (!bActive)
		{
			if (m_itemCostSlotArray != null)
			{
				int num = m_itemCostSlotArray.Length;
				for (int i = 0; i < num; i++)
				{
					m_itemCostSlotArray[i].SetData(0, 0, 0L);
				}
			}
			for (int j = 0; j < m_HiddenOptionSlotArray.Length; j++)
			{
				m_HiddenOptionSlotArray[j].SetSelected(bValue: false);
			}
			if (m_curUIType == HIDDEN_OPTION_UI_TYPE.LIST)
			{
				NKCUtil.SetLabelText(m_lbEmpty, NKCUtilString.GET_STRING_FACTORY_HIDDEN_OPTION_EMPTY);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbEmpty, NKCUtilString.GET_STRING_FACTORY_HIDDEN_OPTION_REROLL_EMPTY);
			}
			return;
		}
		NKCUtil.SetGameobjectActive(m_objOptionList, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDisable, bValue: true);
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMEquipItemData nKMEquipItemData = myUserData?.m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (nKMEquipItemData == null)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(nKMEquipItemData.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return;
		}
		bool flag = equipTemplet.IsRelic();
		NKCUtil.SetGameobjectActive(m_objOptionList, flag);
		NKCUtil.SetGameobjectActive(m_objOptionListOnly, flag && m_curUIType == HIDDEN_OPTION_UI_TYPE.LIST);
		NKCUtil.SetGameobjectActive(m_objOptionRerollOnly, flag && m_curUIType == HIDDEN_OPTION_UI_TYPE.REROLL);
		NKCUtil.SetGameobjectActive(m_csbtnUnlockOption, m_curUIType == HIDDEN_OPTION_UI_TYPE.LIST);
		NKCUtil.SetGameobjectActive(m_objRerollButtons, m_curUIType == HIDDEN_OPTION_UI_TYPE.REROLL);
		NKCUtil.SetGameobjectActive(m_objDisable, !flag);
		NKCUtil.SetGameobjectActive(m_csbtnOptionInfo, flag);
		NKCUtil.SetGameobjectActive(m_csbtnRerollInfo, flag && NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.OPEN_TAG_RATE_INFO));
		if (flag)
		{
			if (NKMTempletContainer<NKMPotentialOptionGroupTemplet>.Find(equipTemplet.GetPotentialOptionGroupID()) == null)
			{
				Debug.LogError($"Potential Option Group Id: {equipTemplet.GetPotentialOptionGroupID()} of this EquipID: {equipTemplet.m_ItemEquipID} does not exist");
			}
			else
			{
				if (m_HiddenOptionSlotArray == null)
				{
					return;
				}
				if (nKMEquipItemData.potentialOptions.Count <= 0)
				{
					int num2 = m_HiddenOptionSlotArray.Length;
					for (int k = 0; k < num2; k++)
					{
						int num3 = ((m_socketEnchantLv != null && m_socketEnchantLv.Length > k) ? m_socketEnchantLv[k] : 99);
						m_HiddenOptionSlotArray[k].Lock(num3, num3 <= nKMEquipItemData.m_EnchantLevel);
						m_HiddenOptionSlotArray[k].SetSelected(bValue: false);
					}
					NKCUtil.SetGameobjectActive(m_objOptionListOnly, m_curUIType == HIDDEN_OPTION_UI_TYPE.LIST);
					NKCUtil.SetGameobjectActive(m_objOptionRerollOnly, m_curUIType == HIDDEN_OPTION_UI_TYPE.REROLL);
					NKCUtil.SetGameobjectActive(m_objRerollButtons, bActive && m_curUIType == HIDDEN_OPTION_UI_TYPE.REROLL);
					if (m_curUIType == HIDDEN_OPTION_UI_TYPE.LIST)
					{
						NKCUtil.SetLabelText(m_lbResult, NKCUtilString.GET_STRING_EQUIP_POTENTIAL_OPEN_REQUIRED);
						NKCUtil.SetLabelText(m_lbResultDetail, "");
					}
					else if (m_curUIType == HIDDEN_OPTION_UI_TYPE.REROLL)
					{
						m_slotRerollResult.SetEmpty();
						m_btnReroll.SetLock(value: true);
						m_btnRerollConfirm.SetLock(value: true);
					}
					m_csbtnUnlockOption.SetLock(value: false);
				}
				else
				{
					int num4 = Mathf.Min(nKMEquipItemData.potentialOptions[0].sockets.Length, m_HiddenOptionSlotArray.Length);
					for (int l = 0; l < num4; l++)
					{
						int num5 = ((m_socketEnchantLv != null && m_socketEnchantLv.Length > l) ? m_socketEnchantLv[l] : 99);
						if (m_HiddenOptionSlotArray.Length <= l || nKMEquipItemData.potentialOptions[0].sockets[l] == null)
						{
							m_HiddenOptionSlotArray[l].Lock(num5, num5 <= nKMEquipItemData.m_EnchantLevel);
							continue;
						}
						m_HiddenOptionSlotArray[l].Unlocked(l, nKMEquipItemData.potentialOptions);
						m_HiddenOptionSlotArray[l].SetSelected(bValue: false);
						m_iOpenedSocketCount++;
					}
					NKCUtil.SetGameobjectActive(m_objOptionListOnly, m_curUIType == HIDDEN_OPTION_UI_TYPE.LIST);
					NKCUtil.SetGameobjectActive(m_objOptionRerollOnly, m_curUIType == HIDDEN_OPTION_UI_TYPE.REROLL);
					NKCUtil.SetGameobjectActive(m_objRerollButtons, bActive && m_curUIType == HIDDEN_OPTION_UI_TYPE.REROLL);
					if (m_curUIType == HIDDEN_OPTION_UI_TYPE.LIST)
					{
						if (m_iOpenedSocketCount > 0)
						{
							NKCUtil.SetLabelText(m_lbResult, NKCUtil.GetPotentialStatText(nKMEquipItemData));
							NKCUtil.SetLabelText(m_lbResultDetail, "(" + NKCUtil.GetPotentialStatText(nKMEquipItemData, 0, bShowName: false, bShowDetail: true) + ")");
						}
						else
						{
							NKCUtil.SetLabelText(m_lbResult, NKCUtilString.GET_STRING_EQUIP_POTENTIAL_OPEN_REQUIRED);
							NKCUtil.SetLabelText(m_lbResultDetail, "");
						}
						m_csbtnUnlockOption.SetLock(m_iOpenedSocketCount >= nKMEquipItemData.potentialOptions[0].sockets.Length);
					}
					else if (m_curUIType == HIDDEN_OPTION_UI_TYPE.REROLL)
					{
						SetRerollButtonState();
					}
				}
				if (m_itemCostSlotArray == null)
				{
					return;
				}
				if (m_curUIType == HIDDEN_OPTION_UI_TYPE.LIST)
				{
					NKCUtil.SetLabelText(m_lbCostTitle, NKCUtilString.GET_STRING_FACTORY_HIDDEN_OPTION_COST);
					List<MiscItemUnit> socketOpenResource = equipTemplet.GetSocketOpenResource(m_iOpenedSocketCount);
					int num6 = m_itemCostSlotArray.Length;
					for (int m = 0; m < num6; m++)
					{
						if (socketOpenResource.Count <= m)
						{
							m_itemCostSlotArray[m].SetData(0, 0, 0L);
							continue;
						}
						long curCnt = 0L;
						if (myUserData != null)
						{
							curCnt = myUserData.m_InventoryData.GetCountMiscItem(socketOpenResource[m].ItemId);
						}
						long credit = socketOpenResource[m].Count32;
						bool flag2 = false;
						if (socketOpenResource[m].ItemId == 1)
						{
							flag2 = NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT);
							if (flag2)
							{
								NKCCompanyBuff.SetDiscountOfCreditInPotentialSocket(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref credit);
							}
						}
						m_itemCostSlotArray[m].SetData(socketOpenResource[m].ItemId, (int)credit, curCnt, bShowTooltip: true, bShowBG: true, flag2);
					}
				}
				else
				{
					if (m_curUIType != HIDDEN_OPTION_UI_TYPE.REROLL)
					{
						return;
					}
					NKCUtil.SetLabelText(m_lbCostTitle, NKCUtilString.GET_STRING_FACTORY_HIDDEN_OPTION_REROLL_COST);
					int changedCount = 0;
					if (nKMEquipItemData.potentialOptions.Count > 0 && nKMEquipItemData.potentialOptions[0] != null && myUserData.GetPotentialData() != null)
					{
						changedCount = Mathf.Max(nKMEquipItemData.potentialOptions[0].precisionChangeCount, myUserData.GetPotentialData().accumulateCount);
					}
					List<MiscItemUnit> relicRerollResource = equipTemplet.GetRelicRerollResource(changedCount);
					int num7 = m_itemCostSlotArray.Length;
					for (int n = 0; n < num7; n++)
					{
						if (relicRerollResource.Count <= n)
						{
							m_itemCostSlotArray[n].SetData(0, 0, 0L);
							continue;
						}
						long curCnt2 = 0L;
						if (myUserData != null)
						{
							curCnt2 = myUserData.m_InventoryData.GetCountMiscItem(relicRerollResource[n].ItemId);
						}
						long credit2 = relicRerollResource[n].Count32;
						bool flag3 = false;
						if (relicRerollResource[n].ItemId == 1)
						{
							flag3 = NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT);
							if (flag3)
							{
								NKCCompanyBuff.SetDiscountOfCreditInPotentialSocket(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref credit2);
							}
						}
						m_itemCostSlotArray[n].SetData(relicRerollResource[n].ItemId, (int)credit2, curCnt2, bShowTooltip: true, bShowBG: true, flag3);
					}
					SelectRerollSlot(m_SelectedSocketIndex);
				}
			}
			return;
		}
		if (m_itemCostSlotArray != null)
		{
			int num8 = m_itemCostSlotArray.Length;
			for (int num9 = 0; num9 < num8; num9++)
			{
				m_itemCostSlotArray[num9].SetData(0, 0, 0L);
			}
			for (int num10 = 0; num10 < m_HiddenOptionSlotArray.Length; num10++)
			{
				m_HiddenOptionSlotArray[num10].SetSelected(bValue: false);
			}
		}
		m_csbtnUnlockOption.SetLock(value: true);
		if (m_curUIType == HIDDEN_OPTION_UI_TYPE.LIST)
		{
			NKCUtil.SetLabelText(m_lbDisable, NKCUtilString.GET_STRING_FACTORY_HIDDEN_OPTION_DISABLE);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbDisable, NKCUtilString.GET_STRING_FACTORY_HIDDEN_OPTION_REROLL_DISABLE);
		}
	}

	private void SetRerollButtonState()
	{
		NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		NKMPotentialOptionChangeCandidate potentialData = NKCScenManager.CurrentUserData().GetPotentialData();
		if (itemEquip == null || m_SelectedSocketIndex < 0)
		{
			m_slotRerollResult.SetEmpty();
			m_btnReroll.SetLock(value: true);
			m_btnRerollConfirm.SetLock(value: true);
		}
		else if (itemEquip.potentialOptions.Count <= 0 || itemEquip.potentialOptions[0] == null || itemEquip.potentialOptions[0].precisionChangeCount >= NKMCommonConst.RelicRerollLimitCount)
		{
			m_slotRerollResult.SetEmpty();
			m_btnReroll.SetLock(value: true);
			m_btnRerollConfirm.SetLock(value: true);
		}
		else if (potentialData == null || potentialData.equipUid == 0L)
		{
			m_slotRerollResult.SetEmpty();
			m_btnReroll.SetLock(value: false);
			m_btnRerollConfirm.SetLock(value: true);
		}
		else if (potentialData.equipUid == m_LeftEquipUID)
		{
			m_slotRerollResult.Unlocked(potentialData.socketIndex, itemEquip.potentialOptions, potentialData.precision);
			m_btnReroll.SetLock((potentialData.accumulateCount >= NKMCommonConst.RelicRerollLimitCount) ? true : false);
			m_btnRerollConfirm.SetLock(value: false);
		}
		else
		{
			m_slotRerollResult.SetEmpty();
			m_btnReroll.SetLock(value: true);
			m_btnRerollConfirm.SetLock(value: true);
		}
	}

	private void OnClickReroll()
	{
		if (!IsEffectStopped())
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip == null)
		{
			return;
		}
		if (itemEquip.potentialOptions.Count <= 0 || itemEquip.potentialOptions[0] == null || itemEquip.potentialOptions[0].precisionChangeCount >= NKMCommonConst.RelicRerollLimitCount)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_EQUIP_RELIC_REROLL_COUNT_LIMIT, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (!NKMItemManager.IsRelicRerollTarget(itemEquip))
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_EQUIP_RELIC_REROLL_SLOT_OPEN, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else
		{
			if (m_SelectedSocketIndex < 0)
			{
				return;
			}
			if (myUserData.GetPotentialData() != null && myUserData.GetPotentialData().accumulateCount >= NKMCommonConst.RelicRerollLimitCount)
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_EQUIP_RELIC_REROLL_COUNT_LIMIT, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				return;
			}
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet == null)
			{
				return;
			}
			int changedCount = Mathf.Max(itemEquip.potentialOptions[0].precisionChangeCount, myUserData.GetPotentialData().accumulateCount);
			List<MiscItemUnit> relicRerollResource = equipTemplet.GetRelicRerollResource(changedCount);
			int count = relicRerollResource.Count;
			for (int i = 0; i < count; i++)
			{
				long credit = relicRerollResource[i].Count;
				if (relicRerollResource[i].ItemId == 1)
				{
					NKCCompanyBuff.SetDiscountOfCreditInPotentialSocket(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref credit);
				}
				long countMiscItem = myUserData.m_InventoryData.GetCountMiscItem(relicRerollResource[i].ItemId);
				if (credit > countMiscItem)
				{
					int itemId = relicRerollResource[i].ItemId;
					if (itemId == 1 || itemId == 101)
					{
						NKCShopManager.OpenItemLackPopup(relicRerollResource[i].ItemId, (int)credit);
					}
					else
					{
						NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM);
					}
					return;
				}
			}
			NKCPacketSender.Send_NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_REQ(itemEquip.m_ItemUid, m_SelectedSocketIndex);
		}
	}

	private void OnClickRerollConfirm()
	{
		if (!IsEffectStopped())
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKMEquipItemData equipItemData = myUserData.m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (equipItemData != null && NKMItemManager.GetEquipTemplet(equipItemData.m_ItemEquipID) != null && m_curUIType == HIDDEN_OPTION_UI_TYPE.REROLL)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, GetRerollString(), delegate
			{
				NKCPacketSender.Send_NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CONFIRM_REQ(equipItemData.m_ItemUid, m_SelectedSocketIndex);
			});
		}
	}

	private string GetRerollString()
	{
		NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		int num = m_SelectedSocketIndex + 1;
		bool flag = NKMUnitStatManager.IsPercentStat(itemEquip.potentialOptions[0].statType);
		float statValue = itemEquip.potentialOptions[0].sockets[m_SelectedSocketIndex].statValue;
		bool flag2 = flag && statValue < 0f;
		statValue = (flag2 ? Mathf.Abs(statValue) : statValue);
		string potentialSocketStatText = NKCUtil.GetPotentialSocketStatText(flag, statValue);
		NKMPotentialOptionTemplet nKMPotentialOptionTemplet = NKMPotentialOptionTemplet.Find(itemEquip.potentialOptions[0].optionKey);
		int precision = NKCScenManager.CurrentUserData().GetPotentialData().precision;
		float num2 = nKMPotentialOptionTemplet.sockets[m_SelectedSocketIndex].CalcStatValue(precision);
		num2 = (flag2 ? Mathf.Abs(num2) : num2);
		string potentialSocketStatText2 = NKCUtil.GetPotentialSocketStatText(flag, num2);
		string statShortName = NKCUtilString.GetStatShortName(itemEquip.potentialOptions[0].statType, flag2);
		if (itemEquip.potentialOptions.Count > 1)
		{
			flag = NKMUnitStatManager.IsPercentStat(itemEquip.potentialOptions[1].statType);
			float statValue2 = itemEquip.potentialOptions[0].sockets[m_SelectedSocketIndex].statValue;
			bool flag3 = flag && statValue2 < 0f;
			statValue2 = (flag3 ? Mathf.Abs(statValue2) : statValue2);
			string potentialSocketStatText3 = NKCUtil.GetPotentialSocketStatText(flag, statValue2);
			float num3 = NKMPotentialOptionTemplet.Find(itemEquip.potentialOptions[0].optionKey).sockets[m_SelectedSocketIndex].CalcStatValue(precision);
			num3 = (flag3 ? Mathf.Abs(num3) : num3);
			string potentialSocketStatText4 = NKCUtil.GetPotentialSocketStatText(flag, num3);
			string statShortName2 = NKCUtilString.GetStatShortName(itemEquip.potentialOptions[0].statType, flag3);
			return string.Format(NKCUtilString.GET_STRING_FORGE_RELIC_REROLL_CONFIRM_DESC_TWO_PARAM, num, statShortName, potentialSocketStatText, statShortName, potentialSocketStatText2, statShortName2, potentialSocketStatText3, statShortName2, potentialSocketStatText4);
		}
		return string.Format(NKCUtilString.GET_STRING_FORGE_RELIC_REROLL_CONFIRM_DESC_TWO_PARAM, num, statShortName, potentialSocketStatText, statShortName, potentialSocketStatText2);
	}

	private void OnClickUnlockOption()
	{
		if (!IsEffectStopped())
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip == null)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
		if (equipTemplet == null || m_curUIType != HIDDEN_OPTION_UI_TYPE.LIST)
		{
			return;
		}
		List<MiscItemUnit> socketOpenResource = equipTemplet.GetSocketOpenResource(m_iOpenedSocketCount);
		int count = socketOpenResource.Count;
		for (int i = 0; i < count; i++)
		{
			long countMiscItem = myUserData.m_InventoryData.GetCountMiscItem(socketOpenResource[i].ItemId);
			long credit = socketOpenResource[i].Count;
			if (socketOpenResource[i].ItemId == 1 && NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT))
			{
				NKCCompanyBuff.SetDiscountOfCreditInPotentialSocket(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref credit);
			}
			if (credit > countMiscItem)
			{
				int itemId = socketOpenResource[i].ItemId;
				if (itemId == 1 || itemId == 101)
				{
					NKCShopManager.OpenItemLackPopup(socketOpenResource[i].ItemId, (int)credit);
				}
				else
				{
					NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM);
				}
				return;
			}
		}
		int num = ((m_socketEnchantLv != null && m_socketEnchantLv.Length > m_iOpenedSocketCount) ? m_socketEnchantLv[m_iOpenedSocketCount] : 0);
		if (num > itemEquip.m_EnchantLevel)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCUtilString.GET_STRING_EQUIP_POTENTIAL_CANNOT_OPEN, num));
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_OPEN_SOCKET_REQ(itemEquip.m_ItemUid, m_iOpenedSocketCount);
		}
	}

	private void OnClickOptionInfo()
	{
		if (!IsEffectStopped() || m_LeftEquipUID == 0L)
		{
			return;
		}
		NKMEquipItemData nKMEquipItemData = NKCScenManager.GetScenManager().GetMyUserData()?.m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (nKMEquipItemData != null)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(nKMEquipItemData.m_ItemEquipID);
			if (equipTemplet != null)
			{
				NKCPopupHiddenOptionPopup.Instance.Open(equipTemplet.GetPotentialOptionGroupID(), equipTemplet.GetPotentialOptionGroupID2());
			}
		}
	}

	private void OnClickRerollInfo()
	{
		if (m_LeftEquipUID == 0L)
		{
			return;
		}
		NKMEquipItemData nKMEquipItemData = NKCScenManager.GetScenManager().GetMyUserData()?.m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (nKMEquipItemData != null && nKMEquipItemData.potentialOptions.Count != 0 && NKMItemManager.GetEquipTemplet(nKMEquipItemData.m_ItemEquipID) != null)
		{
			NKM_STAT_TYPE statType_ = NKM_STAT_TYPE.NST_RANDOM;
			int potentialOpKey_ = 0;
			if (nKMEquipItemData.potentialOptions.Count > 1)
			{
				statType_ = nKMEquipItemData.potentialOptions[1].statType;
				potentialOpKey_ = nKMEquipItemData.potentialOptions[1].optionKey;
			}
			NKCPopupEquipSocketReRollRateInfo.Instance.Open(nKMEquipItemData.potentialOptions[0].statType, nKMEquipItemData.potentialOptions[0].optionKey, statType_, potentialOpKey_);
		}
	}

	private void DeactivateAllFx()
	{
		if (m_objSocketUnlockFx != null)
		{
			int num = m_objSocketUnlockFx.Length;
			for (int i = 0; i < num; i++)
			{
				NKCUtil.SetGameobjectActive(m_objSocketUnlockFx[i], bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_objFxRoot, bValue: false);
	}

	private void StopFxCoroutine()
	{
		if (m_fxExplosionCoroutine != null)
		{
			StopCoroutine(m_fxExplosionCoroutine);
			m_fxExplosionCoroutine = null;
		}
	}

	private void OnTglList(bool bValue)
	{
		if (bValue)
		{
			m_curUIType = HIDDEN_OPTION_UI_TYPE.LIST;
			SetUI();
		}
	}

	private void OnTglReroll(bool bValue)
	{
		if (!m_tglReroll.m_bLock && bValue)
		{
			m_curUIType = HIDDEN_OPTION_UI_TYPE.REROLL;
			SetUI();
		}
	}

	public void SetEffect(HIDDEN_OPTION_CHANGE_EFFECT_TYPE effectType)
	{
		switch (effectType)
		{
		case HIDDEN_OPTION_CHANGE_EFFECT_TYPE.CHANGE:
			m_slotRerollResult.SetActiveFxRerollResult();
			NKCSoundManager.PlaySound("FX_UI_UNIT_GET_MAIN", 1f, 0f, 0f);
			break;
		case HIDDEN_OPTION_CHANGE_EFFECT_TYPE.CHANGE_CONFIRM:
			m_HiddenOptionSlotArray[m_SelectedSocketIndex].SetActiveFxRerollConfirm();
			NKCSoundManager.PlaySound("FX_UI_UNIT_GET_STAR", 1f, 0f, 0f);
			break;
		}
	}

	public void SelectRerollSlot(int socketIndex)
	{
		m_SelectedSocketIndex = socketIndex;
		SetRerollButtonState();
		for (int i = 0; i < m_HiddenOptionSlotArray.Length; i++)
		{
			m_HiddenOptionSlotArray[i].SetSelected(m_SelectedSocketIndex == i);
		}
		NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (m_SelectedSocketIndex < 0 || itemEquip == null || itemEquip.potentialOptions.Count <= 0 || itemEquip.potentialOptions[0].OpenedSocketCount < 3)
		{
			m_slotRerollResult.SetEmpty();
			return;
		}
		NKMPotentialOptionChangeCandidate potentialData = NKCScenManager.CurrentUserData().GetPotentialData();
		NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID).GetPotentialOptionGroupID();
		if (potentialData == null || potentialData.equipUid == 0L || potentialData.equipUid != m_LeftEquipUID)
		{
			m_slotRerollResult.SetRerollData(m_SelectedSocketIndex, itemEquip.potentialOptions, itemEquip.potentialOptions[0].sockets[socketIndex].precision);
		}
		else if (potentialData.equipUid == m_LeftEquipUID)
		{
			m_slotRerollResult.SetRerollData(m_SelectedSocketIndex, itemEquip.potentialOptions, potentialData.precision);
		}
		else
		{
			m_slotRerollResult.SetEmpty();
		}
	}
}
