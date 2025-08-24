using System;
using ClientPacket.Community;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitReviewSlot : MonoBehaviour
{
	private const int MAX_REVIEW_COUNT = 9999;

	public GameObject m_objUserReview;

	public Text m_lbUserLevel;

	public Text m_lbUserName;

	public GameObject m_objUserBest;

	public GameObject m_objMyReviewTag;

	public Text m_lbUserDate;

	public NKCUIComStateButton m_btnDelete;

	public Text m_lbDelete;

	public NKCUIComToggle m_tglUserLike;

	public Text m_lbUserLikeCountOn;

	public Text m_lbUserLikeCountOff;

	public Text m_lbDesc;

	private bool bInitComplete;

	private int m_nUnitID;

	private long m_CommentUID;

	private long m_UserUid;

	private bool m_bIsMyReview;

	private bool m_bIsBannedUser;

	private Color COLOR_MY_REVIEW_TEXT = new Color(1f, 0.8745098f, 31f / 85f);

	private Color COLOR_NORMAL_REVIEW_TEXT = new Color(26f / 85f, 0.7607843f, 81f / 85f);

	private void InitUI()
	{
		m_btnDelete.PointerClick.RemoveAllListeners();
		m_btnDelete.PointerClick.AddListener(OnClickDelete);
		m_tglUserLike.OnValueChanged.RemoveAllListeners();
		m_tglUserLike.OnValueChanged.AddListener(OnClickVote);
		bInitComplete = true;
	}

	public void SetData(NKMUnitReviewCommentData commentData, int unitID, bool bBest)
	{
		if (!bInitComplete)
		{
			InitUI();
		}
		if (commentData != null && !string.IsNullOrEmpty(commentData.content))
		{
			m_nUnitID = unitID;
			m_CommentUID = commentData.commentUID;
			m_UserUid = commentData.userUID;
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
				DateTime dateTime = NKMTime.UTCtoLocal(new DateTime(commentData.regDate));
				m_bIsMyReview = commentData.userUID == nKMUserData.m_UserUID;
				m_bIsBannedUser = NKCUnitReviewManager.IsBannedUser(m_UserUid);
				NKCUtil.SetLabelText(m_lbUserLevel, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, commentData.level);
				NKCUtil.SetLabelText(m_lbUserName, commentData.nickName ?? "");
				NKCUtil.SetGameobjectActive(m_objUserBest, bBest);
				NKCUtil.SetGameobjectActive(m_objMyReviewTag, m_bIsMyReview);
				NKCUtil.SetLabelTextColor(m_lbUserLevel, m_bIsMyReview ? COLOR_MY_REVIEW_TEXT : COLOR_NORMAL_REVIEW_TEXT);
				NKCUtil.SetLabelTextColor(m_lbUserName, m_bIsMyReview ? COLOR_MY_REVIEW_TEXT : COLOR_NORMAL_REVIEW_TEXT);
				NKCUtil.SetLabelText(m_lbUserDate, $"{dateTime.Year}.{dateTime.Month}.{dateTime.Day}");
				m_tglUserLike.Select(commentData.isVoted && !m_bIsMyReview, bForce: true, bImmediate: true);
				m_tglUserLike.enabled = !m_bIsMyReview;
				NKCUtil.SetLabelText(m_lbUserLikeCountOn, (commentData.votedCount > 9999) ? $"{commentData.votedCount}+" : commentData.votedCount.ToString());
				NKCUtil.SetLabelText(m_lbUserLikeCountOff, (commentData.votedCount > 9999) ? $"{commentData.votedCount}+" : commentData.votedCount.ToString());
				NKCUtil.SetLabelText(m_lbDelete, GetDeleteDesc());
				NKCUtil.SetLabelText(m_lbDesc, m_bIsBannedUser ? NKCUtilString.GET_STRING_REVIEW_BANNED_CONTENT : commentData.content);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
	}

	private string GetDeleteDesc()
	{
		if (m_bIsMyReview)
		{
			return NKCUtilString.GET_STRING_POPUP_UNIT_REVIEW_DELETE;
		}
		if (!m_bIsBannedUser)
		{
			return NKCUtilString.GET_STRING_REVIEW_BAN;
		}
		return NKCUtilString.GET_STRING_REVIEW_UNBAN;
	}

	public void ChangeVotedCount(int changedVotedCount, bool bVote)
	{
		m_tglUserLike.Select(bVote, bForce: true, bImmediate: true);
		NKCUtil.SetLabelText(m_lbUserLikeCountOn, (changedVotedCount > 9999) ? $"{changedVotedCount}+" : changedVotedCount.ToString());
		NKCUtil.SetLabelText(m_lbUserLikeCountOff, (changedVotedCount > 9999) ? $"{changedVotedCount}+" : changedVotedCount.ToString());
	}

	public long GetCommentUID()
	{
		return m_CommentUID;
	}

	private void OnClickDelete()
	{
		if (m_bIsMyReview)
		{
			NKCPopupUnitReviewDelete.Instance.OpenUI(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_REVIEW_DELETE, OnDelete);
		}
		else if (m_bIsBannedUser)
		{
			NKCPopupUnitReviewDelete.Instance.OpenUI(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_REVIEW_BAN_CANCEL_DESC, OnUnBan);
		}
		else
		{
			NKCPopupUnitReviewDelete.Instance.OpenUI(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_REVIEW_BAN_DESC, OnBan);
		}
	}

	private void OnDelete()
	{
		NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_COMMENT_DELETE_REQ(m_nUnitID);
	}

	private void OnBan()
	{
		NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_USER_BAN_REQ(m_UserUid);
	}

	private void OnUnBan()
	{
		NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_USER_BAN_CANCEL_REQ(m_UserUid);
	}

	private void OnClickVote(bool bSelect)
	{
		if (bSelect)
		{
			NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_COMMENT_VOTE_REQ(m_nUnitID, m_CommentUID);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_COMMENT_VOTE_CANCEL_REQ(m_nUnitID, m_CommentUID);
		}
	}
}
