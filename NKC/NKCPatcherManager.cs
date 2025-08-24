using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using AssetBundles;
using Cs.Engine.Util;
using Cs.Logging;
using DG.Tweening;
using NKA.Service;
using NKC.Localization;
using NKC.Patcher;
using NKC.Publisher;
using NKC.UI;
using NKC.UI.Option;
using NKM;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace NKC;

public class NKCPatcherManager : MonoBehaviour
{
	private enum PATCHER_STATE
	{
		None,
		Init,
		DisplayLogo,
		InitPM,
		UpdateDownloadConfigProcess,
		UpdateServerInfomation,
		GetTagFromLoginServer,
		WaitStartUpProcess,
		SelectServer,
		SetDefaultTagFromSelectedServer,
		TryRecoverTag,
		SetNationCode,
		LoadPatcherString,
		SelectVoice,
		DisplayGameGrade,
		Localizing,
		CheckPCVersionUp,
		CheckCanStartPatch,
		InitObb,
		MakeDownloadList,
		MakeDownloadListForExtraAsset,
		MakeDownloadListForTutorialAsset,
		SelectPatchDownloadType,
		DownloadRequiredPatchFiles,
		Patch,
		GetTagFromSelectedLoginServer,
		StartGame
	}

	private static NKCPatcherManager m_PatcherManager;

	public GameObject m_NKC_PATCHER_UI_FRONT;

	private RectTransform m_NKC_PATCHER_UI_FRONT_RectTransform;

	private Mask m_NKC_PATCHER_UI_FRONT_Mask;

	public GameObject m_NKM_UI_WAIT;

	public GameObject m_NKM_UI_TAG_REQUEST_NOTICE;

	public Text m_TagListRequestMsg;

	public NKCPatchChecker m_NkcPatchChecker;

	public NKCLogo m_NKCLogo;

	public NKCPopupSelectServer m_NKCSelectServer;

	public NKCPopupNoticeWeb m_NKCNoticeWeb;

	public NKCFontChanger m_NKCFontChanger;

	[Header("언어 선택 팝업")]
	public NKCUIPopupLanguageSelect m_popupLanguageSelect;

	[Header("음성 선택 팝업")]
	public NKCPopupVoiceLanguageSelect m_popupVoiceSelect;

	[Header("오류 표시용 팝업")]
	public NKCPopupOKCancel m_popupBox;

	[Header("다운 선택 팝업")]
	public NKCPopupDownloadTypeSelection m_popupDownloadTypeSelection;

	private static bool s_bInit;

	private static bool s_bPubModuleInit;

	private bool m_bRunningMsgCoroutine;

	private PATCHER_STATE m_currentPatcherState;

	private static GameObject s_objMultiClientPrevent;

	private NKC_PUBLISHER_RESULT_CODE m_eResultCode { get; set; }

	public static NKCPatcherManager GetPatcherManager()
	{
		return m_PatcherManager;
	}

	private void Start()
	{
		NKCUtil.SetGameobjectActive(m_NKCLogo, bValue: false);
		m_NkcPatchChecker.SetActive(active: false);
		NKCUtil.SetGameobjectActive(m_NKCSelectServer, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_WAIT, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_TAG_REQUEST_NOTICE, bValue: false);
		m_NKC_PATCHER_UI_FRONT_RectTransform = m_NKC_PATCHER_UI_FRONT.GetComponent<RectTransform>();
		m_NKC_PATCHER_UI_FRONT_Mask = m_NKC_PATCHER_UI_FRONT.GetComponent<Mask>();
		m_NKC_PATCHER_UI_FRONT_Mask.enabled = true;
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		StartCoroutine(TryPatcherProcess());
	}

	private IEnumerator TryPatcherProcess()
	{
		Coroutine<string> routine = this.StartCoroutine<string>(PatcherProcess());
		yield return routine.coroutine;
		try
		{
			UnityEngine.Debug.Log(routine.Value);
		}
		catch (WebException ex)
		{
			if (ex.Status == WebExceptionStatus.ProtocolError)
			{
				string text = $"{((HttpWebResponse)ex.Response).StatusCode} : {((HttpWebResponse)ex.Response).StatusDescription}";
				UnityEngine.Debug.LogError(text);
				ShowError(text);
			}
			else
			{
				ShowError(ex);
			}
		}
		catch (Exception e)
		{
			ShowError(e);
		}
	}

