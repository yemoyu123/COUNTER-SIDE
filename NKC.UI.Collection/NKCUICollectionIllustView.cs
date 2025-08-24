using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionIllustView : NKCUIBase
{
	public struct IllustData
	{
		public bool IsSpine;

		public int InstanceIdx;

		public string AssetName;

		public string AniName;

		public IllustData(bool spine, int idx, string name = "", string bgName = "")
		{
			IsSpine = spine;
			InstanceIdx = idx;
			AssetName = name;
			AniName = bgName;
		}
	}

	public RectTransform m_rt_NKM_UI_COLLECTION_ALBUM_VIEW_THUMBNAIL;

	public Text m_NKM_UI_COLLECTION_ALBUM_VIEW_TEXT_01;

	public Text m_NKM_UI_COLLECTION_ALBUM_VIEW_TEXT_02;

	public NKCUIComStateButton m_csbtn_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_PREVIOUS;

	public NKCUIComStateButton m_csbtn_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_NEXT;

	public Image m_Img_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_PREVIOUS;

	public Image m_Img_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_NEXT;

	public NKCUIComStateButton m_csbtn_NKM_UI_COLLECTION_ALBUM_VIEW_CLICK_AREA;

	public Image m_NKM_UI_COLLECTION_ILLUST_VIEW_THUMBNAIL;

	private int m_iSelectIndex = -1;

	private List<IllustData> m_lstIllust = new List<IllustData>();

	private List<NKCAssetInstanceData> m_ListAssetInstance = new List<NKCAssetInstanceData>();

	private RectTransform m_rtGoBG;

	private Vector2 m_orgPos = Vector2.zero;

	private Vector2 m_OffsetPos = Vector2.zero;

	private Vector3 m_OrgGOScale = Vector3.one;

	private Vector3 m_OffsetScale = Vector3.one;

	private bool m_bCanMovePrev;

	private bool m_bCanMoveNext;

	private bool m_bShowSubUI = true;

	public override string MenuName => "";

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.BackButtonOnly;

	public override void OnBackButton()
	{
		base.OnBackButton();
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		for (int i = 0; i < m_ListAssetInstance.Count; i++)
		{
			m_ListAssetInstance[i].m_Instant.transform.SetParent(null);
			m_ListAssetInstance[i].Unload();
		}
		m_ListAssetInstance.Clear();
	}

	public void Init()
	{
		if (null != m_csbtn_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_PREVIOUS)
		{
			m_csbtn_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_PREVIOUS.PointerClick.AddListener(MovePrev);
		}
		if (null != m_csbtn_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_NEXT)
		{
			m_csbtn_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_NEXT.PointerClick.AddListener(MoveNext);
		}
		if (null != m_csbtn_NKM_UI_COLLECTION_ALBUM_VIEW_CLICK_AREA)
		{
			m_csbtn_NKM_UI_COLLECTION_ALBUM_VIEW_CLICK_AREA.PointerClick.AddListener(OnSubUI);
		}
	}

	public void Open(int CategoryID, int BGGroupID)
	{
		SetData(CategoryID, BGGroupID);
		UIOpened();
	}

	private void SetData(int CategoryID, int BGGroupID)
	{
		m_iSelectIndex = -1;
		NKCCollectionIllustTemplet illustTemplet = NKCCollectionManager.GetIllustTemplet(CategoryID);
		if (illustTemplet != null && illustTemplet.m_dicIllustData.ContainsKey(BGGroupID))
		{
			m_ListAssetInstance.Clear();
			NKCCollectionIllustData nKCCollectionIllustData = illustTemplet.m_dicIllustData[BGGroupID];
			NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_ALBUM_VIEW_TEXT_01, nKCCollectionIllustData.m_BGGroupTitle);
			NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_ALBUM_VIEW_TEXT_02, nKCCollectionIllustData.m_BGGroupText);
			m_lstIllust.Clear();
			for (int i = 0; i < nKCCollectionIllustData.m_FileData.Count; i++)
			{
				SetIllustData(nKCCollectionIllustData.m_FileData[i].m_BGFileName, nKCCollectionIllustData.m_FileData[i].m_BGFileName, nKCCollectionIllustData.m_FileData[i].m_GameObjectBGAniName);
			}
			UpdateBG(0);
		}
	}

	private void SetIllustData(string bundleName, string assName, string BGAniName)
	{
		if (assName.Contains("AB_UI_NKM_UI_CUTSCEN"))
		{
			NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assName);
			if (nKCAssetInstanceData.m_Instant != null)
			{
				nKCAssetInstanceData.m_Instant.transform.SetParent(m_rt_NKM_UI_COLLECTION_ALBUM_VIEW_THUMBNAIL);
				NKCUtil.SetGameobjectActive(nKCAssetInstanceData.m_Instant, bValue: false);
				PrepareBackground(nKCAssetInstanceData.m_Instant);
				m_ListAssetInstance.Add(nKCAssetInstanceData);
			}
			else
			{
				Debug.Log("Create 실패! 썸네일!");
			}
			m_lstIllust.Add(new IllustData(spine: true, m_ListAssetInstance.Count - 1, "", BGAniName));
		}
		else
		{
			m_lstIllust.Add(new IllustData(spine: false, -1, assName));
		}
	}

	public static Sprite GetThumbnail(string AssetName)
	{
		string text = "AB_UI_NKM_UI_CUTSCEN_BG_" + AssetName;
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(text, text);
		if (orLoadAssetResource == null)
		{
			Debug.LogError("can not found sprite " + AssetName);
		}
		return orLoadAssetResource;
	}

	private void PrepareBackground(GameObject objBG)
	{
		RectTransform component = objBG.GetComponent<RectTransform>();
		if (component != null)
		{
			m_rtGoBG = component;
			float a = NKCUIManager.UIFrontCanvasSafeRectTransform.GetWidth() / 1920f;
			float b = NKCUIManager.UIFrontCanvasSafeRectTransform.GetHeight() / 1080f;
			float num = Mathf.Max(a, b);
			m_OrgGOScale = new Vector3(num, num, 1f);
			component.localScale = new Vector3(m_OrgGOScale.x * m_OffsetScale.x, m_OrgGOScale.y * m_OffsetScale.y, m_OrgGOScale.z * m_OffsetScale.z);
			component.offsetMin = new Vector2(0f, 0f);
			component.offsetMax = new Vector2(0f, 0f);
			component.anchoredPosition = m_OffsetPos;
			m_orgPos = component.anchoredPosition;
		}
		else
		{
			m_rtGoBG = null;
			objBG.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	private void UpdateBG(int selIdx)
	{
		if (selIdx == -1 || m_lstIllust.Count <= selIdx || m_iSelectIndex == selIdx)
		{
			return;
		}
		if (m_iSelectIndex != -1)
		{
			if (m_lstIllust[m_iSelectIndex].IsSpine)
			{
				NKCUtil.SetGameobjectActive(m_ListAssetInstance[m_lstIllust[m_iSelectIndex].InstanceIdx].m_Instant, bValue: false);
			}
			m_iSelectIndex = selIdx;
			if (m_lstIllust[m_iSelectIndex].IsSpine)
			{
				NKCUtil.SetGameobjectActive(m_ListAssetInstance[m_lstIllust[m_iSelectIndex].InstanceIdx].m_Instant, bValue: true);
				SkeletonGraphic componentInChildren = m_ListAssetInstance[m_lstIllust[m_iSelectIndex].InstanceIdx].m_Instant.GetComponentInChildren<SkeletonGraphic>();
				if (componentInChildren != null && m_lstIllust[m_iSelectIndex].AniName != "")
				{
					componentInChildren.AnimationState.SetAnimation(0, m_lstIllust[m_iSelectIndex].AniName, loop: true);
				}
			}
			else
			{
				NKCUtil.SetImageSprite(m_NKM_UI_COLLECTION_ILLUST_VIEW_THUMBNAIL, GetThumbnail(m_lstIllust[m_iSelectIndex].AssetName));
			}
		}
		m_bCanMovePrev = false;
		m_bCanMoveNext = false;
		if (m_iSelectIndex - 1 >= 0)
		{
			m_bCanMovePrev = true;
		}
		if (m_iSelectIndex + 1 < m_lstIllust.Count)
		{
			m_bCanMoveNext = true;
		}
		UpdateMoveButtonColor();
	}

	private void UpdateMoveButtonColor()
	{
		if (null != m_Img_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_PREVIOUS)
		{
			if (m_bCanMovePrev)
			{
				m_Img_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_PREVIOUS.color = Color.white;
			}
			else
			{
				m_Img_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_PREVIOUS.color = NKCUtil.GetColor("#5D5D64");
			}
		}
		if (null != m_Img_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_NEXT)
		{
			if (m_bCanMoveNext)
			{
				m_Img_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_NEXT.color = Color.white;
			}
			else
			{
				m_Img_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_NEXT.color = NKCUtil.GetColor("#5D5D64");
			}
		}
	}

	public void MoveNext()
	{
		if (m_bCanMoveNext)
		{
			UpdateBG(m_iSelectIndex + 1);
		}
	}

	public void MovePrev()
	{
		if (m_bCanMovePrev)
		{
			UpdateBG(m_iSelectIndex - 1);
		}
	}

	public void OnSubUI()
	{
		if (!m_bShowSubUI)
		{
			m_bShowSubUI = true;
		}
		else
		{
			m_bShowSubUI = false;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_ALBUM_VIEW_TEXT_01.gameObject, m_bShowSubUI);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_ALBUM_VIEW_TEXT_02.gameObject, m_bShowSubUI);
		NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_PREVIOUS.gameObject, m_bShowSubUI);
		NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_ALBUM_VIEW_BUTTON_NEXT.gameObject, m_bShowSubUI);
	}
}
