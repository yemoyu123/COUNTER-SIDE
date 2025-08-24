using System;
using Cs.Logging;
using NKC.Dev;
using NKC.Localization;
using NKC.Patcher;
using NKC.Publisher;
using NKC.Util;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUILoginDevMenu : MonoBehaviour
{
	public NKCUILoginBaseMenu m_loginBaseMenu;

	public GameObject m_NUF_LOGIN_DEV_SERVER_LIST;

	public GameObject m_NUF_LOGIN_DEV_LOGIN_SIGN_BUTTON;

	private Dropdown m_NUF_LOGIN_DEV_SERVER_LIST_DROPDOWN;

	public NKCUIComButton m_comBtnServer1STSave;

	public NKCUIComButton m_comBtnServer1STLoad;

	public NKCUIComButton m_comBtnServer2NDSave;

	public NKCUIComButton m_comBtnServer2NDLoad;

	public NKCUIComButton m_comBtnServer3RDSave;

	public NKCUIComButton m_comBtnServer3RDLoad;

	public InputField m_IFServerAddress;

	public InputField m_IFID;

	public InputField m_IFPassword;

	public NKCUIComButton m_comBtnLogin;

	public NKCUIComButton m_comBtnPlaySingle;

	public InputField m_ifPlaySingleMap;

	public NKCUIComButton m_comBtnCutscen;

	public NKCUIComButton m_comBtnVoiceList;

	public NKCUIComButton m_comBtnLanguageSelect;

	public NKCUIComButton m_comBtnMoveToPatch;

	public NKCUIComButton m_comBtnPatchSkipTest;

	public NKCUIComButton m_comBtnConsentRevocation;

	public NKCUIComButton m_comBtnReplayViewer;

	public const string NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING = "NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING";

	public const string NKM_LOCAL_SAVE_KEY_LOGIN_PASSWORD_STRING = "NKM_LOCAL_SAVE_KEY_LOGIN_PASSWORD_STRING";

	private string NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED = "NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED";

	private string NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_1ST = "NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_1ST";

	private string NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_2ND = "NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_2ND";

	private string NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_3RD = "NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_3RD";

	public GameObject m_objNUF_LOGIN_DEV_LOGIN;

	[Obsolete]
	public GameObject m_NUF_LOGIN_DEV_SINGLE;

	public GameObject m_NUF_LOGIN_DEV_CUTSCEN;

	public GameObject m_NUF_LOGIN_DEV_VOICE_LIST;

	public NKCUIComStateButton m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL;

	public Text m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL_TEXT;

	private NKM_USER_AUTH_LEVEL m_NKM_USER_AUTH_LEVEL = NKM_USER_AUTH_LEVEL.NORMAL_USER;

	private float m_fUpdateTime;

	private const float LOGIN_PANEL_SHOW_UP_TIME = 2.5f;

	private bool m_bShowUpLoginPanel;

	public NKM_USER_AUTH_LEVEL AuthLevel => m_NKM_USER_AUTH_LEVEL;

	private void OnDestroy()
	{
	}

	public void InitUI()
	{
		if (m_comBtnServer1STSave != null)
		{
			m_comBtnServer1STSave.PointerClick.RemoveAllListeners();
			m_comBtnServer1STSave.PointerClick.AddListener(OnSaveServer1ST);
		}
		if (m_comBtnServer1STLoad != null)
		{
			m_comBtnServer1STLoad.PointerClick.RemoveAllListeners();
			m_comBtnServer1STLoad.PointerClick.AddListener(OnLoadServer1ST);
		}
		if (m_comBtnServer2NDSave != null)
		{
			m_comBtnServer2NDSave.PointerClick.RemoveAllListeners();
			m_comBtnServer2NDSave.PointerClick.AddListener(OnSaveServer2ND);
		}
		if (m_comBtnServer2NDLoad != null)
		{
			m_comBtnServer2NDLoad.PointerClick.RemoveAllListeners();
			m_comBtnServer2NDLoad.PointerClick.AddListener(OnLoadServer2ND);
		}
		if (m_comBtnServer3RDSave != null)
		{
			m_comBtnServer3RDSave.PointerClick.RemoveAllListeners();
			m_comBtnServer3RDSave.PointerClick.AddListener(OnSaveServer3RD);
		}
		if (m_comBtnServer3RDLoad != null)
		{
			m_comBtnServer3RDLoad.PointerClick.RemoveAllListeners();
			m_comBtnServer3RDLoad.PointerClick.AddListener(OnLoadServer3RD);
		}
		if (m_comBtnLogin != null)
		{
			m_comBtnLogin.PointerClick.RemoveAllListeners();
			m_comBtnLogin.PointerClick.AddListener(delegate
			{
				OnDevLogin();
				m_loginBaseMenu.OnLogin();
			});
		}
		NKCUtil.SetButtonClickDelegate(m_comBtnPlaySingle, OnPlaySingle);
		if (m_comBtnCutscen != null)
		{
			m_comBtnCutscen.PointerClick.RemoveAllListeners();
			m_comBtnCutscen.PointerClick.AddListener(OnCutScenSim);
		}
		if (m_comBtnVoiceList != null)
		{
			m_comBtnVoiceList.PointerClick.RemoveAllListeners();
			m_comBtnVoiceList.PointerClick.AddListener(OnVoiceList);
		}
		if (m_comBtnLanguageSelect != null)
		{
			m_comBtnLanguageSelect.PointerClick.RemoveAllListeners();
			m_comBtnLanguageSelect.PointerClick.AddListener(OnLanguageSelect);
		}
		if (m_comBtnReplayViewer != null)
		{
			m_comBtnReplayViewer.PointerClick.RemoveAllListeners();
			m_comBtnReplayViewer.PointerClick.AddListener(OnReplayViewer);
		}
		NKCUtil.SetButtonClickDelegate(m_comBtnMoveToPatch, OnMoveToPatch);
		NKCUtil.SetButtonClickDelegate(m_comBtnPatchSkipTest, OnPatchSkipTest);
		NKCUtil.SetGameobjectActive(m_comBtnPatchSkipTest, NKCDefineManager.DEFINE_PATCH_SKIP());
		if (m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL != null)
		{
			m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL.PointerClick.RemoveAllListeners();
			m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL.PointerClick.AddListener(OnClickAuthLevelChange);
		}
		string text = $"{NKCUtilString.GetAppVersionText()} {NKCConnectionInfo.s_ServerType}";
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
		if (m_NUF_LOGIN_DEV_SERVER_LIST != null)
		{
			m_NUF_LOGIN_DEV_SERVER_LIST_DROPDOWN = m_NUF_LOGIN_DEV_SERVER_LIST.transform.GetComponentInChildren<Dropdown>();
			if (m_NUF_LOGIN_DEV_SERVER_LIST_DROPDOWN != null)
			{
				m_NUF_LOGIN_DEV_SERVER_LIST_DROPDOWN.onValueChanged.AddListener(delegate
				{
					OnServerListDropDown(m_NUF_LOGIN_DEV_SERVER_LIST_DROPDOWN);
				});
			}
		}
		m_NUF_LOGIN_DEV_SERVER_LIST.SetActive(value: false);
		m_NUF_LOGIN_DEV_CUTSCEN.SetActive(value: false);
		m_NUF_LOGIN_DEV_VOICE_LIST.SetActive(value: false);
		NKCUtil.SetGameobjectActive(m_comBtnLanguageSelect, bValue: false);
		NKCUtil.SetGameobjectActive(m_comBtnMoveToPatch, bValue: false);
		NKCUtil.SetGameobjectActive(m_comBtnPatchSkipTest, bValue: false);
		if (!NKCDefineManager.DEFINE_SERVICE() || NKCDefineManager.DEFINE_USE_CHEAT())
		{
			m_NUF_LOGIN_DEV_SERVER_LIST.SetActive(!NKCDefineManager.DEFINE_SELECT_SERVER());
		}
		if (m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL != null)
		{
			NKCUtil.SetGameobjectActive(m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL.gameObject, bValue: false);
		}
		if (m_comBtnConsentRevocation != null)
		{
			NKCUtil.SetGameobjectActive(m_comBtnConsentRevocation, bValue: false);
		}
	}

	private void OnClickPcQR()
	{
		NKMPopUpBox.OpenWaitBox();
		NKCPublisherModule.Auth.QR_Login(OnCompleteQR_Login);
	}

	private void OnCompleteQR_Login(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		NKCPublisherModule.CheckError(resultCode, additionalError);
	}

	private void OnClickAuthLevelChange()
	{
		NKCPMPushSelf.OnClickTestNotification();
		int num = Enum.GetNames(typeof(NKM_USER_AUTH_LEVEL)).Length;
		if ((int)m_NKM_USER_AUTH_LEVEL >= (int)(byte)num)
		{
			m_NKM_USER_AUTH_LEVEL = NKM_USER_AUTH_LEVEL.NORMAL_USER;
		}
		else
		{
			m_NKM_USER_AUTH_LEVEL++;
		}
		ApplyNowAuthLevelToText();
	}

	private void OnClickNoticeButton()
	{
		NKCPublisherModule.Notice.OpenNotice(null);
	}

	public void Open()
	{
		base.gameObject.SetActive(value: true);
		m_fUpdateTime = 0f;
		LoadLastEditServerAddress();
		Log.Debug("[Login] Open PublisherModule[" + NKCPublisherModule.PublisherType.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginDevMenu.cs", 301);
		if (NKCPublisherModule.IsPublisherNoneType())
		{
			if (PlayerPrefs.HasKey("NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING"))
			{
				m_objNUF_LOGIN_DEV_LOGIN.SetActive(value: false);
				m_bShowUpLoginPanel = false;
				Log.Debug("[Login] HasKey[" + "NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING".ToString() + "] Value[" + PlayerPrefs.GetString("NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING") + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginDevMenu.cs", 309);
			}
			else
			{
				m_objNUF_LOGIN_DEV_LOGIN.SetActive(value: true);
				m_bShowUpLoginPanel = true;
				Log.Debug("[Login] NUF_LOGIN_DEV_LOGIN active", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginDevMenu.cs", 316);
				Log.Debug("[Login] HasKey[" + "NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING".ToString() + "] Value[" + PlayerPrefs.GetString("NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING") + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginDevMenu.cs", 317);
			}
		}
		else
		{
			m_objNUF_LOGIN_DEV_LOGIN.SetActive(value: false);
			m_bShowUpLoginPanel = false;
		}
		if (NKCPublisherModule.IsPublisherNoneType())
		{
			NKCUtil.SetGameobjectActive(m_NUF_LOGIN_DEV_CUTSCEN, bValue: true);
			NKCUtil.SetGameobjectActive(m_NUF_LOGIN_DEV_VOICE_LIST, bValue: true);
			NKCUtil.SetGameobjectActive(m_comBtnLanguageSelect, bValue: true);
			NKCUtil.SetGameobjectActive(m_comBtnMoveToPatch, bValue: true);
			NKCUtil.SetGameobjectActive(m_comBtnPatchSkipTest, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NUF_LOGIN_DEV_CUTSCEN, bValue: false);
			NKCUtil.SetGameobjectActive(m_NUF_LOGIN_DEV_VOICE_LIST, bValue: false);
			NKCUtil.SetGameobjectActive(m_comBtnLanguageSelect, bValue: false);
			NKCUtil.SetGameobjectActive(m_comBtnMoveToPatch, bValue: false);
			NKCUtil.SetGameobjectActive(m_comBtnPatchSkipTest, bValue: false);
		}
		if (NKCDefineManager.DEFINE_USE_CHEAT())
		{
			NKCUtil.SetGameobjectActive(m_comBtnPlaySingle, bValue: true);
			NKCUtil.SetGameobjectActive(m_comBtnReplayViewer, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_comBtnPlaySingle, bValue: false);
			NKCUtil.SetGameobjectActive(m_comBtnReplayViewer, bValue: false);
		}
	}

	private void ApplyNowAuthLevelToText()
	{
		if (m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL_TEXT != null)
		{
			m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL_TEXT.text = m_NKM_USER_AUTH_LEVEL.ToString();
		}
	}

	private void LoadLastEditServerAddress()
	{
		if (PlayerPrefs.HasKey(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED))
		{
			string text = PlayerPrefs.GetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED);
			m_IFServerAddress.text = text;
		}
		else
		{
			m_IFServerAddress.text = "192.168.0.201";
			PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED, m_IFServerAddress.text);
		}
	}

	public void OnEndEditServerAddress()
	{
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED, m_IFServerAddress.text);
	}

	public void SaveIDPass()
	{
		Log.Debug($"[Login] SaveIDPass ShowUpLoginPanel[{m_bShowUpLoginPanel}]  ID[{m_IFID.text}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginDevMenu.cs", 397);
		if (m_bShowUpLoginPanel)
		{
			PlayerPrefs.SetString("NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING", m_IFID.text);
			PlayerPrefs.SetString("NKM_LOCAL_SAVE_KEY_LOGIN_PASSWORD_STRING", m_IFPassword.text);
		}
	}

	private void OnServerListDropDown(Dropdown change)
	{
		string text = m_IFServerAddress.text;
		switch (change.captionText.text)
		{
		case "Dev":
			text = "studiobsidedev.com";
			break;
		case "Next":
			text = "Build02:4100";
			break;
		case "Stage":
			text = "52.231.15.96";
			break;
		case "Review":
			text = "52.141.21.24";
			break;
		case "SeaStage":
			text = "DataManager:52000";
			break;
		case "SeaLive":
			text = "DataManager:32000";
			break;
		case "SeaQa":
			text = "DataManager:42000";
			break;
		case "TaiwanQa":
			text = "build02";
			break;
		case "TaiwanStage":
			text = "DataManager";
			break;
		case "Vietnam":
			text = "192.168.0.145";
			break;
		case "JapanQa":
			text = "Build02:5310";
			break;
		case "JapanStage":
			text = "Build02:6310";
			break;
		case "JapanLive":
			text = "Build02:7310";
			break;
		case "devChina":
			text = "DataManager:8100";
			break;
		case "ChinaQa":
			text = "DataManager:7100";
			break;
		case "ChinaStage":
			text = "DataManager:9100";
			break;
		}
		m_IFServerAddress.text = text;
	}

	private void LoadConnectionAddressFromUI()
	{
		Log.Debug("[Login] LoadConnectionAddressFromUI m_IFServerAddress[" + m_IFServerAddress.text + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginDevMenu.cs", 499);
		string text = m_IFServerAddress.text;
		int num = 22000;
		string[] array = m_IFServerAddress.text.Split(':');
		text = array[0];
		if (array.Length > 1)
		{
			num = Convert.ToInt32(array[1]);
		}
		NKCConnectionInfo.SetLoginServerInfo(NKCConnectionInfo.LOGIN_SERVER_TYPE.Default, text, num);
		Log.Debug($"[Login] NKCConnectionInfo s_ServiceIP[{text}] s_ServicePort[{num}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginDevMenu.cs", 512);
	}

	public string GetCurrentServerAddress()
	{
		return NKCConnectionInfo.ServiceIP;
	}

	public int GetCurrentServerPort()
	{
		return NKCConnectionInfo.ServicePort;
	}

	private void InitConnect()
	{
		NKCConnectLogin connectLogin = NKCScenManager.GetScenManager().GetConnectLogin();
		connectLogin.SetRemoteAddress(GetCurrentServerAddress(), GetCurrentServerPort());
		connectLogin.ResetConnection();
	}

	public bool ProceedLogin()
	{
		if (m_bShowUpLoginPanel)
		{
			return false;
		}
		if (m_NUF_LOGIN_DEV_SERVER_LIST_DROPDOWN != null && m_NUF_LOGIN_DEV_SERVER_LIST_DROPDOWN.transform.Find("Dropdown List") != null)
		{
			return false;
		}
		return true;
	}

	public void OnDevLogin()
	{
		if (m_bShowUpLoginPanel)
		{
			SaveIDPass();
		}
		if (m_NUF_LOGIN_DEV_SERVER_LIST != null && m_NUF_LOGIN_DEV_SERVER_LIST.activeInHierarchy)
		{
			LoadConnectionAddressFromUI();
		}
	}

	private void OnPlaySingle()
	{
		NKMContentsVersionManager.LoadDefaultVersion();
		if (NKCDefineManager.DEFINE_UNITY_EDITOR())
		{
			NKCContentsVersionManager.TryRecoverTag();
		}
		NKCTempletUtility.PostJoin();
		string text = ((m_ifPlaySingleMap != null) ? m_ifPlaySingleMap.text : string.Empty);
		text = ((!string.IsNullOrEmpty(text)) ? text.Trim() : "NKM_DUNGEON_TEST");
		if (NKCDefineManager.DEFINE_USE_CHEAT())
		{
			NKCMemoryStats.MakeInstance();
			NKCScenManager.GetScenManager().Get_SCEN_GAME().SetDungeonStrIDForLocal(text);
			NKCPacketSender.Send_NKMPacket_DEV_GAME_LOAD_REQ(text);
		}
	}

	public void OnCutScenSim()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_SIM);
	}

	public void OnVoiceList()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_VOICE_LIST);
	}

	public void OnLanguageSelect()
	{
		NKCUIPopupLanguageSelect.Instance.Open(NKCLocalization.GetSelectLanguageSet(), OnChangeLanguage);
	}

	private void OnMoveToPatch()
	{
		NKCScenManager.GetScenManager().ShowBundleUpdate(bCallFromTutorial: false);
	}

	private void OnPatchSkipTest()
	{
		NKCPatchUtility.DeleteTutorialClearedStatus();
		NKCPatchUtility.ReservePatchSkipTest();
		Application.Quit();
	}

	public void OnChangeAccountDev()
	{
		m_bShowUpLoginPanel = true;
		if (!m_objNUF_LOGIN_DEV_LOGIN.activeSelf)
		{
			m_objNUF_LOGIN_DEV_LOGIN.SetActive(value: true);
		}
		if (PlayerPrefs.HasKey("NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING"))
		{
			string text = PlayerPrefs.GetString("NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING");
			string text2 = PlayerPrefs.GetString("NKM_LOCAL_SAVE_KEY_LOGIN_PASSWORD_STRING");
			m_IFID.text = text;
			m_IFPassword.text = text2;
		}
	}

	private void OnChangeLanguage(NKM_NATIONAL_CODE eNKM_NATIONAL_CODE)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null && eNKM_NATIONAL_CODE != NKM_NATIONAL_CODE.NNC_END)
		{
			gameOptionData.NKM_NATIONAL_CODE = eNKM_NATIONAL_CODE;
			NKCGameOptionData.SaveOnlyLang(eNKM_NATIONAL_CODE);
			Application.Quit();
		}
	}

	private void OnSaveServer1ST()
	{
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_1ST, m_IFServerAddress.text);
	}

	private void OnLoadServer1ST()
	{
		if (PlayerPrefs.HasKey(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_1ST))
		{
			string text = PlayerPrefs.GetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_1ST);
			m_IFServerAddress.text = text;
		}
		else
		{
			m_IFServerAddress.text = "192.168.0.201";
		}
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED, m_IFServerAddress.text);
	}

	private void OnSaveServer2ND()
	{
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_2ND, m_IFServerAddress.text);
	}

	private void OnLoadServer2ND()
	{
		if (PlayerPrefs.HasKey(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_2ND))
		{
			string text = PlayerPrefs.GetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_2ND);
			m_IFServerAddress.text = text;
		}
		else
		{
			m_IFServerAddress.text = "";
		}
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED, m_IFServerAddress.text);
	}

	private void OnSaveServer3RD()
	{
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_3RD, m_IFServerAddress.text);
	}

	private void OnLoadServer3RD()
	{
		if (PlayerPrefs.HasKey(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_3RD))
		{
			string text = PlayerPrefs.GetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_3RD);
			m_IFServerAddress.text = text;
		}
		else
		{
			m_IFServerAddress.text = "";
		}
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED, m_IFServerAddress.text);
	}

	private void OnReplayViewer()
	{
		NKCPopupReplayViewer.Instance.Open();
	}

	public void Update()
	{
		m_fUpdateTime += Time.deltaTime;
		if (!m_objNUF_LOGIN_DEV_LOGIN.activeSelf && m_bShowUpLoginPanel && m_fUpdateTime > 2.5f)
		{
			m_objNUF_LOGIN_DEV_LOGIN.SetActive(value: true);
		}
	}
}