	private IEnumerator PatcherProcess()
	{
		SetState(PATCHER_STATE.Init);
		int pmConnectionErrorCount = 0;
		while (true)
		{
			switch (m_currentPatcherState)
			{
			case PATCHER_STATE.Init:
				Init();
				SetState(PATCHER_STATE.DisplayLogo);
				break;
			case PATCHER_STATE.DisplayLogo:
				NKCUtil.SetGameobjectActive(m_NKCLogo, bValue: true);
				m_NKCLogo.Init();
				StartCoroutine(m_NKCLogo.DisplayLogo());
				SetState(PATCHER_STATE.InitPM);
				break;
			case PATCHER_STATE.InitPM:
				yield return PublisherModuleInitProcess();
				if (m_eResultCode != NKC_PUBLISHER_RESULT_CODE.NPRC_OK)
				{
					if (m_eResultCode == NKC_PUBLISHER_RESULT_CODE.NPRC_GAMEBASE_WEBSOCKET_READ_TIMEOUT && pmConnectionErrorCount < 10)
					{
						UnityEngine.Debug.Log($"[PatcherManager] InitPM Gamebase Timeout Count[{pmConnectionErrorCount}]");
						m_eResultCode = NKC_PUBLISHER_RESULT_CODE.NPRC_OK;
						SetState(PATCHER_STATE.InitPM);
						pmConnectionErrorCount++;
					}
					else
					{
						UnityEngine.Debug.Log("[PatcherManager] InitPM failed.");
						SetState(PATCHER_STATE.WaitStartUpProcess);
					}
					break;
				}
				s_bPubModuleInit = true;
				UnityEngine.Debug.Log("[PatcherManager] InitPM finished.");
				UnityEngine.Debug.Log("[PatcherManager] AppStart");
				NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.AppStart);
				NKCMMPManager.OnCustomEvent("01_appLaunch");
				if (NKCDefineManager.DEFINE_DOWNLOAD_CONFIG())
				{
					SetState(PATCHER_STATE.UpdateDownloadConfigProcess);
				}
				else
				{
					SetState(PATCHER_STATE.UpdateServerInfomation);
				}
				break;
			case PATCHER_STATE.UpdateDownloadConfigProcess:
				yield return UpdateDownloadConfigProcess();
				if (m_eResultCode != NKC_PUBLISHER_RESULT_CODE.NPRC_OK)
				{
					SetState(PATCHER_STATE.WaitStartUpProcess);
				}
				else
				{
					SetState(PATCHER_STATE.GetTagFromLoginServer);
				}
				break;
			case PATCHER_STATE.UpdateServerInfomation:
				yield return UpdateServerInfomationProcess();
				if (m_eResultCode != NKC_PUBLISHER_RESULT_CODE.NPRC_OK || NKCDefineManager.DEFINE_SELECT_SERVER())
				{
					SetState(PATCHER_STATE.WaitStartUpProcess);
				}
				else
				{
					SetState(PATCHER_STATE.GetTagFromLoginServer);
				}
				break;
			case PATCHER_STATE.GetTagFromLoginServer:
				yield return GetServerContentTagProcess();
				SetState(PATCHER_STATE.WaitStartUpProcess);
				break;
			case PATCHER_STATE.WaitStartUpProcess:
				while (!NKCLogo.s_bLogoPlayed)
				{
					yield return null;
				}
				if (m_eResultCode != NKC_PUBLISHER_RESULT_CODE.NPRC_OK)
				{
					SetState(PATCHER_STATE.TryRecoverTag);
				}
				else if (NKCDefineManager.DEFINE_SELECT_SERVER())
				{
					SetState(PATCHER_STATE.SelectServer);
				}
				else
				{
					SetState(PATCHER_STATE.TryRecoverTag);
				}
				break;
			case PATCHER_STATE.SelectServer:
			{
				NKM_NATIONAL_CODE nKM_NATIONAL_CODE = NKCGameOptionData.LoadLanguageCode(NKM_NATIONAL_CODE.NNC_END);
				if (nKM_NATIONAL_CODE == NKM_NATIONAL_CODE.NNC_END)
				{
					nKM_NATIONAL_CODE = NKCPublisherModule.Localization.GetDefaultLanguage();
				}
				UnityEngine.Debug.Log($"[PatcherManager] Set nation code for select server. : [{nKM_NATIONAL_CODE}]");
				NKCStringTable.SetNationalCode(nKM_NATIONAL_CODE);
				UnityEngine.Debug.Log($"[PatcherManager] Load patcher string table for select server. : [{nKM_NATIONAL_CODE}]");
				NKCPatcherStringList.LoadStrings("LUA_PATCH_STRING", "m_dicString", bOverwriteDuplicate: false);
				if (m_eResultCode == NKC_PUBLISHER_RESULT_CODE.NPRC_SERVERINFO_FAIL_SERVERINFO_UPDATE)
				{
					SetState(PATCHER_STATE.CheckCanStartPatch);
					break;
				}
				if (NKCConnectionInfo.GetLoginServerCount() == 1)
				{
					NKCConnectionInfo.CurrentLoginServerType = NKCConnectionInfo.GetFirstLoginServerType();
				}
				else if (NKCConnectionInfo.LastLoginServerType != NKCConnectionInfo.LOGIN_SERVER_TYPE.None && NKCConnectionInfo.HasLoginServerInfo(NKCConnectionInfo.LastLoginServerType))
				{
					NKCConnectionInfo.CurrentLoginServerType = NKCConnectionInfo.LastLoginServerType;
				}
				else
				{
					yield return ProcessSelectServer();
				}
				SetState(PATCHER_STATE.SetDefaultTagFromSelectedServer);
				break;
			}
			case PATCHER_STATE.SetDefaultTagFromSelectedServer:
				if (NKCConnectionInfo.CurrentLoginServerTagSet != null)
				{
					foreach (string item in NKCConnectionInfo.CurrentLoginServerTagSet)
					{
						NKMContentsVersionManager.AddTag(item);
					}
				}
				if (NKCDefineManager.DEFINE_USE_CHEAT())
				{
					NKMContentsVersionManager.PrintCurrentTagSet();
				}
				SetState(PATCHER_STATE.SetNationCode);
				break;
			case PATCHER_STATE.TryRecoverTag:
				NKCContentsVersionManager.TryRecoverTag();
				SetState(PATCHER_STATE.SetNationCode);
				break;
			case PATCHER_STATE.SetNationCode:
				yield return SetNationCode();
				SetState(PATCHER_STATE.LoadPatcherString);
				break;
			case PATCHER_STATE.LoadPatcherString:
				NKCPatcherStringList.LoadStrings("LUA_PATCH_STRING", "m_dicString", bOverwriteDuplicate: true);
				SetState(PATCHER_STATE.SelectVoice);
				break;
			case PATCHER_STATE.SelectVoice:
				if (!NKCUIVoiceManager.NeedSelectVoice())
				{
					UnityEngine.Debug.Log("[PatcherManager] Skip select voice process.");
				}
				else
				{
					yield return ProcessSelectVoice();
				}
				SetState(PATCHER_STATE.DisplayGameGrade);
				break;
			case PATCHER_STATE.DisplayGameGrade:
				NKCUtil.SetGameobjectActive(m_NKCLogo, bValue: true);
				m_NKCLogo.Init();
				if (NKCDefineManager.DEFINE_STEAM() && NKCConnectionInfo.CurrentLoginServerType == NKCConnectionInfo.LOGIN_SERVER_TYPE.Korea)
				{
					yield return m_NKCLogo.DisplayGameGrade();
				}
				SetState(PATCHER_STATE.Localizing);
				break;
			case PATCHER_STATE.Localizing:
			{
				m_NKCFontChanger.ChagneAllMainFont(NKCStringTable.GetNationalCode());
				NKC_VOICE_CODE nKC_VOICE_CODE = NKCUIVoiceManager.LoadLocalVoiceCode();
				NKCUIVoiceManager.SetVoiceCode(nKC_VOICE_CODE);
				AssetBundleManager.ActiveVariants = NKCLocalization.GetVariants(NKCStringTable.GetNationalCode(), nKC_VOICE_CODE);
				if (NKCDefineManager.DEFINE_PC_FORCE_VERSION_UP())
				{
					SetState(PATCHER_STATE.CheckPCVersionUp);
				}
				else
				{
					SetState(PATCHER_STATE.CheckCanStartPatch);
				}
				break;
			}
			case PATCHER_STATE.CheckPCVersionUp:
				if (ProcessPcForceVersionUp())
				{
					yield break;
				}
				SetState(PATCHER_STATE.CheckCanStartPatch);
				break;
			case PATCHER_STATE.CheckCanStartPatch:
				if (!CheckCanStartPatch())
				{
					yield break;
				}
				if (NKCDefineManager.DEFINE_OBB())
				{
					SetState(PATCHER_STATE.InitObb);
				}
				else
				{
					SetState(PATCHER_STATE.Patch);
				}
				break;
			case PATCHER_STATE.InitObb:
				NKCObbUtil.Init();
				SetState(PATCHER_STATE.Patch);
				break;
			case PATCHER_STATE.Patch:
				m_NkcPatchChecker.SetActive(active: true);
				yield return m_NkcPatchChecker.ProcessPatch();
				if (!m_NkcPatchChecker.PatchSuccess)
				{
					UnityEngine.Debug.LogError("ProcessPatch Error Occurred");
				}
				if (NKCDefineManager.DEFINE_SELECT_SERVER())
				{
					SetState(PATCHER_STATE.GetTagFromSelectedLoginServer);
				}
				else
				{
					SetState(PATCHER_STATE.StartGame);
				}
				break;
			case PATCHER_STATE.MakeDownloadList:
				yield return NKCPatchManifestManager.MakeDownloadList();
				SetState(PATCHER_STATE.MakeDownloadListForExtraAsset);
				break;
			case PATCHER_STATE.MakeDownloadListForExtraAsset:
				yield return NKCPatchManifestManager.MakeDownloadListForExtraAsset();
				SetState(PATCHER_STATE.MakeDownloadListForTutorialAsset);
				break;
			case PATCHER_STATE.MakeDownloadListForTutorialAsset:
				yield return NKCPatchManifestManager.MakeDownloadListForTutorialAsset();
				SetState(PATCHER_STATE.SelectPatchDownloadType);
				break;
			case PATCHER_STATE.SelectPatchDownloadType:
				if (NKCPatchUtility.BackgroundPatchEnabled())
				{
					List<NKCPopupDownloadTypeData> downloadTypeDataList = new List<NKCPopupDownloadTypeData>();
					m_popupDownloadTypeSelection.Open(NKCPatchUtility.SaveDownloadType, downloadTypeDataList);
					yield return m_popupDownloadTypeSelection.WaitForClick();
				}
				break;
			case PATCHER_STATE.GetTagFromSelectedLoginServer:
				yield return UpdateServerMaintenanceData();
				if (NKCConnectionInfo.IsServerUnderMaintenance())
				{
					Log.Debug("[PatcherManager] Selected server is under maintenance.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatcherManager.cs", 502);
					bool bWait = true;
					NKCUtil.SetGameobjectActive(m_popupBox, bValue: true);
					m_popupBox.OpenOKCancel(NKCUtilString.GET_STRING_PATCHER_NOTICE, NKCStringTable.GetString("SI_SYSTEM_NOTICE_MAINTENANCE_DESC"), delegate
					{
						m_NKCNoticeWeb.Open(NKCPublisherModule.Notice.NoticeUrl(bPatcher: true), delegate
						{
							bWait = false;
						}, bPatcher: true);
					}, delegate
					{
						NKCPopupOKCancel.ClosePopupBox();
						m_NKCSelectServer.Open(bShowCloseMenu: false, bMoveToPatcher: true, delegate
						{
							bWait = false;
						});
					}, null, NKCStringTable.GetString("SI_DP_LOGIN_SELECTSERVER"), bUseRedButton: false, bShowNpcIllust: false);
					while (bWait)
					{
						yield return null;
					}
					break;
				}
				NKCUtil.SetGameobjectActive(m_NKM_UI_WAIT, bValue: true);
				yield return GetServerContentTagProcess();
				NKCUtil.SetGameobjectActive(m_NKM_UI_WAIT, bValue: false);
				if (m_eResultCode != NKC_PUBLISHER_RESULT_CODE.NPRC_OK)
				{
					NKCConnectionInfo.DeleteLocalTag();
					ShowRequestTimer(bShow: false);
					bool bWait2 = true;
					NKCUtil.SetGameobjectActive(m_popupBox, bValue: true);
					m_popupBox.OpenOK(NKCUtilString.GET_STRING_PATCHER_NOTICE, NKCStringTable.GetString("SI_DP_PATCHER_ERROR_SERVER_TAG_UPDATE_FAILED"), delegate
					{
						m_NKCNoticeWeb.Open(NKCPublisherModule.Notice.NoticeUrl(bPatcher: true), delegate
						{
							bWait2 = false;
						}, bPatcher: true);
					}, "", bShowNpcIllust: false);
					while (bWait2)
					{
						yield return null;
					}
				}
				else
				{
					NKCConnectionInfo.SaveCurrentLoginServerType();
					SetState(PATCHER_STATE.StartGame);
				}
				break;
			case PATCHER_STATE.StartGame:
				StartGame();
				yield return "Patcher Process Succeed.";
				yield break;
			}
		}
	}

	private void SetState(PATCHER_STATE nextState)
	{
		if (m_currentPatcherState != PATCHER_STATE.None)
		{
			OnExit();
		}
		m_currentPatcherState = nextState;
		OnEnter();
	}

	private void OnEnter()
	{
		UnityEngine.Debug.Log($"[PatcherManager] EnterState [{m_currentPatcherState}]");
	}

	private void OnExit()
	{
		UnityEngine.Debug.Log($"[PatcherManager] ExitState [{m_currentPatcherState}]");
	}

	private void Init()
	{
		if (!s_bInit)
		{
			NKCLogManager.Init();
			UnityEngine.Debug.Log("[PatcherManager] Logmanager Init finished");
			UnityEngine.Debug.Log("[PatcherManager] Aplication Version [" + Application.version + "]");
			UnityEngine.Debug.Log("[PatcherManager] Aplication ProductName [" + Application.productName + "]");
			UnityEngine.Debug.Log($"[PatcherManager] Aplication Platform [{Application.platform}]");
			UnityEngine.Debug.Log("[PatcherManager] Aplication DataPath [" + Application.dataPath + "]");
			UnityEngine.Debug.Log("[PatcherManager] Aplication persistentDataPath [" + Application.persistentDataPath + "]");
			UnityEngine.Debug.unityLogger.logEnabled = NKCDefineManager.DEFINE_UNITY_DEBUG_LOG();
			NKCMMPManager.Init();
			ServiceManager.Init();
			BetterStreamingAssets.Initialize();
			DOTween.Init(true, true, NKCDefineManager.DEFINE_USE_CHEAT() ? LogBehaviour.Verbose : LogBehaviour.ErrorsOnly);
			DOTween.SetTweensCapacity(750, 50);
			QuitAppIfMultiClient();
			WindowedFullScreenForPC();
			s_bInit = true;
		}
		ServiceManager.DownloadService.BindService();
		NKMContentsVersionManager.Drop();
		NKCConnectionInfo.Clear();
		if (m_popupLanguageSelect != null)
		{
			m_popupLanguageSelect.Init();
			m_popupLanguageSelect.gameObject.SetActive(value: false);
		}
		if (m_PatcherManager == null)
		{
			m_PatcherManager = this;
			Screen.sleepTimeout = -1;
			Application.targetFrameRate = 30;
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void WindowedFullScreenForPC()
	{
		if (NKCDefineManager.DEFINE_UNITY_STANDALONE_WIN())
		{
			GameObject obj = new GameObject("NKCStandaloneScreenResolutionController");
			obj.AddComponent<NKCStandaloneScreenResolutionController>();
			UnityEngine.Object.DontDestroyOnLoad(obj);
		}
		if (NKCDefineManager.DEFINE_UNITY_STANDALONE() && !NKCDefineManager.DEFINE_UNITY_EDITOR() && PlayerPrefs.GetInt("NKM_LOCAL_SAVE_WINDOWED_FULL_RESOLUTION_FOR_PC", 0) == 0)
		{
			Resolution[] resolutions = Screen.resolutions;
			if (resolutions != null && resolutions.Length > 1)
			{
				Screen.SetResolution(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height, FullScreenMode.Windowed);
			}
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_WINDOWED_FULL_RESOLUTION_FOR_PC", 1);
		}
	}

	private void QuitAppIfMultiClient()
	{
		UnityEngine.Debug.Log("[PatcherManager] QuitAppIfMultiClient Prepare");
		if (NKCDefineManager.DEFINE_NX_PC())
		{
			UnityEngine.Debug.Log("[PatcherManager] NKCMultiClientPrevent prepare");
			if (s_objMultiClientPrevent == null)
			{
				s_objMultiClientPrevent = new GameObject("NKCMultiClientPrevent");
				UnityEngine.Object.DontDestroyOnLoad(s_objMultiClientPrevent);
				s_objMultiClientPrevent.AddComponent<NKCMultiClientPrevent>();
			}
		}
		if (!NKCDefineManager.DEFINE_ALLOW_MULTIPC() && Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
		{
			UnityEngine.Debug.Log("QuitAppIfMultiClient");
			Application.Quit();
		}
	}

	private IEnumerator PublisherModuleInitProcess()
	{
		if (s_bPubModuleInit)
		{
			yield break;
		}
		UnityEngine.Debug.Log("[PatcherManager] startup.initinstance start");
		bool bWait = true;
		NKCPublisherModule.InitInstance(delegate(NKC_PUBLISHER_RESULT_CODE resultCode, string add)
		{
			UnityEngine.Debug.Log($"[PatcherManager] startup.initinstance. resultCode : {resultCode}");
			bWait = false;
			if (resultCode == NKC_PUBLISHER_RESULT_CODE.NPRC_STEAM_INITIALIZE_FAIL || resultCode == NKC_PUBLISHER_RESULT_CODE.NPRC_GAMEBASE_WEBSOCKET_READ_TIMEOUT || resultCode == NKC_PUBLISHER_RESULT_CODE.NPRC_GAMEBASE_INITIALIZE_FAIL)
			{
				m_eResultCode = resultCode;
			}
		});
		while (bWait)
		{
			yield return null;
		}
	}

	private IEnumerator UpdateDownloadConfigProcess()
	{
		string localDownloadPath = AssetBundleManager.GetLocalDownloadPath();
		string text = "CSConfigServerAddress.txt";
		if (NKCDefineManager.DEFINE_SB_GB())
		{
			text = "csconfigserveraddress.txt";
		}
		string text2 = Application.streamingAssetsPath + "/" + text;
		UnityEngine.Debug.Log("[PatcherManager] CSConfigServerAddressPath : " + text2);
		if (!NKCPatchUtility.IsFileExists(text2))
		{
			UnityEngine.Debug.Log("[PatcherManager] CSConfigServerAddress does not exist");
			m_eResultCode = NKC_PUBLISHER_RESULT_CODE.NPRC_SERVERINFO_FAIL_FETCH_DOWNLOAD_ADDRESS;
			yield break;
		}
		UnityEngine.Debug.Log("[PatcherManager] CSConfigServerAddress exist");
		string aJSON = ((!text2.Contains("jar:")) ? File.ReadAllText(text2) : BetterStreamingAssets.ReadAllText(NKCAssetbundleInnerStream.GetJarRelativePath(text2)));
		JSONNode jSONNode = JSONNode.Parse(aJSON);
		if (jSONNode != null)
		{
			string targetFileName = Path.Combine(localDownloadPath, "CSConfig.txt");
			string vidSavePath = targetFileName;
			string url = jSONNode["address"];
			string text3 = jSONNode["languageTag"];
			if (text3 != null)
			{
				NKCPublisherModule.Localization.SetDefaultLanage((NKM_NATIONAL_CODE)Enum.Parse(typeof(NKM_NATIONAL_CODE), text3));
			}
			if (!Directory.Exists(Path.GetDirectoryName(vidSavePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(vidSavePath));
			}
			int tryCountMax = 1;
			if (NKCDefineManager.DEFINE_ZLONG())
			{
				tryCountMax = 10;
			}
			UnityEngine.Debug.Log("[PatcherManager] UpdateDownloadConfig Url[" + url + "] Tag[" + text3 + "]");
			for (int i = 0; i < tryCountMax; i++)
			{
				bool flag;
				using (UnityWebRequest uwr = new UnityWebRequest(url))
				{
					uwr.method = "GET";
					DownloadHandlerFile downloadHandlerFile = new DownloadHandlerFile(vidSavePath);
					downloadHandlerFile.removeFileOnAbort = true;
					uwr.downloadHandler = downloadHandlerFile;
					yield return uwr.SendWebRequest();
					if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
					{
						UnityEngine.Debug.Log("[PatcherManager] WebRequest error : " + uwr.error);
						if (i + 1 >= tryCountMax)
						{
							m_eResultCode = NKC_PUBLISHER_RESULT_CODE.NPRC_SERVERINFO_FAIL_FETCH_DOWNLOAD_ADDRESS;
							yield break;
						}
						yield return new WaitForSecondsRealtime(1f);
						continue;
					}
					UnityEngine.Debug.Log("[PatcherManager] Download saved to: " + vidSavePath.Replace("/", "\\") + "\r\n" + uwr.error);
					flag = true;
					if (NKCPatchUtility.IsFileExists(targetFileName))
					{
						aJSON = File.ReadAllText(targetFileName);
						JSONNode jSONNode2 = JSONNode.Parse(aJSON);
						if (jSONNode2 != null)
						{
							NKCConnectionInfo.DownloadServerAddress = jSONNode2["PatchServerAddress1"];
							NKCConnectionInfo.DownloadServerAddress2 = jSONNode2["PatchServerAddress2"];
							NKCDownloadConfig.s_ServerID = jSONNode2["ServerId"];
							NKCDownloadConfig.s_ServerName = jSONNode2["ServerName"];
							NKCConnectionInfo.SetLoginServerInfo(NKCConnectionInfo.LOGIN_SERVER_TYPE.Default, jSONNode2["CSLoginServerIP"], int.Parse(jSONNode2["CSLoginServerPort"]));
							foreach (JSONNode child in (jSONNode2["IgnoreVariantList"]?.AsArray).Children)
							{
								NKCConnectionInfo.IgnoreVariantList.Add(child);
							}
							if (jSONNode2["DefaultCountryTagSet"] != null)
							{
								NKCContentsVersionManager.TagVariableName = jSONNode2["DefaultCountryTagSet"];
							}
							if (jSONNode2["LOGIN_FAIL_MSG"] != null)
							{
								NKCConnectionInfo.s_LoginFailMsg = jSONNode2["LOGIN_FAIL_MSG"];
							}
							if (jSONNode2["ServerType"] != null)
							{
								NKCConnectionInfo.s_ServerType = jSONNode2["ServerType"];
							}
							if (NKCDefineManager.DEFINE_USE_CUSTOM_SERVERS())
							{
								UnityEngine.Debug.Log("[PatcherManager] Defined custom servers - checking for ServiceAddress Redirection");
								string text4 = PlayerPrefs.GetString("LOCAL_SAVE_CONTENTS_TAG_LAST_SERVER_IP");
								int num = PlayerPrefs.GetInt("LOCAL_SAVE_CONTENTS_TAG_LAST_SERVER_PORT");
								if (!string.IsNullOrEmpty(text4))
								{
									UnityEngine.Debug.Log("[PatcherManager] ServiceIP Redirected [" + NKCConnectionInfo.ServiceIP + "] -> [" + text4 + "]");
									NKCConnectionInfo.SetLoginServerInfo(NKCConnectionInfo.LOGIN_SERVER_TYPE.Default, text4);
								}
								if (num != 0)
								{
									UnityEngine.Debug.Log($"[PatcherManager] ServicePort Redirected [{NKCConnectionInfo.ServicePort}] -> [{num}]");
									NKCConnectionInfo.SetLoginServerInfo(NKCConnectionInfo.LOGIN_SERVER_TYPE.Default, "", num);
								}
							}
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		m_eResultCode = NKC_PUBLISHER_RESULT_CODE.NPRC_OK;
	}

	private IEnumerator UpdateServerInfomationProcess()
	{
		UnityEngine.Debug.Log($"[PatcherManager] UpdateServerInfomationProcess ServerInfo[{NKCPublisherModule.ServerInfo}] ServerInfoFileName[{NKCConnectionInfo.ServerInfoFileName}]");
		string serverConfigPath = NKCPublisherModule.ServerInfo.GetServerConfigPath();
		UnityEngine.Debug.Log("[PatcherManager] ServerInfo from " + serverConfigPath);
		using (UnityWebRequest uwr = new UnityWebRequest(serverConfigPath))
		{
			uwr.downloadHandler = new DownloadHandlerBuffer();
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
			{
				UnityEngine.Debug.Log("[PatcherManager] " + uwr.error);
				m_eResultCode = NKC_PUBLISHER_RESULT_CODE.NPRC_SERVERINFO_FAIL_SERVERINFO_UPDATE;
				yield break;
			}
			NKCConnectionInfo.LoadFromJSON(uwr.downloadHandler.text);
		}
		m_eResultCode = NKC_PUBLISHER_RESULT_CODE.NPRC_OK;
	}

	public IEnumerator UpdateServerMaintenanceData()
	{
		Log.Debug($"[PatcherManager][Maintenance] UpdateServerMaintenanceData ServerInfo[{NKCPublisherModule.ServerInfo}] ServerInfoFileName[{NKCConnectionInfo.ServerInfoFileName}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatcherManager.cs", 934);
		string serverConfigPath = NKCPublisherModule.ServerInfo.GetServerConfigPath();
		Log.Debug("[PatcherManager][Maintenance] UpdateServerMaintenanceData ServerInfo from " + serverConfigPath, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatcherManager.cs", 938);
		if (!NKCConnectionInfo.CheckDownloadInterval())
		{
			Log.Debug("[PatcherManager][Maintenance][CheckDownloadInterval] Interval", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatcherManager.cs", 942);
		}
		else
		{
			using UnityWebRequest uwr = new UnityWebRequest(serverConfigPath);
			uwr.downloadHandler = new DownloadHandlerBuffer();
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
			{
				Log.Debug("[PatcherManager] " + uwr.error, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatcherManager.cs", 954);
				m_eResultCode = NKC_PUBLISHER_RESULT_CODE.NPRC_SERVERINFO_FAIL_SERVERINFO_UPDATE;
				yield break;
			}
			NKCConnectionInfo.SetConfigJSONString(uwr.downloadHandler.text);
			Log.Debug("[PatcherManager][Maintenance] request success", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatcherManager.cs", 961);
		}
		NKCConnectionInfo.LoadMaintenanceDataFromJSON();
		m_eResultCode = NKC_PUBLISHER_RESULT_CODE.NPRC_OK;
	}

	private IEnumerator GetServerContentTagProcess()
	{
		string serviceIP = NKCConnectionInfo.ServiceIP;
		UnityEngine.Debug.Log("[PatcherManager] Trying to retrieve server tag from " + serviceIP);
		yield return ContentsVersionChecker.GetVersion(serviceIP, -1, NKCPublisherModule.ServerInfo.GetUseLocalSaveLastServerInfoToGetTags());
		if (ContentsVersionChecker.Ack != null)
		{
			m_eResultCode = NKC_PUBLISHER_RESULT_CODE.NPRC_OK;
			NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.Patch_TagProvided);
		}
		else
		{
			m_eResultCode = NKC_PUBLISHER_RESULT_CODE.NPRC_SERVERINFO_FAIL_FETCH_TAG;
			NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.Patch_TagGetFailed);
		}
	}

	public IEnumerator ProcessSelectServer()
	{
		m_NKCSelectServer.Open(bShowCloseMenu: false, bMoveToPatcher: false);
		NKCConnectionInfo.CurrentLoginServerType = NKCConnectionInfo.LOGIN_SERVER_TYPE.None;
		while (NKCConnectionInfo.CurrentLoginServerType == NKCConnectionInfo.LOGIN_SERVER_TYPE.None)
		{
			yield return null;
		}
		m_NKCSelectServer.Close();
	}

	private IEnumerator ProcessSelectVoice()
	{
		bool bWait = true;
		m_popupVoiceSelect.Init(delegate
		{
			bWait = false;
		}, bPatcher: true);
		m_popupVoiceSelect.Open();
		while (bWait)
		{
			yield return null;
		}
		m_popupVoiceSelect.Close();
	}

	private IEnumerator SetNationCode()
	{
		NKM_NATIONAL_CODE currentCode = NKCGameOptionData.LoadLanguageCode(NKM_NATIONAL_CODE.NNC_END);
		UnityEngine.Debug.Log("[PatcherManager] LoadedLanguageCode : [" + currentCode.ToString() + "]");
		HashSet<NKM_NATIONAL_CODE> setLanguages = NKCLocalization.GetSelectLanguageSet();
		if (currentCode == NKM_NATIONAL_CODE.NNC_END)
		{
			if (NKCPublisherModule.Localization.UseDefaultLanguageOnFirstRun)
			{
				currentCode = NKCPublisherModule.Localization.GetDefaultLanguage();
			}
			else if (setLanguages.Count == 0)
			{
				currentCode = NKM_NATIONAL_CODE.NNC_KOREA;
			}
			else if (setLanguages.Count == 1)
			{
				using HashSet<NKM_NATIONAL_CODE>.Enumerator enumerator = setLanguages.GetEnumerator();
				if (enumerator.MoveNext())
				{
					NKM_NATIONAL_CODE current = enumerator.Current;
					currentCode = current;
				}
			}
			else
			{
				bool bWait = true;
				m_popupLanguageSelect.Open(setLanguages, delegate(NKM_NATIONAL_CODE language)
				{
					currentCode = language;
					bWait = false;
				});
				while (bWait)
				{
					yield return null;
				}
			}
		}
		if (setLanguages.Count > 0 && !setLanguages.Contains(currentCode))
		{
			UnityEngine.Debug.Log($"[PatcherManager] LanguageSet does not contains selected code. : selected code : [{currentCode}]");
			using HashSet<NKM_NATIONAL_CODE>.Enumerator enumerator = setLanguages.GetEnumerator();
			if (enumerator.MoveNext())
			{
				NKM_NATIONAL_CODE current2 = enumerator.Current;
				currentCode = current2;
			}
		}
		UnityEngine.Debug.Log($"[PatcherManager] Set NationalCode : [{currentCode}]");
		SaveLanguageCode(currentCode);
		NKCStringTable.SetNationalCode(currentCode);
	}

	private bool CheckCanStartPatch()
	{
		UnityEngine.Debug.Log($"[PatcherManager] m_eResultCode : {m_eResultCode}]");
		if (m_eResultCode != NKC_PUBLISHER_RESULT_CODE.NPRC_OK)
		{
			string strID = "";
			bool flag = true;
			switch (m_eResultCode)
			{
			case NKC_PUBLISHER_RESULT_CODE.NPRC_SERVERINFO_FAIL_SERVERINFO_UPDATE:
				strID = "SI_DP_PATCHER_ERROR_SERVER_INFO_UPDATE_FAILED";
				break;
			case NKC_PUBLISHER_RESULT_CODE.NPRC_SERVERINFO_FAIL_FETCH_DOWNLOAD_ADDRESS:
				strID = "SI_DP_PATCHER_ERROR_DOWNLOAD_ADDRESS_FETCH_FAILED";
				break;
			case NKC_PUBLISHER_RESULT_CODE.NPRC_SERVERINFO_FAIL_FETCH_TAG:
				flag = false;
				AssetBundleManager.ActiveVariants = NKCLocalization.GetVariants(NKCStringTable.GetNationalCode(), NKCUIVoiceManager.LoadLocalVoiceCode());
				break;
			case NKC_PUBLISHER_RESULT_CODE.NPRC_STEAM_INITIALIZE_FAIL:
				strID = "SI_DP_PATCHER_FAIL_STEAM_INITIALIZE";
				break;
			case NKC_PUBLISHER_RESULT_CODE.NPRC_GAMEBASE_WEBSOCKET_READ_TIMEOUT:
				strID = "SI_DP_PATCHER_DOWNLOAD_ERROR";
				break;
			case NKC_PUBLISHER_RESULT_CODE.NPRC_GAMEBASE_INITIALIZE_FAIL:
				strID = "SI_DP_PATCHER_DOWNLOAD_ERROR";
				break;
			default:
				flag = false;
				break;
			}
			if (flag)
			{
				NKCUtil.SetGameobjectActive(m_popupBox, bValue: true);
				m_popupBox.OpenOK(NKCUtilString.GET_STRING_PATCHER_ERROR, NKCStringTable.GetString(strID), delegate
				{
					Application.Quit();
				}, "", bShowNpcIllust: false);
				return false;
			}
		}
		return true;
	}

	private bool ProcessPcForceVersionUp()
	{
		if (NKCDownloadConfig.s_vecAllowedVersion.Count <= 0)
		{
			return false;
		}
		bool flag = false;
		for (int i = 0; i < NKCDownloadConfig.s_vecAllowedVersion.Count; i++)
		{
			if (Application.version == NKCDownloadConfig.s_vecAllowedVersion[i])
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			m_NkcPatchChecker.gameObject.SetActive(value: true);
			NKCUtil.SetGameobjectActive(m_popupBox, bValue: true);
			string text = "";
			text = ((NKCPublisherModule.PublisherType != NKCPublisherModule.ePublisherType.Zlong) ? NKCUtilString.GET_STRING_PATCHER_NEED_UPDATE : NKCUtilString.GET_STRING_PATCHER_PC_NEW_APP_AVAILABLE_ZLONG);
			m_popupBox.OpenOK(NKCUtilString.GET_STRING_PATCHER_ERROR, text, delegate
			{
				Application.Quit();
			}, "", bShowNpcIllust: false);
			return true;
		}
		return false;
	}

	private void SaveLanguageCode(NKM_NATIONAL_CODE code)
	{
		NKCGameOptionData.SaveOnlyLang(code);
		NKCPublisherModule.Localization.SetPublisherModuleLanguage(code);
	}

	public void SetSafeArea()
	{
		if (m_NKC_PATCHER_UI_FRONT_RectTransform != null)
		{
			Vector2 vector = m_NKC_PATCHER_UI_FRONT_RectTransform.localScale;
			vector.x = Screen.safeArea.width / (float)Screen.currentResolution.width;
			vector.y = Screen.safeArea.height / (float)Screen.currentResolution.height;
			if (vector.x > vector.y)
			{
				vector.x = vector.y;
			}
			else
			{
				vector.y = vector.x;
			}
			m_NKC_PATCHER_UI_FRONT_RectTransform.localScale = vector;
		}
	}

	public void StartGame()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_WAIT, bValue: true);
		Application.backgroundLoadingPriority = ThreadPriority.High;
		AssetBundleManager.LoadLevelAsync("NKM_SCEN_APP", isAddictive: false, null);
	}

	private void UnloadFonts(Font[] fonts)
	{
		foreach (Font font in fonts)
		{
			if (font.name.Contains("MainFont") || font.name.Contains("Rajdhani"))
			{
				Resources.UnloadAsset(font);
			}
		}
	}

	public void ShowRequestTimer(bool bShow)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_WAIT, bShow);
		NKCUtil.SetGameobjectActive(m_NKM_UI_TAG_REQUEST_NOTICE, bShow);
		NKCUtil.SetGameobjectActive(m_TagListRequestMsg, bValue: false);
		if (!string.IsNullOrEmpty(NKCConnectionInfo.s_LoginFailMsg))
		{
			NKCUtil.SetGameobjectActive(m_TagListRequestMsg, bValue: true);
			NKCUtil.SetLabelText(m_TagListRequestMsg, NKCConnectionInfo.s_LoginFailMsg);
		}
		else if (!m_bRunningMsgCoroutine)
		{
			StartCoroutine(UpdateLoginMsg());
		}
	}

	private IEnumerator UpdateLoginMsg()
	{
		m_bRunningMsgCoroutine = true;
		string localDownloadPath = AssetBundleManager.GetLocalDownloadPath();
		string text = Application.streamingAssetsPath + "/CSConfigServerAddress.txt";
		if (!NKCPatchUtility.IsFileExists(text))
		{
			yield break;
		}
		UnityEngine.Debug.Log("[PatcherManager] CSConfigServerAddress exist");
		string aJSON = ((!text.Contains("jar:")) ? File.ReadAllText(text) : BetterStreamingAssets.ReadAllText(NKCAssetbundleInnerStream.GetJarRelativePath(text)));
		JSONNode jSONNode = JSONNode.Parse(aJSON);
		if (jSONNode == null)
		{
			yield break;
		}
		string url = jSONNode["address"];
		string targetFileName = Path.Combine(localDownloadPath, "CSConfig.txt");
		if (!Directory.Exists(Path.GetDirectoryName(targetFileName)))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));
		}
		using (UnityWebRequest uwr = new UnityWebRequest(url))
		{
			uwr.method = "GET";
			DownloadHandlerFile downloadHandlerFile = new DownloadHandlerFile(targetFileName);
			downloadHandlerFile.removeFileOnAbort = true;
			uwr.downloadHandler = downloadHandlerFile;
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
			{
				UnityEngine.Debug.Log("[PatcherManager] " + uwr.error);
				yield break;
			}
			UnityEngine.Debug.Log("[PatcherManager] Download saved to: " + targetFileName.Replace("/", "\\") + "\r\n" + uwr.error);
			if (!NKCPatchUtility.IsFileExists(targetFileName))
			{
				yield break;
			}
			aJSON = File.ReadAllText(targetFileName);
			JSONNode jSONNode2 = JSONNode.Parse(aJSON);
			if (jSONNode2 != null)
			{
				NKCConnectionInfo.s_LoginFailMsg = jSONNode2["LOGIN_FAIL_MSG"];
				NKCUtil.SetLabelText(m_TagListRequestMsg, NKCConnectionInfo.s_LoginFailMsg);
			}
		}
		m_bRunningMsgCoroutine = false;
	}

	public void ShowError(Exception e)
	{
		UnityEngine.Debug.LogError(e);
		int currentPatcherState = (int)m_currentPatcherState;
		string text = currentPatcherState.ToString();
		string msg = ((!NKCStringTable.CheckExistString("SI_DP_PATCH_PROCESS_EXCEPTION")) ? ("SI_DP_PATCH_PROCESS_EXCEPTION [" + text + "]") : NKCStringTable.GetString("SI_DP_PATCH_PROCESS_EXCEPTION", false, text));
		ShowError(msg);
	}

	public void ShowError(string msg)
	{
		NKCUtil.SetGameobjectActive(m_popupBox, bValue: true);
		if (NKCDefineManager.DEFINE_USE_CHEAT() || !NKCDefineManager.DEFINE_SERVICE())
		{
			msg = msg + "\nNationTag: " + NKMContentsVersionManager.GetCountryTag();
		}
		m_popupBox.OpenOK(NKCUtilString.GET_STRING_PATCHER_ERROR, msg, delegate
		{
			Application.Quit();
		}, "", bShowNpcIllust: false);
		StopAllCoroutines();
	}

	public void ShowUpdate()
	{
		StopAllCoroutines();
		NKCUtil.SetGameobjectActive(m_popupBox, bValue: true);
		m_popupBox.OpenOK(NKCUtilString.GET_STRING_PATCHER_NOTICE, NKCUtilString.GET_STRING_PATCHER_NEED_UPDATE, MoveToMarket, "", bShowNpcIllust: false);
	}

	public void MoveToMarket()
	{
		if (NKCPatchDownloader.Instance != null)
		{
			NKCPatchDownloader.Instance.MoveToMarket();
		}
		else
		{
			Application.Quit();
		}
	}

	public IEnumerator WaitForOKCancel(string title, string msg, string OKButtonString, string cancelButtonString, NKCPopupOKCancel.OnButton onOK)
	{
		bool bChecked = false;
		NKCUtil.SetGameobjectActive(m_popupBox, bValue: true);
		m_popupBox.OpenOKCancel(title, msg, delegate
		{
			onOK?.Invoke();
			bChecked = true;
		}, delegate
		{
			bChecked = true;
		}, OKButtonString, cancelButtonString, bUseRedButton: false, bShowNpcIllust: false);
		while (!bChecked)
		{
			if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Keypad5))
			{
				m_popupBox.OnOK();
			}
			else if (Input.GetKeyUp(KeyCode.Escape))
			{
				m_popupBox.OnBackButton();
			}
			yield return null;
		}
	}

	public IEnumerator WaitForDownloadTypeSelect(Action<NKCPatchDownloader.DownType> okAction, float totalDownloadSize, float essentialDownloadSize, float nonEssentialDownloadSize, float tutorialDownloadSize)
	{
		if (m_popupDownloadTypeSelection == null)
		{
			NKCPatchUtility.SaveDownloadType(NKCPatchDownloader.DownType.FullDownload);
			UnityEngine.Debug.LogWarning("DownloadTypeSelect is null");
		}
		else
		{
			m_popupDownloadTypeSelection.Open(okAction, totalDownloadSize, essentialDownloadSize, nonEssentialDownloadSize, tutorialDownloadSize);
			yield return m_popupDownloadTypeSelection.WaitForClick();
		}
	}

	public IEnumerator WaitForOKBox(string title, string msg, string OKButtonString = "")
	{
		bool bChecked = false;
		NKCUtil.SetGameobjectActive(m_popupBox, bValue: true);
		m_popupBox.OpenOK(title, msg, delegate
		{
			bChecked = true;
		}, OKButtonString);
		while (!bChecked)
		{
			if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Keypad5))
			{
				m_popupBox.OnOK();
			}
			yield return null;
		}
	}
}
