using ClientPacket.Common;
using NKM;

namespace NKC;

public struct NKCGameOptionDataSt
{
	public enum GraphicOptionAnimationQuality
	{
		Normal,
		High,
		ENUM_COUNT
	}

	public enum GraphicOptionGameFrameLimit
	{
		Thirty,
		Sixty,
		ENUM_COUNT
	}

	public enum GraphicOptionQualityLevel
	{
		Low,
		High,
		ENUM_COUNT
	}

	public enum GraphicOptionLoginCutin
	{
		Always,
		Random,
		OncePerDay,
		Off,
		ENUM_COUNT
	}

	public enum GameOptionCameraShake
	{
		None,
		Low,
		Normal,
		ENUM_COUNT
	}

	public enum GameOptionDamageNumber
	{
		Off,
		Limited,
		On,
		ENUM_COUNT
	}

	public enum GameOptionStreamingOption
	{
		Normal,
		HideOpponent,
		HideAll
	}

	public enum GameOptionMouseCursorState
	{
		Normal,
		Default,
		Large
	}

	public const int MAX_SOUND_VOLUME = 100;

	public const int DEFAULT_MUSIC_VOLUME = 70;

	public const int DEFAULT_EFFECT_VOLUME = 60;

	public const int DEFAULT_VOICE_VOLUME = 80;

	public ActionCameraType m_ActionCameraType;

	public bool m_bTrackCamera;

	public bool m_bViewSkillCutIn;

	public NKM_GAME_AUTO_RESPAWN_TYPE m_bUsePvPAutoRespawn;

	public bool m_bAutoSyncFriendDeck;

	public bool m_bDisabledGraphicQuality;

	public NKC_GAME_OPTION_GRAPHIC_QUALITY m_eGraphicQuality;

	public GameOptionCameraShake m_CameraShakeLevel;

	public bool m_bUseGameEffect;

	public bool m_bUseCommonEffect;

	public bool m_bUseHitEffect;

	public int m_EffectOpacity;

	public int m_EffectEnemyOpacity;

	public NKM_NATIONAL_CODE m_eNKM_NATIONAL_CODE;

	public bool m_bKeyStringView;

	public GraphicOptionAnimationQuality m_AnimationQuality;

	public GraphicOptionGameFrameLimit m_GameFrameLimit;

	public GraphicOptionQualityLevel m_QualityLevel;

	public GraphicOptionLoginCutin m_LoginCutin;

	public bool m_bUseBuffEffect;

	public int[] m_SoundVolumes;

	public bool m_bSoundMute;

	public bool m_bSoundEffect;

	public NKC_GAME_OPTION_VOICE_LANGUAGE m_eVoiceLanguage;

	public bool[] m_bAllowAlarms;

	public bool[] m_bAllowPushes;

	public bool m_bUseClassGuide;

	public bool m_bUseEmoticonBlock;

	public GameOptionDamageNumber m_bUseDamageAndBuffNumberFx;

	public bool m_bUseVideoTexture;

	public NKC_GAME_OPTION_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO m_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO;

	public bool m_bNpcSubtitle;

	public bool m_bShowNormalSubtitleAfterLifeTime;

	public bool m_bMemoryOptimize;

	public bool m_bHideGameHud;

	public GameOptionStreamingOption m_eStreamingOption;

	public PrivatePvpInvitation m_ePrivatePvpInviteOption;

	public bool m_bUseChatContent;

	public bool m_bUseChatNotifySound;

	public GameOptionMouseCursorState m_eMouseCursorState;

