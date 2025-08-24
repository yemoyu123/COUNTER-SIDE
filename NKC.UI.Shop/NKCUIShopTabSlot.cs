using System.Collections;
using NKM.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIShopTabSlot : MonoBehaviour
{
	public delegate void OnClicked(string tabType, int subTabIndex = 0);

	public Text m_lbTitle;

	public Image m_imgSelected;

	public Image m_imgSpecialBG;

	public NKCUIComStateButton m_btnContent;

	public GameObject m_objRedDot;

	public GameObject m_objReddot_RED;

	public GameObject m_objReddot_YELLOW;

	public Text m_lbReddotCount;

	[Header("")]
	public LayoutElement m_LayoutElement;

	public float fActiveSpeed = 10f;

	private float m_fMaxHeight;

	public Color BASE_SELECTED_COLOR;

	private OnClicked dOnClicked;

	private ShopTabTemplet m_tabTemplet;

	private bool m_bHasProduct = true;

	private bool m_bTabSoldOut;

	public void SetData(ShopTabTemplet shopTabTemplet, OnClicked clicked)
	{
		NKCUtil.SetLabelText(m_lbTitle, shopTabTemplet.GetTabName());
		m_tabTemplet = shopTabTemplet;
		m_bHasProduct = NKCShopManager.GetItemTempletListByTab(shopTabTemplet, bIncludeLockedItemWithReason: true).Count > 0;
		m_bTabSoldOut = NKCShopManager.IsTabSoldOut(shopTabTemplet);
		if (HideTabRequired())
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		if (m_LayoutElement != null)
		{
			m_fMaxHeight = m_LayoutElement.preferredHeight;
			m_LayoutElement.preferredHeight = 0f;
		}
		dOnClicked = clicked;
		NKCUtil.SetGameobjectActive(m_imgSelected, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUtil.SetBindFunction(m_btnContent, OnBtnClick);
		if (!string.IsNullOrEmpty(shopTabTemplet.m_SpecialColorCode))
		{
			if (!shopTabTemplet.m_SpecialColorCode.Contains("#"))
			{
				shopTabTemplet.m_SpecialColorCode = "#" + shopTabTemplet.m_SpecialColorCode;
			}
			NKCUtil.SetImageColor(m_imgSelected, NKCUtil.GetColor(shopTabTemplet.m_SpecialColorCode));
			NKCUtil.SetImageColor(m_imgSpecialBG, NKCUtil.GetColor(shopTabTemplet.m_SpecialColorCode));
			NKCUtil.SetGameobjectActive(m_imgSpecialBG, bValue: true);
		}
		else
		{
			NKCUtil.SetImageColor(m_imgSelected, BASE_SELECTED_COLOR);
			NKCUtil.SetGameobjectActive(m_imgSpecialBG, bValue: false);
		}
	}

	public void SetRedDot(ShopTabTemplet tabTemplet)
	{
		ShopReddotType reddotType;
		int reddotCount = NKCShopManager.CheckTabReddotCount(out reddotType, tabTemplet.TabType, tabTemplet.SubIndex);
		NKCUtil.SetShopReddotImage(reddotType, m_objRedDot, m_objReddot_RED, m_objReddot_YELLOW);
		NKCUtil.SetShopReddotLabel(reddotType, m_lbReddotCount, reddotCount);
	}

	public bool HideTabRequired()
	{
		if (m_tabTemplet == null)
		{
			return true;
		}
		if (!m_tabTemplet.m_HideWhenSoldOut)
		{
			return false;
		}
		if (!m_bHasProduct)
		{
			return true;
		}
		if (m_bTabSoldOut && !NKCUtil.IsUsingSuperUserFunction())
		{
			return true;
		}
		return false;
	}

	public bool HasProduct()
	{
		return m_bHasProduct;
	}

	public bool TabSoldOut()
	{
		return m_bTabSoldOut;
	}

	public void OnActive(bool Active, bool bAnimate)
	{
		if (HideTabRequired())
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		Color color = NKCUtil.GetColor("#FFFFFF");
		if (!Active)
		{
			color.a = 0.6f;
		}
		Active &= m_tabTemplet != null && NKCSynchronizedTime.IsEventTime(m_tabTemplet.intervalId, m_tabTemplet.EventDateStartUtc, m_tabTemplet.EventDateEndUtc);
		NKCUtil.SetLabelTextColor(m_lbTitle, color);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		StopAllCoroutines();
		if (bAnimate && base.gameObject.activeInHierarchy)
		{
			StartCoroutine(ActiveButton(Active));
		}
		else if (Active)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			m_LayoutElement.preferredHeight = m_fMaxHeight;
		}
		else
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			m_LayoutElement.preferredHeight = 0f;
		}
	}

	public void OnBtnClick()
	{
		if (dOnClicked != null)
		{
			dOnClicked(m_tabTemplet.TabType, m_tabTemplet.SubIndex);
		}
	}

	public bool OnSelected(string tabType, int subTabIndex)
	{
		bool flag = tabType == m_tabTemplet.TabType && subTabIndex == m_tabTemplet.SubIndex;
		m_btnContent.Select(flag, bForce: true);
		return flag;
	}

	private IEnumerator ActiveButton(bool bActive)
	{
		if (bActive)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			while (m_LayoutElement.preferredHeight < m_fMaxHeight)
			{
				m_LayoutElement.preferredHeight += fActiveSpeed;
				yield return new WaitForEndOfFrame();
			}
		}
		else
		{
			while (m_LayoutElement.preferredHeight > 0f)
			{
				m_LayoutElement.preferredHeight -= fActiveSpeed;
				yield return new WaitForEndOfFrame();
			}
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
		m_LayoutElement.preferredHeight = Mathf.Clamp(m_LayoutElement.preferredHeight, 0f, m_fMaxHeight);
		yield return null;
	}

	public int GetSubIndex()
	{
		return m_tabTemplet.SubIndex;
	}
}
