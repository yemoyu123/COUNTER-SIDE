using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupMessageGuild : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_TOAST_POPUP";

	private static NKCPopupMessageGuild m_Instance;

	private const float MESSAGE_STAY_TIME = 3f;

	public Animator m_Ani;

	public RectTransform m_rtMessageRoot;

	public Text m_lbTitle;

	public Text m_lbMessage;

	private Queue<GuildMessage> m_lstMessage = new Queue<GuildMessage>();

	private Queue<bool> m_lstIsGoodNews = new Queue<bool>();

	private bool m_bPlaying;

	public static NKCPopupMessageGuild Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupMessageGuild>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_TOAST_POPUP", NKCUIManager.eUIBaseRect.UIOverlay, CleanupInstance).GetInstance<NKCPopupMessageGuild>();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Overlay;

	public override string MenuName => "Message";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public void Open(string title, string message, bool bIsGoodNews)
	{
		GuildMessage guildMessage = new GuildMessage();
		guildMessage.title = title;
		guildMessage.desc = message;
		m_lstMessage.Enqueue(guildMessage);
		m_lstIsGoodNews.Enqueue(bIsGoodNews);
		if (!m_bPlaying)
		{
			base.gameObject.SetActive(value: true);
			UIOpened();
			StartCoroutine(Process());
			m_bPlaying = true;
		}
	}

	private IEnumerator Process()
	{
		while (m_lstMessage.Count > 0)
		{
			GuildMessage guildMessage = m_lstMessage.Dequeue();
			bool bIsGoodNews = m_lstIsGoodNews.Dequeue();
			if (guildMessage != null)
			{
				NKCUtil.SetGameobjectActive(m_rtMessageRoot, bValue: false);
				m_lbTitle.text = guildMessage.title;
				m_lbMessage.text = guildMessage.desc;
				yield return StartCoroutine(ProcessShowMessage(bIsGoodNews));
			}
		}
		Close();
	}

	private IEnumerator ProcessShowMessage(bool bIsGoodNews)
	{
		NKCUtil.SetGameobjectActive(m_rtMessageRoot, bValue: true);
		if (bIsGoodNews)
		{
			m_Ani.Play("NKM_UI_POPUP_MESSAGE_EVENTBUFF_INTRO");
		}
		else
		{
			m_Ani.Play("NKM_UI_POPUP_MESSAGE_EVENTBUFF_OFF");
		}
		yield return new WaitForSeconds(3f);
		NKCUtil.SetGameobjectActive(m_rtMessageRoot, bValue: false);
	}

	public override void CloseInternal()
	{
		m_bPlaying = false;
		m_lstMessage.Clear();
		m_lstIsGoodNews.Clear();
		NKCUtil.SetGameobjectActive(m_rtMessageRoot, bValue: false);
		base.gameObject.SetActive(value: false);
	}
}
