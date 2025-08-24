using ClientPacket.Event;
using NKC.UI.Component;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEventRaceV2HistorySlot : MonoBehaviour
{
	public NKCComTMPUIText m_lbDate;

	public Image m_imgWinTeam;

	public NKCComTMPUIText m_lbWinReward;

	public GameObject m_objVictory;

	public void SetData(NKMEventBetPrivateResult _privateResult)
	{
		if (_privateResult == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_lbDate, string.Format(NKCUtilString.GET_STRING_EVENT_RACE_BET_HISTORY_DAY_01, _privateResult.eventIndex + 1));
		if (_privateResult.selectTeam == EventBetTeam.None)
		{
			NKCUtil.SetGameobjectActive(m_objVictory, bValue: false);
			NKCUtil.SetLabelText(m_lbWinReward, NKCUtilString.GET_STRING_EVENT_RACE_NONE_BETTING_HISTORY);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objVictory, bValue: true);
		string text = ((_privateResult.selectTeam == EventBetTeam.TeamA) ? NKCUtilString.GET_STRING_EVENT_RACE_BET_TEAM_RED : NKCUtilString.GET_STRING_EVENT_RACE_BET_TEAM_BLUE);
		string msg = string.Format(NKCUtilString.GET_STRING_EVENT_RACE_BET_HISTORY_REWARD_DESC_04, text, _privateResult.betCount, text, _privateResult.dividentRate, _privateResult.rewardCount);
		NKCUtil.SetLabelText(m_lbWinReward, msg);
		switch (_privateResult.selectTeam)
		{
		case EventBetTeam.TeamA:
			if (_privateResult.isWin)
			{
				NKCUtil.SetImageSprite(m_imgWinTeam, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ui_single_race_sprite_loc", "UI_SINGLE_RACE_TEAM_RED"));
			}
			else
			{
				NKCUtil.SetImageSprite(m_imgWinTeam, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ui_single_race_sprite_loc", "UI_SINGLE_RACE_TEAM_BLUE"));
			}
			break;
		case EventBetTeam.TeamB:
			if (!_privateResult.isWin)
			{
				NKCUtil.SetImageSprite(m_imgWinTeam, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ui_single_race_sprite_loc", "UI_SINGLE_RACE_TEAM_RED"));
			}
			else
			{
				NKCUtil.SetImageSprite(m_imgWinTeam, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ui_single_race_sprite_loc", "UI_SINGLE_RACE_TEAM_BLUE"));
			}
			break;
		}
	}
}
