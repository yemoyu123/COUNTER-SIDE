using NKC.UI.Event;
using NKM;
using NKM.Event;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUIComEventNotice : MonoBehaviour
{
	public NKCUIComStateButton m_cstbnButton;

	public Image m_imgIcon;

	public Text m_lbEventType;

	public GameObject m_objReddot;

	public GameObject m_objSpeechbubble;

	public Text m_lbSpeechbubble;

	private int m_EventID;

	public void SetData(NKMUserData userData)
	{
		bool num = NKCContentManager.IsContentsUnlocked(ContentsType.LOBBY_SUBMENU);
		bool flag = NKCContentManager.IsContentsUnlocked(ContentsType.LOBBY_EVENT);
		bool flag2 = NKCContentManager.IsContentsUnlocked(ContentsType.ATTENDANCE);
		if (!num || !flag2 || !flag)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		NKCUtil.SetButtonClickDelegate(m_cstbnButton, OnClick);
		NKMEventTabTemplet nKMEventTabTemplet = null;
		foreach (NKMEventTabTemplet value in NKMTempletContainer<NKMEventTabTemplet>.Values)
		{
			if (value.IsAvailable && !string.IsNullOrEmpty(value.m_LobbyButtonImage) && !NKMEventManager.IsEventCompleted(value) && (nKMEventTabTemplet == null || value.m_OrderList < nKMEventTabTemplet.m_OrderList))
			{
				nKMEventTabTemplet = value;
			}
		}
		if (nKMEventTabTemplet == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		m_EventID = nKMEventTabTemplet.m_EventID;
		SetImage(nKMEventTabTemplet.m_LobbyButtonImage);
		if (!string.IsNullOrEmpty(nKMEventTabTemplet.m_LobbyButtonString))
		{
			string remainTimeString = NKCUtilString.GetRemainTimeString(nKMEventTabTemplet.TimeLimit, 1);
			string msg = NKCStringTable.GetString(nKMEventTabTemplet.m_LobbyButtonString, remainTimeString);
			NKCUtil.SetLabelText(m_lbSpeechbubble, msg);
		}
		switch (nKMEventTabTemplet.m_EventType)
		{
		case NKM_EVENT_TYPE.BINGO:
			NKCUtil.SetLabelText(m_lbEventType, NKCStringTable.GetString("SI_DP_EVENT_NOTICE_LABEL_BINGO"));
			break;
		case NKM_EVENT_TYPE.MISSION:
		case NKM_EVENT_TYPE.KAKAOEMOTE:
		case NKM_EVENT_TYPE.MISSION_ROW:
			NKCUtil.SetLabelText(m_lbEventType, NKCStringTable.GetString("SI_DP_EVENT_NOTICE_LABEL_MISSION"));
			break;
		case NKM_EVENT_TYPE.SIMPLE:
			NKCUtil.SetLabelText(m_lbEventType, NKCStringTable.GetString("SI_DP_EVENT_NOTICE_LABEL_SIMPLE"));
			break;
		case NKM_EVENT_TYPE.ONTIME:
			NKCUtil.SetLabelText(m_lbEventType, NKCStringTable.GetString("SI_DP_EVENT_NOTICE_LABEL_COMPLETE_MISSION"));
			break;
		default:
			NKCUtil.SetLabelText(m_lbEventType, "");
			break;
		}
		bool bValue = NKMEventManager.CheckRedDot(nKMEventTabTemplet);
		NKCUtil.SetGameobjectActive(m_objReddot, bValue);
		NKCUtil.SetGameobjectActive(m_objSpeechbubble, bValue);
	}

	private void OnClick()
	{
		NKMEventTabTemplet reservedTabTemplet = NKMEventTabTemplet.Find(m_EventID);
		NKCUIEvent.Instance.Open(reservedTabTemplet);
	}

	private void SetImage(string assetName)
	{
		NKMAssetName cNKMAssetName = NKMAssetName.ParseBundleName("ab_ui_nkm_ui_lobby_texture", assetName);
		NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>(cNKMAssetName));
	}
}
