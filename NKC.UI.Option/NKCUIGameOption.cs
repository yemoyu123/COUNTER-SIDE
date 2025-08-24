using System.Collections.Generic;
using NKC.Publisher;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Option;

public class NKCUIGameOption : NKCUIBase
{
	public enum GameOptionGroup
	{
		None = -1,
		Mission,
		Game,
		Graphic,
		Sound,
		Alarm,
		Account,
		Replay,
		Observe,
		Max
	}

	public delegate void OnCloseCallBack();

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_game_option";

	private const string UI_ASSET_NAME = "NKM_UI_GAME_OPTION_REAL";

	private static NKCUIGameOption m_Instance;

	private static bool s_bShowHiddenOption = false;

	private static GameOptionGroup[] s_arrComboToOpenHidden = new GameOptionGroup[13]
	{
		GameOptionGroup.Game,
		GameOptionGroup.Game,
		GameOptionGroup.Game,
		GameOptionGroup.Sound,
		GameOptionGroup.Sound,
		GameOptionGroup.Sound,
		GameOptionGroup.Graphic,
		GameOptionGroup.Graphic,
		GameOptionGroup.Graphic,
		GameOptionGroup.Graphic,
		GameOptionGroup.Graphic,
		GameOptionGroup.Graphic,
		GameOptionGroup.Graphic
	};

	private static List<GameOptionGroup> s_lstCurrHiddenCombo = new List<GameOptionGroup>();

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private NKC_GAME_OPTION_MENU_TYPE m_MenuType;

	private GameOptionGroup m_SelectedGroup = GameOptionGroup.None;

	private OnCloseCallBack dOnCloseCallBack;

	private NKCUIGameOptionMenuButton[] m_MenuButtons = new NKCUIGameOptionMenuButton[8];

	private NKCUIGameOptionContentBase[] m_Contents = new NKCUIGameOptionContentBase[8];

	[Header("상부 텍스트")]
	public Text m_NKM_UI_GAME_OPTION_VERSION_TEXT;

	[Header("메뉴 버튼")]
	public NKCUIGameOptionMenuButton m_NKM_UI_GAME_OPTION_MENU_MISSION;

	public NKCUIGameOptionMenuButton m_NKM_UI_GAME_OPTION_MENU_GAME;

	public NKCUIGameOptionMenuButton m_NKM_UI_GAME_OPTION_MENU_GRAPHIC;

	public NKCUIGameOptionMenuButton m_NKM_UI_GAME_OPTION_MENU_SOUND;

	public NKCUIGameOptionMenuButton m_NKM_UI_GAME_OPTION_MENU_ALARM;

	public NKCUIGameOptionMenuButton m_NKM_UI_GAME_OPTION_MENU_ACCOUNT;

	public NKCUIGameOptionMenuButton m_NKM_UI_GAME_OPTION_MENU_REPLAY;

	public NKCUIGameOptionMenuButton m_NKM_UI_GAME_OPTION_MENU_OBSERVE;

	[Header("컨텐츠")]
	public NKCUIGameOptionContentBase m_NKM_UI_GAME_OPTION_MISSION;

	public NKCUIGameOptionContentBase m_NKM_UI_GAME_OPTION_GAME;

	public NKCUIGameOptionContentBase m_NKM_UI_GAME_OPTION_GRAPHIC;

	public NKCUIGameOptionContentBase m_NKM_UI_GAME_OPTION_SOUND;

	public NKCUIGameOptionContentBase m_NKM_UI_GAME_OPTION_ALARM;

	public NKCUIGameOptionContentBase m_NKM_UI_GAME_OPTION_PUSH;

	public NKCUIGameOptionContentBase m_NKM_UI_GAME_OPTION_ACCOUNT;

	public NKCUIGameOptionContentBase m_NKM_UI_GAME_OPTION_REPLAY;

	public NKCUIGameOptionContentBase m_NKM_UI_GAME_OPTION_OBSERVE;

	public Color m_SelectedMenuIconColor;

	public Color m_SelectedMenuTextColor;

	public Color m_SelectedMenuSubTextColor;

	[Header("하부 버튼")]
	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_BTN_RESET;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_BTN_CONFIRM;

	[Header("개인 정책 버튼")]
	public GameObject m_AGREEMENT;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_BTN_CHECK_AGREEMENT;

	public NKCUIComStateButton m_NKM_UI_GAME_OPTION_BTN_RESET_AGREEMENT;

