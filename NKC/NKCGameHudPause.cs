using NKC.UI;
using NKC.UI.Option;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC;

public class NKCGameHudPause : MonoBehaviour
{
	public delegate void dOnClickContinue();

	public EventTrigger m_etBG;

	public NKCUIComStateButton m_csbtnOption;

	public NKCUIComStateButton m_csbtnContinue;

	public NKCUIComStateButton m_csbtnRestart;

	public NKCUIComStateButton m_csbtnObserveLeave;

	[Header("항복")]
	public NKCUIComStateButton m_csbtnSurrender;

	public GameObject m_objSurrenderOff;

	public GameObject m_objSurrenderOn;

	private dOnClickContinue m_dOnClickContinue;

	private int m_iSurrenderRemainTime;

	private int m_iSurrenderPossibleTime;

	private NKM_GAME_TYPE m_curGameType;

	private void Start()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnOption, OnClickOption);
		NKCUtil.SetButtonClickDelegate(m_csbtnContinue, OnClickContinue);
		NKCUtil.SetButtonClickDelegate(m_csbtnRestart, OnClickRestart);
		NKCUtil.SetButtonClickDelegate(m_csbtnObserveLeave, OnClickObserveLeave);
		NKCUtil.SetButtonClickDelegate(m_csbtnSurrender, OnClickSurrender);
		NKCUtil.SetEventTriggerDelegate(m_etBG, delegate(BaseEventData data)
		{
			UI_GAME_CAMERA_DRAG_BEGIN(data);
		}, EventTriggerType.BeginDrag);
		NKCUtil.SetEventTriggerDelegate(m_etBG, delegate(BaseEventData data)
		{
			UI_GAME_CAMERA_DRAG(data);
		}, EventTriggerType.Drag, bInit: false);
		NKCUtil.SetEventTriggerDelegate(m_etBG, delegate(BaseEventData data)
		{
			UI_GAME_CAMERA_DRAG_END(data);
		}, EventTriggerType.EndDrag, bInit: false);
		NKCUtil.SetEventTriggerDelegate(m_etBG, delegate(BaseEventData data)
		{
			UI_GAME_CAMERA_TOUCH_DOWN(data);
		}, EventTriggerType.PointerDown, bInit: false);
		NKCUtil.SetEventTriggerDelegate(m_etBG, delegate(BaseEventData data)
		{
			UI_GAME_CAMERA_TOUCH_UP(data);
		}, EventTriggerType.PointerUp, bInit: false);
		m_iSurrenderRemainTime = 0;
	}

	public bool IsOpen()
	{
		return base.gameObject.activeSelf;
	}

	public void Open(dOnClickContinue _dOnClickContinue = null)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetOnClickContinue(_dOnClickContinue);
		NKCUtil.SetGameobjectActive(m_csbtnRestart, bValue: false);
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		if (gameClient != null)
		{
			NKMGameData gameData = gameClient.GetGameData();
			if (gameData != null)
			{
				m_curGameType = gameData.GetGameType();
				NKCUtil.SetGameobjectActive(m_csbtnRestart, NKMDungeonManager.IsRestartAllowed(m_curGameType));
				InitSurrnederUI();
				NKCUtil.SetGameobjectActive(m_csbtnObserveLeave, m_curGameType == NKM_GAME_TYPE.NGT_PVP_PRIVATE && gameClient.IsObserver(NKCScenManager.CurrentUserData()));
			}
		}
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void InitSurrnederUI()
	{
		NKCUtil.SetGameobjectActive(m_csbtnSurrender, NKMGame.IsPVP(m_curGameType) && !NKCReplayMgr.IsPlayingReplay());
		if (m_csbtnSurrender.gameObject.activeSelf)
		{
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			if (gameClient != null && gameClient.IsObserver(NKCScenManager.CurrentUserData()))
			{
				NKCUtil.SetGameobjectActive(m_csbtnSurrender, bValue: false);
			}
		}
		switch (m_curGameType)
		{
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
			m_iSurrenderPossibleTime = NKMPvpCommonConst.Instance.PvpAsyncSurrenderPossibilityTime;
			break;
		case NKM_GAME_TYPE.NGT_PVP_PRIVATE:
			m_iSurrenderPossibleTime = NKMPvpCommonConst.Instance.PvpFriendlySurrenderPossibilityTime;
			break;
		case NKM_GAME_TYPE.NGT_PVP_EVENT:
			m_iSurrenderPossibleTime = NKMPvpCommonConst.Instance.PvpEventSurrenderPossibilityTime;
			break;
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
			m_iSurrenderPossibleTime = NKMPvpCommonConst.Instance.PvpLeagueSurrenderPossibilityTime;
			break;
		default:
			m_iSurrenderPossibleTime = NKMPvpCommonConst.Instance.PvpRankSurrenderPossibilityTime;
			break;
		}
		NKCUtil.SetGameobjectActive(m_objSurrenderOff, m_iSurrenderRemainTime < m_iSurrenderPossibleTime);
		NKCUtil.SetGameobjectActive(m_objSurrenderOn, m_iSurrenderRemainTime >= m_iSurrenderPossibleTime);
	}

	private void SetOnClickContinue(dOnClickContinue _dOnClickContinue)
	{
		m_dOnClickContinue = _dOnClickContinue;
	}

	private void OnClickOption()
	{
		if (NKCReplayMgr.IsPlayingReplay())
		{
			NKCUIGameOption.Instance.Open(NKC_GAME_OPTION_MENU_TYPE.REPLAY);
		}
		else if (NKMGame.IsPVP(m_curGameType))
		{
			NKCUIGameOption.Instance.Open(NKC_GAME_OPTION_MENU_TYPE.NORMAL);
		}
		else
		{
			NKCUIGameOption.Instance.Open(NKC_GAME_OPTION_MENU_TYPE.DUNGEON);
		}
	}

	public void OnClickContinue()
	{
		if (m_dOnClickContinue != null)
		{
			m_dOnClickContinue();
		}
	}

	public void OnClickRestart()
	{
		NKM_GAME_TYPE nKM_GAME_TYPE = NKCScenManager.GetScenManager().GetGameClient().GetGameData()
			.m_NKM_GAME_TYPE;
		string strID = ((nKM_GAME_TYPE != NKM_GAME_TYPE.NGT_FIERCE && nKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVE_DEFENCE) ? "SI_PF_DEFENCE_DUNGEON_RESTART_POPUP_TEXT" : "SI_PF_FIERCE_BATTLE_RESTART_POPUP");
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCStringTable.GetString(strID), NKCPacketSender.Send_NKMPacket_GAME_RESTART_REQ);
	}

	public void OnClickObserveLeave()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_PRIVATE_PVP_OBSERVE_EXIT, delegate
		{
			NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_EXIT_REQ();
		});
	}

	public void UI_GAME_CAMERA_DRAG_BEGIN(BaseEventData cBaseEventData)
	{
		PointerEventData pointerEventData = cBaseEventData as PointerEventData;
		if (NKCScenManager.GetScenManager() != null && NKCScenManager.GetScenManager().GetGameClient() != null)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_GAME_CAMERA_DRAG_BEGIN(pointerEventData.position);
		}
	}

	public void UI_GAME_CAMERA_DRAG(BaseEventData cBaseEventData)
	{
		PointerEventData pointerEventData = cBaseEventData as PointerEventData;
		if (NKCScenManager.GetScenManager() != null && NKCScenManager.GetScenManager().GetGameClient() != null)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_GAME_CAMERA_DRAG(pointerEventData.delta, pointerEventData.position);
		}
	}

	public void UI_GAME_CAMERA_DRAG_END(BaseEventData cBaseEventData)
	{
		PointerEventData pointerEventData = cBaseEventData as PointerEventData;
		if (NKCScenManager.GetScenManager() != null && NKCScenManager.GetScenManager().GetGameClient() != null)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_GAME_CAMERA_DRAG_END(pointerEventData.delta, pointerEventData.position);
		}
	}

	public void UI_GAME_CAMERA_TOUCH_DOWN(BaseEventData cBaseEventData)
	{
		if (NKCScenManager.GetScenManager() != null && NKCScenManager.GetScenManager().GetGameClient() != null)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_GAME_CAMERA_TOUCH_DOWN();
		}
	}

	public void UI_GAME_CAMERA_TOUCH_UP(BaseEventData cBaseEventData)
	{
		if (NKCScenManager.GetScenManager() != null && NKCScenManager.GetScenManager().GetGameClient() != null)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_GAME_CAMERA_TOUCH_UP();
		}
	}

	public bool IsPossibleSurrenderTime(int iRemainTimeInt)
	{
		m_iSurrenderRemainTime = iRemainTimeInt;
		if (m_iSurrenderRemainTime >= m_iSurrenderPossibleTime)
		{
			NKCUtil.SetGameobjectActive(m_objSurrenderOff, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSurrenderOn, bValue: true);
			return true;
		}
		return false;
	}

	public void OnClickSurrender()
	{
		if (m_iSurrenderRemainTime < m_iSurrenderPossibleTime)
		{
			NKCPopupMessageManager.AddPopupMessage(string.Format(NKCUtilString.GET_STRING_GAUNTLET_SURRENDER_IMPOSSIBILITY, m_iSurrenderPossibleTime));
			return;
		}
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_GAUNTLET_SURRENDER_WARNING, delegate
		{
			NKCPacketSender.Send_NKMPacket_GAME_SURRENDER_REQ();
		});
	}
}
