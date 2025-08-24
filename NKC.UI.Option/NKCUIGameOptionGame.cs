using ClientPacket.Common;
using NKC.Localization;
using NKC.Publisher;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Option;

public class NKCUIGameOptionGame : NKCUIGameOptionContentBase
{
	private string[] GAME_CAMERA_SHAKE_STRINGS;

	private string[] GAME_DAMAGE_NUMBER_STRINGS;

	private string[] GAME_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO_STRINGS;

	private string[] GAME_STREAMING_OPTION_STRINGS;

	private string[] GAME_PRIVATE_PVP_INVITE_OPTION_STRINGS;

	private string[] GAME_MOUSE_CURSOR_STATE_STRINGS;

	[Header("스킬 카메라")]
	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GAME_SLOT1_BTN_ALL;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GAME_SLOT1_BTN_MY_TEAM;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GAME_SLOT1_BTN_NONE;

	[Header("카메라 추적")]
	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GAME_SLOT2_BTN;

	[Header("스킬 사용시 컷인")]
	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GAME_SLOT3_BTN;

	[Header("카메라 흔들림")]
	public NKCUIGameOptionMultiStateButton m_NKM_UI_GAME_OPTION_GAME_SLOT4_BTN;

	[Header("상성표 표시")]
	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GAME_SLOT7_BTN;

	[Header("피해량/버프")]
	public NKCUIGameOptionMultiStateButton m_NKM_UI_GAME_OPTION_GAME_DAMAGE_AND_BUFF_NUMBER_FX;

	[Header("랭크전 자동전투")]
	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_GAME_SLOT8_BTN_Auto_Respawn;

	public GameObject m_ObjAutoRespawnOFF;

	public GameObject m_ObjAutoRespawnALL;

	public GameObject m_ObjAutoRespawnASYNC;

	[Header("대표 소대 갱신")]
	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GAME_SLOT9_Auto_Sync_Friend_Deck;

	[Header("언어")]
	public GameObject m_NKM_UI_GAME_OPTION_GAME_LANG_SLOT;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GAME_LANG_KEY_ONLY;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_GAME_LANG_CHANGE;

	public Text m_NKM_UI_GAME_OPTION_GAME_LANG_CHANGE_Text;

	[Header("대화 속도")]
	public NKCUIGameOptionMultiStateButton m_NKM_UI_GAME_OPTION_GAME_CUTSCEN_NEXT_TALK_CHANGE_SPEED;

	[Header("자막 관련")]
	public GameObject m_objNpcSubtitle;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GAME_NPC_SUBTITLE;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GAME_NPC_SUBTITLE_SHOW_NORMAL_AFTER_LIFE_TIME;

	[Header("메모리 최적화")]
	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GAME_MEM_OPTIMIZE;

	[Header("친선전 초대")]
	public NKCUIGameOptionMultiStateButton m_MBtnPrivatePvpInviteOption;

	[Header("스트리밍 옵션")]
	public NKCUIGameOptionMultiStateButton m_MBtnStreamingOption;

	[Header("채팅 기능")]
	public GameObject m_objChatContent;

	public NKCUIComToggle m_tglChatContent;

	[Header("마우스 커서")]
	public GameObject m_objMouseCursorState;

	public NKCUIGameOptionMultiStateButton m_MbtnMouseCursorState;

	private NKM_NATIONAL_CODE m_Reserved_NKM_NATIONAL_CODE_ToChange = NKM_NATIONAL_CODE.NNC_END;