	public static NKCUIGameOption Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIGameOption>("ab_ui_nkm_ui_game_option", "NKM_UI_GAME_OPTION_REAL", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIGameOption>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public static bool HasInstance => m_Instance != null;

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "GameOption";

	public string RESET_BUTTON_CLICK_TITLE_STRING => NKCUtilString.GET_STRING_WARNING;

	public string RESET_BUTTON_CLICK_CONTENT_STRING => NKCUtilString.GET_STRING_OPTION_RESET_WARNING;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public static bool GetShowHiddenOption()
	{
		return s_bShowHiddenOption;
	}

	public static void InvalidShowHiddenOption()
	{
		s_bShowHiddenOption = false;
		if (s_lstCurrHiddenCombo != null)
		{
			s_lstCurrHiddenCombo.Clear();
		}
	}

	public override void OnBackButton()
	{
		OnClickConfirmButton();
	}

	private void Init()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_MenuButtons[0] = m_NKM_UI_GAME_OPTION_MENU_MISSION;
		m_MenuButtons[1] = m_NKM_UI_GAME_OPTION_MENU_GAME;
		m_NKM_UI_GAME_OPTION_MENU_GAME.m_Toggle.PointerDown.RemoveAllListeners();
		m_NKM_UI_GAME_OPTION_MENU_GAME.m_Toggle.PointerDown.AddListener(delegate
		{
			ProcessHiddenOption(GameOptionGroup.Game);
		});
		m_MenuButtons[2] = m_NKM_UI_GAME_OPTION_MENU_GRAPHIC;
		m_NKM_UI_GAME_OPTION_MENU_GRAPHIC.m_Toggle.PointerDown.RemoveAllListeners();
		m_NKM_UI_GAME_OPTION_MENU_GRAPHIC.m_Toggle.PointerDown.AddListener(delegate
		{
			ProcessHiddenOption(GameOptionGroup.Graphic);
		});
		m_MenuButtons[3] = m_NKM_UI_GAME_OPTION_MENU_SOUND;
		m_NKM_UI_GAME_OPTION_MENU_SOUND.m_Toggle.PointerDown.RemoveAllListeners();
		m_NKM_UI_GAME_OPTION_MENU_SOUND.m_Toggle.PointerDown.AddListener(delegate
		{
			ProcessHiddenOption(GameOptionGroup.Sound);
		});
		m_MenuButtons[4] = m_NKM_UI_GAME_OPTION_MENU_ALARM;
		m_MenuButtons[5] = m_NKM_UI_GAME_OPTION_MENU_ACCOUNT;
		m_MenuButtons[6] = m_NKM_UI_GAME_OPTION_MENU_REPLAY;
		m_MenuButtons[7] = m_NKM_UI_GAME_OPTION_MENU_OBSERVE;
		m_Contents[0] = m_NKM_UI_GAME_OPTION_MISSION;
		m_Contents[1] = m_NKM_UI_GAME_OPTION_GAME;
		m_Contents[2] = m_NKM_UI_GAME_OPTION_GRAPHIC;
		m_Contents[3] = m_NKM_UI_GAME_OPTION_SOUND;
		m_Contents[4] = m_NKM_UI_GAME_OPTION_ALARM;
		m_Contents[5] = m_NKM_UI_GAME_OPTION_ACCOUNT;
		m_Contents[6] = m_NKM_UI_GAME_OPTION_REPLAY;
		m_Contents[7] = m_NKM_UI_GAME_OPTION_OBSERVE;
		m_NKM_UI_GAME_OPTION_BTN_RESET.PointerClick.AddListener(OnClickResetButton);
		m_NKM_UI_GAME_OPTION_BTN_CONFIRM.PointerClick.AddListener(OnClickConfirmButton);
		for (int num = 0; num < 8; num++)
		{
			GameOptionGroup group = (GameOptionGroup)num;
			m_MenuButtons[num].Init(m_SelectedMenuIconColor, m_SelectedMenuTextColor, m_SelectedMenuSubTextColor, delegate
			{
				ChangeContent(group);
			});
			m_Contents[num].Init();
		}
		string text = "CounterSide";
		text = text + " " + NKCUtilString.GetAppVersionText();
		if (!string.IsNullOrEmpty(NKCUtil.PatchVersion))
		{
			string[] array = NKCUtil.PatchVersion.Split('_');
			if (array.Length != 0)
			{
				text = text + " A." + array[array.Length - 1];
			}
		}
		if (!string.IsNullOrEmpty(NKCUtil.PatchVersionEA))
		{
			string[] array2 = NKCUtil.PatchVersionEA.Split('_');
			if (array2.Length != 0)
			{
				text = text + " E." + array2[array2.Length - 1];
			}
		}
		text = text + " " + NKCUtilString.GetProtocolVersionText();
		m_NKM_UI_GAME_OPTION_VERSION_TEXT.text = text;
		NKCUtil.SetGameobjectActive(m_AGREEMENT, bValue: false);
		if (NKMContentsVersionManager.HasTag("CHECK_AGREEMENT_NOTICE"))
		{
			NKCUtil.SetGameobjectActive(m_AGREEMENT, bValue: true);
			m_NKM_UI_GAME_OPTION_BTN_CHECK_AGREEMENT.PointerClick.AddListener(OnClickCheckAgreement);
			m_NKM_UI_GAME_OPTION_BTN_RESET_AGREEMENT.PointerClick.AddListener(OnClickResetAgreement);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (dOnCloseCallBack != null)
		{
			dOnCloseCallBack();
			dOnCloseCallBack = null;
		}
	}

	public void Open(NKC_GAME_OPTION_MENU_TYPE menuType, OnCloseCallBack closeCallBack = null)
	{
		if (s_lstCurrHiddenCombo != null)
		{
			s_lstCurrHiddenCombo.Clear();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_MenuType = menuType;
		dOnCloseCallBack = closeCallBack;
		ChangeMenu(menuType);
		GameOptionGroup defaultContentGroupByMenuType = GetDefaultContentGroupByMenuType(menuType);
		if (defaultContentGroupByMenuType != GameOptionGroup.None)
		{
			m_MenuButtons[(int)defaultContentGroupByMenuType].m_Toggle.Select(bSelect: true);
		}
		else if (m_SelectedGroup != GameOptionGroup.None)
		{
			m_MenuButtons[(int)m_SelectedGroup].m_Toggle.Select(bSelect: false, bForce: true);
		}
		ChangeContent(defaultContentGroupByMenuType);
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	private void RefreshContent(bool bForceAll = false)
	{
		if (m_SelectedGroup == GameOptionGroup.None)
		{
			return;
		}
		if (bForceAll)
		{
			NKCUIGameOptionContentBase[] contents = m_Contents;
			for (int i = 0; i < contents.Length; i++)
			{
				contents[i].SetContent();
			}
		}
		else
		{
			m_Contents[(int)m_SelectedGroup].SetContent();
		}
	}

	public NKC_GAME_OPTION_MENU_TYPE GetMenuType()
	{
		return m_MenuType;
	}

	public void RemoveCloseCallBack()
	{
		dOnCloseCallBack = null;
	}

	public void Update()
	{
		if (base.IsOpen && m_NKCUIOpenAnimator != null)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	private void ChangeMenu(NKC_GAME_OPTION_MENU_TYPE selectedMenuType)
	{
		for (int i = 0; i < 8; i++)
		{
			bool bValue = IsMenuGroupIncluded(selectedMenuType, (GameOptionGroup)i);
			NKCUtil.SetGameobjectActive(m_MenuButtons[i], bValue);
		}
	}

	private bool IsMenuGroupIncluded(NKC_GAME_OPTION_MENU_TYPE currentMenuType, GameOptionGroup targetGameOptionGroup)
	{
		if (targetGameOptionGroup == GameOptionGroup.Alarm && NKCPublisherModule.IsPCBuild())
		{
			return false;
		}
		if (targetGameOptionGroup == GameOptionGroup.Replay && currentMenuType != NKC_GAME_OPTION_MENU_TYPE.REPLAY)
		{
			return false;
		}
		if (targetGameOptionGroup == GameOptionGroup.Observe && currentMenuType != NKC_GAME_OPTION_MENU_TYPE.OBSERVE)
		{
			return false;
		}
		if ((currentMenuType == NKC_GAME_OPTION_MENU_TYPE.NORMAL || (uint)(currentMenuType - 4) <= 1u) && targetGameOptionGroup == GameOptionGroup.Mission)
		{
			return false;
		}
		return true;
	}

	private GameOptionGroup GetDefaultContentGroupByMenuType(NKC_GAME_OPTION_MENU_TYPE menuType)
	{
		GameOptionGroup result = GameOptionGroup.None;
		switch (menuType)
		{
		case NKC_GAME_OPTION_MENU_TYPE.NORMAL:
			result = GameOptionGroup.Game;
			break;
		case NKC_GAME_OPTION_MENU_TYPE.WARFARE:
		case NKC_GAME_OPTION_MENU_TYPE.DUNGEON:
		case NKC_GAME_OPTION_MENU_TYPE.DIVE:
			result = GameOptionGroup.Mission;
			break;
		case NKC_GAME_OPTION_MENU_TYPE.REPLAY:
			result = GameOptionGroup.Replay;
			break;
		case NKC_GAME_OPTION_MENU_TYPE.OBSERVE:
			result = GameOptionGroup.Observe;
			break;
		}
		return result;
	}

	private void ProcessHiddenOption(GameOptionGroup group)
	{
		if (s_lstCurrHiddenCombo == null || s_arrComboToOpenHidden == null || s_bShowHiddenOption)
		{
			return;
		}
		if (s_lstCurrHiddenCombo.Count >= s_arrComboToOpenHidden.Length)
		{
			s_lstCurrHiddenCombo.Clear();
		}
		s_lstCurrHiddenCombo.Add(group);
		bool flag = false;
		for (int i = 0; i < s_lstCurrHiddenCombo.Count && i < s_arrComboToOpenHidden.Length; i++)
		{
			if (s_lstCurrHiddenCombo[i] != s_arrComboToOpenHidden[i])
			{
				s_lstCurrHiddenCombo.Clear();
				flag = true;
			}
		}
		if (!flag && s_lstCurrHiddenCombo.Count == s_arrComboToOpenHidden.Length)
		{
			s_bShowHiddenOption = true;
		}
	}

	private void ChangeContent(GameOptionGroup group)
	{
		for (int i = 0; i < 8; i++)
		{
			if (i == (int)group)
			{
				NKCUtil.SetGameobjectActive(m_Contents[i], bValue: true);
				m_Contents[i].SetContent();
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_Contents[i], bValue: false);
			}
		}
		m_SelectedGroup = group;
	}

	private void OnClickResetButton()
	{
		NKCPopupOKCancel.OpenOKCancelBox(RESET_BUTTON_CLICK_TITLE_STRING, RESET_BUTTON_CLICK_CONTENT_STRING, delegate
		{
			NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
			if (gameOptionData != null)
			{
				gameOptionData.Rollback();
				RefreshContent(bForceAll: true);
			}
		});
	}

	private void OnClickConfirmButton()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData == null)
		{
			return;
		}
		if (gameOptionData.ChangedPrivatePvpInvite)
		{
			NKCPacketSender.Send_NKMPacket_UPDATE_PVP_INVITATION_OPTION_REQ(gameOptionData.ePrivatePvpInviteOption);
		}
		if (gameOptionData.ChangedServerOption)
		{
			bool bLocal = false;
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME && NKCScenManager.GetScenManager().GetGameClient().GetGameData() != null)
			{
				bLocal = NKCScenManager.GetScenManager().GetGameClient().GetGameData()
					.m_bLocal;
			}
			NKCPacketSender.Send_NKMPacket_GAME_OPTION_CHANGE_REQ(gameOptionData.ActionCamera, gameOptionData.TrackCamera, gameOptionData.ViewSkillCutIn, gameOptionData.PvPAutoRespawn, gameOptionData.AutSyncFriendDeck, bLocal);
		}
		else
		{
			gameOptionData.Save();
			Close();
		}
	}

