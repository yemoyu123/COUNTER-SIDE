using System;
using ClientPacket.Common;
using NKC.Publisher;
using NKM;
using Spine.Unity;
using UnityEngine;

namespace NKC;

public class NKCGameOptionData
{
	private const string NKM_LOCAL_SAVE_GAME_OPTION_DISABLED_GRAPHIC_QUALITY = "NKM_LOCAL_SAVE_GAME_OPTION_DISABLED_GRAPHIC_QUALITY";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_QUALITY = "NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_QUALITY";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_CAMERA_SHAKE = "NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_CAMERA_SHAKE";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_GAME_EFFECT = "NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_GAME_EFFECT";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_COMMON_EFFECT = "NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_COMMON_EFFECT";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_ANIMATION_QUALITY = "NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_ANIMATION_QUALITY";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_GAME_FRAME_LIMIT = "NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_GAME_FRAME_LIMIT";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_QUALITY_LEVEL = "NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_QUALITY_LEVEL";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_LOGIN_CUTIN = "NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_LOGIN_CUTIN";

	public const string NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_ASPECT_RATIO = "NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_ASPECT_RATIO";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_BUFF_EFFECT = "NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_BUFF_EFFECT";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_EFFECT_OPACITY = "NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_EFFECT_OPACITY";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_EFFECT_ENEMY_OPACITY = "NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_EFFECT_ENEMY_OPACITY";

	public const string NKM_LOCAL_SAVE_GAME_OPTION_SOUND_VOLUMES = "NKM_LOCAL_SAVE_GAME_OPTION_SOUND_VOLUMES";

	public const string NKM_LOCAL_SAVE_GAME_OPTION_SOUND_MUTE = "NKM_LOCAL_SAVE_GAME_OPTION_SOUND_MUTE";

	public const string NKM_LOCAL_SAVE_GAME_OPTION_SOUND_EFFECT = "NKM_LOCAL_SAVE_GAME_OPTION_SOUND_EFFECT";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_VOICE_LANGUAGE = "NKM_LOCAL_SAVE_GAME_OPTION_VOICE_LANGUAGE";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_ALLOW_ALARMS = "NKM_LOCAL_SAVE_GAME_OPTION_ALLOW_ALARMS_";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_ALLOW_PUSHES = "NKM_LOCAL_SAVE_GAME_OPTION_ALLOW_PUSHES";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_HIT_EFFECT = "NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_HIT_EFFECT";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GAME_LANG = "NKM_LOCAL_SAVE_GAME_OPTION_GAME_LANG";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GAME_LANG_KEY_STR_VIEW = "NKM_LOCAL_SAVE_GAME_OPTION_GAME_LANG_KEY_STR_VIEW";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_CLASS_GUIDE = "NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_CLASS_GUIDE";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_EMOTICON_BLOCK = "NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_EMOTICON_BLOCK_VER2";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_DAMAGE_BUFF_NUMBER_FX = "NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_DAMAGE_AND_BUFF_NUMBER_FX";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_VIDEO_TEXTURE = "NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_VIDEO_TEXTURE";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GAME_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO = "NKM_LOCAL_SAVE_GAME_OPTION_GAME_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GAME_NPC_SUBTITLE = "NKM_LOCAL_SAVE_GAME_OPTION_GAME_NPC_SUBTITLE";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GAME_NPC_SUBTITLE_SHOW_NORMAL_AFTER_LIFE_TIME = "NKM_LOCAL_SAVE_GAME_OPTION_GAME_NPC_SUBTITLE_SHOW_NORMAL_AFTER_LIFE_TIME";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GAME_MEMORY_OPTIMIZE = "NKM_LOCAL_SAVE_GAME_OPTION_GAME_MEMORY_OPTIMIZE";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_GAME_STREAMING_OPTION = "NKM_LOCAL_SAVE_GAME_OPTION_GAME_STREAMING_OPTION";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_CHAT_NOTIFY = "NKM_LOCAL_SAVE_GAME_OPTION_CHAT_NOTIFY";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_CHAT_NOTIFY_SOUND = "NKM_LOCAL_SAVE_GAME_OPTION_CHAT_NOTIFY_SOUND";

	private const string NKM_LOCAL_SAVE_GAME_OPTION_MOUSE_CURSOR_STATE = "NKM_LOCAL_SAVE_GAME_OPTION_MOUSE_CURSOR_STATE";

