using System.Collections.Generic;
using NKM.EventPass;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComEventPassBuySlot : MonoBehaviour
{
	public delegate void ClickBuy();

	public Text m_lbTitle;

	public Text m_lbDesc;

	public Text m_lbPrice;

	public Text m_lbDiscountRate;

	public GameObject m_objDiscountRate;

	public Text m_lbOriginalPrice;

	public GameObject m_objOriginalPrice;

	public Image m_imgPriceIcon;

	public Transform m_slotContent;

	public NKCUIComStateButton m_purchaseButton;

	public int m_iPassLevelInterval;

	public int m_iSlotIconLabelFontSize;

	private List<NKCUISlot> m_listCorePassRewardSlot = new List<NKCUISlot>();

	private ClickBuy m_onClickBuy;

	public void Init()
	{
		NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
		if (eventPassDataManager == null)
		{
			return;
		}
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(eventPassDataManager.EventPassId);
		if (nKMEventPassTemplet == null)
		{
			return;
		}
		int num = nKMEventPassTemplet.PassMaxLevel / m_iPassLevelInterval;
		for (int i = 0; i < num; i++)
		{
			NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_slotContent);
			if (newInstance != null)
			{
				m_listCorePassRewardSlot.Add(newInstance);
				newInstance.SetActive(bSet: true);
			}
		}
		NKCUtil.SetButtonClickDelegate(m_purchaseButton, OnClickBuy);
	}

	public void SetData(string title, string desc, int userPassLevel, int addPassLevel, int priceId, int price, float discountPercent, int discountedPrice, ClickBuy onClickBuy)
	{
		NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
		if (eventPassDataManager == null)
		{
			return;
		}
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(eventPassDataManager.EventPassId);
		if (nKMEventPassTemplet == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_lbTitle, title);
		NKCUtil.SetLabelText(m_lbDesc, desc);
		int count = m_listCorePassRewardSlot.Count;
		for (int i = 0; i < count; i++)
		{
			int num = (i + 1) * m_iPassLevelInterval;
			NKMEventPassRewardTemplet rewardTemplet = NKMEventPassRewardTemplet.GetRewardTemplet(nKMEventPassTemplet.PassRewardGroupId, num);
			if (rewardTemplet != null)
			{
				NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(rewardTemplet.CoreRewardItemType, rewardTemplet.CoreRewardItemId, rewardTemplet.CoreRewardItemCount);
				if (slotData.eType == NKCUISlot.eSlotMode.ItemMisc)
				{
					m_listCorePassRewardSlot[i].SetData(slotData, bEnableLayoutElement: true, OnClickRewardIcon);
				}
				else
				{
					m_listCorePassRewardSlot[i].SetData(slotData);
				}
				m_listCorePassRewardSlot[i].OverrideName($"<size={m_iSlotIconLabelFontSize}>Lv {num}</size>", supportRichText: true, forceShow: true);
				if (num <= userPassLevel + addPassLevel)
				{
					m_listCorePassRewardSlot[i].SetTopNotice(NKCUtilString.GET_STRING_EVENTPASS_REWARD_POSSIBLE, bActive: true);
				}
			}
		}
		Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(priceId);
		NKCUtil.SetImageSprite(m_imgPriceIcon, orLoadMiscItemSmallIcon);
		if (discountPercent > 0f)
		{
			NKCUtil.SetGameobjectActive(m_objDiscountRate, bValue: true);
			NKCUtil.SetLabelText(m_lbDiscountRate, string.Format(NKCUtilString.GET_STRING_EVENTPASS_COREPASS_DISCOUNT_RATE, $"-{discountPercent:###%}"));
			NKCUtil.SetGameobjectActive(m_objOriginalPrice, bValue: true);
			NKCUtil.SetLabelText(m_lbOriginalPrice, $"{price}");
			NKCUtil.SetLabelText(m_lbPrice, $"{discountedPrice}");
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objDiscountRate, bValue: false);
			NKCUtil.SetGameobjectActive(m_objOriginalPrice, bValue: false);
			NKCUtil.SetLabelText(m_lbPrice, $"{price}");
		}
		if (addPassLevel > 0)
		{
			m_purchaseButton.SetLock(userPassLevel >= nKMEventPassTemplet.PassMaxLevel);
		}
		m_onClickBuy = onClickBuy;
	}

	private void OnClickRewardIcon(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, slotData, null, singleOpenOnly: false, bShowCount: false, showDropInfo: false);
	}

	private void OnClickBuy()
	{
		if (m_onClickBuy != null)
		{
			m_onClickBuy();
		}
	}

	private void OnDestroy()
	{
		m_lbTitle = null;
		m_lbDesc = null;
		m_lbPrice = null;
		m_imgPriceIcon = null;
		m_slotContent = null;
		m_purchaseButton = null;
		m_listCorePassRewardSlot?.Clear();
		m_listCorePassRewardSlot = null;
		m_onClickBuy = null;
	}
}