	public void UpdateOptionContent(GameOptionGroup group)
	{
		if (group == m_SelectedGroup)
		{
			RefreshContent();
		}
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		if (m_NKM_UI_GAME_OPTION_MISSION.Processhotkey(hotkey))
		{
			return true;
		}
		if (m_NKM_UI_GAME_OPTION_GAME.Processhotkey(hotkey))
		{
			return true;
		}
		if (m_NKM_UI_GAME_OPTION_GRAPHIC.Processhotkey(hotkey))
		{
			return true;
		}
		if (m_NKM_UI_GAME_OPTION_SOUND.Processhotkey(hotkey))
		{
			return true;
		}
		if (m_NKM_UI_GAME_OPTION_ALARM.Processhotkey(hotkey))
		{
			return true;
		}
		if (m_NKM_UI_GAME_OPTION_ACCOUNT.Processhotkey(hotkey))
		{
			return true;
		}
		if (m_NKM_UI_GAME_OPTION_REPLAY.Processhotkey(hotkey))
		{
			return true;
		}
		return false;
	}

	public void OnClickCheckAgreement()
	{
		NKCUIAgreementNotice.OnClickPrivacy();
	}

	public void OnClickResetAgreement()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUIAgreementNotice.ResetPrivacyPolicyMessage, delegate
		{
			NKCUIAgreementNotice.OnResetAgreement(bApplicationQuit: true);
		});
	}
}
