using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NKC.Templet;
using NKC.UI.Component;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guide;

public class NKCUIPopUpGuide : NKCUIBase
{
	private struct GuideInfo
	{
		public string GUIDE_BUNDLE_PATH;

		public string GUIDE_BUNDLE_NAME;

		public bool IsSprite;

		public GuideInfo(string path, string name, bool bSprite)
		{
			GUIDE_BUNDLE_PATH = path;
			GUIDE_BUNDLE_NAME = name;
			IsSprite = bSprite;
		}
	}

	private struct BannerObj
	{
		public int bannerIdx;

		public int dataIdx;

		public GameObject bannerObj;
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_tutorial";

	private const string UI_ASSET_NAME = "NKM_UI_TOTAL_GUIDE";

	private static NKCUIPopUpGuide m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public ScrollRect m_NKM_UI_TOTAL_GUIDE_SCROLL;

	public NKCUIPopupGuideSlot m_pfbGuideSlot;

	public NKCUIPopupGuideSubSlot m_pbfGuideSubSlot;

	private Dictionary<int, NKCUIPopupGuideSlot> m_dicCategory = new Dictionary<int, NKCUIPopupGuideSlot>();

	private float m_fGuideSlotHeight;

	private string m_strPreOpendArticleID = "";

	[Header("상단 탭")]
	public NKCUIComToggle m_NKM_UI_TOTAL_GUIDE_MANUAL;

	public NKCUIComToggle NKM_UI_TOTAL_GUIDE_GLOSSARY;

	public RectTransform m_rtNKM_UI_TOTAL_GUIDE_CONTENT;

	public NKCUIComStateButton m_NKM_UI_TOTAL_GUIDE_CLOSE;

	public NKCUIComToggleGroup m_CONTENT_TOGGLE_GROUP;

	public NKCUIComDragSelectablePanel m_NKM_UI_TOTAL_GUIDE_IMAGE;

	private Stack<RectTransform> m_stkBannerObjects = new Stack<RectTransform>();

	private string m_selectedArticleID = "";

	private Dictionary<string, List<GuideInfo>> m_dicBundleData = new Dictionary<string, List<GuideInfo>>();

	private List<BannerObj> m_lstBanner = new List<BannerObj>();

	private List<NKCAssetInstanceData> m_lstAssetInstanceData = new List<NKCAssetInstanceData>();

	public static NKCUIPopUpGuide Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopUpGuide>("ab_ui_nkm_ui_tutorial", "NKM_UI_TOTAL_GUIDE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopUpGuide>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public static bool HasInstance => m_Instance != null;

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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "Guide";

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
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		ClearSlot();
	}

	private void Init()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetBindFunction(m_NKM_UI_TOTAL_GUIDE_CLOSE, base.Close);
		if (m_NKM_UI_TOTAL_GUIDE_MANUAL != null)
		{
			m_NKM_UI_TOTAL_GUIDE_MANUAL.OnValueChanged.RemoveAllListeners();
			m_NKM_UI_TOTAL_GUIDE_MANUAL.OnValueChanged.AddListener(OnGuideManual);
		}
		if (NKM_UI_TOTAL_GUIDE_GLOSSARY != null)
		{
			NKM_UI_TOTAL_GUIDE_GLOSSARY.OnValueChanged.RemoveAllListeners();
			NKM_UI_TOTAL_GUIDE_GLOSSARY.OnValueChanged.AddListener(OnGuideGlossary);
		}
		InitUI();
	}

	private void InitUI()
	{
		m_NKM_UI_TOTAL_GUIDE_IMAGE.Init();
		m_NKM_UI_TOTAL_GUIDE_IMAGE.dOnGetObject += MakeBannerObject;
		m_NKM_UI_TOTAL_GUIDE_IMAGE.dOnReturnObject += ReturnObject;
		m_NKM_UI_TOTAL_GUIDE_IMAGE.dOnProvideData += ProvideData;
		m_NKM_UI_TOTAL_GUIDE_IMAGE.dOnFocus += Focus;
	}

