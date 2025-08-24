using Cs.Logging;
using NKC.Publisher;
using NKC.Templet;
using NKM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuCurrentEvent : NKCUILobbyMenuButtonBase
{
	public NKCUIComStateButton m_csbtnCurrentEvent;

	public Image m_imgCurrentEvent;

	private NKCEpisodeSummaryTemplet m_summaryTemplet;

	public void Init()
	{
		NKCUtil.SetGameobjectActive(this, bValue: true);
		NKCUtil.SetButtonClickDelegate(m_csbtnCurrentEvent, OnBtn);
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		if (NKCPublisherModule.Instance.IsReviewServer())
		{
			NKCUtil.SetGameobjectActive(this, bValue: false);
			return;
		}
		m_summaryTemplet = NKMEpisodeMgr.GetMainSummaryTemplet();
		if (m_summaryTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_imgCurrentEvent, bValue: false);
			NKCUtil.SetButtonClickDelegate(m_csbtnCurrentEvent, (UnityAction)delegate
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NORMAL, NKCStringTable.GetString("SI_PF_POPUP_NO_EVENT"));
			});
			return;
		}
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("AB_UI_LOBBY_THUMB_EPISODE", m_summaryTemplet.m_LobbyResourceID));
		if (orLoadAssetResource == null)
		{
			Log.Error("[NKCUILobbyMenuCurrentEvent] summarty m_LobbyResourceID " + m_summaryTemplet.m_LobbyResourceID + " not found.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Lobby/NKCUILobbyMenuCurrentEvent.cs", 45);
			NKCUtil.SetGameobjectActive(m_imgCurrentEvent, bValue: false);
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgCurrentEvent, orLoadAssetResource);
			NKCUtil.SetGameobjectActive(m_imgCurrentEvent, bValue: true);
		}
	}

	private void OnBtn()
	{
		if (m_summaryTemplet != null)
		{
			NKCContentManager.MoveToShortCut(m_summaryTemplet.m_ShortcutType, m_summaryTemplet.m_ShortcutParam, bForce: true);
		}
	}
}
