using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NKC.UI.Shop;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComItemCount : MonoBehaviour
{
	public delegate bool OnEndDateEvent();

	public delegate void OnClickPlusBtn();

	public Image m_imgIcon;

	public List<Image> m_lstImgIcon;

	public Text m_lbCount;

	public Text m_lbPlusCount;

	public Text m_lbRemainTimeForAddEvent;

	public GameObject m_objRemainTime;

	public NKCUIComStateButton m_csbtnPlus;

	private DateTime m_EndDate;

	private float m_fPrevUpdateTime;

	private OnEndDateEvent m_dOnEndDateEvent;

	private bool m_bEndDateEvent;

	private OnClickPlusBtn m_dOnClickPlusBtn;

	public GameObject m_TempHideObject;

	private long m_MaxCount;

	private string m_strTimeText;

	private float m_fChangeTime = 0.33f;

	private int m_iMaxGap = 100;

	public int CurrentItemID { get; private set; }

	public void SetEndDate(DateTime endDate)
	{
		m_EndDate = endDate;
		m_bEndDateEvent = true;
	}

	public void SetEndDateEvent(OnEndDateEvent dOnEndDateEvent)
	{
		m_dOnEndDateEvent = dOnEndDateEvent;
	}

	public void SetOnClickPlusBtn(OnClickPlusBtn dOnClickPlusBtn)
	{
		m_dOnClickPlusBtn = dOnClickPlusBtn;
	}

	public void SetMaxCount(long max)
	{
		m_MaxCount = max;
	}

	public void SetTimeLabel(string text)
	{
		m_strTimeText = text;
	}

	private void Start()
	{
		if (m_csbtnPlus != null)
		{
			m_csbtnPlus.PointerClick.RemoveAllListeners();
			m_csbtnPlus.PointerClick.AddListener(OnClickPlusBtnImpl);
		}
	}

	private void OnClickPlusBtnImpl()
	{
		if (m_dOnClickPlusBtn != null)
		{
			m_dOnClickPlusBtn();
		}
	}

	private void OnEnable()
	{
		UpdateRemainTimeForAddEvent();
	}

	private void UpdateRemainTimeForAddEvent()
	{
		if (!(m_lbRemainTimeForAddEvent != null))
		{
			return;
		}
		TimeSpan timeSpan = m_EndDate - NKCSynchronizedTime.GetServerUTCTime();
		if (timeSpan.TotalHours > 0.0)
		{
			m_lbRemainTimeForAddEvent.text = NKCUtilString.GetTimeSpanString(timeSpan);
		}
		else if (timeSpan.TotalSeconds > 0.0)
		{
			m_lbRemainTimeForAddEvent.text = NKCUtilString.GetTimeSpanStringMS(timeSpan);
		}
		else if (string.IsNullOrEmpty(m_strTimeText))
		{
			m_lbRemainTimeForAddEvent.text = NKCUtilString.GetTimeSpanStringMS(timeSpan);
		}
		else
		{
			m_lbRemainTimeForAddEvent.text = m_strTimeText;
		}
		if (m_bEndDateEvent && NKCSynchronizedTime.IsFinished(m_EndDate))
		{
			if (m_dOnEndDateEvent != null)
			{
				m_bEndDateEvent = !m_dOnEndDateEvent();
			}
			else
			{
				m_bEndDateEvent = false;
			}
		}
	}

	private void Update()
	{
		if (m_lbRemainTimeForAddEvent != null && m_fPrevUpdateTime + 1f < Time.time)
		{
			m_fPrevUpdateTime = Time.time;
			UpdateRemainTimeForAddEvent();
		}
	}

	public void SetData(NKMUserData userData, int itemID)
	{
		if (userData != null)
		{
			NKMItemMiscData itemMisc = userData.m_InventoryData.GetItemMisc(itemID);
			UpdateData(itemMisc, itemID);
		}
	}

	public void UpdateData(NKMUserData userData)
	{
		if (CurrentItemID != 0)
		{
			SetData(userData, CurrentItemID);
		}
	}

	public void UpdateData(NKMItemMiscData itemData, int itemID = 0)
	{
		if (m_imgIcon != null && ((itemData != null && CurrentItemID != itemData.ItemID) || itemID != 0))
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
			if (itemMiscTempletByID != null)
			{
				Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID);
				m_imgIcon.sprite = orLoadMiscItemSmallIcon;
			}
		}
		if (m_lstImgIcon.Count > 0 && itemData != null)
		{
			for (int i = 0; i < m_lstImgIcon.Count; i++)
			{
				if (i < itemData.TotalCount)
				{
					NKCUtil.SetGameobjectActive(m_lstImgIcon[i], bValue: true);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstImgIcon[i], bValue: false);
				}
			}
		}
		long num = 0L;
		if (itemData != null)
		{
			if (m_lbPlusCount == null)
			{
				NKCUtil.SetLabelText(m_lbCount, GetItemCountString(itemData.CountFree, itemData.CountPaid));
			}
			else
			{
				NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
				if (inventoryData.IsFirstUpdate(itemData.ItemID, itemData.TotalCount))
				{
					if (base.gameObject.activeInHierarchy)
					{
						StopCoroutine(ChangeItemCount(null, 0L));
						StartCoroutine(ChangeItemCount(itemData, inventoryData.GetPreviousItemCount(itemData.ItemID, itemData.TotalCount)));
					}
					else
					{
						NKCUtil.SetLabelText(m_lbCount, GetItemCountString(itemData.CountFree, itemData.CountPaid));
					}
				}
				else
				{
					NKCUtil.SetLabelText(m_lbCount, GetItemCountString(itemData.CountFree, itemData.CountPaid));
				}
			}
			num = itemData.TotalCount;
		}
		else
		{
			NKCUtil.SetLabelText(m_lbCount, GetItemCountString(0L, 0L));
		}
		bool bValue = m_MaxCount > 0 && num < m_MaxCount;
		NKCUtil.SetGameobjectActive(m_objRemainTime, bValue);
		CurrentItemID = itemData?.ItemID ?? itemID;
	}

	public void CleanUp()
	{
		CurrentItemID = 0;
	}

	private string GetItemCountString(long freeCount, long cashCount)
	{
		return (freeCount + cashCount).ToString("N0");
	}

	private IEnumerator ChangeItemCount(NKMItemMiscData miscItem, long oldVal)
	{
		if (oldVal == 0L)
		{
			NKCUtil.SetLabelText(m_lbCount, GetItemCountString(miscItem.CountFree, miscItem.CountPaid));
		}
		else
		{
			if (oldVal == miscItem.TotalCount)
			{
				yield break;
			}
			bool num = oldVal < miscItem.TotalCount;
			long num2 = miscItem.TotalCount - oldVal;
			string msg = (num ? $"(+{num2})" : $"({num2})");
			NKCUtil.SetGameobjectActive(m_lbPlusCount.gameObject, bValue: true);
			NKCUtil.SetLabelText(m_lbPlusCount, msg);
			Color col = (num ? NKCUtil.GetColor("#FFCF3B") : NKCUtil.GetColor("#E92322"));
			NKCUtil.SetLabelTextColor(m_lbPlusCount, col);
			NKCUtil.SetGameobjectActive(m_TempHideObject, bValue: false);
			m_lbPlusCount.DOFade(0f, 1f).SetDelay(1f).OnComplete(delegate
			{
				NKCUtil.SetGameobjectActive(m_lbPlusCount.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(m_TempHideObject, bValue: true);
			});
			if (num2 <= 0)
			{
				NKCUtil.SetLabelText(m_lbCount, GetItemCountString(miscItem.CountFree, miscItem.CountPaid));
				yield return null;
				yield break;
			}
			long increseVal = 1L;
			float fUpdateTime;
			if (num2 > m_iMaxGap)
			{
				fUpdateTime = m_fChangeTime / (float)m_iMaxGap;
				increseVal = num2 / m_iMaxGap;
			}
			else
			{
				fUpdateTime = m_fChangeTime / (float)num2;
			}
			while (oldVal < miscItem.TotalCount)
			{
				oldVal += increseVal;
				NKCUtil.SetLabelText(m_lbCount, ((int)oldVal).ToString());
				yield return new WaitForSeconds(fUpdateTime);
			}
			NKCUtil.SetLabelText(m_lbCount, GetItemCountString(miscItem.CountFree, miscItem.CountPaid));
			yield return null;
		}
	}

	public void OpenMoveToShopPopup()
	{
		if (NKCShopManager.IsMoveToShopDefined(CurrentItemID))
		{
			NKCPopupItemBox nKCPopupItemBox = NKCPopupItemBox.OpenNewInstance();
			if (nKCPopupItemBox != null)
			{
				nKCPopupItemBox.OpenItemBox(CurrentItemID, NKCPopupItemBox.eMode.MoveToShop, OnMoveToShop);
			}
		}
		else
		{
			NKCPopupItemBox nKCPopupItemBox2 = NKCPopupItemBox.OpenNewInstance();
			if (nKCPopupItemBox2 != null)
			{
				nKCPopupItemBox2.OpenItemBox(CurrentItemID);
			}
		}
	}

	private void OnMoveToShop()
	{
		if (NKCShopManager.CanUsePopupShopBuy(CurrentItemID))
		{
			NKCPopupShopBuyShortcut.Open(CurrentItemID);
		}
		else if (NKCUIForge.IsInstanceOpen && NKCScenManager.GetScenManager().GetMyUserData().hasReservedEquipTuningData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_TUNING_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
				MoveToShop();
			});
		}
		else if (NKCUIForge.IsInstanceOpen && NKCScenManager.GetScenManager().GetMyUserData().hasReservedSetOptionData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_SET_OPTION_TUNING_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
				MoveToShop();
			});
		}
		else if (NKCScenManager.CurrentUserData().hasReservedHiddenOptionRerollData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_RELIC_REROLL_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CANCLE_REQ();
				MoveToShop();
			});
		}
		else
		{
			MoveToShop();
		}
	}

	private void MoveToShop()
	{
		TabId shopMoveTab = NKCShopManager.GetShopMoveTab(CurrentItemID);
		if (shopMoveTab.Type == "TAB_NONE")
		{
			Debug.LogWarning($"상점 바로가기가 정의되지 않은 타입 - {CurrentItemID}");
		}
		else
		{
			NKCUIShop.ShopShortcut(shopMoveTab.Type, shopMoveTab.SubIndex);
		}
	}
}
