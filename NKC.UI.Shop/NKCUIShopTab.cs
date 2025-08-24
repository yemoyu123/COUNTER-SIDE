using System;
using System.Collections.Generic;
using NKC.Templet;
using NKM.Shop;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIShopTab : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public delegate void OnTabSelected(string type, int subIndex = 0);

	private const float ONE_SECOND = 1f;

	public Sprite m_sprBaseBGSelected;

	public Sprite m_sprBaseBGUnSelected;

	public NKCUIComToggle m_ctglTab;

	public Image m_imgIcon;

	public Image m_imgBG;

	public Text m_lbTabName;

	public Image m_imgIconUnSelected;

	public Image m_imgBGUnSelected;

	public Text m_lbTabNameUnSelected;

	public GameObject m_objRemainTimeSelected;

	public Text m_lbRemainTimeSelected;

	public GameObject m_objRemainTimeUnSelected;

	public Text m_lbRemainTimeUnSelected;

	public GameObject m_objRedDot;

	public GameObject m_objReddot_RED;

	public GameObject m_objReddot_YELLOW;

	public Text m_lbReddotCount;

	public Color BASE_COLOR_SELECTED_TEXT;

	[Header("리본")]
	public Image m_imgRibbon;

	public Text m_lbRibbon;

	private List<NKCUIShopTabSlot> m_lstSubSlot = new List<NKCUIShopTabSlot>();

	private OnTabSelected dOnTabSelected;

	public ShopTabTemplet m_tabTemplet;

	private DateTime m_tEndTimeUtc = DateTime.MinValue;

	private bool m_bSubTabOpened;

	private float m_tDeltaTime;

	private string m_eType => m_tabTemplet.TabType;

	private int m_subIndex => m_tabTemplet.SubIndex;

	public void SetData(ShopTabTemplet tabTemplet, OnTabSelected onTabSelected, NKCUIComToggleGroup toggleGroup)
	{
		m_tabTemplet = tabTemplet;
		dOnTabSelected = onTabSelected;
		m_ctglTab.OnValueChanged.RemoveAllListeners();
		m_ctglTab.OnValueChanged.AddListener(OnToggle);
		m_ctglTab.SetToggleGroup(toggleGroup);
		NKCUtil.SetImageSprite(m_imgIconUnSelected, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_shop_sprite", tabTemplet.m_TabImageSelect));
		NKCUtil.SetLabelText(m_lbTabName, tabTemplet.GetTabName());
		NKCUtil.SetLabelText(m_lbTabNameUnSelected, tabTemplet.GetTabName());
		SetRibbon(tabTemplet.m_TagImage);
		if (!string.IsNullOrEmpty(tabTemplet.m_ImgBGSelected) && !string.IsNullOrEmpty(tabTemplet.m_ImgBGUnSelected))
		{
			NKCUtil.SetImageSprite(m_imgBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_shop_sprite", tabTemplet.m_ImgBGSelected));
			NKCUtil.SetImageSprite(m_imgBGUnSelected, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_shop_sprite", tabTemplet.m_ImgBGUnSelected));
			if (m_imgBG != null && m_imgBGUnSelected != null)
			{
				NKCUtil.SetLabelTextColor(m_lbTabName, Color.white);
			}
			NKCUtil.SetGameobjectActive(m_imgIcon, bValue: false);
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgBG, m_sprBaseBGSelected);
			NKCUtil.SetImageSprite(m_imgBGUnSelected, m_sprBaseBGUnSelected);
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_shop_sprite", tabTemplet.m_TabImageSelect));
			NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
			if (m_imgBG != null && m_imgBGUnSelected != null)
			{
				NKCUtil.SetLabelTextColor(m_lbTabName, BASE_COLOR_SELECTED_TEXT);
			}
		}
		NKCUtil.SetGameobjectActive(m_objRemainTimeSelected, tabTemplet.HasDateLimit);
		NKCUtil.SetGameobjectActive(m_objRemainTimeUnSelected, tabTemplet.HasDateLimit);
		if (tabTemplet.HasDateLimit)
		{
			m_tEndTimeUtc = tabTemplet.EventDateEndUtc;
			SetTimeText(m_tEndTimeUtc);
		}
		else
		{
			m_tEndTimeUtc = DateTime.MinValue;
		}
	}

	public void Clear()
	{
		foreach (NKCUIShopTabSlot item in m_lstSubSlot)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		m_lstSubSlot.Clear();
	}

	private void OnToggle(bool bChecked)
	{
		if (bChecked && dOnTabSelected != null)
		{
			dOnTabSelected(m_eType, m_subIndex);
		}
	}

	public void SetRedDot()
	{
		ShopTabTemplet shopTabTemplet = ShopTabTemplet.Find(m_eType, 0);
		ShopReddotType reddotType;
		int reddotCount = NKCShopManager.CheckTabReddotCount(out reddotType, shopTabTemplet.TabType);
		NKCUtil.SetShopReddotImage(reddotType, m_objRedDot, m_objReddot_RED, m_objReddot_YELLOW);
		NKCUtil.SetShopReddotLabel(reddotType, m_lbReddotCount, reddotCount);
		for (int i = 0; i < m_lstSubSlot.Count; i++)
		{
			if (shopTabTemplet != null)
			{
				ShopTabTemplet redDot = ShopTabTemplet.Find(shopTabTemplet.TabType, m_lstSubSlot[i].GetSubIndex());
				m_lstSubSlot[i].SetRedDot(redDot);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstSubSlot[i].m_objRedDot, bValue: false);
			}
		}
	}

	public void Toggle(bool bChecked)
	{
		m_ctglTab.Select(bChecked, bForce: true);
	}

	private void SetTimeText(DateTime endDateUtc)
	{
		NKCUtil.SetLabelText(m_lbRemainTimeSelected, string.Format(NKCUtilString.GET_STRING_REMAIN_TIME_LEFT_ONE_PARAM, NKCUtilString.GetRemainTimeString(endDateUtc, 1)));
		NKCUtil.SetLabelText(m_lbRemainTimeUnSelected, string.Format(NKCUtilString.GET_STRING_REMAIN_TIME_LEFT_ONE_PARAM, NKCUtilString.GetRemainTimeString(endDateUtc, 1)));
	}

	private void Update()
	{
		if (m_tEndTimeUtc > DateTime.MinValue)
		{
			m_tDeltaTime += Time.deltaTime;
			if (m_tDeltaTime > 1f)
			{
				SetTimeText(m_tEndTimeUtc);
				m_tDeltaTime = 0f;
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ChangeSubTabState();
	}

	public void AddSubSlot(NKCUIShopTabSlot subSlot)
	{
		m_lstSubSlot.Add(subSlot);
	}

	public GameObject GetSubSlotObject(int subTabIndex)
	{
		if (subTabIndex == 0)
		{
			return base.gameObject;
		}
		for (int i = 0; i < m_lstSubSlot.Count; i++)
		{
			if (m_lstSubSlot[i].GetSubIndex() == subTabIndex)
			{
				return m_lstSubSlot[i].gameObject;
			}
		}
		return null;
	}

	public int GetSubSlotCount()
	{
		return m_lstSubSlot.Count;
	}

	public bool HideTabRequired()
	{
		if (!m_tabTemplet.m_HideWhenSoldOut)
		{
			return false;
		}
		if (GetSubSlotCount() > 0)
		{
			for (int i = 0; i < m_lstSubSlot.Count; i++)
			{
				if (!m_lstSubSlot[i].HideTabRequired())
				{
					return false;
				}
			}
			return true;
		}
		if (!string.IsNullOrEmpty(m_tabTemplet.m_PackageGroupID))
		{
			List<NKCShopFeaturedTemplet> featuredList = NKCShopManager.GetFeaturedList(NKCScenManager.CurrentUserData(), m_tabTemplet.m_PackageGroupID, bUseExhibitCount: false);
			if (featuredList == null || featuredList.Count == 0)
			{
				return true;
			}
			return false;
		}
		if (NKCShopManager.GetItemTempletListByTab(m_tabTemplet, bIncludeLockedItemWithReason: true).Count <= 0)
		{
			return true;
		}
		if (!NKCUtil.IsUsingSuperUserFunction() && NKCShopManager.IsTabSoldOut(m_tabTemplet))
		{
			return true;
		}
		return false;
	}

	public bool SelectSubSlot(string tabType, int subTabIndex, bool bAnimate)
	{
		bool flag = false;
		foreach (NKCUIShopTabSlot item in m_lstSubSlot)
		{
			bool flag2 = item.OnSelected(tabType, subTabIndex);
			if (!flag)
			{
				flag = flag2;
			}
		}
		bool flag3 = m_eType == tabType && subTabIndex == 0;
		m_ctglTab.Select(flag || flag3, bForce: true);
		foreach (NKCUIShopTabSlot item2 in m_lstSubSlot)
		{
			item2.OnActive(flag || flag3, bAnimate);
		}
		m_bSubTabOpened = flag || flag3;
		return flag || flag3;
	}

	public void ChangeSubTabState()
	{
		m_bSubTabOpened = !m_bSubTabOpened;
		foreach (NKCUIShopTabSlot item in m_lstSubSlot)
		{
			item.OnActive(m_bSubTabOpened, bAnimate: true);
		}
	}

	protected void SetRibbon(ShopItemRibbon ribbonType)
	{
		NKCUtil.SetImageColor(m_imgRibbon, NKCShopManager.GetRibbonColor(ribbonType));
		NKCUtil.SetLabelText(m_lbRibbon, NKCShopManager.GetRibbonString(ribbonType));
		NKCUtil.SetGameobjectActive(m_lbRibbon, ribbonType != ShopItemRibbon.None);
		NKCUtil.SetGameobjectActive(m_imgRibbon, ribbonType != ShopItemRibbon.None);
	}
}