	public void Init()
	{
		m_ActionCameraType = ActionCameraType.All;
		m_bTrackCamera = true;
		m_bViewSkillCutIn = true;
		m_bUsePvPAutoRespawn = NKM_GAME_AUTO_RESPAWN_TYPE.OFF;
		m_bAutoSyncFriendDeck = true;
		m_bDisabledGraphicQuality = false;
		m_eGraphicQuality = NKC_GAME_OPTION_GRAPHIC_QUALITY.HIGH;
		m_CameraShakeLevel = GameOptionCameraShake.None;
		m_bUseGameEffect = false;
		m_bUseCommonEffect = false;
		m_bUseHitEffect = true;
		m_EffectOpacity = 100;
		m_EffectEnemyOpacity = 70;
		m_eNKM_NATIONAL_CODE = NKM_NATIONAL_CODE.NNC_KOREA;
		m_bKeyStringView = false;
		m_AnimationQuality = GraphicOptionAnimationQuality.Normal;
		m_GameFrameLimit = GraphicOptionGameFrameLimit.Sixty;
		m_QualityLevel = GraphicOptionQualityLevel.High;
		m_LoginCutin = GraphicOptionLoginCutin.Always;
		m_SoundVolumes = new int[4];
		for (int i = 0; i < 4; i++)
		{
			switch ((NKC_GAME_OPTION_SOUND_GROUP)i)
			{
			case NKC_GAME_OPTION_SOUND_GROUP.SE:
				m_SoundVolumes[i] = 60;
				break;
			case NKC_GAME_OPTION_SOUND_GROUP.VOICE:
				m_SoundVolumes[i] = 80;
				break;
			case NKC_GAME_OPTION_SOUND_GROUP.BGM:
				m_SoundVolumes[i] = 70;
				break;
			default:
				m_SoundVolumes[i] = 100;
				break;
			}
		}
		m_eVoiceLanguage = NKC_GAME_OPTION_VOICE_LANGUAGE.KOREAN;
		m_bAllowAlarms = new bool[7];
		for (int j = 0; j < 7; j++)
		{
			m_bAllowAlarms[j] = false;
		}
		m_bAllowPushes = new bool[1];
		for (int k = 0; k < 1; k++)
		{
			m_bAllowPushes[k] = false;
		}
		m_bUseClassGuide = true;
		m_bUseEmoticonBlock = false;
		m_bUseDamageAndBuffNumberFx = GameOptionDamageNumber.Off;
		m_bUseVideoTexture = true;
		m_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO = NKC_GAME_OPTION_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO.FAST;
		m_eStreamingOption = GameOptionStreamingOption.Normal;
		m_ePrivatePvpInviteOption = PrivatePvpInvitation.Friend;
		m_bNpcSubtitle = true;
		m_bShowNormalSubtitleAfterLifeTime = false;
		m_bMemoryOptimize = false;
		m_bHideGameHud = false;
		m_bUseChatContent = true;
		m_bUseChatNotifySound = true;
		m_eMouseCursorState = GameOptionMouseCursorState.Default;
	}

	public void DeepCopyFromSource(NKCGameOptionDataSt source)
	{
		m_ActionCameraType = source.m_ActionCameraType;
		m_bTrackCamera = source.m_bTrackCamera;
		m_bViewSkillCutIn = source.m_bViewSkillCutIn;
		m_bUsePvPAutoRespawn = source.m_bUsePvPAutoRespawn;
		m_bAutoSyncFriendDeck = source.m_bAutoSyncFriendDeck;
		m_bDisabledGraphicQuality = source.m_bDisabledGraphicQuality;
		m_eGraphicQuality = source.m_eGraphicQuality;
		m_CameraShakeLevel = source.m_CameraShakeLevel;
		m_bUseGameEffect = source.m_bUseGameEffect;
		m_bUseCommonEffect = source.m_bUseCommonEffect;
		m_AnimationQuality = source.m_AnimationQuality;
		m_GameFrameLimit = source.m_GameFrameLimit;
		m_QualityLevel = source.m_QualityLevel;
		m_LoginCutin = source.m_LoginCutin;
		m_bUseHitEffect = source.m_bUseHitEffect;
		m_EffectOpacity = source.m_EffectOpacity;
		m_EffectEnemyOpacity = source.m_EffectEnemyOpacity;
		m_eNKM_NATIONAL_CODE = source.m_eNKM_NATIONAL_CODE;
		m_bKeyStringView = source.m_bKeyStringView;
		m_bUseClassGuide = source.m_bUseClassGuide;
		m_bUseEmoticonBlock = source.m_bUseEmoticonBlock;
		m_bUseDamageAndBuffNumberFx = source.m_bUseDamageAndBuffNumberFx;
		m_bUseVideoTexture = source.m_bUseVideoTexture;
		m_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO = source.m_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO;
		m_eStreamingOption = source.m_eStreamingOption;
		m_ePrivatePvpInviteOption = source.m_ePrivatePvpInviteOption;
		m_bNpcSubtitle = source.m_bNpcSubtitle;
		m_bShowNormalSubtitleAfterLifeTime = source.m_bShowNormalSubtitleAfterLifeTime;
		m_bMemoryOptimize = source.m_bMemoryOptimize;
		for (int i = 0; i < 4; i++)
		{
			m_SoundVolumes[i] = source.m_SoundVolumes[i];
		}
		m_eVoiceLanguage = source.m_eVoiceLanguage;
		for (int j = 0; j < 7; j++)
		{
			m_bAllowAlarms[j] = source.m_bAllowAlarms[j];
		}
		m_bUseChatContent = source.m_bUseChatContent;
		m_bUseChatNotifySound = source.m_bUseChatNotifySound;
		m_eMouseCursorState = source.m_eMouseCursorState;
	}
}
