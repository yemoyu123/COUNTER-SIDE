using System.Collections.Generic;
using ClientPacket.Community;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitReview : NKCUIBase
{
	private const int MAX_REVIEW_COUNT = 9999;

	private const int MAX_MY_COMMENT_VIEW_LENGTH = 25;

	private const int COMMENT_COUNT_PER_PAGE = 10;

	private const string UI_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_unit_review";

	private const string UI_ASSET_NAME = "NKM_UI_UNIT_REVIEW";

	private static NKCUIUnitReview m_Instance;

	public NKCUIUnitReviewSlot m_pfbReviewSlot;

	[Header("좌측 캐릭터 정보 / 평점")]
	public NKCUICharacterView m_CharacterView;

	public Text m_lbUnitClass;

	public Text m_lbUnitTitle;

	public Text m_lbUnitName;

	public Text m_lbCurScore;

	public GameObject m_objMyScoreComplete;

	public Text m_lbMyScore;

	public NKCUIComStateButton m_btnScore;

	[Header("상단 메뉴")]
	public Text m_lbReviewCount;

	public NKCUIComStateButton m_btnSortByDate;

	public NKCUIComStateButton m_btnSortByVoted;

	[Header("우측 리뷰 목록")]
	public LoopScrollRect m_loopScrollRect;

	public Transform m_trSlotParent;

	public NKCUIComStateButton m_btnRegistReview;

	public InputField m_input;

	public Text m_lbReviewText;

	public Text m_lbReviewInputCount;

	private List<NKCUIUnitReviewSlot> m_lstReviewSlot = new List<NKCUIUnitReviewSlot>();

	private Stack<NKCUIUnitReviewSlot> m_stkReviewSlot = new Stack<NKCUIUnitReviewSlot>();

	private List<NKMUnitReviewCommentData> m_lstCommentDataOrderByLike = new List<NKMUnitReviewCommentData>();

	private List<NKMUnitReviewCommentData> m_lstCommentDataOrderByNew = new List<NKMUnitReviewCommentData>();

	private List<NKMUnitReviewCommentData> m_lstBestCommentData = new List<NKMUnitReviewCommentData>();

	private NKMUnitReviewScoreData m_cScoreData = new NKMUnitReviewScoreData();

	private bool m_bInitComplete;

	private bool m_bSortByLike;

	private int m_nRequestedPageByNew;

	private int m_nRequestedPageLike;

	private bool m_bIsLastPageByNew;

	private bool m_bIsLastPageByLike;

	private bool m_bMyReviewExist;

	private NKMUnitTempletBase m_cUnitTempletBase;

	private bool m_bFirstOpen = true;

	public static NKCUIUnitReview Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIUnitReview>("ab_ui_nkm_ui_unit_review", "NKM_UI_UNIT_REVIEW", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIUnitReview>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

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

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_UNIT_REVIEW;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		m_cUnitTempletBase = null;
		m_lstCommentDataOrderByLike.Clear();
		m_lstCommentDataOrderByNew.Clear();
		m_lstBestCommentData.Clear();
		m_cScoreData = new NKMUnitReviewScoreData();
		NKCUtil.SetGameobjectActive(m_btnSortByVoted, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnSortByDate, bValue: true);
		m_bSortByLike = false;
		m_bIsLastPageByNew = false;
		m_bIsLastPageByLike = false;
		m_bFirstOpen = true;
		m_nRequestedPageByNew = 0;
		m_nRequestedPageLike = 0;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void NullReferenceCheck()
	{
		if (m_pfbReviewSlot == null)
		{
			Debug.LogError("m_pfbReviewSlot is null");
		}
		if (m_CharacterView == null)
		{
			Debug.LogError("m_CharacterView is null");
		}
		if (m_lbUnitClass == null)
		{
			Debug.LogError("m_lbUnitClass is null");
		}
		if (m_lbUnitTitle == null)
		{
			Debug.LogError("m_lbUnitTitle is null");
		}
		if (m_lbUnitName == null)
		{
			Debug.LogError("m_lbUnitName is null");
		}
		if (m_lbCurScore == null)
		{
			Debug.LogError("m_lbCurScore is null");
		}
		if (m_objMyScoreComplete == null)
		{
			Debug.LogError("m_objMyScoreComplete is null");
		}
		if (m_lbMyScore == null)
		{
			Debug.LogError("m_lbMyScore is null");
		}
		if (m_btnScore == null)
		{
			Debug.LogError("m_btnScore is null");
		}
		if (m_lbReviewCount == null)
		{
			Debug.LogError("m_lbReviewCount is null");
		}
		if (m_btnSortByDate == null)
		{
			Debug.LogError("m_btnSortByDate is null");
		}
		if (m_btnSortByVoted == null)
		{
			Debug.LogError("m_btnSortByVoted is null");
		}
		if (m_loopScrollRect == null)
		{
			Debug.LogError("m_loopScrollRect is null");
		}
		if (m_trSlotParent == null)
		{
			Debug.LogError("m_trSlotParent is null");
		}
		if (m_btnRegistReview == null)
		{
			Debug.LogError("m_btnRegistReview is null");
		}
		if (m_input == null)
		{
			Debug.LogError("m_input is null");
		}
		if (m_lbReviewText == null)
		{
			Debug.LogError("m_lbReviewText is null");
		}
		if (m_lbReviewInputCount == null)
		{
			Debug.LogError("m_lbReviewInputCount is null");
		}
	}

	private void InitUI()
	{
		NullReferenceCheck();
		m_CharacterView.Init();
		m_btnScore.PointerClick.RemoveAllListeners();
		m_btnScore.PointerClick.AddListener(OnClickScore);
		m_btnSortByDate.PointerClick.RemoveAllListeners();
		m_btnSortByDate.PointerClick.AddListener(OnClickSortByDateToVote);
		m_btnSortByVoted.PointerClick.RemoveAllListeners();
		m_btnSortByVoted.PointerClick.AddListener(OnClickSortByVoteToDate);
		m_btnRegistReview.PointerClick.RemoveAllListeners();
		m_btnRegistReview.PointerClick.AddListener(OnClickRegistReview);
		m_loopScrollRect.dOnGetObject += OnGetObject;
		m_loopScrollRect.dOnReturnObject += OnReturnObject;
		m_loopScrollRect.dOnProvideData += OnProvideData;
		m_loopScrollRect.ContentConstraintCount = 1;
		m_loopScrollRect.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loopScrollRect);
		m_input.onValidateInput = NKCFilterManager.FilterEmojiInput;
		m_input.onValueChanged.RemoveAllListeners();
		m_input.onValueChanged.AddListener(OnChangeInput);
		m_input.onEndEdit.RemoveAllListeners();
		m_input.onEndEdit.AddListener(OnEndEditReview);
		m_bInitComplete = true;
	}

	public void OpenUI(int unitID)
	{
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.UNIT_REVIEW_SYSTEM))
		{
			return;
		}
		if (!m_bInitComplete)
		{
			InitUI();
		}
		m_cUnitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (m_cUnitTempletBase != null)
		{
			UIOpened();
			m_nRequestedPageByNew++;
			if (!NKCUnitReviewManager.m_bReceivedUnitReviewBanList)
			{
				NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_USER_BAN_LIST_REQ();
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_REQ(unitID);
			}
			SetUnitData(m_cUnitTempletBase);
		}
	}

	private void SetUnitData(NKMUnitTempletBase unitTempletBase)
	{
		m_CharacterView.SetCharacterIllust(unitTempletBase);
		NKCUtil.SetLabelText(m_lbUnitClass, NKCUtilString.GetUnitStyleString(unitTempletBase));
		NKCUtil.SetLabelText(m_lbUnitTitle, unitTempletBase.GetUnitTitle());
		NKCUtil.SetLabelText(m_lbUnitName, unitTempletBase.GetUnitName());
	}

	private void SetCommentList(List<NKMUnitReviewCommentData> commentDataList)
	{
		int num = ((commentDataList != null) ? commentDataList.Count : 0);
		int i;
		for (i = commentDataList.Count - 1; i >= 0; i--)
		{
			if (m_lstBestCommentData.Find((NKMUnitReviewCommentData x) => x.userUID == commentDataList[i].userUID) != null)
			{
				commentDataList.RemoveAt(i);
			}
		}
		if (commentDataList != null)
		{
			if (m_bSortByLike)
			{
				if (num < 10)
				{
					m_bIsLastPageByLike = true;
				}
				m_lstCommentDataOrderByLike.AddRange(commentDataList);
				m_loopScrollRect.TotalCount = m_lstCommentDataOrderByLike.Count;
			}
			else
			{
				if (num < 10)
				{
					m_bIsLastPageByNew = true;
				}
				m_lstCommentDataOrderByNew.AddRange(commentDataList);
				m_loopScrollRect.TotalCount = m_lstCommentDataOrderByNew.Count;
			}
		}
		else if (m_bSortByLike)
		{
			m_loopScrollRect.TotalCount = m_lstCommentDataOrderByLike.Count;
			m_bIsLastPageByLike = true;
		}
		else
		{
			m_loopScrollRect.TotalCount = m_lstCommentDataOrderByNew.Count;
			m_bIsLastPageByNew = true;
		}
		m_loopScrollRect.RefreshCells();
		int num2 = (m_bSortByLike ? m_lstCommentDataOrderByLike.Count : m_lstCommentDataOrderByNew.Count);
		if (m_bMyReviewExist && m_lstBestCommentData.Find((NKMUnitReviewCommentData x) => x.userUID == NKCScenManager.CurrentUserData().m_UserUID) == null)
		{
			num2--;
		}
		if (num2 > 9999)
		{
			NKCUtil.SetLabelText(m_lbReviewCount, $"{num2}+");
		}
		else
		{
			NKCUtil.SetLabelText(m_lbReviewCount, num2.ToString());
		}
		if (m_bFirstOpen)
		{
			m_bFirstOpen = false;
			OnClickSortByVoteToDate();
		}
	}

	private void SetScoreData(NKMUnitReviewScoreData scoreData)
	{
		m_cScoreData = scoreData;
		if (scoreData != null && scoreData.votedCount > 0)
		{
			NKCUtil.SetLabelText(m_lbCurScore, $"{scoreData.avgScore:F1}");
			if (scoreData.myScore > 0)
			{
				NKCUtil.SetGameobjectActive(m_objMyScoreComplete, bValue: true);
				NKCUtil.SetLabelText(m_lbMyScore, scoreData.myScore.ToString());
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objMyScoreComplete, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbCurScore, "  -  ");
			NKCUtil.SetGameobjectActive(m_objMyScoreComplete, bValue: false);
		}
	}

	public void RecvReviewData(NKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_ACK sPacket)
	{
		m_lstCommentDataOrderByLike.Clear();
		m_lstCommentDataOrderByNew.Clear();
		if (sPacket.myUnitReviewCommentData != null && !string.IsNullOrEmpty(sPacket.myUnitReviewCommentData.content))
		{
			m_bMyReviewExist = true;
			if (sPacket.bestUnitReviewCommentDataList != null && sPacket.bestUnitReviewCommentDataList.Find((NKMUnitReviewCommentData x) => x.userUID == sPacket.myUnitReviewCommentData.userUID) == null)
			{
				m_lstCommentDataOrderByNew.Add(sPacket.myUnitReviewCommentData);
				m_lstCommentDataOrderByLike.Add(sPacket.myUnitReviewCommentData);
			}
		}
		else
		{
			m_bMyReviewExist = false;
		}
		if (sPacket.bestUnitReviewCommentDataList != null)
		{
			m_lstBestCommentData = sPacket.bestUnitReviewCommentDataList;
			m_lstCommentDataOrderByLike.AddRange(sPacket.bestUnitReviewCommentDataList);
			m_lstCommentDataOrderByNew.AddRange(sPacket.bestUnitReviewCommentDataList);
		}
		m_input.text = "";
		m_lbReviewInputCount.text = "0/130";
		if (sPacket.unitReviewCommentDataList != null)
		{
			SetCommentList(sPacket.unitReviewCommentDataList);
		}
		SetScoreData(sPacket.unitReviewScoreData);
	}

	public void RecvCommentList(List<NKMUnitReviewCommentData> commentDataList)
	{
		SetCommentList(commentDataList);
	}

	public void RecvScoreVoteAck(int unitID, NKMUnitReviewScoreData scoreData)
	{
		if (unitID == m_cUnitTempletBase.m_UnitID)
		{
			SetScoreData(scoreData);
		}
	}

	public void RecvMyCommentChanged(NKMUnitReviewCommentData myReview)
	{
		m_lstCommentDataOrderByNew.Clear();
		m_lstCommentDataOrderByLike.Clear();
		if (m_bSortByLike)
		{
			m_nRequestedPageByNew = 0;
			m_nRequestedPageLike = 1;
		}
		else
		{
			m_nRequestedPageByNew = 1;
			m_nRequestedPageLike = 0;
		}
		m_bIsLastPageByLike = false;
		m_bIsLastPageByNew = false;
		if (myReview != null)
		{
			if (m_lstBestCommentData.Find((NKMUnitReviewCommentData x) => x.userUID == myReview.userUID) == null)
			{
				m_lstCommentDataOrderByNew.Add(myReview);
				m_lstCommentDataOrderByLike.Add(myReview);
			}
			m_bMyReviewExist = true;
		}
		else
		{
			m_bMyReviewExist = false;
		}
		m_lstCommentDataOrderByNew.AddRange(m_lstBestCommentData);
		m_lstCommentDataOrderByLike.AddRange(m_lstBestCommentData);
		m_input.text = string.Empty;
		NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_COMMENT_LIST_REQ(m_cUnitTempletBase.m_UnitID, 1, m_bSortByLike);
	}

	public void RecvCommentVote(int unitID, NKMUnitReviewCommentData commentData, bool bVote)
	{
		if (unitID == m_cUnitTempletBase.m_UnitID)
		{
			List<NKMUnitReviewCommentData> list = m_lstCommentDataOrderByLike.FindAll((NKMUnitReviewCommentData x) => x.commentUID == commentData.commentUID);
			for (int num = 0; num < list.Count; num++)
			{
				list[num].votedCount = commentData.votedCount;
				list[num].isVoted = commentData.isVoted;
			}
			list = m_lstCommentDataOrderByNew.FindAll((NKMUnitReviewCommentData x) => x.commentUID == commentData.commentUID);
			for (int num2 = 0; num2 < list.Count; num2++)
			{
				list[num2].votedCount = commentData.votedCount;
				list[num2].isVoted = commentData.isVoted;
			}
			List<NKCUIUnitReviewSlot> list2 = m_lstReviewSlot.FindAll((NKCUIUnitReviewSlot x) => x.GetCommentUID() == commentData.commentUID);
			for (int num3 = 0; num3 < list2.Count; num3++)
			{
				list2[num3].ChangeVotedCount(commentData.votedCount, bVote);
			}
		}
	}

	public void OnRecvBanList()
	{
		NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_REQ(m_cUnitTempletBase.m_UnitID);
	}

	public void RefreshUI()
	{
		m_loopScrollRect.RefreshCells();
	}

	private void OnClickScore()
	{
		NKCPopupUnitReviewScore.Instance.OpenUI(m_cScoreData, OnVoteScore);
	}

	private void OnClickSortByDateToVote()
	{
		if (!m_bSortByLike)
		{
			NKCUtil.SetGameobjectActive(m_btnSortByVoted, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnSortByDate, bValue: false);
			m_loopScrollRect.SetIndexPosition(0);
			m_bSortByLike = true;
			if (m_nRequestedPageLike > 0)
			{
				m_loopScrollRect.RefreshCells();
				return;
			}
			m_nRequestedPageLike = 1;
			NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_COMMENT_LIST_REQ(m_cUnitTempletBase.m_UnitID, m_nRequestedPageLike, m_bSortByLike);
		}
	}

	private void OnClickSortByVoteToDate()
	{
		if (m_bSortByLike)
		{
			NKCUtil.SetGameobjectActive(m_btnSortByVoted, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnSortByDate, bValue: true);
			m_bSortByLike = false;
			m_loopScrollRect.SetIndexPosition(0);
			if (m_nRequestedPageByNew > 0)
			{
				m_loopScrollRect.RefreshCells();
				return;
			}
			m_nRequestedPageByNew = 1;
			NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_COMMENT_LIST_REQ(m_cUnitTempletBase.m_UnitID, m_nRequestedPageByNew, m_bSortByLike);
		}
	}

	private void OnChangeInput(string inputText)
	{
		m_lbReviewInputCount.text = $"{inputText.Length} / 130";
	}

	private void OnEndEditReview(string inputText)
	{
		if (NKCInputManager.IsChatSubmitEnter())
		{
			if (!m_btnRegistReview.m_bLock)
			{
				OnClickRegistReview();
			}
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	private void OnClickRegistReview()
	{
		if (!string.IsNullOrWhiteSpace(m_input.text))
		{
			if (m_bMyReviewExist)
			{
				NKCPopupUnitReviewDelete.Instance.OpenUI(NKCUtilString.GET_STRING_POPUP_UNIT_REVIEW_DELETE, NKCUtilString.GET_STRING_REVIEW_DELETE_AND_WRITE, OnRewrite);
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_COMMENT_WRITE_REQ(m_cUnitTempletBase.m_UnitID, m_input.text, bRewrite: false);
			}
		}
	}

	private void OnRewrite()
	{
		NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_COMMENT_WRITE_REQ(m_cUnitTempletBase.m_UnitID, m_input.text, bRewrite: true);
	}

	private void OnVoteScore(int votedScore)
	{
		NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_SCORE_VOTE_REQ(m_cUnitTempletBase.m_UnitID, votedScore);
	}

	public RectTransform OnGetObject(int index)
	{
		if (m_stkReviewSlot.Count == 0)
		{
			NKCUIUnitReviewSlot nKCUIUnitReviewSlot = Object.Instantiate(m_pfbReviewSlot, m_trSlotParent);
			nKCUIUnitReviewSlot.SetData(null, 0, bBest: false);
			nKCUIUnitReviewSlot.transform.localScale = Vector3.one;
			m_lstReviewSlot.Add(nKCUIUnitReviewSlot);
			NKCUtil.SetGameobjectActive(nKCUIUnitReviewSlot.gameObject, bValue: true);
			return nKCUIUnitReviewSlot.GetComponent<RectTransform>();
		}
		NKCUIUnitReviewSlot nKCUIUnitReviewSlot2 = m_stkReviewSlot.Pop();
		m_lstReviewSlot.Add(nKCUIUnitReviewSlot2);
		NKCUtil.SetGameobjectActive(nKCUIUnitReviewSlot2.gameObject, bValue: true);
		return nKCUIUnitReviewSlot2.GetComponent<RectTransform>();
	}

	public void OnReturnObject(Transform go)
	{
		NKCUIUnitReviewSlot component = go.GetComponent<NKCUIUnitReviewSlot>();
		m_lstReviewSlot.Remove(component);
		m_stkReviewSlot.Push(component);
		NKCUtil.SetGameobjectActive(component.gameObject, bValue: false);
		go.SetParent(base.transform);
	}

	public void OnProvideData(Transform transform, int idx)
	{
		NKCUIUnitReviewSlot component = transform.GetComponent<NKCUIUnitReviewSlot>();
		if (m_bSortByLike)
		{
			if (m_lstCommentDataOrderByLike.Count > idx)
			{
				bool bBest = m_lstBestCommentData.Find((NKMUnitReviewCommentData x) => x.commentUID == m_lstCommentDataOrderByLike[idx].commentUID) != null;
				component.SetData(m_lstCommentDataOrderByLike[idx], m_cUnitTempletBase.m_UnitID, bBest);
				if (!component.gameObject.activeInHierarchy)
				{
					NKCUtil.SetGameobjectActive(component.gameObject, bValue: true);
				}
			}
			else
			{
				Debug.LogWarning($"리뷰데이터가 없음 - idx : {idx}");
			}
			if (!m_bIsLastPageByLike && idx == m_lstCommentDataOrderByLike.Count - 1)
			{
				m_nRequestedPageLike++;
				NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_COMMENT_LIST_REQ(m_cUnitTempletBase.m_UnitID, m_nRequestedPageLike, m_bSortByLike);
			}
			return;
		}
		if (m_lstCommentDataOrderByNew.Count > idx)
		{
			bool bBest2 = m_lstBestCommentData.Find((NKMUnitReviewCommentData x) => x.commentUID == m_lstCommentDataOrderByNew[idx].commentUID) != null;
			component.SetData(m_lstCommentDataOrderByNew[idx], m_cUnitTempletBase.m_UnitID, bBest2);
			if (!component.gameObject.activeInHierarchy)
			{
				NKCUtil.SetGameobjectActive(component.gameObject, bValue: true);
			}
		}
		else
		{
			Debug.LogWarning($"리뷰데이터가 없음 - idx : {idx}");
		}
		if (!m_bIsLastPageByNew && idx == m_lstCommentDataOrderByNew.Count - 1)
		{
			m_nRequestedPageByNew++;
			NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_COMMENT_LIST_REQ(m_cUnitTempletBase.m_UnitID, m_nRequestedPageByNew, m_bSortByLike);
		}
	}
}
