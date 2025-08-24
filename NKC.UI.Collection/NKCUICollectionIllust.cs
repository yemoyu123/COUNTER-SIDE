using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionIllust : MonoBehaviour
{
	public delegate void OnIllustView(int CatagoryID, int BGGroupID);

	public NKCUICollectionIllustSlot m_pfCollectionAlbumSlot;

	public LoopVerticalScrollFlexibleRect m_LoopVerticalScrollFlexibleRect;

	public RectTransform m_rtNKM_UI_COLLECTION_ALBUM_Pool;

	private Stack<RectTransform> m_stkCollectionSlotPool = new Stack<RectTransform>();

	private NKCUICollection.OnSyncCollectingData dOnSyncCollectingData;

	public NKCUIIllustSlot m_pfUISlot;

	public RectTransform m_rtIllustSlotPool;

	private Stack<RectTransform> m_stkIllustSlotPool = new Stack<RectTransform>();

	private int m_iClearCount;

	private int m_iTotalCount;

	private string ILLUST_VIEW_ASSET_NAME = "NKM_UI_COLLECTION_ILLUST_VIEW";

	private string ILLUST_VIEW_BUNDLE_NAME = "ab_ui_nkm_ui_collection";

	private NKCUICollectionIllustView m_NKCUICollectionIllustView;

	private static NKCAssetInstanceData m_AssetInstanceData;

	private bool m_bPrepareCollectionSlot;

	private List<int> m_lstIllustSlot = new List<int>();

	public void Init(NKCUICollection.OnSyncCollectingData callBack)
	{
		if (null != m_LoopVerticalScrollFlexibleRect)
		{
			m_LoopVerticalScrollFlexibleRect.dOnGetObject += MakeIllustSlot;
			m_LoopVerticalScrollFlexibleRect.dOnReturnObject += ReturnIllustSlot;
			m_LoopVerticalScrollFlexibleRect.dOnProvideData += ProvideIllustSlotData;
		}
		if (callBack != null)
		{
			dOnSyncCollectingData = callBack;
		}
		m_bPrepareCollectionSlot = false;
		ReserveUISlot(40);
	}

	private void SyncCollectingUnitData()
	{
		if (dOnSyncCollectingData != null)
		{
			dOnSyncCollectingData(NKCUICollectionGeneral.CollectionType.CT_ILLUST, m_iClearCount, m_iTotalCount);
		}
	}

	private RectTransform MakeIllustSlot(int index)
	{
		if (m_stkCollectionSlotPool.Count > 0)
		{
			RectTransform rectTransform = m_stkCollectionSlotPool.Pop();
			NKCUtil.SetGameobjectActive(rectTransform, bValue: true);
			return rectTransform;
		}
		NKCUICollectionIllustSlot nKCUICollectionIllustSlot = Object.Instantiate(m_pfCollectionAlbumSlot);
		nKCUICollectionIllustSlot.Init();
		nKCUICollectionIllustSlot.transform.localPosition = Vector3.zero;
		nKCUICollectionIllustSlot.transform.localScale = Vector3.one;
		return nKCUICollectionIllustSlot.GetComponent<RectTransform>();
	}

	private void ReturnIllustSlot(Transform go)
	{
		NKCUICollectionIllustSlot component = go.GetComponent<NKCUICollectionIllustSlot>();
		List<RectTransform> rentalSlot = component.GetRentalSlot();
		for (int i = 0; i < rentalSlot.Count; i++)
		{
			rentalSlot[i].SetParent(m_rtIllustSlotPool);
			m_stkIllustSlotPool.Push(rentalSlot[i]);
		}
		component.ClearRentalList();
		NKCUtil.SetGameobjectActive(go, bValue: false);
		go.SetParent(m_rtNKM_UI_COLLECTION_ALBUM_Pool);
		m_stkCollectionSlotPool.Push(go.GetComponent<RectTransform>());
	}

	private void ProvideIllustSlotData(Transform tr, int idx)
	{
		NKCUICollectionIllustSlot component = tr.GetComponent<NKCUICollectionIllustSlot>();
		if (!(component == null))
		{
			List<RectTransform> uISlot = GetUISlot(m_lstIllustSlot[idx]);
			component.SetData(m_lstIllustSlot[idx], uISlot, IllustView);
		}
	}

	private List<RectTransform> GetUISlot(int CategoryID)
	{
		List<RectTransform> list = new List<RectTransform>();
		NKCCollectionIllustTemplet illustTemplet = NKCCollectionManager.GetIllustTemplet(CategoryID);
		if (illustTemplet != null)
		{
			for (int i = 0; i < illustTemplet.m_dicIllustData.Count; i++)
			{
				if (m_stkIllustSlotPool.Count > 0)
				{
					RectTransform item = m_stkIllustSlotPool.Pop();
					list.Add(item);
					continue;
				}
				NKCUIIllustSlot nKCUIIllustSlot = Object.Instantiate(m_pfUISlot);
				nKCUIIllustSlot.transform.localPosition = Vector3.zero;
				nKCUIIllustSlot.transform.localScale = Vector3.one;
				RectTransform component = nKCUIIllustSlot.GetComponent<RectTransform>();
				list.Add(component);
			}
		}
		return list;
	}

	private void ReserveUISlot(int size)
	{
		for (int i = 0; i < size; i++)
		{
			NKCUIIllustSlot nKCUIIllustSlot = Object.Instantiate(m_pfUISlot);
			nKCUIIllustSlot.transform.localPosition = Vector3.zero;
			nKCUIIllustSlot.transform.localScale = Vector3.one;
			RectTransform component = nKCUIIllustSlot.GetComponent<RectTransform>();
			m_stkIllustSlotPool.Push(component);
		}
	}

	public void IllustView(int CategoryID, int BGGroupID)
	{
		if (null == m_NKCUICollectionIllustView)
		{
			m_AssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(ILLUST_VIEW_BUNDLE_NAME, ILLUST_VIEW_ASSET_NAME);
			if (m_AssetInstanceData.m_Instant != null)
			{
				RectTransform component = NKCUIManager.OpenUI("NUF_COMMON_Panel").GetComponent<RectTransform>();
				m_AssetInstanceData.m_Instant.transform.SetParent(component.transform, worldPositionStays: false);
				m_NKCUICollectionIllustView = m_AssetInstanceData.m_Instant.GetComponent<NKCUICollectionIllustView>();
				m_NKCUICollectionIllustView.Init();
				m_NKCUICollectionIllustView.Open(CategoryID, BGGroupID);
			}
		}
		else
		{
			m_NKCUICollectionIllustView.Open(CategoryID, BGGroupID);
		}
	}

	public void Open()
	{
		m_iClearCount = 0;
		m_iTotalCount = 0;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			Debug.Log("NKCUICollectionAlbum - Curret User Data is null");
			return;
		}
		m_lstIllustSlot.Clear();
		foreach (KeyValuePair<int, NKCCollectionIllustTemplet> illustDatum in NKCCollectionManager.GetIllustData())
		{
			m_lstIllustSlot.Add(illustDatum.Key);
			foreach (KeyValuePair<int, NKCCollectionIllustData> dicIllustDatum in illustDatum.Value.m_dicIllustData)
			{
				bool flag = NKMContentUnlockManager.IsContentUnlocked(nKMUserData, new UnlockInfo(dicIllustDatum.Value.m_UnlockReqType, dicIllustDatum.Value.m_UnlockReqValue));
				dicIllustDatum.Value.SetClearState(flag);
				if (flag)
				{
					m_iClearCount++;
				}
				m_iTotalCount++;
			}
		}
		if (!m_bPrepareCollectionSlot)
		{
			m_bPrepareCollectionSlot = true;
			m_LoopVerticalScrollFlexibleRect.TotalCount = m_lstIllustSlot.Count;
			m_LoopVerticalScrollFlexibleRect.PrepareCells();
			m_LoopVerticalScrollFlexibleRect.velocity = new Vector2(0f, 0f);
			m_LoopVerticalScrollFlexibleRect.SetIndexPosition(0);
		}
		SyncCollectingUnitData();
	}

	public void Clear()
	{
		if (m_AssetInstanceData != null)
		{
			m_AssetInstanceData.Unload();
		}
	}
}