	private bool m_bChanged;

	private bool m_bChangedServerOption;

	private bool m_bChangedPrivatePvpInvite;

	private NKCGameOptionDataSt m_DataSt;

	private NKCGameOptionDataSt m_DefaultDataSt;

	public bool ChangedPrivatePvpInvite => m_bChangedPrivatePvpInvite;

	public bool ChangedServerOption => m_bChangedServerOption;

	public ActionCameraType ActionCamera => m_DataSt.m_ActionCameraType;

	public bool TrackCamera => m_DataSt.m_bTrackCamera;

	public bool ViewSkillCutIn => m_DataSt.m_bViewSkillCutIn;

	public NKM_GAME_AUTO_RESPAWN_TYPE PvPAutoRespawn => m_DataSt.m_bUsePvPAutoRespawn;

	public bool AutSyncFriendDeck => m_DataSt.m_bAutoSyncFriendDeck;

	public bool DisabledGraphicQuality
	{
		get
		{
			return m_DataSt.m_bDisabledGraphicQuality;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bDisabledGraphicQuality = value;
		}
	}

	public NKC_GAME_OPTION_GRAPHIC_QUALITY GraphicQuality
	{
		get
		{
			return m_DataSt.m_eGraphicQuality;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_eGraphicQuality = value;
		}
	}

	public NKCGameOptionDataSt.GameOptionCameraShake CameraShakeLevel
	{
		get
		{
			return m_DataSt.m_CameraShakeLevel;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_CameraShakeLevel = value;
		}
	}

	public bool UseGameEffect
	{
		get
		{
			return m_DataSt.m_bUseGameEffect;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bUseGameEffect = value;
		}
	}

	public bool UseCommonEffect
	{
		get
		{
			return m_DataSt.m_bUseCommonEffect;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bUseCommonEffect = value;
		}
	}

	public bool UseHitEffect
	{
		get
		{
			return m_DataSt.m_bUseHitEffect;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bUseHitEffect = value;
		}
	}

	public int EffectOpacity
	{
		get
		{
			return m_DataSt.m_EffectOpacity;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_EffectOpacity = value;
		}
	}

	public int EffectEnemyOpacity
	{
		get
		{
			return m_DataSt.m_EffectEnemyOpacity;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_EffectEnemyOpacity = value;
		}
	}

	public NKM_NATIONAL_CODE NKM_NATIONAL_CODE
	{
		get
		{
			return m_DataSt.m_eNKM_NATIONAL_CODE;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_eNKM_NATIONAL_CODE = value;
		}
	}

	public bool UseKeyStringView
	{
		get
		{
			return false;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bKeyStringView = value;
		}
	}

	public bool HideGameHud
	{
		get
		{
			return false;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bHideGameHud = value;
		}
	}

	public NKCGameOptionDataSt.GraphicOptionAnimationQuality AnimationQuality
	{
		get
		{
			return m_DataSt.m_AnimationQuality;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_AnimationQuality = value;
		}
	}

	public NKCGameOptionDataSt.GraphicOptionGameFrameLimit GameFrameLimit
	{
		get
		{
			return m_DataSt.m_GameFrameLimit;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_GameFrameLimit = value;
		}
	}

	public NKCGameOptionDataSt.GraphicOptionQualityLevel QualityLevel
	{
		get
		{
			return m_DataSt.m_QualityLevel;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_QualityLevel = value;
		}
	}

	public NKCGameOptionDataSt.GraphicOptionLoginCutin LoginCutin
	{
		get
		{
			return m_DataSt.m_LoginCutin;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_LoginCutin = value;
		}
	}

	public NKC_GAME_OPTION_VOICE_LANGUAGE VoiceLanguage
	{
		get
		{
			return m_DataSt.m_eVoiceLanguage;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_eVoiceLanguage = value;
		}
	}

	public bool UseClassGuide
	{
		get
		{
			return m_DataSt.m_bUseClassGuide;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bUseClassGuide = value;
		}
	}

	public bool UseNpcSubtitle
	{
		get
		{
			return m_DataSt.m_bNpcSubtitle;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bNpcSubtitle = value;
		}
	}