	public override void Init()
	{
		GAME_CAMERA_SHAKE_STRINGS = new string[3]
		{
			NKCUtilString.GET_STRING_NO_EXIST,
			NKCUtilString.GET_STRING_WEAK,
			NKCUtilString.GET_STRING_NORMAL
		};
		GAME_DAMAGE_NUMBER_STRINGS = new string[3]
		{
			NKCUtilString.GET_STRING_NO_EXIST,
			NKCUtilString.GET_STRING_LIMITED,
			NKCUtilString.GET_STRING_EXIST
		};
		GAME_STREAMING_OPTION_STRINGS = new string[3]
		{
			NKCStringTable.GetString("SI_DP_OPTION_STREAMING_NORMAL"),
			NKCStringTable.GetString("SI_DP_OPTION_STREAMING_HIDE_OPPONENT"),
			NKCStringTable.GetString("SI_DP_OPTION_STREAMING_HIDE_ALL")
		};
		GAME_PRIVATE_PVP_INVITE_OPTION_STRINGS = new string[4]
		{
			NKCStringTable.GetString("SI_DP_OPTION_FRIENDLY_MATCH_INVITE_FRIEND"),
			NKCStringTable.GetString("SI_DP_OPTION_FRIENDLY_MATCH_INVITE_CONSORTIUM_MEMBER"),
			NKCStringTable.GetString("SI_DP_OPTION_FRIENDLY_MATCH_INVITE_REJECT_ALL"),
			NKCStringTable.GetString("SI_DP_OPTION_FRIENDLY_MATCH_INVITE_ACCEPT_ALL")
		};
		GAME_MOUSE_CURSOR_STATE_STRINGS = new string[3]
		{
			NKCStringTable.GetString("SI_DP_OPTION_MOUSE_CURSOR_OFF"),
			NKCStringTable.GetString("SI_DP_OPTION_MOUSE_CURSOR_BASIC_SIZE"),
			NKCStringTable.GetString("SI_DP_OPTION_MOUSE_CURSOR_BIG_SIZE")
		};
		GAME_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO_STRINGS = new string[3]
		{
			NKCUtilString.GET_STRING_OPTION_CUTSCEN_NEXT_TALK_SPEED_WHEN_AUTO_FAST,
			NKCUtilString.GET_STRING_OPTION_CUTSCEN_NEXT_TALK_SPEED_WHEN_AUTO_NORMAL,
			NKCUtilString.GET_STRING_OPTION_CUTSCEN_NEXT_TALK_SPEED_WHEN_AUTO_SLOW
		};
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			m_NKM_UI_GAME_OPTION_GAME_SLOT1_BTN_ALL.OnValueChanged.AddListener(OnClickUseActionCameraButtonAll);
			m_NKM_UI_GAME_OPTION_GAME_SLOT1_BTN_MY_TEAM.OnValueChanged.AddListener(OnClickUseActionCameraButtonMyTeam);
			m_NKM_UI_GAME_OPTION_GAME_SLOT1_BTN_NONE.OnValueChanged.AddListener(OnClickUseActionCameraButtonNone);
			m_NKM_UI_GAME_OPTION_GAME_SLOT2_BTN.OnValueChanged.AddListener(OnClickUseTrackCameraButton);
			m_NKM_UI_GAME_OPTION_GAME_SLOT3_BTN.OnValueChanged.AddListener(OnClickViewSkillCutInButton);
			m_NKM_UI_GAME_OPTION_GAME_SLOT7_BTN.OnValueChanged.AddListener(OnClickUseUseClassGuideButton);
			NKCUtil.SetBindFunction(m_NKM_UI_GAME_OPTION_GAME_SLOT8_BTN_Auto_Respawn, OnClickUsePvPAutoRespawn);
			m_NKM_UI_GAME_OPTION_GAME_SLOT9_Auto_Sync_Friend_Deck.OnValueChanged.AddListener(OnClickAutoSyncFriendDeck);
			m_NKM_UI_GAME_OPTION_GAME_NPC_SUBTITLE.OnValueChanged.AddListener(OnClickNpcSubtitle);
			m_NKM_UI_GAME_OPTION_GAME_NPC_SUBTITLE_SHOW_NORMAL_AFTER_LIFE_TIME.OnValueChanged.AddListener(OnClickNpcSubtitleShowNormalAfterLifeTime);
			NKCUtil.SetGameobjectActive(m_objNpcSubtitle, NKCContentManager.IsContentsUnlocked(ContentsType.NPC_SUBTITLE));
			m_NKM_UI_GAME_OPTION_GAME_SLOT4_BTN.Init(0, 2, (int)gameOptionData.CameraShakeLevel, GAME_CAMERA_SHAKE_STRINGS, OnClickUseCameraShakeButton);
			m_NKM_UI_GAME_OPTION_GAME_DAMAGE_AND_BUFF_NUMBER_FX.Init(0, 2, (int)gameOptionData.UseDamageAndBuffNumberFx, GAME_DAMAGE_NUMBER_STRINGS, OnClickUseDamageNumberFx);
			m_NKM_UI_GAME_OPTION_GAME_CUTSCEN_NEXT_TALK_CHANGE_SPEED.Init(0, 2, (int)gameOptionData.CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO, GAME_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO_STRINGS, OnClickUseCutscenNextTalkChangeSpeedButton);
			if (m_MBtnStreamingOption != null)
			{
				m_MBtnStreamingOption.Init(0, 2, (int)gameOptionData.eStreamingOption, GAME_STREAMING_OPTION_STRINGS, OnClickBtnStreamingOption);
			}
			if (m_MBtnPrivatePvpInviteOption != null)
			{
				m_MBtnPrivatePvpInviteOption.Init(0, 3, (int)gameOptionData.ePrivatePvpInviteOption, GAME_PRIVATE_PVP_INVITE_OPTION_STRINGS, OnClickBtnPrivatePvpInviteOption);
			}
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_GAME_LANG_KEY_ONLY.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_GAME_LANG_SLOT, NKCLocalization.GetSelectLanguageSet().Count > 1);
			m_Reserved_NKM_NATIONAL_CODE_ToChange = NKM_NATIONAL_CODE.NNC_END;
			m_NKM_UI_GAME_OPTION_GAME_LANG_CHANGE_Text.text = NKCUtilString.GET_STRING_OPTION_LANGUAGE_CHANGE;
			if (m_NKM_UI_GAME_OPTION_GAME_MEM_OPTIMIZE != null)
			{
				m_NKM_UI_GAME_OPTION_GAME_MEM_OPTIMIZE.OnValueChanged.RemoveAllListeners();
				m_NKM_UI_GAME_OPTION_GAME_MEM_OPTIMIZE.OnValueChanged.AddListener(OnClickMemOptimize);
			}
			if (m_tglChatContent != null)
			{
				m_tglChatContent.OnValueChanged.RemoveAllListeners();
				m_tglChatContent.OnValueChanged.AddListener(OnClickChatNotify);
			}
			if (m_MbtnMouseCursorState != null)
			{
				m_MbtnMouseCursorState.Init(0, 2, (int)gameOptionData.eMouseCursorState, GAME_MOUSE_CURSOR_STATE_STRINGS, OnClickMouseCursorState);
			}
		}
	}

	public void OnValueKeyOnlyChanged(bool bValue)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.UseKeyStringView = bValue;
		}
	}

	public override void SetContent()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData == null)
		{
			return;
		}
		m_NKM_UI_GAME_OPTION_GAME_SLOT1_BTN_ALL.Select(gameOptionData.ActionCamera == ActionCameraType.All, bForce: true);
		m_NKM_UI_GAME_OPTION_GAME_SLOT1_BTN_MY_TEAM.Select(gameOptionData.ActionCamera == ActionCameraType.MyTeam, bForce: true);
		m_NKM_UI_GAME_OPTION_GAME_SLOT1_BTN_NONE.Select(gameOptionData.ActionCamera == ActionCameraType.None, bForce: true);
		m_NKM_UI_GAME_OPTION_GAME_SLOT2_BTN.Select(gameOptionData.TrackCamera, bForce: true);
		m_NKM_UI_GAME_OPTION_GAME_SLOT3_BTN.Select(gameOptionData.ViewSkillCutIn, bForce: true);
		m_NKM_UI_GAME_OPTION_GAME_SLOT4_BTN.ChangeValue((int)gameOptionData.CameraShakeLevel);
		m_NKM_UI_GAME_OPTION_GAME_SLOT7_BTN.Select(gameOptionData.UseClassGuide, bForce: true);
		m_NKM_UI_GAME_OPTION_GAME_DAMAGE_AND_BUFF_NUMBER_FX.ChangeValue((int)gameOptionData.UseDamageAndBuffNumberFx);
		OnClickUsePvPAutoRespawn(gameOptionData.PvPAutoRespawn);
		m_NKM_UI_GAME_OPTION_GAME_SLOT9_Auto_Sync_Friend_Deck.Select(gameOptionData.AutSyncFriendDeck, bForce: true);
		m_MBtnStreamingOption.ChangeValue((int)gameOptionData.eStreamingOption);
		m_MBtnPrivatePvpInviteOption.ChangeValue((int)gameOptionData.ePrivatePvpInviteOption);
		m_NKM_UI_GAME_OPTION_GAME_LANG_CHANGE.PointerClick.RemoveAllListeners();
		m_NKM_UI_GAME_OPTION_GAME_LANG_CHANGE.PointerClick.AddListener(OpenLangChangeList);
		m_NKM_UI_GAME_OPTION_GAME_CUTSCEN_NEXT_TALK_CHANGE_SPEED.ChangeValue((int)gameOptionData.CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO);
		m_NKM_UI_GAME_OPTION_GAME_NPC_SUBTITLE.Select(gameOptionData.UseNpcSubtitle, bForce: true);
		m_NKM_UI_GAME_OPTION_GAME_NPC_SUBTITLE_SHOW_NORMAL_AFTER_LIFE_TIME.Select(gameOptionData.UseShowNormalSubtitleAfterLifeTime, bForce: true);
		if (!NKCDefineManager.DEFINE_SERVICE() || NKCUIGameOption.GetShowHiddenOption())
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_GAME_MEM_OPTIMIZE.transform.parent, bValue: true);
			if (m_NKM_UI_GAME_OPTION_GAME_MEM_OPTIMIZE != null)
			{
				m_NKM_UI_GAME_OPTION_GAME_MEM_OPTIMIZE.Select(gameOptionData.UseMemoryOptimize, bForce: true);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_GAME_OPTION_GAME_MEM_OPTIMIZE.transform.parent, bValue: false);
		}
		m_Reserved_NKM_NATIONAL_CODE_ToChange = NKM_NATIONAL_CODE.NNC_END;
		if (NKMOpenTagManager.IsOpened("CHAT_PRIVATE"))
		{
			NKCUtil.SetGameobjectActive(m_objChatContent, bValue: true);
			m_tglChatContent?.Select(gameOptionData.UseChatContent, bForce: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objChatContent, bValue: false);
		}
		if (NKCDefineManager.DEFINE_UNITY_EDITOR() || NKCDefineManager.DEFINE_UNITY_STANDALONE())
		{
			NKCUtil.SetGameobjectActive(m_objMouseCursorState, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objMouseCursorState, bValue: false);
		}
		m_MbtnMouseCursorState.ChangeValue((int)gameOptionData.eMouseCursorState);
		OnClickMouseCursorState();
	}

	private void OpenLangChangeList()
	{
		NKCUIPopupLanguageSelect.Instance.Open(NKCLocalization.GetSelectLanguageSet(), ChangeLangDoubleCheck);
	}

	private void OnClickOKToChangeLang()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			if (m_Reserved_NKM_NATIONAL_CODE_ToChange != NKM_NATIONAL_CODE.NNC_END)
			{
				gameOptionData.NKM_NATIONAL_CODE = m_Reserved_NKM_NATIONAL_CODE_ToChange;
				NKCGameOptionData.SaveOnlyLang(m_Reserved_NKM_NATIONAL_CODE_ToChange);
				NKCPublisherModule.Localization.SetPublisherModuleLanguage(m_Reserved_NKM_NATIONAL_CODE_ToChange);
				Application.Quit();
			}
			m_Reserved_NKM_NATIONAL_CODE_ToChange = NKM_NATIONAL_CODE.NNC_END;
		}
	}

	private void ChangeLangDoubleCheck(NKM_NATIONAL_CODE eNKM_NATIONAL_CODE)
	{
		if (NKCStringTable.GetNationalCode() != eNKM_NATIONAL_CODE)
		{
			m_Reserved_NKM_NATIONAL_CODE_ToChange = eNKM_NATIONAL_CODE;
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_OPTION_GAME_LANG_CHANGE_REQ, OnClickOKToChangeLang);
		}
	}

	private void OnClickUseActionCameraButtonAll(bool use)
	{
		if (use)
		{
			NKCScenManager.GetScenManager().GetGameOptionData()?.SetUseActionCamera(ActionCameraType.All);
		}
	}

	private void OnClickUseActionCameraButtonMyTeam(bool use)
	{
		if (use)
		{
			NKCScenManager.GetScenManager().GetGameOptionData()?.SetUseActionCamera(ActionCameraType.MyTeam);
		}
	}

	private void OnClickUseActionCameraButtonNone(bool use)
	{
		if (use)
		{
			NKCScenManager.GetScenManager().GetGameOptionData()?.SetUseActionCamera(ActionCameraType.None);
		}
	}

	private void OnClickUseTrackCameraButton(bool use)
	{
		NKCScenManager.GetScenManager().GetGameOptionData()?.SetUseTrackCamera(use);
	}

	private void OnClickViewSkillCutInButton(bool view)
	{
		NKCScenManager.GetScenManager().GetGameOptionData()?.SetViewSkillCutIn(view);
	}

	private void OnClickNpcSubtitle(bool use)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.UseNpcSubtitle = use;
		}
	}

	private void OnClickNpcSubtitleShowNormalAfterLifeTime(bool use)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.UseShowNormalSubtitleAfterLifeTime = use;
		}
	}

	public void OnClickMemOptimize(bool use)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.UseMemoryOptimize = use;
			NKCPacketObjectPool.SetUsePool(use);
		}
	}

	public void OnClickChatNotify(bool bUse)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.UseChatContent = bUse;
		}
	}

	private void OnClickUseCameraShakeButton()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			int value = m_NKM_UI_GAME_OPTION_GAME_SLOT4_BTN.GetValue();
			gameOptionData.CameraShakeLevel = (NKCGameOptionDataSt.GameOptionCameraShake)value;
		}
	}

	private void OnClickUseCutscenNextTalkChangeSpeedButton()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			int value = m_NKM_UI_GAME_OPTION_GAME_CUTSCEN_NEXT_TALK_CHANGE_SPEED.GetValue();
			gameOptionData.CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO = (NKC_GAME_OPTION_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO)value;
		}
	}

	private void OnClickBtnStreamingOption()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			int value = m_MBtnStreamingOption.GetValue();
			gameOptionData.eStreamingOption = (NKCGameOptionDataSt.GameOptionStreamingOption)value;
		}
	}

	private void OnClickBtnPrivatePvpInviteOption()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			int value = m_MBtnPrivatePvpInviteOption.GetValue();
			gameOptionData.ePrivatePvpInviteOption = (PrivatePvpInvitation)value;
		}
	}

	private void OnClickUseUseClassGuideButton(bool use)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.UseClassGuide = use;
		}
	}

	private void OnClickUseDamageNumberFx()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			int value = m_NKM_UI_GAME_OPTION_GAME_DAMAGE_AND_BUFF_NUMBER_FX.GetValue();
			gameOptionData.UseDamageAndBuffNumberFx = (NKCGameOptionDataSt.GameOptionDamageNumber)value;
		}
	}

	private void OnClickUsePvPAutoRespawn()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			switch (gameOptionData.PvPAutoRespawn)
			{
			case NKM_GAME_AUTO_RESPAWN_TYPE.OFF:
				OnClickUsePvPAutoRespawn(NKM_GAME_AUTO_RESPAWN_TYPE.ALL);
				break;
			case NKM_GAME_AUTO_RESPAWN_TYPE.ALL:
				OnClickUsePvPAutoRespawn(NKM_GAME_AUTO_RESPAWN_TYPE.STRATEGY);
				break;
			default:
				OnClickUsePvPAutoRespawn(NKM_GAME_AUTO_RESPAWN_TYPE.OFF);
				break;
			}
		}
	}

	private void OnClickUsePvPAutoRespawn(NKM_GAME_AUTO_RESPAWN_TYPE type)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			NKCUtil.SetGameobjectActive(m_ObjAutoRespawnALL, type == NKM_GAME_AUTO_RESPAWN_TYPE.ALL);
			NKCUtil.SetGameobjectActive(m_ObjAutoRespawnASYNC, type == NKM_GAME_AUTO_RESPAWN_TYPE.STRATEGY);
			NKCUtil.SetGameobjectActive(m_ObjAutoRespawnOFF, type == NKM_GAME_AUTO_RESPAWN_TYPE.OFF);
			gameOptionData.SetPvPAutoRespawn(type);
		}
	}

	private void OnClickAutoSyncFriendDeck(bool use)
	{
		NKCScenManager.GetScenManager().GetGameOptionData()?.SetAutoSyncFriendDeck(use);
	}

	private void OnClickMouseCursorState()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			int value = m_MbtnMouseCursorState.GetValue();
			gameOptionData.eMouseCursorState = (NKCGameOptionDataSt.GameOptionMouseCursorState)value;
			NKCScenManager.GetScenManager().GetComponent<NKCCursor>()?.SetMouseCursor();
		}
	}
}
