using NKM;
using UnityEngine;

namespace NKC.UI.Option;

public class NKCUIGameOptionAlarm : NKCUIGameOptionContentBase
{
	private NKCUIComToggle[] m_AllowAlarmToggles = new NKCUIComToggle[7];

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_ALARM_SLOT1_TOGGLE;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_ALARM_SLOT2_TOGGLE;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_ALARM_SLOT3_TOGGLE;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_ALARM_SLOT4_TOGGLE;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_ALARM_SLOT5_TOGGLE;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_ALARM_SLOT6_TOGGLE;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_ALARM_SLOT7_TOGGLE;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_ALARM_SLOT8_TOGGLE;

	public GameObject m_NKM_UI_GAME_OPTION_ALARM_SLOT9;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_ALARM_SLOT9_TOGGLE;

	public GameObject m_NKM_UI_GAME_OPTION_ALARM_DESCRIPTION;

	public override void Init()
	{
		m_AllowAlarmToggles[0] = m_NKM_UI_GAME_OPTION_ALARM_SLOT1_TOGGLE;
		m_AllowAlarmToggles[1] = m_NKM_UI_GAME_OPTION_ALARM_SLOT2_TOGGLE;
		m_AllowAlarmToggles[2] = m_NKM_UI_GAME_OPTION_ALARM_SLOT3_TOGGLE;
		m_AllowAlarmToggles[3] = m_NKM_UI_GAME_OPTION_ALARM_SLOT4_TOGGLE;
		m_AllowAlarmToggles[4] = m_NKM_UI_GAME_OPTION_ALARM_SLOT6_TOGGLE;
		m_AllowAlarmToggles[6] = m_NKM_UI_GAME_OPTION_ALARM_SLOT9_TOGGLE;
		for (int i = 0; i < 7; i++)
		{
			NKC_GAME_OPTION_ALARM_GROUP alarmGroup = (NKC_GAME_OPTION_ALARM_GROUP)i;
			m_AllowAlarmToggles[i]?.OnValueChanged.AddListener(delegate(bool allow)
			{
				OnClickAllowAlarmButton(alarmGroup, allow);
			});
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_ALARM_DESCRIPTION, NKMContentsVersionManager.HasCountryTag(CountryTagType.KOR));
	}

	public override void SetContent()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData == null)
		{
			return;
		}
		for (int i = 0; i < 7; i++)
		{
			NKC_GAME_OPTION_ALARM_GROUP type = (NKC_GAME_OPTION_ALARM_GROUP)i;
			m_AllowAlarmToggles[i]?.Select(gameOptionData.GetAllowAlarm(type), bForce: true);
		}
		bool bSelect = true;
		for (int j = 1; j < m_AllowAlarmToggles.Length; j++)
		{
			NKCUIComToggle obj = m_AllowAlarmToggles[j];
			if ((object)obj != null && !obj.m_bChecked)
			{
				bSelect = false;
			}
		}
		m_AllowAlarmToggles[0].Select(bSelect, bForce: true);
	}

	private void OnClickAllowAlarmButton(NKC_GAME_OPTION_ALARM_GROUP alarmGroup, bool allow)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			if (alarmGroup == NKC_GAME_OPTION_ALARM_GROUP.ALLOW_ALL_ALARM)
			{
				gameOptionData.SetAllLocalAlarm(allow);
			}
			else
			{
				gameOptionData.SetAllowAlarm(alarmGroup, allow);
			}
			SetContent();
		}
	}
}
