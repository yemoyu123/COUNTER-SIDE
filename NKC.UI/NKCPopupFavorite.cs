using System.Collections.Generic;
using System.Linq;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupFavorite : NKCUIBase
{
	public delegate void OnClose();

	private const string ASSET_BUNDLE_NAME = "AB_UI_OPERATION";

	private const string UI_ASSET_NAME = "AB_UI_OPERATION_FAV";

	private static NKCPopupFavorite m_Instance;

	public NKCUIComStateButton m_btnClose;

	public NKCPopupFavoriteSlot m_pfbSlot;

	public NKCUIComToggle m_tglAll;

	public NKCUIComToggle m_tglMainStream;

	public NKCUIComToggle m_tglStory;

	public NKCUIComToggle m_tglGrowth;

	public NKCUIComToggle m_tglChallenge;

	public TMP_Text m_lbCategoryName;

	public LoopScrollRect m_Loop;

	public TMP_Text m_lbFavoriteCount;

	public GameObject m_objNone;

	[SerializeField]
	private NKCUIComStateButton m_upButton;

	[SerializeField]
	private NKCUIComStateButton m_downButton;

	[SerializeField]
	private NKCUIComStateButton m_editModeCancelButton;

	[SerializeField]
	private NKCUIComToggle m_editModeToggle;

	[SerializeField]
	private GameObject m_editModeRaycastArea;

	private bool _editMode;

	private Stack<NKCPopupFavoriteSlot> m_stkSlot = new Stack<NKCPopupFavoriteSlot>();

	private Dictionary<EPISODE_GROUP, List<NKMStageTempletV2>> m_dicData = new Dictionary<EPISODE_GROUP, List<NKMStageTempletV2>>();

	private Dictionary<EPISODE_GROUP, NKCUIComToggle> m_dicToggle = new Dictionary<EPISODE_GROUP, NKCUIComToggle>();

	private EPISODE_GROUP m_CurGroupCategory = EPISODE_GROUP.Count;

	private OnClose m_dCallback;

	public static NKCPopupFavorite Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFavorite>("AB_UI_OPERATION", "AB_UI_OPERATION_FAV", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupFavorite>();
				m_Instance.Initialize();
			}
			return m_Instance;
		}
	}

	private int _selectedStageID { get; set; } = int.MinValue;

	private int _selectedIdx { get; set; } = int.MinValue;

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

	public override void Initialize()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_Loop.dOnGetObject += GetObject;
		m_Loop.dOnReturnObject += ReturnObject;
		m_Loop.dOnProvideData += ProvideData;
		m_Loop.PrepareCells();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_CurGroupCategory = EPISODE_GROUP.Count;
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		InitToggle(m_tglAll, EPISODE_GROUP.Count);
		InitToggle(m_tglMainStream, EPISODE_GROUP.EG_MAINSTREAM);
		InitToggle(m_tglStory, EPISODE_GROUP.EG_SUBSTREAM);
		InitToggle(m_tglGrowth, EPISODE_GROUP.EG_GROWTH);
		InitToggle(m_tglChallenge, EPISODE_GROUP.EG_CHALLENGE);
		if (m_upButton != null)
		{
			m_upButton.PointerClick.RemoveAllListeners();
			m_upButton.PointerClick.AddListener(Up);
		}
		if (m_downButton != null)
		{
			m_downButton.PointerClick.RemoveAllListeners();
			m_downButton.PointerClick.AddListener(Down);
		}
		if (m_editModeCancelButton != null)
		{
			m_editModeCancelButton.PointerClick.RemoveAllListeners();
			m_editModeCancelButton.PointerClick.AddListener(CancelEditMode);
		}
		if (m_editModeToggle != null)
		{
			m_editModeToggle.OnValueChanged.RemoveAllListeners();
			m_editModeToggle.OnValueChanged.AddListener(OnChangedEditToggle);
		}
		_editMode = false;
		ResetSelectedSlot();
		ActiveEditMode(active: false);
	}

	private void InitToggle(NKCUIComToggle toggle, EPISODE_GROUP category)
	{
		m_dicToggle.Add(category, toggle);
		toggle.OnValueChanged.RemoveAllListeners();
		toggle.OnValueChanged.AddListener(delegate
		{
			OnClickCategory(category);
		});
	}

	private RectTransform GetObject(int idx)
	{
		NKCPopupFavoriteSlot nKCPopupFavoriteSlot = null;
		if (m_stkSlot.Count > 0)
		{
			nKCPopupFavoriteSlot = m_stkSlot.Pop();
		}
		else
		{
			nKCPopupFavoriteSlot = Object.Instantiate(m_pfbSlot, m_Loop.content);
			nKCPopupFavoriteSlot.InitUI(SetSelectedSlot, () => _editMode);
		}
		return nKCPopupFavoriteSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCPopupFavoriteSlot component = tr.GetComponent<NKCPopupFavoriteSlot>();
		if (!(component == null))
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			m_stkSlot.Push(component);
		}
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCPopupFavoriteSlot component = tr.GetComponent<NKCPopupFavoriteSlot>();
		if (!(component == null))
		{
			NKCUtil.SetGameobjectActive(component, bValue: true);
			if (m_dicData.ContainsKey(m_CurGroupCategory))
			{
				component.SetData(idx, m_dicData[m_CurGroupCategory][idx]);
			}
			else
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
			}
			component.Select(component.IDX == _selectedIdx);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		CancelEditMode();
		m_dCallback?.Invoke();
	}

	public void Open(OnClose callback)
	{
		m_dCallback = callback;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_CurGroupCategory = EPISODE_GROUP.Count;
		RefreshList();
		UIOpened();
		TutorialCheck();
	}

	public void RefreshList()
	{
		BuildFavoriteData();
		foreach (KeyValuePair<EPISODE_GROUP, NKCUIComToggle> item in m_dicToggle)
		{
			item.Value.Select(item.Key == m_CurGroupCategory, bForce: true);
		}
		NKCUtil.SetLabelText(m_lbCategoryName, GetCategoryName(m_CurGroupCategory));
		m_Loop.TotalCount = (m_dicData.ContainsKey(m_CurGroupCategory) ? m_dicData[m_CurGroupCategory].Count : 0);
		if (m_Loop.TotalCount > 0)
		{
			m_Loop.SetIndexPosition(0);
		}
		else
		{
			m_Loop.RefreshCells();
		}
		NKCUtil.SetGameobjectActive(m_objNone, m_Loop.TotalCount == 0);
		NKCUtil.SetLabelText(m_lbFavoriteCount, $"{m_dicData[EPISODE_GROUP.Count].Count}/{NKMCommonConst.MaxStageFavoriteCount}");
	}

	private string GetCategoryName(EPISODE_GROUP category)
	{
		switch (category)
		{
		case EPISODE_GROUP.EG_MAINSTREAM:
		case EPISODE_GROUP.EG_SUBSTREAM:
		case EPISODE_GROUP.EG_GROWTH:
		case EPISODE_GROUP.EG_CHALLENGE:
		{
			NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = NKMEpisodeGroupTemplet.Find(category);
			if (nKMEpisodeGroupTemplet != null)
			{
				return NKCStringTable.GetString(nKMEpisodeGroupTemplet.m_EPGroupName);
			}
			return "";
		}
		default:
			return NKCStringTable.GetString("SI_PF_EPISODE_MENU_FAVORITE_ALL");
		}
	}

	private void BuildFavoriteData()
	{
		m_dicData = new Dictionary<EPISODE_GROUP, List<NKMStageTempletV2>>();
		if (!_editMode)
		{
			Dictionary<int, NKMStageTempletV2> favoriteStageList = NKMEpisodeMgr.GetFavoriteStageList();
			List<NKMStageTempletV2> list = new List<NKMStageTempletV2>();
			List<NKMStageTempletV2> list2 = new List<NKMStageTempletV2>();
			for (int i = 0; i < favoriteStageList.Count; i++)
			{
				if (favoriteStageList.ContainsKey(i))
				{
					if (favoriteStageList[i].EpisodeTemplet.IsOpen && favoriteStageList[i].IsOpenedDayOfWeek())
					{
						list.Add(favoriteStageList[i]);
					}
					else
					{
						list2.Add(favoriteStageList[i]);
					}
				}
			}
			m_dicData.Add(EPISODE_GROUP.Count, new List<NKMStageTempletV2>());
			m_dicData[EPISODE_GROUP.Count].AddRange(list);
			m_dicData[EPISODE_GROUP.Count].AddRange(list2);
			for (int j = 0; j < list.Count; j++)
			{
				NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = NKMEpisodeGroupTemplet.Find(list[j].EpisodeTemplet.m_GroupID);
				if (nKMEpisodeGroupTemplet != null)
				{
					if (!m_dicData.ContainsKey(nKMEpisodeGroupTemplet.GroupCategory))
					{
						m_dicData.Add(nKMEpisodeGroupTemplet.GroupCategory, new List<NKMStageTempletV2>());
					}
					m_dicData[nKMEpisodeGroupTemplet.GroupCategory].Add(list[j]);
				}
			}
			for (int k = 0; k < list2.Count; k++)
			{
				NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet2 = NKMEpisodeGroupTemplet.Find(list2[k].EpisodeTemplet.m_GroupID);
				if (nKMEpisodeGroupTemplet2 != null)
				{
					if (!m_dicData.ContainsKey(nKMEpisodeGroupTemplet2.GroupCategory))
					{
						m_dicData.Add(nKMEpisodeGroupTemplet2.GroupCategory, new List<NKMStageTempletV2>());
					}
					m_dicData[nKMEpisodeGroupTemplet2.GroupCategory].Add(list2[k]);
				}
			}
		}
		else
		{
			Dictionary<int, NKMStageTempletV2> favoriteStageListForEdit = NKMEpisodeMgr.GetFavoriteStageListForEdit();
			m_dicData.Add(EPISODE_GROUP.Count, favoriteStageListForEdit.Values.ToList());
		}
	}

	private void OnClickCategory(EPISODE_GROUP category)
	{
		if (!_editMode)
		{
			m_CurGroupCategory = category;
			NKCUtil.SetGameobjectActive(m_editModeToggle.gameObject, m_CurGroupCategory == EPISODE_GROUP.Count);
			RefreshList();
		}
	}

	public void RefreshData()
	{
		m_Loop.RefreshCells();
	}

	private void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Operation_Favorite);
	}

	private void SetSelectedSlot(NKCPopupFavoriteSlot selectedSlot)
	{
		ResetSelectedSlot();
		selectedSlot.Select(select: true);
		_selectedStageID = selectedSlot.StageID;
		_selectedIdx = selectedSlot.IDX;
	}

	private void Up()
	{
		if (_selectedIdx != int.MinValue)
		{
			Dictionary<int, NKMStageTempletV2> favoriteStageListForEdit = NKMEpisodeMgr.GetFavoriteStageListForEdit();
			if (favoriteStageListForEdit.ContainsKey(_selectedIdx - 1) && favoriteStageListForEdit.ContainsKey(_selectedIdx))
			{
				NKMStageTempletV2 value = favoriteStageListForEdit[_selectedIdx];
				NKMStageTempletV2 value2 = favoriteStageListForEdit[_selectedIdx - 1];
				favoriteStageListForEdit[_selectedIdx] = value2;
				favoriteStageListForEdit[_selectedIdx - 1] = value;
				_selectedIdx--;
				BuildFavoriteData();
				m_Loop.RefreshCells();
			}
		}
	}

	private void Down()
	{
		Dictionary<int, NKMStageTempletV2> favoriteStageListForEdit = NKMEpisodeMgr.GetFavoriteStageListForEdit();
		if (favoriteStageListForEdit.ContainsKey(_selectedIdx + 1) && favoriteStageListForEdit.ContainsKey(_selectedIdx))
		{
			NKMStageTempletV2 value = favoriteStageListForEdit[_selectedIdx];
			NKMStageTempletV2 value2 = favoriteStageListForEdit[_selectedIdx + 1];
			favoriteStageListForEdit[_selectedIdx] = value2;
			favoriteStageListForEdit[_selectedIdx + 1] = value;
			_selectedIdx++;
			BuildFavoriteData();
			m_Loop.RefreshCells();
		}
	}

	public void CancelEditMode()
	{
		if (_editMode)
		{
			ActiveEditMode(active: false);
			ResetSelectedSlot();
			m_editModeToggle.Select(bSelect: false, bForce: true);
			_editMode = false;
			RefreshList();
		}
	}

	private void OnChangedEditToggle(bool bValue)
	{
		if (!bValue || m_CurGroupCategory == EPISODE_GROUP.Count)
		{
			_editMode = bValue;
			if (_editMode)
			{
				NKMEpisodeMgr.SetFavDicForEdit();
				RefreshList();
			}
			else
			{
				SendFavoriteUpdateRequest();
			}
			ResetSelectedSlot();
			ActiveEditMode(_editMode);
		}
	}

	private void ResetSelectedSlot()
	{
		for (int i = 0; i < m_Loop.content.childCount; i++)
		{
			m_Loop.content.GetChild(i).GetComponent<NKCPopupFavoriteSlot>().Select(select: false);
		}
		_selectedIdx = int.MinValue;
		_selectedStageID = int.MinValue;
	}

	private void ActiveEditMode(bool active)
	{
		NKCUtil.SetGameobjectActive(m_upButton, active);
		NKCUtil.SetGameobjectActive(m_downButton, active);
		NKCUtil.SetGameobjectActive(m_editModeCancelButton, active);
		NKCUtil.SetGameobjectActive(m_editModeRaycastArea, active);
	}

	private void SendFavoriteUpdateRequest()
	{
		NKMEpisodeMgr.Send_NKMPacket_FAVORITES_STAGE_UPDATE_REQ();
	}
}
