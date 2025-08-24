using System.Collections;
using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Core.Util;
using NKM;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComChat : MonoBehaviour
{
	public delegate void OnClose();

	public delegate void OnSendMessage(long channelUid, ChatMessageType messageType, string message, int emotionId);

	[Header("상단")]
	public Text m_lbTitle;

	public NKCUIComStateButton m_btnInfo;

	public NKCUIComStateButton m_btnClose;

	[Header("프리팹")]
	public NKCPopupChatSlot m_ChatSlot;

	public NKCPopupEmoticonSlotSD m_pfbEmoticon;

	public NKCPopupEmoticonSlotComment m_pfbComment;

	[Header("채팅 루프")]
	public LoopScrollFlexibleRect m_loopScroll;

	public Transform m_trContentParent;

	public Transform m_trObjPool;

	[Header("하단 입력부")]
	public NKCUIComStateButton m_btnEmotion;

	public TMP_InputField m_IFMessage;

	public Text m_lbPlaceholder;

	public NKCUIComStateButton m_btnSend;

	[Header("이모티콘")]
	public Animator m_aniBottom;

	public GameObject m_objEmoticonSet;

	[Header("이모티콘 즐겨찾기")]
	public NKCUIComToggle m_tglFavorite;

	public GameObject m_objFavoriteNotExist;

	[Space]
	public NKCUIComToggle m_tglEmoticon;

	public LoopScrollRect m_loopEmoticon;

	public Transform m_trEmoticonParent;

	[Header("코멘트")]
	public NKCUIComToggle m_tglComment;

	public LoopScrollRect m_loopComment;

	public Transform m_trCommentParent;

	[Header("채팅 차단 표시")]
	public GameObject m_objMute;

	public Text m_lbMuteRemainTime;

	[Header("최근 사용")]
	public NKCUIComToggle m_tglEmoticonRecent;

	private OnClose m_dOnClose;

	private OnSendMessage m_dOnSendMessage;

	private Stack<NKCPopupChatSlot> m_stkSlot = new Stack<NKCPopupChatSlot>();

	private List<NKCPopupChatSlot> m_lstVisibleSlot = new List<NKCPopupChatSlot>();

	private Stack<NKCPopupEmoticonSlotSD> m_stkEmoticonSlot = new Stack<NKCPopupEmoticonSlotSD>();

	private Stack<NKCPopupEmoticonSlotComment> m_stkCommentSlot = new Stack<NKCPopupEmoticonSlotComment>();

	private List<NKMChatMessageData> m_lstChatMessages = new List<NKMChatMessageData>();

	private List<int> m_lstEmoticon = new List<int>();

	private List<int> m_lstComment = new List<int>();

	private string m_Message = "";

	private long m_CurChannelUid;

	private bool m_bWaitForData = true;

	private int m_SelectedEmoticonID;

	private float m_fDeltaTime;

	private bool m_bEnableMute;

	private bool m_bEmotionInitComplete;

	private bool m_bCommentInitComplete;

	private bool m_bDisableTranslate;

	private bool m_bEmoticonFavorite;

	public void InitUI(OnSendMessage dOnSendMessage, OnClose dOnClose, bool bDisableTranslate)
	{
		m_bEmotionInitComplete = false;
		m_bCommentInitComplete = false;
		m_dOnSendMessage = dOnSendMessage;
		m_dOnClose = dOnClose;
		m_bDisableTranslate = bDisableTranslate;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loopScroll.dOnGetObject += GetObject;
		m_loopScroll.dOnReturnObject += ReturnObject;
		m_loopScroll.dOnProvideData += ProvideData;
		if (m_tglEmoticon != null)
		{
			m_tglEmoticon.OnValueChanged.RemoveAllListeners();
			m_tglEmoticon.OnValueChanged.AddListener(OnClickEmoticonTab);
		}
		if (m_tglComment != null)
		{
			m_tglComment.OnValueChanged.RemoveAllListeners();
			m_tglComment.OnValueChanged.AddListener(OnClickCommentTab);
		}
		if (m_loopEmoticon != null)
		{
			m_loopEmoticon.dOnGetObject += GetObjectEmoticon;
			m_loopEmoticon.dOnReturnObject += ReturnObjectEmoticon;
			m_loopEmoticon.dOnProvideData += ProvideDataEmoticon;
		}
		if (m_loopComment != null)
		{
			m_loopComment.dOnGetObject += GetObjectComment;
			m_loopComment.dOnReturnObject += ReturnObjectComment;
			m_loopComment.dOnProvideData += ProvideDataComment;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (m_btnEmotion != null)
		{
			m_btnEmotion.PointerClick.RemoveAllListeners();
			m_btnEmotion.PointerClick.AddListener(OnClickEmotion);
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener(OnSelectIF);
		m_IFMessage.GetComponent<EventTrigger>().triggers.Add(entry);
		m_IFMessage.onEndEdit.RemoveAllListeners();
		m_IFMessage.onEndEdit.AddListener(OnMessageChanged);
		m_btnSend.PointerClick.RemoveAllListeners();
		m_btnSend.PointerClick.AddListener(OnClickSend);
		if (m_btnInfo != null)
		{
			m_btnInfo.PointerClick.RemoveAllListeners();
			m_btnInfo.PointerClick.AddListener(OnInfo);
		}
		if (m_btnClose != null)
		{
			m_btnClose.PointerClick.RemoveAllListeners();
			m_btnClose.PointerClick.AddListener(Close);
		}
		NKCUtil.SetToggleValueChangedDelegate(m_tglFavorite, OnClickFavoriteFilter);
		NKCUtil.SetToggleValueChangedDelegate(m_tglEmoticonRecent, OnClickEmoticonRecentTab);
	}

	public void Close()
	{
		if (m_lstChatMessages.Count > 0)
		{
			NKCChatManager.SetLastCheckedMeesageUid(m_CurChannelUid, m_lstChatMessages[m_lstChatMessages.Count - 1].messageUid);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_dOnClose?.Invoke();
	}

	public void SetData(long defaultChannel = 0L, bool bEnableMute = false, string title = "")
	{
		m_IFMessage.text = "";
		m_Message = "";
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_CurChannelUid = defaultChannel;
		m_bEnableMute = bEnableMute;
		NKCUtil.SetLabelText(m_lbTitle, title);
		if (m_aniBottom != null)
		{
			m_aniBottom.Play("CONSORTIUM_CHAT_Bottom_CLOSE_IDLE");
		}
		NKCUtil.SetGameobjectActive(m_objEmoticonSet, bValue: false);
		m_lstChatMessages = NKCChatManager.GetChatList(defaultChannel, out m_bWaitForData);
		if (m_bEnableMute)
		{
			CheckMute();
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objMute, bValue: false);
		}
		m_btnEmotion?.UnLock();
		m_tglEmoticon.Select(bSelect: true, bForce: true);
		NKCUtil.SetGameobjectActive(m_loopEmoticon, bValue: true);
		NKCUtil.SetGameobjectActive(m_loopComment, bValue: false);
		if (NKMOpenTagManager.IsOpened("TAG_NOTICE_CHAT_ADVICE"))
		{
			NKCUtil.SetLabelText(m_lbPlaceholder, NKCStringTable.GetString("SI_PF_NOTICE_CHAT_ADVICE"));
		}
		else if (NKCGuildManager.MyGuildData != null && m_CurChannelUid == NKCGuildManager.MyGuildData.guildUid)
		{
			NKCUtil.SetLabelText(m_lbPlaceholder, NKCStringTable.GetString("SI_PF_CONSORTIUM_CHAT_COMMENT_EMPTY"));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbPlaceholder, NKCStringTable.GetString("SI_PF_CHAT_COMMENT_EMPTY"));
		}
		StartCoroutine(WaitForData());
	}

	private RectTransform GetObject(int index)
	{
		NKCPopupChatSlot nKCPopupChatSlot = null;
		nKCPopupChatSlot = ((m_stkSlot.Count <= 0) ? Object.Instantiate(m_ChatSlot, m_trContentParent) : m_stkSlot.Pop());
		m_lstVisibleSlot.Add(nKCPopupChatSlot);
		return nKCPopupChatSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCPopupChatSlot component = tr.GetComponent<NKCPopupChatSlot>();
		NKCUtil.SetGameobjectActive(component, bValue: false);
		m_lstVisibleSlot.Remove(component);
		m_stkSlot.Push(component);
		component.transform.SetParent(m_trObjPool);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCPopupChatSlot component = tr.GetComponent<NKCPopupChatSlot>();
		if (idx < 0 || m_lstChatMessages.Count <= idx || m_lstChatMessages[idx].message == null)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(component, bValue: true);
		component.SetData(m_CurChannelUid, m_lstChatMessages[idx], m_bDisableTranslate);
		LayoutRebuilder.ForceRebuildLayoutImmediate(component.GetComponent<RectTransform>());
		if (NKCChatManager.GetLastCheckedMessageUid(m_CurChannelUid) <= 0 || m_lstChatMessages.FindIndex((NKMChatMessageData x) => x.messageUid == NKCChatManager.GetLastCheckedMessageUid(m_CurChannelUid)) < idx)
		{
			component.PlaySDAni();
		}
	}

	private RectTransform GetObjectEmoticon(int idx)
	{
		NKCPopupEmoticonSlotSD nKCPopupEmoticonSlotSD = null;
		nKCPopupEmoticonSlotSD = ((m_stkEmoticonSlot.Count <= 0) ? Object.Instantiate(m_pfbEmoticon) : m_stkEmoticonSlot.Pop());
		nKCPopupEmoticonSlotSD.transform.SetParent(m_trEmoticonParent);
		return nKCPopupEmoticonSlotSD.GetComponent<RectTransform>();
	}

	private void ReturnObjectEmoticon(Transform tr)
	{
		NKCPopupEmoticonSlotSD component = tr.GetComponent<NKCPopupEmoticonSlotSD>();
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		component.transform.SetParent(base.transform);
		m_stkEmoticonSlot.Push(component);
	}

	private void ProvideDataEmoticon(Transform tr, int idx)
	{
		NKCPopupEmoticonSlotSD component = tr.GetComponent<NKCPopupEmoticonSlotSD>();
		component.StopSDAni();
		component.SetClickEvent(OnClickEmoticonSlot);
		component.SetClickEventForChange(null);
		component.SetUI(m_lstEmoticon[idx]);
		component.SetSelectedWithChangeButton(bSet: false);
		component.RemoveCanvas();
		component.transform.localScale = new Vector3(1f, 1f);
	}

	private RectTransform GetObjectComment(int idx)
	{
		NKCPopupEmoticonSlotComment nKCPopupEmoticonSlotComment = null;
		nKCPopupEmoticonSlotComment = ((m_stkCommentSlot.Count <= 0) ? Object.Instantiate(m_pfbComment) : m_stkCommentSlot.Pop());
		nKCPopupEmoticonSlotComment.transform.SetParent(m_trCommentParent);
		return nKCPopupEmoticonSlotComment.GetComponent<RectTransform>();
	}

	private void ReturnObjectComment(Transform tr)
	{
		NKCPopupEmoticonSlotComment component = tr.GetComponent<NKCPopupEmoticonSlotComment>();
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		component.transform.SetParent(base.transform);
		m_stkCommentSlot.Push(component);
	}

	private void ProvideDataComment(Transform tr, int idx)
	{
		NKCPopupEmoticonSlotComment component = tr.GetComponent<NKCPopupEmoticonSlotComment>();
		component.SetUI(m_lstComment[idx]);
		component.SetSelected(bSet: false);
		component.SetClickEvent(OnClickCommentSlot);
		component.SetClickEventForChange(null);
		component.transform.localScale = new Vector3(1f, 1f);
	}

	private void SetMuteRemainTime()
	{
		if (NKCChatManager.m_MuteEndDate > ServiceTime.Recent)
		{
			NKCUtil.SetLabelText(m_lbMuteRemainTime, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_CHAT_INFORMATION_SANCTION_DESC, NKCUtilString.GetRemainTimeString(NKCChatManager.m_MuteEndDate - ServiceTime.Recent, 2)));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objMute, bValue: false);
		}
	}

	private IEnumerator WaitForData()
	{
		while (m_bWaitForData)
		{
			yield return null;
		}
		m_loopScroll.TotalCount = m_lstChatMessages.Count;
		m_loopScroll.RefillCellsFromEnd();
		if (!NKCEmoticonManager.m_bReceivedEmoticonData)
		{
			NKCPacketSender.Send_NKMPacket_EMOTICON_DATA_REQ(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}
		while (!NKCEmoticonManager.m_bReceivedEmoticonData)
		{
			yield return null;
		}
		if (m_lstChatMessages.Count > 0)
		{
			NKCChatManager.SetLastCheckedMeesageUid(m_CurChannelUid, m_lstChatMessages[m_lstChatMessages.Count - 1].messageUid);
		}
	}

	public void AddMessage(NKMChatMessageData data, bool bIsMyMessage, bool bForceResetScroll = false)
	{
		if (bIsMyMessage || data.commonProfile.userUid == m_CurChannelUid)
		{
			RefreshList(bForceResetScroll || (data.commonProfile != null && data.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID));
		}
	}

	public void RefreshList(bool bResetPosition = false)
	{
		m_lstChatMessages = NKCChatManager.GetChatList(m_CurChannelUid, out var _);
		if (m_lstChatMessages == null)
		{
			m_lstChatMessages = new List<NKMChatMessageData>();
		}
		m_loopScroll.TotalCount = m_lstChatMessages.Count;
		if (bResetPosition)
		{
			m_loopScroll.RefillCellsFromEnd();
			m_loopScroll.StopMovement();
			if (m_lstChatMessages.Count > 0)
			{
				NKCChatManager.SetLastCheckedMeesageUid(m_CurChannelUid, m_lstChatMessages[m_lstChatMessages.Count - 1].messageUid);
			}
		}
		else
		{
			m_loopScroll.RefreshCells();
		}
	}

	public void OnChatDataReceived(long channelUid, List<NKMChatMessageData> lstData, bool bRefresh = false)
	{
		if (m_CurChannelUid != channelUid)
		{
			m_bWaitForData = false;
			return;
		}
		m_lstChatMessages = lstData;
		m_bWaitForData = false;
		if (bRefresh)
		{
			RefreshList();
		}
	}

	private void OnSelectIF(BaseEventData cBaseEventData)
	{
		if (m_objEmoticonSet.activeSelf)
		{
			StartCoroutine(OnEmoticonEnable(bEnabled: false));
			m_SelectedEmoticonID = 0;
		}
	}

	private void OnMessageChanged(string str)
	{
		if (str.Length > 70)
		{
			str = str.Substring(0, 70);
		}
		m_Message = NKCFilterManager.CheckBadChat(str);
		m_IFMessage.text = m_Message;
		if (!string.IsNullOrWhiteSpace(m_Message) && NKCInputManager.IsChatSubmitEnter())
		{
			OnClickSend();
			m_IFMessage.ActivateInputField();
		}
	}

	private void OnClickEmotion()
	{
		StartCoroutine(OnEmoticonEnable(!m_objEmoticonSet.activeSelf));
	}

	private void OnClickEmoticonSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		m_SelectedEmoticonID = slotData.ID;
		NKCPopupEmoticonSlotSD[] componentsInChildren = m_trEmoticonParent.GetComponentsInChildren<NKCPopupEmoticonSlotSD>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].GetEmoticonID() == m_SelectedEmoticonID)
			{
				componentsInChildren[i].PlaySDAni();
				componentsInChildren[i].transform.localScale = new Vector3(1.1f, 1.1f);
			}
			else
			{
				componentsInChildren[i].StopSDAni();
				componentsInChildren[i].transform.localScale = new Vector3(1f, 1f);
			}
		}
	}

	private IEnumerator OnEmoticonEnable(bool bEnabled)
	{
		m_btnEmotion?.Lock();
		m_SelectedEmoticonID = 0;
		if (m_objEmoticonSet.activeSelf)
		{
			m_aniBottom.Play("CONSORTIUM_CHAT_Bottom_CLOSE");
			yield return new WaitForSeconds(0.4f);
			NKCUtil.SetGameobjectActive(m_objEmoticonSet, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objEmoticonSet, bValue: true);
			NKCUtil.SetGameobjectActive(m_objFavoriteNotExist, bValue: false);
			m_tglFavorite?.Select(bSelect: false, bForce: true);
			m_bEmoticonFavorite = false;
			m_lstEmoticon = NKCChatManager.GetEmoticons(NKM_EMOTICON_TYPE.NET_ANI);
			m_loopEmoticon.TotalCount = m_lstEmoticon.Count;
			m_lstComment = NKCChatManager.GetEmoticons(NKM_EMOTICON_TYPE.NET_TEXT);
			m_loopComment.TotalCount = m_lstComment.Count;
			if (m_tglEmoticon.IsSelected)
			{
				m_lstEmoticon = NKCChatManager.GetEmoticons(NKM_EMOTICON_TYPE.NET_ANI);
				m_loopEmoticon.TotalCount = m_lstEmoticon.Count;
				m_loopEmoticon.SetAutoResize(6);
				if (!m_bEmotionInitComplete)
				{
					m_loopEmoticon.PrepareCells();
					m_bEmotionInitComplete = true;
				}
				m_loopEmoticon.RefreshCells();
			}
			else if (m_tglEmoticonRecent.IsSelected)
			{
				m_lstEmoticon = NKCEmoticonManager.GetEmoticonRecent(m_bEmoticonFavorite);
				m_loopEmoticon.TotalCount = m_lstEmoticon.Count;
				m_loopEmoticon.SetAutoResize(6);
				if (!m_bEmotionInitComplete)
				{
					m_loopEmoticon.PrepareCells();
					m_bEmotionInitComplete = true;
				}
				m_loopEmoticon.RefreshCells();
			}
			else if (m_tglComment.IsSelected)
			{
				if (!m_bCommentInitComplete)
				{
					m_loopComment.PrepareCells();
					m_bCommentInitComplete = true;
				}
				m_loopComment.RefreshCells();
			}
			yield return new WaitForSeconds(0.4f);
			m_aniBottom.Play("CONSORTIUM_CHAT_Bottom_OPEN");
		}
		m_btnEmotion?.UnLock();
	}

	public void RefreshEmoticonList()
	{
		if (m_objEmoticonSet == null || !m_objEmoticonSet.activeInHierarchy)
		{
			return;
		}
		if (m_tglEmoticon.IsSelected)
		{
			if (m_bEmoticonFavorite)
			{
				m_lstEmoticon = NKCChatManager.GetEmoticons(NKM_EMOTICON_TYPE.NET_ANI, m_bEmoticonFavorite);
				m_loopEmoticon.TotalCount = m_lstEmoticon.Count;
			}
			m_loopEmoticon.RefreshCells();
			NKCUtil.SetGameobjectActive(m_objFavoriteNotExist, m_bEmoticonFavorite && m_lstEmoticon.Count <= 0);
		}
		else if (m_tglEmoticonRecent.IsSelected)
		{
			if (m_bEmoticonFavorite)
			{
				m_lstEmoticon = NKCEmoticonManager.GetEmoticonRecent(m_bEmoticonFavorite);
				m_loopEmoticon.TotalCount = m_lstEmoticon.Count;
			}
			m_loopEmoticon.RefreshCells();
			NKCUtil.SetGameobjectActive(m_objFavoriteNotExist, m_bEmoticonFavorite && m_lstEmoticon.Count <= 0);
		}
		else if (m_tglComment.IsSelected)
		{
			if (m_bEmoticonFavorite)
			{
				m_lstComment = NKCChatManager.GetEmoticons(NKM_EMOTICON_TYPE.NET_TEXT, m_bEmoticonFavorite);
				m_loopComment.TotalCount = m_lstComment.Count;
			}
			m_loopComment.RefreshCells();
			NKCUtil.SetGameobjectActive(m_objFavoriteNotExist, m_bEmoticonFavorite && m_lstComment.Count <= 0);
		}
	}

	private void OnClickCommentSlot(int emoticonID)
	{
		m_SelectedEmoticonID = emoticonID;
		NKCPopupEmoticonSlotComment[] componentsInChildren = m_trCommentParent.GetComponentsInChildren<NKCPopupEmoticonSlotComment>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].GetEmoticonID() == m_SelectedEmoticonID)
			{
				componentsInChildren[i].SetSelected(bSet: true);
				componentsInChildren[i].transform.localScale = new Vector3(1.1f, 1.1f);
			}
			else
			{
				componentsInChildren[i].SetSelected(bSet: false);
				componentsInChildren[i].transform.localScale = new Vector3(1f, 1f);
			}
		}
	}

	private void OnClickSend()
	{
		if (!string.IsNullOrWhiteSpace(m_Message) || m_SelectedEmoticonID != 0)
		{
			if (!string.IsNullOrWhiteSpace(m_Message))
			{
				m_dOnSendMessage?.Invoke(m_CurChannelUid, ChatMessageType.Normal, m_Message, 0);
			}
			else
			{
				m_dOnSendMessage?.Invoke(m_CurChannelUid, ChatMessageType.Normal, m_Message, m_SelectedEmoticonID);
				NKCEmoticonManager.SaveEmoticonRecent(m_SelectedEmoticonID);
			}
			m_IFMessage.text = "";
			m_Message = "";
			m_SelectedEmoticonID = 0;
			if (m_objEmoticonSet.activeSelf)
			{
				StartCoroutine(OnEmoticonEnable(bEnabled: false));
			}
		}
	}

	private void OnInfo()
	{
		NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(m_CurChannelUid, NKM_DECK_TYPE.NDT_NORMAL);
	}

	private void Update()
	{
		if (m_bEnableMute && m_objMute.activeSelf)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				SetMuteRemainTime();
			}
		}
		if (!Input.GetKeyDown(KeyCode.Return))
		{
			Input.GetKeyDown(KeyCode.KeypadEnter);
		}
	}

	public void CheckMute()
	{
		m_fDeltaTime = 0f;
		NKCUtil.SetGameobjectActive(m_objMute, NKCChatManager.m_MuteEndDate > ServiceTime.Recent);
		if (m_objMute.activeSelf)
		{
			SetMuteRemainTime();
		}
	}

	public long GetChannelUid()
	{
		return m_CurChannelUid;
	}

	public void OnGuildDataChanged()
	{
		if (NKCGuildManager.HasGuild() && !NKCGuildManager.IsGuildMemberByUID(m_CurChannelUid))
		{
			Close();
		}
	}

	public void OnScreenResolutionChanged()
	{
		m_bEmotionInitComplete = false;
		m_bCommentInitComplete = false;
	}

	private void OnClickEmoticonTab(bool bValue)
	{
		if (bValue)
		{
			NKCUtil.SetGameobjectActive(m_loopEmoticon, bValue: true);
			NKCUtil.SetGameobjectActive(m_loopComment, bValue: false);
			m_lstEmoticon = NKCChatManager.GetEmoticons(NKM_EMOTICON_TYPE.NET_ANI, m_bEmoticonFavorite);
			m_loopEmoticon.TotalCount = m_lstEmoticon.Count;
			NKCUtil.SetGameobjectActive(m_objFavoriteNotExist, m_bEmoticonFavorite && m_lstEmoticon.Count <= 0);
			if (!m_bEmotionInitComplete)
			{
				m_loopEmoticon.PrepareCells();
				m_bEmotionInitComplete = true;
			}
			m_loopEmoticon.SetIndexPosition(0);
		}
	}

	private void OnClickCommentTab(bool bValue)
	{
		if (bValue)
		{
			NKCUtil.SetGameobjectActive(m_loopEmoticon, bValue: false);
			NKCUtil.SetGameobjectActive(m_loopComment, bValue: true);
			NKCUtil.SetGameobjectActive(m_objFavoriteNotExist, m_bEmoticonFavorite && m_lstComment.Count <= 0);
			if (!m_bCommentInitComplete)
			{
				m_loopComment.PrepareCells();
				m_bCommentInitComplete = true;
			}
			m_loopComment.SetIndexPosition(0);
		}
	}

	private void OnClickEmoticonRecentTab(bool bValue)
	{
		if (bValue)
		{
			NKCUtil.SetGameobjectActive(m_loopEmoticon, bValue: true);
			NKCUtil.SetGameobjectActive(m_loopComment, bValue: false);
			m_lstEmoticon = NKCEmoticonManager.GetEmoticonRecent(m_bEmoticonFavorite);
			m_loopEmoticon.TotalCount = m_lstEmoticon.Count;
			NKCUtil.SetGameobjectActive(m_objFavoriteNotExist, m_bEmoticonFavorite && m_lstEmoticon.Count <= 0);
			if (!m_bEmotionInitComplete)
			{
				m_loopEmoticon.PrepareCells();
				m_bEmotionInitComplete = true;
			}
			m_loopEmoticon.SetIndexPosition(0);
		}
	}

	private void OnClickFavoriteFilter(bool value)
	{
		m_bEmoticonFavorite = value;
		m_lstComment = NKCChatManager.GetEmoticons(NKM_EMOTICON_TYPE.NET_TEXT, m_bEmoticonFavorite);
		m_loopComment.TotalCount = m_lstComment.Count;
		if (m_tglEmoticon.IsSelected)
		{
			m_lstEmoticon = NKCChatManager.GetEmoticons(NKM_EMOTICON_TYPE.NET_ANI, m_bEmoticonFavorite);
			m_loopEmoticon.TotalCount = m_lstEmoticon.Count;
			m_loopEmoticon.SetIndexPosition(0);
			NKCUtil.SetGameobjectActive(m_objFavoriteNotExist, m_bEmoticonFavorite && m_lstEmoticon.Count <= 0);
		}
		else if (m_tglEmoticonRecent.IsSelected)
		{
			m_lstEmoticon = NKCEmoticonManager.GetEmoticonRecent(m_bEmoticonFavorite);
			m_loopEmoticon.TotalCount = m_lstEmoticon.Count;
			m_loopEmoticon.SetIndexPosition(0);
			NKCUtil.SetGameobjectActive(m_objFavoriteNotExist, m_bEmoticonFavorite && m_lstEmoticon.Count <= 0);
		}
		else if (m_tglComment.IsSelected)
		{
			m_loopComment.SetIndexPosition(0);
			NKCUtil.SetGameobjectActive(m_objFavoriteNotExist, m_bEmoticonFavorite && m_lstComment.Count <= 0);
		}
	}
}