	public bool UseShowNormalSubtitleAfterLifeTime
	{
		get
		{
			return m_DataSt.m_bShowNormalSubtitleAfterLifeTime;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bShowNormalSubtitleAfterLifeTime = value;
		}
	}

	public bool UseEmoticonBlock
	{
		get
		{
			return m_DataSt.m_bUseEmoticonBlock;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bUseEmoticonBlock = value;
		}
	}

	public NKCGameOptionDataSt.GameOptionDamageNumber UseDamageAndBuffNumberFx
	{
		get
		{
			return m_DataSt.m_bUseDamageAndBuffNumberFx;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bUseDamageAndBuffNumberFx = value;
		}
	}

	public bool UseMemoryOptimize
	{
		get
		{
			return m_DataSt.m_bMemoryOptimize;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bMemoryOptimize = value;
		}
	}

	public bool UseChatContent
	{
		get
		{
			return m_DataSt.m_bUseChatContent;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bUseChatContent = value;
		}
	}

	public bool UseChatNotifySound
	{
		get
		{
			return m_DataSt.m_bUseChatNotifySound;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bUseChatNotifySound = value;
		}
	}

	public bool UseVideoTexture
	{
		get
		{
			if (NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable())
			{
				return false;
			}
			return m_DataSt.m_bUseVideoTexture;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_bUseVideoTexture = value;
		}
	}

	public bool SoundMute
	{
		get
		{
			return m_DataSt.m_bSoundMute;
		}
		set
		{
			m_DataSt.m_bSoundMute = value;
			m_bChanged = true;
		}
	}

	public bool SoundEffect
	{
		get
		{
			return m_DataSt.m_bSoundEffect;
		}
		set
		{
			m_DataSt.m_bSoundEffect = value;
			m_bChanged = true;
		}
	}

	public bool UseBuffEffect
	{
		get
		{
			return m_DataSt.m_bUseBuffEffect;
		}
		set
		{
			m_DataSt.m_bUseBuffEffect = value;
			m_bChanged = true;
		}
	}

	public NKC_GAME_OPTION_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO
	{
		get
		{
			return m_DataSt.m_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO;
		}
		set
		{
			m_bChanged = true;
			m_DataSt.m_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO = value;
		}
	}

	public PrivatePvpInvitation ePrivatePvpInviteOption
	{
		get
		{
			return m_DataSt.m_ePrivatePvpInviteOption;
		}
		set
		{
			m_DataSt.m_ePrivatePvpInviteOption = value;
			m_bChangedPrivatePvpInvite = true;
		}
	}

	public NKCGameOptionDataSt.GameOptionStreamingOption eStreamingOption
	{
		get
		{
			return m_DataSt.m_eStreamingOption;
		}
		set
		{
			m_DataSt.m_eStreamingOption = value;
			m_bChanged = true;
		}
	}

	public bool StreamingHideMyInfo => m_DataSt.m_eStreamingOption == NKCGameOptionDataSt.GameOptionStreamingOption.HideAll;

	public bool StreamingHideOpponentInfo => m_DataSt.m_eStreamingOption != NKCGameOptionDataSt.GameOptionStreamingOption.Normal;

	public NKCGameOptionDataSt.GameOptionMouseCursorState eMouseCursorState
	{
		get
		{
			return m_DataSt.m_eMouseCursorState;
		}
		set
		{
			m_DataSt.m_eMouseCursorState = value;
			m_bChanged = true;
		}
	}

	public NKCGameOptionData()
	{
		Init();
	}

	public void Init()
	{
		m_bChanged = false;
		m_bChangedServerOption = false;
		m_bChangedPrivatePvpInvite = false;
		m_DataSt.Init();
		m_DefaultDataSt.Init();
	}

	public int GetFrameLimit()
	{
		if (GameFrameLimit == NKCGameOptionDataSt.GraphicOptionGameFrameLimit.Thirty)
		{
			return 30;
		}
		return 60;
	}

	public void SetUseActionCamera(ActionCameraType actionCameraType, bool bForce = false)
	{
		if (!bForce)
		{
			m_bChanged = true;
			m_bChangedServerOption = true;
		}
		m_DataSt.m_ActionCameraType = actionCameraType;
	}

	public void SetUseTrackCamera(bool bUse, bool bForce = false)
	{
		if (!bForce)
		{
			m_bChanged = true;
			m_bChangedServerOption = true;
		}
		m_DataSt.m_bTrackCamera = bUse;
	}

