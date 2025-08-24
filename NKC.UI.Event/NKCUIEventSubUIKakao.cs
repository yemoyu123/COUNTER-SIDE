using ClientPacket.Event;
using NKM;
using NKM.Event;
using UnityEngine;

namespace NKC.UI.Event;

public class NKCUIEventSubUIKakao : NKCUIEventSubUIMission
{
	private enum KakaoEmoteUIState
	{
		Open,
		Complete,
		SoldOut
	}

	[Header("카카오톡 이벤트용 오브젝트")]
	public GameObject m_objKakaoComplete;

	public GameObject m_objKakaoSoldOut;

	public NKCUIComStateButton m_csbtnKakaoReceive;

	private bool m_bWaitForFocus;

	public override void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnKakaoReceive, OnClickKakao);
		base.Init();
		m_bWaitForFocus = false;
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		m_bWaitForFocus = false;
		base.Open(tabTemplet);
	}

	public override void Refresh()
	{
		KakaoEmoteUIState kakaoObjects = (NKCScenManager.CurrentUserData().IsKakaoMissionOngoing() ? GetKakaoEmoteState(NKCScenManager.CurrentUserData().kakaoMissionData.state) : KakaoEmoteUIState.SoldOut);
		SetKakaoObjects(kakaoObjects);
		base.Refresh();
	}

	private KakaoEmoteUIState GetKakaoEmoteState(KakaoMissionState state)
	{
		switch (state)
		{
		default:
			return KakaoEmoteUIState.Open;
		case KakaoMissionState.Confirmed:
			return KakaoEmoteUIState.Complete;
		case KakaoMissionState.NotEnoughBudget:
		case KakaoMissionState.OutOfDate:
			return KakaoEmoteUIState.SoldOut;
		}
	}

	private void SetKakaoObjects(KakaoEmoteUIState state)
	{
		NKCUtil.SetGameobjectActive(m_objKakaoComplete, state == KakaoEmoteUIState.Complete);
		NKCUtil.SetGameobjectActive(m_objKakaoSoldOut, state == KakaoEmoteUIState.SoldOut);
		m_csbtnKakaoReceive?.SetLock(state != KakaoEmoteUIState.Open);
	}

	private void OnClickKakao()
	{
		string shortCutParam = m_tabTemplet.m_ShortCut.Replace("{user-id}", NKCScenManager.CurrentUserData().m_UserUID.ToString());
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_URL, shortCutParam);
		m_bWaitForFocus = true;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus && m_bWaitForFocus)
		{
			m_bWaitForFocus = false;
			NKCPacketSender.Send_NKMPacket_KAKAO_MISSION_REFRESH_STATE_REQ(m_tabTemplet.m_EventID);
		}
	}
}
