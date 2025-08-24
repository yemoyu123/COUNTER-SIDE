using System.Collections.Generic;
using DG.Tweening;
using NKC.Templet;
using NKC.UI.Component;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guide;

public class NKCUIPopupTutorialImagePanel : NKCUIBase
{
	public delegate void OnClose();

	private struct BannerObj
	{
		public int bannerIdx;

		public int dataIdx;

		public GameObject bannerObj;
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_tutorial";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_IMAGE_TUTORIAL";

	private static NKCUIPopupTutorialImagePanel m_Instance;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComDragSelectablePanel m_UIComDragPanel;

	private OnClose dOnClose;

	private NKCGuideTemplet m_Templet;

	private Stack<RectTransform> m_stkImageObjects = new Stack<RectTransform>();

	private List<BannerObj> m_lstBanner = new List<BannerObj>();

	private List<NKCAssetInstanceData> m_lstAssetInstanceData = new List<NKCAssetInstanceData>();

	public static NKCUIPopupTutorialImagePanel Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupTutorialImagePanel>("ab_ui_nkm_ui_tutorial", "NKM_UI_POPUP_IMAGE_TUTORIAL", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupTutorialImagePanel>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_TUTORIAL_IMAGE;

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

	public override void CloseInternal()
	{
		ClearBannerData();
		base.gameObject.SetActive(value: false);
		dOnClose?.Invoke();
	}

	public void InitUI()
	{
		m_csbtnClose.PointerClick.RemoveAllListeners();
		m_csbtnClose.PointerClick.AddListener(base.Close);
		m_UIComDragPanel.Init();
		m_UIComDragPanel.dOnGetObject += MakeImageObject;
		m_UIComDragPanel.dOnReturnObject += ReturnObject;
		m_UIComDragPanel.dOnProvideData += ProvideData;
		m_UIComDragPanel.dOnFocus += Focus;
	}

	public void Open(int guideID, OnClose onClose)
	{
		Open(NKCGuideTemplet.Find(guideID), onClose);
	}

	public void Open(string guideStrID, OnClose onClose)
	{
		NKCGuideTemplet nKCGuideTemplet = NKCGuideTemplet.Find(guideStrID);
		if (nKCGuideTemplet == null)
		{
			Debug.LogError("GuideTemplet " + guideStrID + " not found");
			NKCUIBase.SetGameObjectActive(base.gameObject, bValue: false);
			onClose?.Invoke();
		}
		else
		{
			Open(nKCGuideTemplet, onClose);
		}
	}

	private void Open(NKCGuideTemplet templet, OnClose onClose)
	{
		if (templet == null)
		{
			Debug.LogError("GuideTemplet not found");
			onClose?.Invoke();
			NKCUIBase.SetGameObjectActive(base.gameObject, bValue: false);
		}
		else
		{
			dOnClose = onClose;
			m_Templet = templet;
			m_UIComDragPanel.TotalCount = m_Templet.lstImages.Count;
			m_UIComDragPanel.SetIndex(0);
			UIOpened();
		}
	}

	private RectTransform MakeImageObject()
	{
		if (m_stkImageObjects.Count > 0)
		{
			return m_stkImageObjects.Pop();
		}
		return new GameObject("ImagePanel", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
	}

	private void ReturnObject(RectTransform rect)
	{
		m_stkImageObjects.Push(rect);
		rect.gameObject.SetActive(value: false);
		rect.parent = base.transform;
	}

	private void ProvideData(RectTransform rect, int idx)
	{
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.sizeDelta = Vector2.zero;
		int childCount = m_UIComDragPanel.m_rtContentRect.childCount;
		int num = idx;
		if (childCount <= idx)
		{
			num = idx % childCount;
			foreach (BannerObj item2 in m_lstBanner)
			{
				if (item2.bannerIdx == num)
				{
					NKCUtil.SetGameobjectActive(item2.bannerObj, bValue: false);
				}
			}
		}
		else
		{
			foreach (BannerObj item3 in m_lstBanner)
			{
				if (item3.bannerIdx != idx && item3.bannerIdx % childCount == idx)
				{
					NKCUtil.SetGameobjectActive(item3.bannerObj, bValue: false);
				}
			}
		}
		if (m_Templet.lstImages[idx].IsSprite)
		{
			Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(m_Templet.lstImages[idx].GUIDE_BUNDLE_PATH, m_Templet.lstImages[idx].GUIDE_IMAGE_PATH);
			if (orLoadAssetResource != null)
			{
				Image component = rect.GetComponent<Image>();
				NKCUtil.SetImageSprite(component, orLoadAssetResource);
				component.enabled = true;
			}
			return;
		}
		Image component2 = rect.GetComponent<Image>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
		BannerObj bannerObj = m_lstBanner.Find((BannerObj e) => e.dataIdx == idx);
		if (bannerObj.bannerObj != null)
		{
			NKCUtil.SetGameobjectActive(bannerObj.bannerObj, bValue: true);
			bannerObj.bannerIdx = idx;
			bannerObj.bannerObj.transform.SetParent(rect);
			return;
		}
		GameObject banner = GetBanner(m_Templet.lstImages[idx].GUIDE_BUNDLE_PATH, m_Templet.lstImages[idx].GUIDE_IMAGE_PATH);
		if (banner != null)
		{
			banner.transform.SetParent(rect, worldPositionStays: false);
			BannerObj item = new BannerObj
			{
				bannerIdx = idx,
				bannerObj = banner,
				dataIdx = idx
			};
			m_lstBanner.Add(item);
		}
	}

	private void Focus(RectTransform rect, bool bFocus)
	{
		Image component = rect.GetComponent<Image>();
		if (bFocus)
		{
			component.DOColor(new Color(1f, 1f, 1f, 1f), 0.4f);
		}
		else
		{
			component.DOColor(new Color(1f, 1f, 1f, 0.5f), 0.4f);
		}
	}

	private Sprite OpenSprite(int index)
	{
		if (m_Templet == null)
		{
			return null;
		}
		if (index < 0)
		{
			return null;
		}
		if (index >= m_Templet.lstImages.Count)
		{
			return null;
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>(m_Templet.lstImages[index].GUIDE_BUNDLE_PATH, m_Templet.lstImages[index].GUIDE_IMAGE_PATH);
	}

	private GameObject GetBanner(string path, string name)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(path, name);
		if (nKCAssetInstanceData != null && nKCAssetInstanceData.m_Instant != null)
		{
			m_lstAssetInstanceData.Add(nKCAssetInstanceData);
			return nKCAssetInstanceData.m_Instant.gameObject;
		}
		return null;
	}

	private void ClearBannerData()
	{
		foreach (NKCAssetInstanceData lstAssetInstanceDatum in m_lstAssetInstanceData)
		{
			NKCAssetResourceManager.CloseInstance(lstAssetInstanceDatum);
		}
		m_lstAssetInstanceData.Clear();
		m_lstBanner.Clear();
	}
}