	public void SetViewSkillCutIn(bool bView, bool bForce = false)
	{
		if (!bForce)
		{
			m_bChanged = true;
			m_bChangedServerOption = true;
		}
		m_DataSt.m_bViewSkillCutIn = bView;
	}

	public void SetNpcSubtitle(bool bView, bool bForce = false)
	{
		if (!bForce)
		{
			m_bChanged = true;
		}
		m_DataSt.m_bViewSkillCutIn = bView;
	}

	public void SetPvPAutoRespawn(NKM_GAME_AUTO_RESPAWN_TYPE type, bool bForce = false)
	{
		if (!bForce)
		{
			m_bChanged = true;
			m_bChangedServerOption = true;
		}
		m_DataSt.m_bUsePvPAutoRespawn = type;
	}

	public void SetAutoSyncFriendDeck(bool bUse, bool bForce = false)
	{
		if (!bForce)
		{
			m_bChanged = true;
			m_bChangedServerOption = true;
		}
		m_DataSt.m_bAutoSyncFriendDeck = bUse;
	}

	public int GetSoundVolume(NKC_GAME_OPTION_SOUND_GROUP type)
	{
		return m_DataSt.m_SoundVolumes[(int)type];
	}

	public float GetSoundVolumeAsFloat(NKC_GAME_OPTION_SOUND_GROUP type)
	{
		return (float)m_DataSt.m_SoundVolumes[(int)type] / 100f;
	}

	public void ChangeSoundVolume(NKC_GAME_OPTION_SOUND_GROUP type, int delta)
	{
		int num = m_DataSt.m_SoundVolumes[(int)type] + delta;
		if (num < 0)
		{
			num = 0;
		}
		if (num > 100)
		{
			num = 100;
		}
		SetSoundVolume(type, num);
	}

	public void SetSoundVolume(NKC_GAME_OPTION_SOUND_GROUP type, int volume)
	{
		m_bChanged = true;
		m_DataSt.m_SoundVolumes[(int)type] = volume;
	}

	public bool GetAllowAlarm(NKC_GAME_OPTION_ALARM_GROUP type)
	{
		return m_DataSt.m_bAllowAlarms[(int)type];
	}

	public void SetAllowAlarm(NKC_GAME_OPTION_ALARM_GROUP type, bool allow)
	{
		m_bChanged = true;
		m_DataSt.m_bAllowAlarms[(int)type] = allow;
		NKCPublisherModule.Push.SetAlarm(type, allow);
	}

