using System.Collections.Generic;
using ClientPacket.Community;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEmoticonSetting : NKCUIBase
{
	public enum NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE
	{
		NPESRSP_NONE,
		NPESRSP_SD,
		NPESRSP_TEXT
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_EMOTICON";

	private const string UI_ASSET_NAME = "NKM_UI_EMOTICON_DECK_POPUP";

	private static NKCPopupEmoticonSetting m_Instance;

	[Header("공통")]
	public EventTrigger m_etBG;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnCancel;

	[Header("왼쪽")]
	public List<NKCPopupEmoticonSlotSD> m_lstNKCPopupEmoticonSlotSDLeft;

	public List<NKCPopupEmoticonSlotComment> m_lstNKCPopupEmoticonSlotCommentLeft;

	[Header("오른쪽")]
	public GameObject m_objEmptyNotice;

	public Text m_lbEmptyNotice;

	private string m_EmptyNoticeString = "";

	public GameObject m_objSDCollection;

	public LoopVerticalScrollRect m_lvsrSD;

	public GameObject m_objTextCollection;

	public GameObject m_objCommentPreviewNone;

	public GameObject m_objCommentPreview;

	public NKCGameHudEmoticonComment m_NKCGameHudEmoticonCommentPreview;

	public LoopVerticalScrollRect m_lvsrComment;

	private NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE m_eRightSidePage;

	private bool m_bFirstOpenPageSD = true;

	private bool m_bFirstOpenPageComment = true;

	private int m_RightSideSelectedEmoticonIDSD = -1;

	private int m_RightSideSelectedEmoticonIDComment = -1;

	private int m_RightSidePreviewEmoticonIDComment = -1;

	private List<NKCPopupEmoticonSlotSD> m_lstSlotSD = new List<NKCPopupEmoticonSlotSD>();

	private List<NKCPopupEmoticonSlotComment> m_lstSlotComment = new List<NKCPopupEmoticonSlotComment>();

	private List<int> m_lstSDCollectionExceptPreset = new List<int>();

	private List<int> m_lstCommentCollectionExceptPreset = new List<int>();

	private const int SD_SELECTED_SORT_LAYER = 101;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupEmoticonSetting";

	public static NKCPopupEmoticonSetting Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEmoticonSetting>("AB_UI_NKM_UI_EMOTICON", "NKM_UI_EMOTICON_DECK_POPUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEmoticonSetting>();
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

	public void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_etBG.triggers.Clear();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_etBG.triggers.Add(entry);
		NKCUtil.SetBindFunction(m_csbtnClose, base.Close);
		for (int num = 0; num < m_lstNKCPopupEmoticonSlotSDLeft.Count; num++)
		{
			m_lstNKCPopupEmoticonSlotSDLeft[num].SetClickEvent(OnClickLeftEmoticon);
			m_lstNKCPopupEmoticonSlotSDLeft[num].Reset_SD_Scale(0.8f);
		}
		for (int num2 = 0; num2 < m_lstNKCPopupEmoticonSlotCommentLeft.Count; num2++)
		{
			m_lstNKCPopupEmoticonSlotCommentLeft[num2].SetClickEvent(OnClickLeftEmoticon);
		}
		m_lvsrSD.dOnGetObject += GetSDSlot;
		m_lvsrSD.dOnReturnObject += ReturnSDSlot;
		m_lvsrSD.dOnProvideData += ProvideSDSlot;
		NKCUtil.SetScrollHotKey(m_lvsrSD);
		m_lvsrComment.dOnGetObject += GetCommentSlot;
		m_lvsrComment.dOnReturnObject += ReturnCommentSlot;
		m_lvsrComment.dOnProvideData += ProvideCommentSlot;
		NKCUtil.SetScrollHotKey(m_lvsrComment);
	}

	private void UpdateSDCollectionExceptPreset()
	{
		m_lstSDCollectionExceptPreset.Clear();
		foreach (NKMEmoticonData emoticonData in NKCEmoticonManager.EmoticonDatas)
		{
			if (emoticonData != null)
			{
				NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(emoticonData.emoticonId);
				if (nKMEmoticonTemplet != null && nKMEmoticonTemplet.m_EmoticonType == NKM_EMOTICON_TYPE.NET_ANI && NKCEmoticonManager.m_lstAniPreset.FindIndex((int key) => key == emoticonData.emoticonId) == -1)
				{
					m_lstSDCollectionExceptPreset.Add(emoticonData.emoticonId);
				}
			}
		}
		m_lstSDCollectionExceptPreset.Sort();
	}

	private void UpdateCommentCollectionExceptPreset()
	{
		m_lstCommentCollectionExceptPreset.Clear();
		foreach (NKMEmoticonData emoticonData in NKCEmoticonManager.EmoticonDatas)
		{
			if (emoticonData != null)
			{
				NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(emoticonData.emoticonId);
				if (nKMEmoticonTemplet != null && nKMEmoticonTemplet.m_EmoticonType == NKM_EMOTICON_TYPE.NET_TEXT && NKCEmoticonManager.m_lstTextPreset.FindIndex((int key) => key == emoticonData.emoticonId) == -1)
				{
					m_lstCommentCollectionExceptPreset.Add(emoticonData.emoticonId);
				}
			}
		}
		m_lstCommentCollectionExceptPreset.Sort();
	}

	public RectTransform GetSDSlot(int index)
	{
		NKCPopupEmoticonSlotSD newInstance = NKCPopupEmoticonSlotSD.GetNewInstance(null);
		newInstance.Reset_SD_Scale(0.62f);
		m_lstSlotSD.Add(newInstance);
		return newInstance.GetComponent<RectTransform>();
	}

	public void ReturnSDSlot(Transform tr)
	{
		NKCPopupEmoticonSlotSD component = tr.GetComponent<NKCPopupEmoticonSlotSD>();
		m_lstSlotSD.Remove(component);
		tr.SetParent(base.transform);
		Object.Destroy(tr.gameObject);
	}

	public void ProvideSDSlot(Transform tr, int index)
	{
		NKCPopupEmoticonSlotSD component = tr.GetComponent<NKCPopupEmoticonSlotSD>();
		if (!(component != null))
		{
			return;
		}
		if (m_lstSDCollectionExceptPreset.Count > index && index >= 0)
		{
			NKCUtil.SetGameobjectActive(component, bValue: true);
			component.StopSDAni();
			component.SetClickEvent(OnClickRightSideSD);
			component.SetClickEventForChange(OnClickRightSideSDForChange);
			component.SetUI(m_lstSDCollectionExceptPreset[index]);
			if (component.GetEmoticonID() == m_RightSideSelectedEmoticonIDSD)
			{
				component.SetSelectedWithChangeButton(bSet: true);
				component.MakeCanvas();
				component.ResetCanvasLayer(101);
			}
			else
			{
				component.SetSelectedWithChangeButton(bSet: false);
				component.RemoveCanvas();
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
	}

	public RectTransform GetCommentSlot(int index)
	{
		NKCPopupEmoticonSlotComment newInstanceLarge = NKCPopupEmoticonSlotComment.GetNewInstanceLarge(null);
		m_lstSlotComment.Add(newInstanceLarge);
		return newInstanceLarge.GetComponent<RectTransform>();
	}

	public void ReturnCommentSlot(Transform tr)
	{
		NKCPopupEmoticonSlotComment component = tr.GetComponent<NKCPopupEmoticonSlotComment>();
		m_lstSlotComment.Remove(component);
		tr.SetParent(base.transform);
		Object.Destroy(tr.gameObject);
	}

	public void ProvideCommentSlot(Transform tr, int index)
	{
		NKCPopupEmoticonSlotComment component = tr.GetComponent<NKCPopupEmoticonSlotComment>();
		if (!(component != null))
		{
			return;
		}
		if (m_lstCommentCollectionExceptPreset.Count > index && index >= 0)
		{
			NKCUtil.SetGameobjectActive(component, bValue: true);
			component.SetClickEvent(OnClickRightSideComment);
			component.SetClickEventForChange(OnClickRightSideCommentForChange);
			component.SetUI(m_lstCommentCollectionExceptPreset[index]);
			if (component.GetEmoticonID() == m_RightSideSelectedEmoticonIDComment)
			{
				component.SetSelected(bSet: true);
			}
			else
			{
				component.SetSelected(bSet: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
	}

	private void OnClickRightSideSD(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (slotData == null)
		{
			return;
		}
		m_RightSideSelectedEmoticonIDSD = slotData.ID;
		for (int i = 0; i < m_lstSlotSD.Count; i++)
		{
			NKCPopupEmoticonSlotSD nKCPopupEmoticonSlotSD = m_lstSlotSD[i];
			if (nKCPopupEmoticonSlotSD != null)
			{
				bool flag = nKCPopupEmoticonSlotSD.GetEmoticonID() == m_RightSideSelectedEmoticonIDSD;
				nKCPopupEmoticonSlotSD.SetSelectedWithChangeButton(flag);
				if (flag)
				{
					nKCPopupEmoticonSlotSD.PlaySDAni();
					nKCPopupEmoticonSlotSD.MakeCanvas();
					nKCPopupEmoticonSlotSD.ResetCanvasLayer(101);
				}
				else
				{
					nKCPopupEmoticonSlotSD.StopSDAni();
					nKCPopupEmoticonSlotSD.RemoveCanvas();
				}
			}
		}
	}

	private void OnClickRightSideSDForChange(int emoticonID)
	{
		int num = -1;
		for (int i = 0; i < m_lstNKCPopupEmoticonSlotSDLeft.Count; i++)
		{
			NKCPopupEmoticonSlotSD nKCPopupEmoticonSlotSD = m_lstNKCPopupEmoticonSlotSDLeft[i];
			if (!(nKCPopupEmoticonSlotSD == null) && nKCPopupEmoticonSlotSD.GetSelected())
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			NKCPacketSender.Send_NKMPacket_EMOTICON_ANI_CHANGE_REQ(num, emoticonID);
		}
	}

	public void OnRecv(NKMPacket_EMOTICON_ANI_CHANGE_ACK cNKMPacket_EMOTICON_ANI_CHANGE_ACK)
	{
		int presetIndex = cNKMPacket_EMOTICON_ANI_CHANGE_ACK.presetIndex;
		int emoticonId = cNKMPacket_EMOTICON_ANI_CHANGE_ACK.emoticonId;
		if (presetIndex < 0 || presetIndex >= NKCEmoticonManager.m_lstAniPreset.Count)
		{
			Debug.LogError("EMOTICON_ANI_CHANGE_ACK preset index invalid, index : " + presetIndex);
			return;
		}
		if (NKMEmoticonTemplet.Find(emoticonId) == null)
		{
			Debug.LogError("EMOTICON_ANI_CHANGE_ACK emoticon ID is invalid, ID : " + emoticonId);
			return;
		}
		m_RightSideSelectedEmoticonIDSD = -1;
		NKCPopupEmoticonSlotSD nKCPopupEmoticonSlotSD = m_lstNKCPopupEmoticonSlotSDLeft[presetIndex];
		nKCPopupEmoticonSlotSD.StopSDAni();
		nKCPopupEmoticonSlotSD.SetUI(emoticonId);
		nKCPopupEmoticonSlotSD.PlayChangeEffect();
		NKCEmoticonManager.m_lstAniPreset[presetIndex] = emoticonId;
		UpdateSDCollectionExceptPreset();
		int index = (presetIndex + 1) % NKCEmoticonManager.m_lstAniPreset.Count;
		nKCPopupEmoticonSlotSD.SetSelected(bSet: false);
		m_lstNKCPopupEmoticonSlotSDLeft[index].SetSelected(bSet: true);
		UpdateSDCollectionUI(bResetPos: true);
	}

	private void OnClickRightSideComment(int emoticonID)
	{
		m_RightSideSelectedEmoticonIDComment = emoticonID;
		m_RightSidePreviewEmoticonIDComment = emoticonID;
		for (int i = 0; i < m_lstSlotComment.Count; i++)
		{
			NKCPopupEmoticonSlotComment nKCPopupEmoticonSlotComment = m_lstSlotComment[i];
			if (!(nKCPopupEmoticonSlotComment != null))
			{
				continue;
			}
			if (nKCPopupEmoticonSlotComment.GetEmoticonID() == m_RightSideSelectedEmoticonIDComment)
			{
				if (!nKCPopupEmoticonSlotComment.GetSelected())
				{
					nKCPopupEmoticonSlotComment.SetSelected(bSet: true);
				}
			}
			else
			{
				nKCPopupEmoticonSlotComment.SetSelected(bSet: false);
			}
		}
		UpdateCommentCollectionUI();
	}

	private void OnClickRightSideCommentForChange(int emoticonID)
	{
		for (int i = 0; i < m_lstSlotComment.Count; i++)
		{
			NKCPopupEmoticonSlotComment nKCPopupEmoticonSlotComment = m_lstSlotComment[i];
			if (!(nKCPopupEmoticonSlotComment != null) || nKCPopupEmoticonSlotComment.GetEmoticonID() != m_RightSideSelectedEmoticonIDComment || !nKCPopupEmoticonSlotComment.GetSelected())
			{
				continue;
			}
			int num = -1;
			for (int j = 0; j < m_lstNKCPopupEmoticonSlotCommentLeft.Count; j++)
			{
				NKCPopupEmoticonSlotComment nKCPopupEmoticonSlotComment2 = m_lstNKCPopupEmoticonSlotCommentLeft[j];
				if (!(nKCPopupEmoticonSlotComment2 == null) && nKCPopupEmoticonSlotComment2.GetSelected())
				{
					num = j;
					break;
				}
			}
			if (num >= 0)
			{
				NKCPacketSender.Send_NKMPacket_EMOTICON_TEXT_CHANGE_REQ(num, emoticonID);
			}
		}
	}

	public void OnRecv(NKMPacket_EMOTICON_TEXT_CHANGE_ACK cNKMPacket_EMOTICON_TEXT_CHANGE_ACK)
	{
		int presetIndex = cNKMPacket_EMOTICON_TEXT_CHANGE_ACK.presetIndex;
		int emoticonId = cNKMPacket_EMOTICON_TEXT_CHANGE_ACK.emoticonId;
		if (presetIndex < 0 || presetIndex >= NKCEmoticonManager.m_lstTextPreset.Count)
		{
			Debug.LogError("EMOTICON_TEXT_CHANGE_ACK preset index invalid, index : " + presetIndex);
			return;
		}
		if (NKMEmoticonTemplet.Find(emoticonId) == null)
		{
			Debug.LogError("EMOTICON_TEXT_CHANGE_ACK emoticon ID is invalid, ID : " + emoticonId);
			return;
		}
		m_RightSideSelectedEmoticonIDComment = -1;
		NKCPopupEmoticonSlotComment nKCPopupEmoticonSlotComment = m_lstNKCPopupEmoticonSlotCommentLeft[presetIndex];
		nKCPopupEmoticonSlotComment.SetUI(emoticonId);
		nKCPopupEmoticonSlotComment.PlayChangeEffect();
		NKCEmoticonManager.m_lstTextPreset[presetIndex] = emoticonId;
		UpdateCommentCollectionExceptPreset();
		int index = (presetIndex + 1) % NKCEmoticonManager.m_lstTextPreset.Count;
		nKCPopupEmoticonSlotComment.SetSelected(bSet: false);
		NKCPopupEmoticonSlotComment nKCPopupEmoticonSlotComment2 = m_lstNKCPopupEmoticonSlotCommentLeft[index];
		m_RightSidePreviewEmoticonIDComment = nKCPopupEmoticonSlotComment2.GetEmoticonID();
		nKCPopupEmoticonSlotComment2.SetSelected(bSet: true);
		UpdateCommentCollectionUI(bResetPos: true);
	}

	private void SetRightSidePage(NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE ePage, string emptyNoticeString = "")
	{
		if (!string.IsNullOrWhiteSpace(emptyNoticeString))
		{
			m_EmptyNoticeString = NKCStringTable.GetString(emptyNoticeString);
		}
		else
		{
			m_EmptyNoticeString = "";
		}
		m_eRightSidePage = ePage;
		SetRightSidePageUI();
	}

	private void SetRightSidePageUI()
	{
		NKCUtil.SetGameobjectActive(m_objEmptyNotice, m_eRightSidePage == NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE.NPESRSP_NONE || m_eRightSidePage == NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE.NPESRSP_TEXT);
		NKCUtil.SetGameobjectActive(m_objSDCollection, m_eRightSidePage == NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE.NPESRSP_SD);
		NKCUtil.SetGameobjectActive(m_objTextCollection, m_eRightSidePage == NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE.NPESRSP_TEXT);
		if (m_eRightSidePage == NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE.NPESRSP_SD)
		{
			UpdateSDCollectionUI();
		}
		else if (m_eRightSidePage == NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE.NPESRSP_TEXT)
		{
			UpdateCommentCollectionUI();
			NKCUtil.SetLabelText(m_lbEmptyNotice, m_EmptyNoticeString);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEmptyNotice, m_EmptyNoticeString);
		}
	}

	private void UpdateSDCollectionUI(bool bResetPos = false)
	{
		m_lvsrSD.TotalCount = m_lstSDCollectionExceptPreset.Count;
		if (m_bFirstOpenPageSD)
		{
			m_bFirstOpenPageSD = false;
			m_lvsrSD.PrepareCells();
			m_lvsrSD.SetIndexPosition(0);
		}
		else if (bResetPos)
		{
			m_lvsrSD.SetIndexPosition(0);
		}
		else
		{
			m_lvsrSD.RefreshCells();
		}
	}

	private void UpdateCommentCollectionUI(bool bResetPos = false)
	{
		m_lvsrComment.TotalCount = m_lstCommentCollectionExceptPreset.Count;
		if (m_bFirstOpenPageComment)
		{
			m_bFirstOpenPageComment = false;
			m_lvsrComment.PrepareCells();
			m_lvsrComment.SetIndexPosition(0);
		}
		else if (bResetPos)
		{
			m_lvsrComment.SetIndexPosition(0);
		}
		else
		{
			m_lvsrComment.RefreshCells();
		}
		NKCUtil.SetGameobjectActive(m_objCommentPreview, m_RightSidePreviewEmoticonIDComment > 0);
		NKCUtil.SetGameobjectActive(m_objCommentPreviewNone, m_RightSidePreviewEmoticonIDComment <= 0);
		if (m_RightSidePreviewEmoticonIDComment > 0)
		{
			m_NKCGameHudEmoticonCommentPreview.PlayPreview(m_RightSidePreviewEmoticonIDComment);
		}
	}

	private void OnClickLeftEmoticon(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (slotData != null)
		{
			OnClickLeftEmoticon(slotData.ID);
		}
	}

	private void OnClickLeftEmoticon(int emoticonID)
	{
		for (int i = 0; i < m_lstNKCPopupEmoticonSlotSDLeft.Count; i++)
		{
			NKCPopupEmoticonSlotSD nKCPopupEmoticonSlotSD = m_lstNKCPopupEmoticonSlotSDLeft[i];
			if (nKCPopupEmoticonSlotSD == null)
			{
				continue;
			}
			if (nKCPopupEmoticonSlotSD.GetEmoticonID() == emoticonID)
			{
				nKCPopupEmoticonSlotSD.SetSelected(bSet: true);
				nKCPopupEmoticonSlotSD.PlaySDAni();
				nKCPopupEmoticonSlotSD.transform.SetAsLastSibling();
				if (m_lstSDCollectionExceptPreset.Count > 0)
				{
					SetRightSidePage(NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE.NPESRSP_SD);
				}
				else
				{
					SetRightSidePage(NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE.NPESRSP_NONE, "SI_DP_EMOTICON_HAVE_NO_EMOTICON");
				}
			}
			else
			{
				nKCPopupEmoticonSlotSD.SetSelected(bSet: false);
				nKCPopupEmoticonSlotSD.StopSDAni();
			}
		}
		for (int j = 0; j < m_lstNKCPopupEmoticonSlotCommentLeft.Count; j++)
		{
			NKCPopupEmoticonSlotComment nKCPopupEmoticonSlotComment = m_lstNKCPopupEmoticonSlotCommentLeft[j];
			if (nKCPopupEmoticonSlotComment == null)
			{
				continue;
			}
			if (nKCPopupEmoticonSlotComment.GetEmoticonID() == emoticonID)
			{
				m_RightSidePreviewEmoticonIDComment = emoticonID;
				nKCPopupEmoticonSlotComment.SetSelected(bSet: true);
				if (m_lstCommentCollectionExceptPreset.Count > 0)
				{
					SetRightSidePage(NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE.NPESRSP_TEXT);
				}
				else
				{
					SetRightSidePage(NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE.NPESRSP_TEXT, "SI_DP_EMOTICON_HAVE_NO_COMMENT");
				}
			}
			else
			{
				nKCPopupEmoticonSlotComment.SetSelected(bSet: false);
			}
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open()
	{
		UIOpened();
		SetUIWhenOpen();
	}

	public void SetUIWhenOpen()
	{
		for (int i = 0; i < m_lstNKCPopupEmoticonSlotSDLeft.Count; i++)
		{
			NKCPopupEmoticonSlotSD nKCPopupEmoticonSlotSD = m_lstNKCPopupEmoticonSlotSDLeft[i];
			if (!(nKCPopupEmoticonSlotSD == null))
			{
				if (i < NKCEmoticonManager.m_lstAniPreset.Count)
				{
					NKCUtil.SetGameobjectActive(nKCPopupEmoticonSlotSD, bValue: true);
					nKCPopupEmoticonSlotSD.SetUI(NKCEmoticonManager.m_lstAniPreset[i]);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstNKCPopupEmoticonSlotSDLeft[i], bValue: false);
				}
			}
		}
		for (int j = 0; j < m_lstNKCPopupEmoticonSlotCommentLeft.Count; j++)
		{
			NKCPopupEmoticonSlotComment nKCPopupEmoticonSlotComment = m_lstNKCPopupEmoticonSlotCommentLeft[j];
			if (!(nKCPopupEmoticonSlotComment == null))
			{
				if (j < NKCEmoticonManager.m_lstTextPreset.Count)
				{
					NKCUtil.SetGameobjectActive(nKCPopupEmoticonSlotComment, bValue: true);
					nKCPopupEmoticonSlotComment.SetUI(NKCEmoticonManager.m_lstTextPreset[j]);
				}
				else
				{
					NKCUtil.SetGameobjectActive(nKCPopupEmoticonSlotComment, bValue: false);
				}
			}
		}
		UpdateSDCollectionExceptPreset();
		UpdateCommentCollectionExceptPreset();
		OnClickLeftEmoticon(0);
		SetRightSidePage(NKC_POPUP_EMOTICON_SETTING_RIGHT_SIDE_PAGE.NPESRSP_NONE, "SI_DP_EMOTICON_SELECT_SLOT");
		m_RightSideSelectedEmoticonIDSD = -1;
		m_RightSideSelectedEmoticonIDComment = -1;
		m_RightSidePreviewEmoticonIDComment = -1;
		for (int k = 0; k < m_lstSlotSD.Count; k++)
		{
			NKCPopupEmoticonSlotSD nKCPopupEmoticonSlotSD2 = m_lstSlotSD[k];
			if (nKCPopupEmoticonSlotSD2 != null)
			{
				nKCPopupEmoticonSlotSD2.StopSDAni();
			}
		}
	}
}
