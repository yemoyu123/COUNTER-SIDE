using NKM;
using UnityEngine;

namespace NKC.UI.Option;

public class NKCUIGameOptionSound : NKCUIGameOptionContentBase
{
	private NKCUIGameOptionSliderWithButton[] m_SoundSliderWithButtons = new NKCUIGameOptionSliderWithButton[4];

	public NKCUIGameOptionSliderWithButton NKM_UI_GAME_OPTION_SOUND_SLOT0_GAUGE;

	public NKCUIGameOptionSliderWithButton NKM_UI_GAME_OPTION_SOUND_SLOT1_GAUGE;

	public NKCUIGameOptionSliderWithButton NKM_UI_GAME_OPTION_SOUND_SLOT2_GAUGE;

	public NKCUIGameOptionSliderWithButton NKM_UI_GAME_OPTION_SOUND_SLOT3_GAUGE;

	public GameObject m_objVoiceLanguageSelectSlot;

	public NKCUIComStateButton m_csbtnVoiceLanguage;

	public NKCUIComToggle m_tglMute;

	public NKCUIComToggle m_tglEffect;

	[Header("채팅 알림음")]
	public GameObject m_objChatNotifySound;

	public NKCUIComToggle m_tglChatNotifySound;

	public override void Init()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData == null)
		{
			return;
		}
		m_SoundSliderWithButtons[0] = NKM_UI_GAME_OPTION_SOUND_SLOT1_GAUGE;
		m_SoundSliderWithButtons[1] = NKM_UI_GAME_OPTION_SOUND_SLOT2_GAUGE;
		m_SoundSliderWithButtons[2] = NKM_UI_GAME_OPTION_SOUND_SLOT3_GAUGE;
		m_SoundSliderWithButtons[3] = NKM_UI_GAME_OPTION_SOUND_SLOT0_GAUGE;
		for (int i = 0; i < 4; i++)
		{
			NKC_GAME_OPTION_SOUND_GROUP soundGroup = (NKC_GAME_OPTION_SOUND_GROUP)i;
			int soundVolume = gameOptionData.GetSoundVolume(soundGroup);
			m_SoundSliderWithButtons[i].Init(0, 100, soundVolume, null, delegate
			{
				ChangeSoundVolume(soundGroup);
			});
		}
		int num = NKCUIVoiceManager.GetAvailableVoiceCode()?.Count ?? 0;
		NKCUtil.SetGameobjectActive(m_objVoiceLanguageSelectSlot, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.VOICE_SWITCH) && num >= 2);
		NKCUtil.SetButtonClickDelegate(m_csbtnVoiceLanguage, OnSelectVoiceLanguage);
		if (m_csbtnVoiceLanguage != null)
		{
			m_csbtnVoiceLanguage.SetTitleText(NKCUIVoiceManager.GetVoiceLanguageName(NKCUIVoiceManager.CurrentVoiceCode));
		}
		NKCUtil.SetToggleValueChangedDelegate(m_tglMute, OnTglMute);
		NKCUtil.SetToggleValueChangedDelegate(m_tglEffect, OnTglEffect);
		if (m_tglChatNotifySound != null)
		{
			m_tglChatNotifySound.OnValueChanged.RemoveAllListeners();
			m_tglChatNotifySound.OnValueChanged.AddListener(OnClickChatNotifySound);
		}
	}

	public override void SetContent()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			for (int i = 0; i < 4; i++)
			{
				NKC_GAME_OPTION_SOUND_GROUP type = (NKC_GAME_OPTION_SOUND_GROUP)i;
				int soundVolume = gameOptionData.GetSoundVolume(type);
				m_SoundSliderWithButtons[i].m_Slider.value = soundVolume;
			}
			m_tglMute.Select(gameOptionData.SoundMute, bForce: true);
			m_tglEffect.Select(gameOptionData.SoundEffect, bForce: true);
			if (NKMOpenTagManager.IsOpened("CHAT_PRIVATE"))
			{
				NKCUtil.SetGameobjectActive(m_objChatNotifySound, bValue: true);
				m_tglChatNotifySound?.Select(gameOptionData.UseChatNotifySound, bForce: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objChatNotifySound, bValue: false);
			}
		}
	}

	private void OnTglMute(bool value)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.SoundMute = value;
			NKCSoundManager.SetMute(value);
		}
	}

	private void OnTglEffect(bool value)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.SoundEffect = value;
			NKCSoundManager.SetSoundEffect(value);
		}
	}

	private void ChangeSoundVolume(NKC_GAME_OPTION_SOUND_GROUP soundGroup)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			int value = m_SoundSliderWithButtons[(int)soundGroup].GetValue();
			gameOptionData.SetSoundVolume(soundGroup, value);
			switch (soundGroup)
			{
			case NKC_GAME_OPTION_SOUND_GROUP.ALL:
				NKCSoundManager.SetAllVolume(gameOptionData.GetSoundVolumeAsFloat(soundGroup));
				break;
			case NKC_GAME_OPTION_SOUND_GROUP.BGM:
				NKCSoundManager.SetMusicVolume(gameOptionData.GetSoundVolumeAsFloat(soundGroup));
				break;
			case NKC_GAME_OPTION_SOUND_GROUP.SE:
				NKCSoundManager.SetSoundVolume(gameOptionData.GetSoundVolumeAsFloat(soundGroup));
				break;
			case NKC_GAME_OPTION_SOUND_GROUP.VOICE:
				NKCSoundManager.SetVoiceVolume(gameOptionData.GetSoundVolumeAsFloat(soundGroup));
				break;
			}
		}
	}

	private void OnSelectVoiceLanguage()
	{
		NKCPopupVoiceLanguageSelect.Instance.Open();
	}

	public void OnClickChatNotifySound(bool bUse)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.UseChatNotifySound = bUse;
		}
	}

	public override bool Processhotkey(HotkeyEventType eventType)
	{
		if (eventType == HotkeyEventType.ShowHotkey)
		{
			if (m_SoundSliderWithButtons[3] != null)
			{
				NKCUIComHotkeyDisplay.OpenInstance(m_SoundSliderWithButtons[3].m_PlusButton?.transform, HotkeyEventType.MasterVolumeUp);
				NKCUIComHotkeyDisplay.OpenInstance(m_SoundSliderWithButtons[3].m_MinusButton?.transform, HotkeyEventType.MasterVolumeDown);
			}
			if (m_tglMute != null)
			{
				NKCUIComHotkeyDisplay.OpenInstance(m_tglMute.transform, HotkeyEventType.Mute);
			}
		}
		return false;
	}
}