	private void InitData()
	{
		for (int i = 0; i < 1000; i++)
		{
			NKCGuideManualTemplet nKCGuideManualTemplet = NKCGuideManualTemplet.Find(i);
			if (nKCGuideManualTemplet == null || m_dicCategory.ContainsKey(nKCGuideManualTemplet.ID))
			{
				continue;
			}
			m_dicCategory.Add(nKCGuideManualTemplet.ID, GetSlot(nKCGuideManualTemplet.GetTitle()));
			foreach (NKCGuideManualTempletData lstManualTemplet in nKCGuideManualTemplet.lstManualTemplets)
			{
				m_dicCategory[nKCGuideManualTemplet.ID].AddSubSlot(GetSubSlot(NKCStringTable.GetString(lstManualTemplet.ARTICLE_STRING_ID), lstManualTemplet.ARTICLE_ID));
				NKCGuideTemplet nKCGuideTemplet = NKCGuideTemplet.Find(lstManualTemplet.GUIDE_ID_STRING);
				if (nKCGuideTemplet == null)
				{
					continue;
				}
				if (!m_dicBundleData.ContainsKey(lstManualTemplet.ARTICLE_ID))
				{
					List<GuideInfo> list = new List<GuideInfo>();
					foreach (NKCGuideTempletImage lstImage in nKCGuideTemplet.lstImages)
					{
						list.Add(new GuideInfo(lstImage.GUIDE_BUNDLE_PATH, lstImage.GUIDE_IMAGE_PATH, lstImage.IsSprite));
					}
					if (list.Count <= 0)
					{
						Debug.Log($"가이드 데이터가 존재하지 않습니다 : id {nKCGuideManualTemplet.ID}({lstManualTemplet.GUIDE_ID_STRING})");
					}
					m_dicBundleData.Add(lstManualTemplet.ARTICLE_ID, list);
				}
				else
				{
					Debug.LogWarning($"중복 타이틀이 있습니다. : id {nKCGuideManualTemplet.ID}({lstManualTemplet.ARTICLE_ID})");
				}
			}
		}
	}

	private NKCUIPopupGuideSlot GetSlot(string title)
	{
		NKCUIPopupGuideSlot nKCUIPopupGuideSlot = Object.Instantiate(m_pfbGuideSlot);
		NKCUtil.SetGameobjectActive(nKCUIPopupGuideSlot, bValue: true);
		nKCUIPopupGuideSlot.Init(title, m_CONTENT_TOGGLE_GROUP);
		nKCUIPopupGuideSlot.GetComponent<RectTransform>().SetParent(m_rtNKM_UI_TOTAL_GUIDE_CONTENT);
		if (m_fGuideSlotHeight == 0f)
		{
			m_fGuideSlotHeight = nKCUIPopupGuideSlot.GetComponent<RectTransform>().GetHeight();
		}
		nKCUIPopupGuideSlot.transform.localScale = Vector3.one;
		return nKCUIPopupGuideSlot;
	}

	private NKCUIPopupGuideSubSlot GetSubSlot(string title, string articleID)
	{
		NKCUIPopupGuideSubSlot nKCUIPopupGuideSubSlot = Object.Instantiate(m_pbfGuideSubSlot);
		NKCUtil.SetGameobjectActive(nKCUIPopupGuideSubSlot, bValue: true);
		nKCUIPopupGuideSubSlot.Init(title, articleID, OnClicked);
		nKCUIPopupGuideSubSlot.GetComponent<RectTransform>().SetParent(m_rtNKM_UI_TOTAL_GUIDE_CONTENT);
		nKCUIPopupGuideSubSlot.transform.localScale = Vector3.one;
		return nKCUIPopupGuideSubSlot;
	}

	public void ClearSlot()
	{
		foreach (KeyValuePair<int, NKCUIPopupGuideSlot> item in m_dicCategory)
		{
			item.Value.Clear();
			Object.Destroy(item.Value.gameObject);
		}
		ClearBannerData();
		m_dicCategory.Clear();
		m_dicBundleData.Clear();
	}

	private void ClearBannerData()
	{
		foreach (NKCAssetInstanceData lstAssetInstanceDatum in m_lstAssetInstanceData)
		{
			lstAssetInstanceDatum.m_Instant.GetComponent<IGuideSubPage>()?.Clear();
			NKCAssetResourceManager.CloseInstance(lstAssetInstanceDatum);
		}
		m_lstAssetInstanceData.Clear();
		m_lstBanner.Clear();
	}

