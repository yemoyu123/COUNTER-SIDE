using System.Collections.Generic;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupOperationSubStoryList : NKCUIBase
{
	public delegate void OnSelectedSlot(int episodeID);

	private const string ASSET_BUNDLE_NAME = "AB_UI_OPERATION";

	private const string UI_ASSET_NAME = "AB_UI_POPUP_OPERATION_SUB_SHORTCUT";

	private static NKCPopupOperationSubStoryList m_Instance;

	public TMP_Text m_Title;

	public NKCUIComStateButton m_btnClose;

	[Space]
	public NKCUIComToggle m_tglSort;

	public NKCUIOperationSubStorySlot m_pfbSlot;

	public LoopScrollRect m_loop;

	private Stack<NKCUIOperationSubStorySlot> m_stkSlot = new Stack<NKCUIOperationSubStorySlot>();

	private Dictionary<bool, List<NKMEpisodeTempletV2>> m_dicData = new Dictionary<bool, List<NKMEpisodeTempletV2>>();

	private bool m_bShowSupplement;

	private OnSelectedSlot m_dOnSelectedSlot;

	public static NKCPopupOperationSubStoryList Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupOperationSubStoryList>("AB_UI_OPERATION", "AB_UI_POPUP_OPERATION_SUB_SHORTCUT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupOperationSubStoryList>();
				m_Instance.Initialize();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static bool isOpen()
	{
		if (m_Instance != null)
		{
			return m_Instance.IsOpen;
		}
		return false;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void Initialize()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_tglSort.OnValueChanged.RemoveAllListeners();
		m_tglSort.OnValueChanged.AddListener(OnTgl);
	}

	private RectTransform GetObject(int idx)
	{
		NKCUIOperationSubStorySlot nKCUIOperationSubStorySlot = null;
		if (m_stkSlot.Count > 0)
		{
			nKCUIOperationSubStorySlot = m_stkSlot.Pop();
		}
		else
		{
			nKCUIOperationSubStorySlot = Object.Instantiate(m_pfbSlot, m_loop.content);
			nKCUIOperationSubStorySlot.InitUI(OnClickSlot);
		}
		return nKCUIOperationSubStorySlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUIOperationSubStorySlot component = tr.GetComponent<NKCUIOperationSubStorySlot>();
		NKCUtil.SetGameobjectActive(component, bValue: false);
		m_stkSlot.Push(component);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUIOperationSubStorySlot component = tr.GetComponent<NKCUIOperationSubStorySlot>();
		NKCUtil.SetGameobjectActive(component, bValue: true);
		component.SetEpisodeID(m_dicData[m_bShowSupplement][idx].m_EpisodeID);
		component.SetData(!m_bShowSupplement);
	}

	public void Open(OnSelectedSlot onSelected, bool bSupplement)
	{
		m_dOnSelectedSlot = onSelected;
		if (isOpen())
		{
			if (m_bShowSupplement == bSupplement)
			{
				Close();
			}
			else
			{
				SetData(bSupplement);
			}
		}
		else
		{
			m_tglSort.Select(bSelect: true, bForce: true);
			BuildData();
			SetData(bSupplement);
			UIOpened();
		}
	}

	public void SetData(bool bSupplement)
	{
		m_bShowSupplement = bSupplement;
		NKCUtil.SetLabelText(m_Title, string.Format(NKCUtilString.GET_STRING_OPERATION_SUBSTREAM_SHORTCUT_TITLE, m_bShowSupplement ? NKCUtilString.GET_STRING_EPISODE_SUPPLEMENT : NKCUtilString.GET_STRING_EPISODE_CATEGORY_EC_SIDESTORY));
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loop.TotalCount = m_dicData[m_bShowSupplement].Count;
		m_loop.SetIndexPosition(0);
	}

	private void BuildData()
	{
		m_dicData.Clear();
		m_dicData.Add(key: true, new List<NKMEpisodeTempletV2>());
		m_dicData.Add(key: false, new List<NKMEpisodeTempletV2>());
		List<NKMEpisodeTempletV2> listNKMEpisodeTempletByCategory = NKMEpisodeMgr.GetListNKMEpisodeTempletByCategory(EPISODE_CATEGORY.EC_SIDESTORY);
		for (int i = 0; i < listNKMEpisodeTempletByCategory.Count; i++)
		{
			m_dicData[listNKMEpisodeTempletByCategory[i].m_bIsSupplement].Add(listNKMEpisodeTempletByCategory[i]);
		}
		m_dicData[true].Sort(SortBySortIndex);
		m_dicData[false].Sort(SortBySortIndex);
	}

	private int SortBySortIndex(NKMEpisodeTempletV2 lItem, NKMEpisodeTempletV2 rItem)
	{
		return lItem.m_SortIndex.CompareTo(rItem.m_SortIndex);
	}

	private int SortBySortIndexRevert(NKMEpisodeTempletV2 lItem, NKMEpisodeTempletV2 rItem)
	{
		return rItem.m_SortIndex.CompareTo(lItem.m_SortIndex);
	}

	private void OnClickSlot(int episodeID)
	{
		m_dOnSelectedSlot(episodeID);
	}

	private void OnTgl(bool bValue)
	{
		m_tglSort.Select(bValue, bForce: true);
		if (bValue)
		{
			m_dicData[true].Sort(SortBySortIndex);
			m_dicData[false].Sort(SortBySortIndex);
		}
		else
		{
			m_dicData[true].Sort(SortBySortIndexRevert);
			m_dicData[false].Sort(SortBySortIndexRevert);
		}
		SetData(m_bShowSupplement);
	}
}