	public string GetAccountLocalSaveKey(string defaultKey)
	{
		string result = "";
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			result = defaultKey + myUserData.m_UserUID;
		}
		return result;
	}

	public bool GetAllowPush(NKC_GAME_OPTION_PUSH_GROUP type)
	{
		return m_DataSt.m_bAllowPushes[(int)type];
	}

	public void SetAllowPush(NKC_GAME_OPTION_PUSH_GROUP type, bool bAllow)
	{
		m_bChanged = true;
		m_DataSt.m_bAllowPushes[(int)type] = bAllow;
		SaveOnlyPush();
	}

	public void SetAllPush(bool bPush, bool bPushAd, bool bPushAdNight)
	{
		m_DataSt.m_bAllowPushes[0] = bPush;
		SaveOnlyPush();
	}

	public void SaveOnlyPush()
	{
		PlayerPrefs.SetString("NKM_LOCAL_SAVE_GAME_OPTION_ALLOW_PUSHES", string.Join(":", m_DataSt.m_bAllowPushes));
	}

	public static NKM_NATIONAL_CODE LoadLanguageCode(NKM_NATIONAL_CODE defaultValue)
	{
		return (NKM_NATIONAL_CODE)PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_LANG" + NKCConnectionInfo.GetCurrentLoginServerPostFix(), (int)defaultValue);
	}

	public static void DeleteLanguageCode()
	{
		PlayerPrefs.DeleteKey("NKM_LOCAL_SAVE_GAME_OPTION_GAME_LANG");
	}

	public static void SaveOnlyLang(NKM_NATIONAL_CODE eCode)
	{
		PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_LANG" + NKCConnectionInfo.GetCurrentLoginServerPostFix(), (int)eCode);
	}

	public void Save()
	{
		if (m_bChanged)
		{
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_DISABLED_GRAPHIC_QUALITY", m_DataSt.m_bDisabledGraphicQuality ? 1 : 0);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_QUALITY", (int)m_DataSt.m_eGraphicQuality);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_CAMERA_SHAKE", (int)m_DataSt.m_CameraShakeLevel);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_GAME_EFFECT", m_DataSt.m_bUseGameEffect ? 1 : 0);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_COMMON_EFFECT", m_DataSt.m_bUseCommonEffect ? 1 : 0);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_ANIMATION_QUALITY", (int)m_DataSt.m_AnimationQuality);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_GAME_FRAME_LIMIT", (int)m_DataSt.m_GameFrameLimit);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_QUALITY_LEVEL", (int)m_DataSt.m_QualityLevel);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_LOGIN_CUTIN", (int)m_DataSt.m_LoginCutin);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_HIT_EFFECT", m_DataSt.m_bUseHitEffect ? 1 : 0);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_EFFECT_OPACITY", m_DataSt.m_EffectOpacity);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_EFFECT_ENEMY_OPACITY", m_DataSt.m_EffectEnemyOpacity);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_CLASS_GUIDE", m_DataSt.m_bUseClassGuide ? 1 : 0);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_EMOTICON_BLOCK_VER2", m_DataSt.m_bUseEmoticonBlock ? 1 : 0);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_DAMAGE_AND_BUFF_NUMBER_FX", (int)m_DataSt.m_bUseDamageAndBuffNumberFx);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_VIDEO_TEXTURE", m_DataSt.m_bUseVideoTexture ? 1 : 0);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO", (int)m_DataSt.m_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_NPC_SUBTITLE", m_DataSt.m_bNpcSubtitle ? 1 : 0);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_NPC_SUBTITLE_SHOW_NORMAL_AFTER_LIFE_TIME", m_DataSt.m_bShowNormalSubtitleAfterLifeTime ? 1 : 0);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_MEMORY_OPTIMIZE", m_DataSt.m_bMemoryOptimize ? 1 : 0);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_STREAMING_OPTION", (int)m_DataSt.m_eStreamingOption);
			SaveOnlyLang(m_DataSt.m_eNKM_NATIONAL_CODE);
			PlayerPrefs.SetString("NKM_LOCAL_SAVE_GAME_OPTION_SOUND_VOLUMES", string.Join(":", m_DataSt.m_SoundVolumes));
			SaveOptionValue("NKM_LOCAL_SAVE_GAME_OPTION_SOUND_MUTE", m_DataSt.m_bSoundMute);
			SaveOptionValue("NKM_LOCAL_SAVE_GAME_OPTION_SOUND_EFFECT", m_DataSt.m_bSoundEffect);
			SaveOptionValue("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_BUFF_EFFECT", m_DataSt.m_bUseBuffEffect);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_VOICE_LANGUAGE", (int)m_DataSt.m_eVoiceLanguage);
			PlayerPrefs.SetString(GetAccountLocalSaveKey("NKM_LOCAL_SAVE_GAME_OPTION_ALLOW_ALARMS_"), string.Join(":", m_DataSt.m_bAllowAlarms));
			PlayerPrefs.SetString("NKM_LOCAL_SAVE_GAME_OPTION_ALLOW_PUSHES", string.Join(":", m_DataSt.m_bAllowPushes));
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_CHAT_NOTIFY", m_DataSt.m_bUseChatContent ? 1 : 0);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_CHAT_NOTIFY_SOUND", m_DataSt.m_bUseChatNotifySound ? 1 : 0);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_GAME_OPTION_MOUSE_CURSOR_STATE", (int)m_DataSt.m_eMouseCursorState);
			ApplyToGame();
			m_bChanged = false;
			m_bChangedServerOption = false;
			m_bChangedPrivatePvpInvite = false;
		}
	}

	private void SaveOptionValue(string prefKey, bool value)
	{
		PlayerPrefs.SetInt(prefKey, value ? 1 : 0);
	}

	private bool LoadOptionValue(string prefKey, bool defaultValue)
	{
		return PlayerPrefs.GetInt(prefKey, defaultValue ? 1 : 0) == 1;
	}

	public void LoadLocal()
	{
		m_DataSt.m_bDisabledGraphicQuality = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_DISABLED_GRAPHIC_QUALITY", 0) == 1;
		m_DataSt.m_eGraphicQuality = (NKC_GAME_OPTION_GRAPHIC_QUALITY)PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_QUALITY", 3);
		m_DataSt.m_CameraShakeLevel = (NKCGameOptionDataSt.GameOptionCameraShake)PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_CAMERA_SHAKE", 1);
		m_DataSt.m_bUseGameEffect = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_GAME_EFFECT", 0) == 1;
		m_DataSt.m_bUseCommonEffect = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_COMMON_EFFECT", 0) == 1;
		m_DataSt.m_AnimationQuality = (NKCGameOptionDataSt.GraphicOptionAnimationQuality)PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_ANIMATION_QUALITY", 0);
		m_DataSt.m_GameFrameLimit = (NKCGameOptionDataSt.GraphicOptionGameFrameLimit)PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_GAME_FRAME_LIMIT", 1);
		m_DataSt.m_QualityLevel = (NKCGameOptionDataSt.GraphicOptionQualityLevel)PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_QUALITY_LEVEL", 1);
		m_DataSt.m_LoginCutin = (NKCGameOptionDataSt.GraphicOptionLoginCutin)PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_LOGIN_CUTIN", 0);
		m_DataSt.m_bUseBuffEffect = LoadOptionValue("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_BUFF_EFFECT", defaultValue: true);
		m_DataSt.m_bUseHitEffect = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_HIT_EFFECT", 1) == 1;
		m_DataSt.m_EffectOpacity = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_EFFECT_OPACITY", 100);
		m_DataSt.m_EffectEnemyOpacity = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_EFFECT_ENEMY_OPACITY", 70);
		m_DataSt.m_eNKM_NATIONAL_CODE = LoadLanguageCode(NKM_NATIONAL_CODE.NNC_KOREA);
		m_DataSt.m_bUseEmoticonBlock = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_EMOTICON_BLOCK_VER2", 0) == 1;
		m_DataSt.m_bUseDamageAndBuffNumberFx = (NKCGameOptionDataSt.GameOptionDamageNumber)PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_DAMAGE_AND_BUFF_NUMBER_FX", 0);
		m_DataSt.m_bUseVideoTexture = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GRAPHIC_USE_VIDEO_TEXTURE", 1) == 1;
		m_DataSt.m_bMemoryOptimize = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_MEMORY_OPTIMIZE", 1) == 1;
		m_DataSt.m_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO = (NKC_GAME_OPTION_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO)PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO", 0);
		m_DataSt.m_bNpcSubtitle = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_NPC_SUBTITLE", 1) == 1;
		m_DataSt.m_bShowNormalSubtitleAfterLifeTime = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_NPC_SUBTITLE_SHOW_NORMAL_AFTER_LIFE_TIME", 0) == 1;
		if (!PlayerPrefs.HasKey("NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_CLASS_GUIDE"))
		{
			m_DataSt.m_bUseClassGuide = true;
		}
		else
		{
			m_DataSt.m_bUseClassGuide = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_USE_CLASS_GUIDE", 0) == 1;
		}
		if (!PlayerPrefs.HasKey("NKM_LOCAL_SAVE_GAME_OPTION_CHAT_NOTIFY"))
		{
			m_DataSt.m_bUseChatContent = true;
		}
		else
		{
			m_DataSt.m_bUseChatContent = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_CHAT_NOTIFY", 0) == 1;
		}
		if (!PlayerPrefs.HasKey("NKM_LOCAL_SAVE_GAME_OPTION_CHAT_NOTIFY_SOUND"))
		{
			m_DataSt.m_bUseChatNotifySound = true;
		}
		else
		{
			m_DataSt.m_bUseChatNotifySound = PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_CHAT_NOTIFY_SOUND", 0) == 1;
		}
		string text = PlayerPrefs.GetString("NKM_LOCAL_SAVE_GAME_OPTION_SOUND_VOLUMES", "");
		if (text != "")
		{
			string[] array = text.Split(':');
			for (int i = 0; i < array.Length && i < m_DataSt.m_SoundVolumes.Length; i++)
			{
				m_DataSt.m_SoundVolumes[i] = int.Parse(array[i]);
			}
		}
		m_DataSt.m_bSoundMute = LoadOptionValue("NKM_LOCAL_SAVE_GAME_OPTION_SOUND_MUTE", defaultValue: false);
		m_DataSt.m_bSoundEffect = LoadOptionValue("NKM_LOCAL_SAVE_GAME_OPTION_SOUND_EFFECT", defaultValue: true);
		m_DataSt.m_eVoiceLanguage = (NKC_GAME_OPTION_VOICE_LANGUAGE)PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_VOICE_LANGUAGE", 0);
		m_DataSt.m_eStreamingOption = (NKCGameOptionDataSt.GameOptionStreamingOption)PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_GAME_STREAMING_OPTION", 0);
		string text2 = PlayerPrefs.GetString("NKM_LOCAL_SAVE_GAME_OPTION_ALLOW_PUSHES", "");
		if (text2 != "")
		{
			string[] array2 = text2.Split(':');
			for (int j = 0; j < array2.Length && m_DataSt.m_bAllowPushes.Length > j; j++)
			{
				if (bool.TryParse(array2[j], out var result))
				{
					m_DataSt.m_bAllowPushes[j] = result;
				}
				else
				{
					m_DataSt.m_bAllowPushes[j] = true;
				}
			}
		}
		m_DataSt.m_eMouseCursorState = (NKCGameOptionDataSt.GameOptionMouseCursorState)PlayerPrefs.GetInt("NKM_LOCAL_SAVE_GAME_OPTION_MOUSE_CURSOR_STATE", 1);
		ApplyToGame();
	}

	private void SetSpineHalfUpdate()
	{
		SkeletonGraphic.UseHalfUpdate = m_DataSt.m_GameFrameLimit == NKCGameOptionDataSt.GraphicOptionGameFrameLimit.Sixty && m_DataSt.m_AnimationQuality != NKCGameOptionDataSt.GraphicOptionAnimationQuality.High;
	}

	public void LoadAccountLocal(NKMUserOption cNKMUserOption)
	{
		m_DataSt.m_ActionCameraType = cNKMUserOption.m_ActionCameraType;
		m_DataSt.m_bTrackCamera = cNKMUserOption.m_bTrackCamera;
		m_DataSt.m_bViewSkillCutIn = cNKMUserOption.m_bViewSkillCutIn;
		m_DataSt.m_bUsePvPAutoRespawn = cNKMUserOption.m_bDefaultPvpAutoRespawn;
		m_DataSt.m_bAutoSyncFriendDeck = cNKMUserOption.m_bAutoSyncFriendDeck;
		m_DataSt.m_ePrivatePvpInviteOption = cNKMUserOption.privatePvpInvitation;
		string text = PlayerPrefs.GetString(GetAccountLocalSaveKey("NKM_LOCAL_SAVE_GAME_OPTION_ALLOW_ALARMS_"), "");
		if (text != "")
		{
			string[] array = text.Split(':');
			for (int i = 0; i < array.Length && m_DataSt.m_bAllowAlarms.Length > i; i++)
			{
				if (bool.TryParse(array[i], out var result))
				{
					m_DataSt.m_bAllowAlarms[i] = result;
				}
				else
				{
					m_DataSt.m_bAllowAlarms[i] = true;
				}
			}
		}
		else if (NKCPublisherModule.IsZlongPublished())
		{
			for (int j = 0; j < 7; j++)
			{
				m_DataSt.m_bAllowAlarms[j] = true;
			}
		}
	}

	public void Rollback()
	{
		m_DataSt.DeepCopyFromSource(m_DefaultDataSt);
		m_bChanged = true;
		m_bChangedServerOption = true;
		m_bChangedPrivatePvpInvite = true;
	}

	public void SetAllLocalAlarm(bool bAllow)
	{
		foreach (NKC_GAME_OPTION_ALARM_GROUP value in Enum.GetValues(typeof(NKC_GAME_OPTION_ALARM_GROUP)))
		{
			if (value != NKC_GAME_OPTION_ALARM_GROUP.MAX)
			{
				SetAllowAlarm(value, bAllow);
			}
		}
	}

	public void SetGameOptionDataByGrahpicQuality(NKC_GAME_OPTION_GRAPHIC_QUALITY quality)
	{
		GraphicQuality = quality;
		switch (quality)
		{
		case NKC_GAME_OPTION_GRAPHIC_QUALITY.VERY_LOW:
			UseGameEffect = false;
			UseCommonEffect = false;
			UseHitEffect = false;
			UseBuffEffect = false;
			AnimationQuality = NKCGameOptionDataSt.GraphicOptionAnimationQuality.Normal;
			GameFrameLimit = NKCGameOptionDataSt.GraphicOptionGameFrameLimit.Thirty;
			QualityLevel = NKCGameOptionDataSt.GraphicOptionQualityLevel.Low;
			UseVideoTexture = false;
			break;
		case NKC_GAME_OPTION_GRAPHIC_QUALITY.LOW:
			UseGameEffect = false;
			UseCommonEffect = false;
			UseHitEffect = false;
			UseBuffEffect = false;
			AnimationQuality = NKCGameOptionDataSt.GraphicOptionAnimationQuality.Normal;
			GameFrameLimit = NKCGameOptionDataSt.GraphicOptionGameFrameLimit.Sixty;
			QualityLevel = NKCGameOptionDataSt.GraphicOptionQualityLevel.High;
			UseVideoTexture = true;
			break;
		case NKC_GAME_OPTION_GRAPHIC_QUALITY.NORMAL:
			UseGameEffect = false;
			UseCommonEffect = false;
			UseHitEffect = true;
			UseBuffEffect = true;
			AnimationQuality = NKCGameOptionDataSt.GraphicOptionAnimationQuality.Normal;
			GameFrameLimit = NKCGameOptionDataSt.GraphicOptionGameFrameLimit.Sixty;
			QualityLevel = NKCGameOptionDataSt.GraphicOptionQualityLevel.High;
			UseVideoTexture = true;
			break;
		case NKC_GAME_OPTION_GRAPHIC_QUALITY.HIGH:
			UseGameEffect = false;
			UseCommonEffect = false;
			UseHitEffect = true;
			UseBuffEffect = true;
			AnimationQuality = NKCGameOptionDataSt.GraphicOptionAnimationQuality.Normal;
			GameFrameLimit = NKCGameOptionDataSt.GraphicOptionGameFrameLimit.Sixty;
			QualityLevel = NKCGameOptionDataSt.GraphicOptionQualityLevel.High;
			UseVideoTexture = true;
			break;
		case NKC_GAME_OPTION_GRAPHIC_QUALITY.VERY_HIGH:
			UseGameEffect = true;
			UseCommonEffect = true;
			UseHitEffect = true;
			UseBuffEffect = true;
			AnimationQuality = NKCGameOptionDataSt.GraphicOptionAnimationQuality.High;
			GameFrameLimit = NKCGameOptionDataSt.GraphicOptionGameFrameLimit.Sixty;
			QualityLevel = NKCGameOptionDataSt.GraphicOptionQualityLevel.High;
			UseVideoTexture = true;
			break;
		}
	}

	public void ApplyToGame()
	{
		NKCCamera.SetBloomEnableUI(UseCommonEffect);
		Application.targetFrameRate = GetFrameLimit();
		if (QualitySettings.GetQualityLevel() != (int)QualityLevel)
		{
			QualitySettings.SetQualityLevel((int)QualityLevel, applyExpensiveChanges: true);
		}
		SkeletonGraphic.UseHalfUpdate = m_DataSt.m_GameFrameLimit == NKCGameOptionDataSt.GraphicOptionGameFrameLimit.Sixty && m_DataSt.m_AnimationQuality != NKCGameOptionDataSt.GraphicOptionAnimationQuality.High;
		NKCSoundManager.SetMute(SoundMute);
		NKCSoundManager.SetSoundEffect(SoundEffect);
		if (SoundEffect)
		{
			NKCSoundManager.SetSoundVolume(GetSoundVolumeAsFloat(NKC_GAME_OPTION_SOUND_GROUP.SE));
			NKCSoundManager.SetMusicVolume(GetSoundVolumeAsFloat(NKC_GAME_OPTION_SOUND_GROUP.BGM));
			NKCSoundManager.SetVoiceVolume(GetSoundVolumeAsFloat(NKC_GAME_OPTION_SOUND_GROUP.VOICE));
		}
		NKCPacketObjectPool.SetUsePool(m_DataSt.m_bMemoryOptimize);
	}
}