	private void Update()
	{
		if (base.IsOpen && m_NKCUIOpenAnimator != null)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public void Open(string ArticleID = "", int idx = 0)
	{
		InitData();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (string.IsNullOrEmpty(ArticleID))
		{
			m_NKM_UI_TOTAL_GUIDE_IMAGE.TotalCount = 1;
			m_NKM_UI_TOTAL_GUIDE_IMAGE.SetIndex(0);
			m_rtNKM_UI_TOTAL_GUIDE_CONTENT.DOAnchorPosY(0f, 0f);
			m_strPreOpendArticleID = "";
		}
		else
		{
			OnClicked(ArticleID, idx);
			if (!string.Equals(m_strPreOpendArticleID, ArticleID))
			{
				m_NKM_UI_TOTAL_GUIDE_SCROLL.vertical = false;
				int num = 0;
				foreach (KeyValuePair<int, NKCUIPopupGuideSlot> item in m_dicCategory)
				{
					if (item.Value.HasChild(ArticleID))
					{
						m_rtNKM_UI_TOTAL_GUIDE_CONTENT.DOAnchorPosY(m_fGuideSlotHeight * (float)num, 0f);
						break;
					}
					num++;
				}
				StartCoroutine(ChangeScrollVertical());
				m_strPreOpendArticleID = ArticleID;
			}
		}
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	private IEnumerator ChangeScrollVertical()
	{
		yield return new WaitForSeconds(0.33f);
		m_NKM_UI_TOTAL_GUIDE_SCROLL.vertical = true;
	}

	private void OnGuideManual(bool bSel)
	{
		if (bSel)
		{
			OnSelectTab(bSelectManual: true);
		}
	}

	private void OnGuideGlossary(bool bSel)
	{
		if (bSel)
		{
			OnSelectTab(bSelectManual: false);
		}
	}

	public void OnSelectTab(bool bSelectManual)
	{
	}

	public void OnClicked(string ARTICLE_ID, int idx = 0)
	{
		ClearBannerData();
		if (!m_dicBundleData.ContainsKey(ARTICLE_ID))
		{
			Debug.Log("식별할 수 없는 가이드 id : " + ARTICLE_ID);
			Open();
			return;
		}
		m_selectedArticleID = ARTICLE_ID;
		m_NKM_UI_TOTAL_GUIDE_IMAGE.TotalCount = m_dicBundleData[m_selectedArticleID].Count;
		m_NKM_UI_TOTAL_GUIDE_IMAGE.SetIndex(idx);
		foreach (KeyValuePair<int, NKCUIPopupGuideSlot> item in m_dicCategory)
		{
			item.Value.SelectSubSlot(ARTICLE_ID);
		}
	}

	private RectTransform MakeBannerObject()
	{
		if (m_stkBannerObjects.Count > 0)
		{
			return m_stkBannerObjects.Pop();
		}
		return new GameObject("Banner", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
	}

	private void ReturnObject(RectTransform rect)
	{
		m_stkBannerObjects.Push(rect);
		rect.gameObject.SetActive(value: false);
		rect.parent = base.transform;
	}

	private void ProvideData(RectTransform rect, int idx)
	{
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.sizeDelta = Vector2.zero;
		if (string.IsNullOrEmpty(m_selectedArticleID))
		{
			Image component = rect.GetComponent<Image>();
			Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_tutorial_texutre", "AB_UI_NKM_UI_TUTORIAL_TEXUTRE_TITLE");
			if (orLoadAssetResource != null)
			{
				NKCUtil.SetImageSprite(component, orLoadAssetResource);
			}
		}
		else
		{
			if (!m_dicBundleData.ContainsKey(m_selectedArticleID) || idx < 0)
			{
				return;
			}
			List<GuideInfo> list = m_dicBundleData[m_selectedArticleID];
			if (idx >= list.Count)
			{
				return;
			}
			int childCount = m_NKM_UI_TOTAL_GUIDE_IMAGE.m_rtContentRect.childCount;
			int num = idx;
			if (childCount <= idx)
			{
				num = idx % childCount;
			}
			foreach (BannerObj item2 in m_lstBanner)
			{
				if (item2.bannerIdx == num)
				{
					NKCUtil.SetGameobjectActive(item2.bannerObj, bValue: false);
				}
			}
			if (list[idx].IsSprite)
			{
				Sprite orLoadAssetResource2 = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(list[idx].GUIDE_BUNDLE_PATH, list[idx].GUIDE_BUNDLE_NAME);
				if (orLoadAssetResource2 != null)
				{
					Image component2 = rect.GetComponent<Image>();
					NKCUtil.SetImageSprite(component2, orLoadAssetResource2);
					component2.enabled = true;
				}
				return;
			}
			rect.GetComponent<Image>().enabled = false;
			BannerObj bannerObj = m_lstBanner.Find((BannerObj e) => e.dataIdx == idx);
			if (bannerObj.bannerObj != null)
			{
				NKCUtil.SetGameobjectActive(bannerObj.bannerObj, bValue: true);
				bannerObj.bannerIdx = num;
				bannerObj.bannerObj.transform.SetParent(rect);
				return;
			}
			GameObject banner = GetBanner(list[idx].GUIDE_BUNDLE_PATH, list[idx].GUIDE_BUNDLE_NAME);
			if (banner != null)
			{
				if (banner.TryGetComponent<IGuideSubPage>(out var component3))
				{
					component3.SetData();
				}
				banner.transform.SetParent(rect, worldPositionStays: false);
				BannerObj item = new BannerObj
				{
					bannerIdx = num,
					bannerObj = banner,
					dataIdx = idx
				};
				m_lstBanner.Add(item);
			}
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
}
