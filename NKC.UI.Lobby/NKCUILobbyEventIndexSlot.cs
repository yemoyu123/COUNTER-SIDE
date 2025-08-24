using NKC.Templet;
using NKM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyEventIndexSlot : MonoBehaviour
{
	public NKCUIComStateButton m_csbtnButton;

	public GameObject m_objEmpty;

	public Image m_imgMain;

	public GameObject m_objTimeWarning;

	public GameObject m_objReddot;

	private NKCLobbyEventIndexTemplet m_Templet;

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnButton, OnClick);
	}

	public void SetData(NKCLobbyEventIndexTemplet templet)
	{
		m_Templet = templet;
		if (templet == null)
		{
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgMain, bValue: false);
			NKCUtil.SetGameobjectActive(m_objTimeWarning, bValue: false);
			NKCUtil.SetGameobjectActive(m_objReddot, bValue: false);
			NKCUtil.SetButtonClickDelegate(m_csbtnButton, (UnityAction)delegate
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NORMAL, NKCStringTable.GetString("SI_PF_POPUP_NO_EVENT"));
			});
			return;
		}
		NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgMain, bValue: true);
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("ab_ui_nkm_ui_lobby_texture", templet.bBigBanner ? templet.BigBannerID : templet.BannerID));
		NKCUtil.SetImageSprite(m_imgMain, orLoadAssetResource);
		if (templet.IntervalTemplet != null && templet.IntervalTemplet.IsValid)
		{
			bool bValue = NKCSynchronizedTime.IsFinished(templet.IntervalTemplet.GetEndDateUtc().AddDays(-7.0));
			NKCUtil.SetGameobjectActive(m_objTimeWarning, bValue);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objTimeWarning, bValue: false);
		}
		UpdateReddot();
	}

	private void UpdateReddot()
	{
		NKCUtil.SetGameobjectActive(m_objReddot, CheckReddot());
	}

	private bool CheckReddot()
	{
		if (m_Templet == null)
		{
			return false;
		}
		if (NKCAlarmManager.HasCompletableMission(m_Templet.m_lstAlarmMissionTab))
		{
			return true;
		}
		return NKCAlarmManager.CheckAlarmByShortcut(m_Templet.ShortCutType, m_Templet.ShortCutParam);
	}

	private void OnClick()
	{
		if (m_Templet != null)
		{
			NKCContentManager.MoveToShortCut(m_Templet.ShortCutType, m_Templet.ShortCutParam);
		}
	}
}
