using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NKC.Office;
using NKC.UI.Office;
using NKM;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIJukeBox : NKCUIBase
{
	public delegate void OnMusicSelected(int id);

	private enum eSrotType
	{
		ST_ORDER_IDX,
		ST_BGM_NAME
	}

	private enum LIST_TYPE
	{
		TOTAL,
		FAVORITE
	}

	private enum REPEAT_STATUS
	{
		NONE,
		TOTAL_REPEAT,
		SINGLE_REPEAT
	}

	private enum MUSIC_STATUS
	{
		PAUSE,
		PLAY
	}

	public const string ASSET_BUNDLE_NAME = "ab_ui_bgm";

	public const string UI_ASSET_NAME = "AB_UI_BGM";

	private static NKCUIJukeBox m_Instance;

	public bool AlreadyJukeBoxMode;

	[Header("Top Menu")]
	public GameObject m_objNewRedDot;

	public NKCUIComToggle m_ctglTotalList;

	public NKCUIComToggle m_ctglFavoriteList;

	public NKCUIComToggle m_ctglSortDefault;

	public NKCUIComToggle m_ctglSortName;

	public LoopVerticalScrollRect m_LoopScroll;

	public GameObject m_objNone;

	[Header("Cover")]
	public GameObject m_ObjCover;

	public Image m_imgCover;

	public NKCComText m_lbBGMTitle;

	public NKCUIComToggle m_ctglFavorite;

	[Header("Controller")]
	public Text m_lbPlayTime;

	public Text m_lbTotalTime;

	public Slider m_sdProgressBar;

	public EventTrigger m_sdEvTrigger;

	public NKCUIComStateButton m_csbtnPrev;

	public NKCUIComStateButton m_csbtnNext;

	public NKCUIComToggle m_ctglPlayOrPause;

	public NKCUIComToggle m_ctglRandom;

	public NKCUIComStateButton m_csbtnRepeat;

	public GameObject m_objRepeatNormal;

	public GameObject m_objRepeatTotal;

	public GameObject m_objRepeatSingle;

	public NKCUIComStateButton m_csbtnClear;

	public NKCUIComStateButton m_csbtnSaveMode;

	public NKCUIComStateButton m_csbtnSaveLobbyMusic;

	public Text m_lbSaveBtnTitle;

	[Header("주크박스 슬롯")]
	public NKCJukeBoxSlot m_pfbJukeBoxSlot;

	private List<NKCJukeBoxSlot> m_lstJukeBoxSlots = new List<NKCJukeBoxSlot>();

	private List<NKCBGMInfoTemplet> m_lstBGMData;

	private OnMusicSelected dOnMusicSelected;

	private bool m_bLobbyMusicSelectMode;

	private List<int> m_lstNewOpendBGMKeys = new List<int>();

	private NKCBGMInfoTemplet m_CurBGMInfoTemplet;

	private eSrotType m_eCurSortType;

	private LIST_TYPE m_curListType;

	private REPEAT_STATUS m_curRepeatStatus;

	private MUSIC_STATUS m_curMusicStatus;

	public float m_fTimeOffSet = 0.02f;

	private float m_fSaveDelayTime;

	private float m_fCurPlayTime;

	private float m_fCurTotalTime;

	public static NKCUIJukeBox Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIJukeBox>("ab_ui_bgm", "AB_UI_BGM", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIJukeBox>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_JUKEBOX_TITLE;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.BackButtonOnly;

	public static bool IsHasInstance => m_Instance != null;

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

	private List<NKCBGMInfoTemplet> lstBGMInfoTemplet
	{
		get
		{
			List<NKCBGMInfoTemplet> list = new List<NKCBGMInfoTemplet>();
			if (m_curListType == LIST_TYPE.FAVORITE)
			{
				list = GetFavoriteList();
			}
			else
			{
				foreach (NKCBGMInfoTemplet value in NKMTempletContainer<NKCBGMInfoTemplet>.Values)
				{
					if (string.IsNullOrEmpty(value.m_OpenTag) || value.EnableByTag)
					{
						list.Add(value);
					}
				}
			}
			if (m_eCurSortType == eSrotType.ST_ORDER_IDX)
			{
				list.Sort((NKCBGMInfoTemplet x, NKCBGMInfoTemplet y) => x.m_OrderIdx.CompareTo(y.m_OrderIdx));
			}
			else
			{
				list.Sort((NKCBGMInfoTemplet x, NKCBGMInfoTemplet y) => NKCStringTable.GetString(x.m_BgmNameStringID).CompareTo(NKCStringTable.GetString(y.m_BgmNameStringID)));
			}
			return list;
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		ClearSlots();
		AlreadyJukeBoxMode = NKCScenManager.GetScenManager().GetNKCPowerSaveMode().IsJukeBoxMode;
		NKCUIManager.GetNKCUIPowerSaveMode().SetFinishJukeBox(bFinish: false);
		NKCScenManager.GetScenManager().GetNKCPowerSaveMode().SetJukeBoxMode(bEnable: false);
		NKCSoundManager.StopAllSound();
		if (!m_bLobbyMusicSelectMode)
		{
			NKCSoundManager.StopMusic();
			NKCSoundManager.PlayScenMusic(NKCScenManager.GetScenManager().GetNowScenID());
		}
		if (SaveAllSongList() && NKCUIOffice.IsInstanceOpen)
		{
			NKCUIOffice.GetInstance().UpdateAlarm();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		NKCUtil.SetHotkey(m_csbtnPrev, HotkeyEventType.Left);
		NKCUtil.SetHotkey(m_csbtnNext, HotkeyEventType.Right);
		NKCUtil.SetHotkey(m_ctglPlayOrPause, HotkeyEventType.Confirm);
		if (null != m_sdProgressBar)
		{
			if (m_sdProgressBar.minValue != 0f || m_sdProgressBar.maxValue != 1f)
			{
				Debug.LogError("[JukeBox:Init]Error - Not Matched Slider Value : 0 to 1");
			}
			m_sdProgressBar.value = 0f;
		}
		if (NKMTempletContainer<NKCBGMInfoTemplet>.Values.ToList().Count <= 0)
		{
			Debug.LogError("[JukeBox:Init]Error - NKCBGMInfoTempletis empty");
		}
		NKCUtil.SetEventTriggerDelegate(m_sdEvTrigger, delegate(BaseEventData data)
		{
			OnBeginDrag(data);
		}, EventTriggerType.BeginDrag);
		NKCUtil.SetEventTriggerDelegate(m_sdEvTrigger, delegate(BaseEventData data)
		{
			OnDrag(data);
		}, EventTriggerType.Drag, bInit: false);
		NKCUtil.SetEventTriggerDelegate(m_sdEvTrigger, delegate(BaseEventData data)
		{
			EndDragDrag(data);
		}, EventTriggerType.EndDrag, bInit: false);
		NKCUtil.SetEventTriggerDelegate(m_sdEvTrigger, delegate(BaseEventData data)
		{
			OnSilderClick(data);
		}, EventTriggerType.PointerClick, bInit: false);
		if (null != m_LoopScroll)
		{
			m_LoopScroll.dOnGetObject += GetSlot;
			m_LoopScroll.dOnReturnObject += ReturnSlot;
			m_LoopScroll.dOnProvideData += ProvideSlot;
			m_LoopScroll.ContentConstraintCount = 1;
			m_LoopScroll.PrepareCells();
			NKCUtil.SetScrollHotKey(m_LoopScroll);
		}
		NKCUtil.SetBindFunction(m_csbtnClear, OnClickClearLobbyMusic);
		NKCUtil.SetBindFunction(m_csbtnSaveLobbyMusic, OnClickSaveLobbyMusic);
		NKCUtil.SetBindFunction(m_csbtnSaveMode, OnClickSaveMode);
		NKCUtil.SetBindFunction(m_csbtnRepeat, OnClickRepeat);
		NKCUtil.SetBindFunction(m_csbtnPrev, OnClickPrevMusic);
		NKCUtil.SetBindFunction(m_csbtnNext, OnClickNextMusic);
		NKCUtil.SetToggleValueChangedDelegate(m_ctglTotalList, OnClickTotalListToggle);
		NKCUtil.SetToggleValueChangedDelegate(m_ctglFavoriteList, OnClickFavoriteListToggle);
		NKCUtil.SetToggleValueChangedDelegate(m_ctglFavorite, OnClickFavorite);
		NKCUtil.SetToggleValueChangedDelegate(m_ctglPlayOrPause, OnClickPlayToggle);
		NKCUtil.SetToggleValueChangedDelegate(m_ctglRandom, OnClickRandomToggle);
		NKCUtil.SetToggleValueChangedDelegate(m_ctglSortDefault, OnClickSortToggle);
		NKCUtil.SetToggleValueChangedDelegate(m_ctglSortName, OnClickSortToggleName);
		m_curListType = LIST_TYPE.TOTAL;
	}

	public void Open(bool bLobbyMusicSelectMode, int selectedMusicID = 0, OnMusicSelected onMusicSelected = null)
	{
		m_bLobbyMusicSelectMode = bLobbyMusicSelectMode;
		dOnMusicSelected = onMusicSelected;
		AlreadyJukeBoxMode = false;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCSoundManager.StopAllSound();
		NKCSoundManager.StopMusic();
		OnceUpdate(selectedMusicID, bLobbyMusicSelectMode);
		NKCUtil.SetBindFunction(m_csbtnSaveLobbyMusic, OnClickSaveLobbyMusic);
		NKCUtil.SetBindFunction(m_csbtnClear, OnClickClearLobbyMusic);
		NKCUtil.SetLabelText(m_lbSaveBtnTitle, NKCUtilString.GET_STRING_JUKEBOX_SAVE_BTN_TITLE_APPLY_LOBBY);
		UIOpened();
	}

	public void Open(int gauntletBgmID = 0)
	{
		AlreadyJukeBoxMode = false;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCSoundManager.StopAllSound();
		NKCSoundManager.StopMusic();
		OnceUpdate(gauntletBgmID, bInLobby: true);
		NKCUtil.SetLabelText(m_lbSaveBtnTitle, NKCUtilString.GET_STRING_JUKEBOX_SAVE_BTN_TITLE_APPLY);
		NKCUtil.SetBindFunction(m_csbtnSaveLobbyMusic, OnClickSaveGauntletBGM);
		NKCUtil.SetBindFunction(m_csbtnClear, OnClickClearGauntleBGM);
		UIOpened();
	}

	private void OnceUpdate(int selectedMusicID, bool bInLobby)
	{
		m_curListType = LIST_TYPE.TOTAL;
		m_lstBGMData = lstBGMInfoTemplet;
		m_lstBGMData.Sort((NKCBGMInfoTemplet x, NKCBGMInfoTemplet y) => x.m_OrderIdx.CompareTo(y.m_OrderIdx));
		m_eCurSortType = eSrotType.ST_ORDER_IDX;
		m_ctglSortDefault.Select(bSelect: true, bForce: true);
		RefreshSlots();
		UpdateNewBGMList();
		int num = selectedMusicID;
		if (num == 0)
		{
			List<NKCBGMInfoTemplet> list = NKMTempletContainer<NKCBGMInfoTemplet>.Values.ToList();
			list.Sort((NKCBGMInfoTemplet x, NKCBGMInfoTemplet y) => x.m_OrderIdx.CompareTo(y.m_OrderIdx));
			foreach (NKCBGMInfoTemplet item in list)
			{
				if (CheckOpenCond(item))
				{
					num = item.Key;
					break;
				}
			}
		}
		m_fCurPlayTime = 0f;
		OnSelectedMusic(num);
		m_ctglRandom.Select(bSelect: false, bForce: true);
		m_curRepeatStatus = REPEAT_STATUS.NONE;
		UpdateRepeatUI();
		m_curMusicStatus = MUSIC_STATUS.PAUSE;
		m_ctglPlayOrPause.Select(bSelect: false, bForce: true);
		NKCUtil.SetGameobjectActive(m_csbtnClear, bInLobby);
		NKCUtil.SetGameobjectActive(m_csbtnSaveLobbyMusic, bInLobby);
		NKCUtil.SetGameobjectActive(m_csbtnSaveMode, !bInLobby);
		NKCUtil.SetGameobjectActive(m_ctglRandom, !bInLobby);
		NKCUtil.SetGameobjectActive(m_csbtnRepeat, !bInLobby);
		m_ctglTotalList.Select(bSelect: true, bForce: true);
	}

	private bool SaveAllSongList()
	{
		List<int> list = LoadOpendSongList();
		List<int> list2 = new List<int>();
		foreach (NKCBGMInfoTemplet value in NKMTempletContainer<NKCBGMInfoTemplet>.Values)
		{
			if (CheckOpenCond(value))
			{
				list2.Add(value.Key);
			}
		}
		SaveOpendBGMList(list2);
		if (list != null)
		{
			return list.Count < list2.Count;
		}
		return false;
	}

	private List<int> LoadOpendSongList()
	{
		List<int> list = new List<int>();
		List<int> list2 = LoadAlreadyOpendBGMList();
		foreach (NKCBGMInfoTemplet lstBGMDatum in m_lstBGMData)
		{
			if (CheckOpenCond(lstBGMDatum) && !list2.Contains(lstBGMDatum.Key))
			{
				list.Add(lstBGMDatum.Key);
			}
		}
		return list;
	}

	private void UpdateNewBGMList()
	{
		if (m_lstBGMData != null)
		{
			m_lstNewOpendBGMKeys = LoadOpendSongList();
			NKCUtil.SetGameobjectActive(m_objNewRedDot, m_lstNewOpendBGMKeys.Count > 0);
		}
	}

	private static List<int> LoadAlreadyOpendBGMList()
	{
		List<int> list = new List<int>();
		string userKey = GetUserKey(bFavorite: false);
		if (PlayerPrefs.HasKey(userKey))
		{
			string[] array = PlayerPrefs.GetString(userKey).Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				int.TryParse(array[i], out var result);
				NKCBGMInfoTemplet nKCBGMInfoTemplet = NKMTempletContainer<NKCBGMInfoTemplet>.Find(result);
				if (nKCBGMInfoTemplet != null)
				{
					list.Add(result);
				}
			}
		}
		return list;
	}

	public static bool HasNewMusic()
	{
		List<int> list = LoadAlreadyOpendBGMList();
		foreach (NKCBGMInfoTemplet value in NKMTempletContainer<NKCBGMInfoTemplet>.Values)
		{
			if (!list.Contains(value.Key) && CheckOpenCond(value))
			{
				return true;
			}
		}
		return false;
	}

	private void SaveOpendBGMList(List<int> lstMusic)
	{
		string userKey = GetUserKey(bFavorite: false);
		if (lstMusic == null || lstMusic.Count <= 0)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (int item in lstMusic)
		{
			stringBuilder.Append($"{item},");
		}
		PlayerPrefs.SetString(userKey, stringBuilder.ToString());
	}

	private static string GetUserKey(bool bFavorite)
	{
		long userUID = NKCScenManager.CurrentUserData().m_UserUID;
		if (bFavorite)
		{
			return string.Format("JukeF_" + userUID);
		}
		return string.Format("Juke_" + userUID);
	}

	private void SaveFavoriteList(List<int> lstMusic)
	{
		string userKey = GetUserKey(bFavorite: true);
		if (lstMusic != null && lstMusic.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int item in lstMusic)
			{
				stringBuilder.Append($"{item},");
			}
			PlayerPrefs.SetString(userKey, stringBuilder.ToString());
		}
		else
		{
			PlayerPrefs.DeleteKey(userKey);
		}
	}

	private List<int> LoadFavoriteList()
	{
		List<int> list = new List<int>();
		string userKey = GetUserKey(bFavorite: true);
		if (!string.IsNullOrEmpty(userKey) && PlayerPrefs.HasKey(userKey))
		{
			string[] array = PlayerPrefs.GetString(userKey).Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				int.TryParse(array[i], out var result);
				NKCBGMInfoTemplet nKCBGMInfoTemplet = NKMTempletContainer<NKCBGMInfoTemplet>.Find(result);
				if (nKCBGMInfoTemplet != null)
				{
					list.Add(result);
				}
			}
		}
		return list;
	}

	private List<NKCBGMInfoTemplet> GetFavoriteList()
	{
		List<NKCBGMInfoTemplet> list = new List<NKCBGMInfoTemplet>();
		foreach (int item in LoadFavoriteList())
		{
			NKCBGMInfoTemplet nKCBGMInfoTemplet = NKMTempletContainer<NKCBGMInfoTemplet>.Find(item);
			if (nKCBGMInfoTemplet != null)
			{
				list.Add(nKCBGMInfoTemplet);
			}
		}
		return list;
	}

	private void RefreshSlots()
	{
		m_LoopScroll.TotalCount = m_lstBGMData.Count;
		m_LoopScroll.PrepareCells();
		m_LoopScroll.RefreshCells(bForce: true);
		NKCUtil.SetGameobjectActive(m_objNone, m_lstBGMData.Count <= 0);
	}

	private RectTransform GetSlot(int index)
	{
		NKCJukeBoxSlot nKCJukeBoxSlot = UnityEngine.Object.Instantiate(m_pfbJukeBoxSlot);
		nKCJukeBoxSlot.transform.localScale = Vector3.one;
		m_lstJukeBoxSlots.Add(nKCJukeBoxSlot);
		return nKCJukeBoxSlot.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
	}

	private void ProvideSlot(Transform tr, int idx)
	{
		NKCJukeBoxSlot component = tr.GetComponent<NKCJukeBoxSlot>();
		if (component != null)
		{
			List<int> list = LoadFavoriteList();
			tr.SetParent(m_LoopScroll.content);
			component.Init();
			component.SetData(m_lstBGMData[idx], OnClickJukeBoxSlot, list.Contains(m_lstBGMData[idx].Key), CheckOpenCond(m_lstBGMData[idx]), m_lstNewOpendBGMKeys.Contains(m_lstBGMData[idx].Key));
			if (m_CurBGMInfoTemplet != null && m_lstBGMData[idx].Key == m_CurBGMInfoTemplet.Key)
			{
				component.SetStat(bPlay: true);
			}
			NKCUtil.SetGameobjectActive(tr.gameObject, bValue: true);
		}
	}

	private void ClearSlots()
	{
		for (int i = 0; i < m_lstJukeBoxSlots.Count; i++)
		{
			if (m_lstJukeBoxSlots[i] != null)
			{
				UnityEngine.Object.DestroyImmediate(m_lstJukeBoxSlots[i].gameObject);
				m_lstJukeBoxSlots[i] = null;
			}
		}
		m_lstJukeBoxSlots.Clear();
	}

	private void OnSelectedMusic(int idx)
	{
		NKCBGMInfoTemplet nKCBGMInfoTemplet = m_lstBGMData.Find((NKCBGMInfoTemplet e) => e.Key == idx);
		if (nKCBGMInfoTemplet != null)
		{
			m_CurBGMInfoTemplet = nKCBGMInfoTemplet;
			UpdateAlbumUI();
			UpdateSlotUI();
			AudioClip audioClip = GetAudioClip(m_CurBGMInfoTemplet.m_BgmAssetID);
			if (null != audioClip)
			{
				m_fCurTotalTime = audioClip.length;
				UpdatePlayTimeUI();
			}
		}
	}

	public void OnClickJukeBoxSlot(int idx)
	{
		if (m_CurBGMInfoTemplet == null || m_CurBGMInfoTemplet.Key != idx)
		{
			OnSelectedMusic(idx);
			PlayMusic();
		}
	}

	private void OnBeginDrag(BaseEventData evtData)
	{
		PauseMusic();
	}

	public void OnDrag(BaseEventData evtData)
	{
		UpdateMusicTime(m_sdProgressBar.value);
	}

	public void EndDragDrag(BaseEventData evtData)
	{
		if (m_ctglPlayOrPause.m_bSelect)
		{
			PlayMusic();
		}
		UpdateMusicTime(m_sdProgressBar.value);
	}

	private void OnSilderClick(BaseEventData evtData)
	{
		UpdateMusicTime(m_sdProgressBar.value);
	}

	private void OnClickClearLobbyMusic()
	{
		Close();
		dOnMusicSelected?.Invoke(0);
	}

	private void OnClickSaveLobbyMusic()
	{
		if (m_CurBGMInfoTemplet != null)
		{
			Close();
			dOnMusicSelected?.Invoke(m_CurBGMInfoTemplet.Key);
		}
	}

	private void OnClickSaveGauntletBGM()
	{
		if (m_CurBGMInfoTemplet != null)
		{
			if (m_fSaveDelayTime > 0f)
			{
				Debug.Log("<color=red>잠시 후 시도해 주세요!</color>");
				return;
			}
			NKCPacketSender.Send_NKMPacket_JUKEBOX_CHANGE_BGM_REQ(NKM_BGM_TYPE.PVP_INGAME, m_CurBGMInfoTemplet.Key);
			m_fSaveDelayTime = 1f;
			Close();
		}
	}

	private void OnClickClearGauntleBGM()
	{
		Close();
		NKCPacketSender.Send_NKMPacket_JUKEBOX_CHANGE_BGM_REQ(NKM_BGM_TYPE.PVP_INGAME, 0);
	}

	private void OnClickSaveMode()
	{
		if (m_curMusicStatus == MUSIC_STATUS.PAUSE)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_JUKEBOX_BLOCK_SLEEP_MODE);
			return;
		}
		NKCUIManager.GetNKCUIPowerSaveMode().SetFinishJukeBox(bFinish: false);
		NKCScenManager.GetScenManager().GetNKCPowerSaveMode().SetJukeBoxMode(bEnable: true);
		UpdateJukeBoxTitle();
		NKCScenManager.GetScenManager().GetNKCPowerSaveMode().SetEnable(bEnable: true);
	}

	private void UpdateJukeBoxTitle()
	{
		if (NKCScenManager.GetScenManager().GetNKCPowerSaveMode().IsJukeBoxMode && m_CurBGMInfoTemplet != null)
		{
			NKCUIManager.GetNKCUIPowerSaveMode().SetJukeBoxTitle(NKCStringTable.GetString(m_CurBGMInfoTemplet.m_BgmNameStringID));
		}
	}

	private void OnClickRepeat()
	{
		switch (m_curRepeatStatus)
		{
		case REPEAT_STATUS.NONE:
			m_curRepeatStatus = REPEAT_STATUS.TOTAL_REPEAT;
			break;
		case REPEAT_STATUS.TOTAL_REPEAT:
			m_curRepeatStatus = REPEAT_STATUS.SINGLE_REPEAT;
			break;
		case REPEAT_STATUS.SINGLE_REPEAT:
			m_curRepeatStatus = REPEAT_STATUS.NONE;
			break;
		}
		UpdateRepeatUI();
	}

	private void OnClickPrevMusic()
	{
		int playMusicID = GetPlayMusicID(m_CurBGMInfoTemplet.Key, bNext: false);
		if (playMusicID > 0)
		{
			OnSelectedMusic(playMusicID);
			if (m_ctglPlayOrPause.m_bSelect)
			{
				PlayMusic();
			}
		}
		else
		{
			m_ctglPlayOrPause.Select(bSelect: false, bForce: true);
			PauseMusic();
		}
	}

	private void OnClickNextMusic()
	{
		int playMusicID = GetPlayMusicID(m_CurBGMInfoTemplet.Key);
		if (playMusicID > 0)
		{
			OnSelectedMusic(playMusicID);
			if (m_ctglPlayOrPause.m_bSelect)
			{
				PlayMusic();
			}
		}
		else
		{
			m_ctglPlayOrPause.Select(bSelect: false, bForce: true);
			PauseMusic();
		}
	}

	private int GetPlayMusicID(int iStartKey, bool bNext = true)
	{
		bool flag = false;
		if (bNext)
		{
			for (int i = 0; i < m_lstBGMData.Count; i++)
			{
				if (m_lstBGMData[i].Key != iStartKey)
				{
					continue;
				}
				flag = true;
				if (m_lstBGMData.Count <= i + 1)
				{
					if (m_curRepeatStatus == REPEAT_STATUS.TOTAL_REPEAT)
					{
						if (CheckOpenCond(m_lstBGMData[0]))
						{
							return m_lstBGMData[0].Key;
						}
						return GetPlayMusicID(m_lstBGMData[0].Key, bNext);
					}
					return -1;
				}
				if (CheckOpenCond(m_lstBGMData[i + 1]))
				{
					return m_lstBGMData[i + 1].Key;
				}
				return GetPlayMusicID(m_lstBGMData[i + 1].Key, bNext);
			}
		}
		else
		{
			for (int j = 0; j < m_lstBGMData.Count; j++)
			{
				if (m_lstBGMData[j].Key != iStartKey)
				{
					continue;
				}
				flag = true;
				if (j == 0)
				{
					if (CheckOpenCond(m_lstBGMData[m_lstBGMData.Count - 1]))
					{
						return m_lstBGMData[m_lstBGMData.Count - 1].Key;
					}
					return GetPlayMusicID(m_lstBGMData[m_lstBGMData.Count - 1].Key, bNext);
				}
				if (CheckOpenCond(m_lstBGMData[j - 1]))
				{
					return m_lstBGMData[j - 1].Key;
				}
				return GetPlayMusicID(m_lstBGMData[j - 1].Key, bNext);
			}
		}
		if (m_curListType == LIST_TYPE.FAVORITE && !flag && m_lstBGMData.Count > 0)
		{
			return m_lstBGMData[0].Key;
		}
		return -1;
	}

	private static bool CheckOpenCond(NKCBGMInfoTemplet bgmTemplet)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return false;
		}
		if (nKMUserData.IsSuperUser())
		{
			return true;
		}
		switch (bgmTemplet.m_UnlockCond)
		{
		case NKC_BGM_COND.COLLECT_BACKGRUND:
		{
			NKMInventoryData inventoryData2 = nKMUserData.m_InventoryData;
			foreach (int item in bgmTemplet.m_UnlockCondValue1)
			{
				if (bgmTemplet.bAllCollecte && inventoryData2.GetCountMiscItem(item) <= 0)
				{
					return false;
				}
				if (!bgmTemplet.bAllCollecte && inventoryData2.GetCountMiscItem(item) > 0)
				{
					return true;
				}
			}
			break;
		}
		case NKC_BGM_COND.COLLECT_UNIT:
		{
			NKMArmyData armyData = nKMUserData.m_ArmyData;
			foreach (int item2 in bgmTemplet.m_UnlockCondValue1)
			{
				if (bgmTemplet.bAllCollecte && armyData.IsFirstGetUnit(item2))
				{
					return false;
				}
				if (!bgmTemplet.bAllCollecte && !armyData.IsFirstGetUnit(item2))
				{
					return true;
				}
			}
			break;
		}
		case NKC_BGM_COND.STAGE_CLEAR_TOTAL_CNT:
			foreach (int item3 in bgmTemplet.m_UnlockCondValue1)
			{
				if (nKMUserData.CheckStageCleared(item3))
				{
					if (bgmTemplet.bAllCollecte && nKMUserData.GetStatePlayCnt(item3, IsServiceTime: false, bSkipNextResetData: true, bTotalCnt: true) < bgmTemplet.m_UnlockCondValue2)
					{
						return false;
					}
					if (!bgmTemplet.bAllCollecte && nKMUserData.GetStatePlayCnt(item3, IsServiceTime: false, bSkipNextResetData: true, bTotalCnt: true) >= bgmTemplet.m_UnlockCondValue2)
					{
						return true;
					}
				}
			}
			break;
		case NKC_BGM_COND.COLLECT_SKIN:
			foreach (int item4 in bgmTemplet.m_UnlockCondValue1)
			{
				if (bgmTemplet.bAllCollecte && !nKMUserData.m_InventoryData.HasItemSkin(item4))
				{
					return false;
				}
				if (!bgmTemplet.bAllCollecte && nKMUserData.m_InventoryData.HasItemSkin(item4))
				{
					return true;
				}
			}
			break;
		case NKC_BGM_COND.COLLECT_ITEM_MISC:
		{
			NKMInventoryData inventoryData = nKMUserData.m_InventoryData;
			foreach (int item5 in bgmTemplet.m_UnlockCondValue1)
			{
				if (bgmTemplet.bAllCollecte && inventoryData.GetCountMiscItem(item5) < 0)
				{
					return false;
				}
				if (!bgmTemplet.bAllCollecte && inventoryData.GetCountMiscItem(item5) > 0)
				{
					return true;
				}
			}
			break;
		}
		case NKC_BGM_COND.COLLECT_ITEM_INTERIOR:
		{
			NKCOfficeData officeData = nKMUserData.OfficeData;
			foreach (int item6 in bgmTemplet.m_UnlockCondValue1)
			{
				if (bgmTemplet.bAllCollecte && officeData.GetInteriorCount(item6) < 0)
				{
					return false;
				}
				if (!bgmTemplet.bAllCollecte && officeData.GetInteriorCount(item6) > 0)
				{
					return true;
				}
			}
			break;
		}
		default:
			Debug.Log($"<color=red>unknow unlock cond type : {bgmTemplet.m_UnlockCond}, key : {bgmTemplet.Key}</color>");
			return false;
		}
		if (bgmTemplet.bAllCollecte)
		{
			return true;
		}
		return false;
	}

	private void OnClickRandomToggle(bool bSet)
	{
		UpdateMusicSlotList();
		OnSelectedMusic(m_CurBGMInfoTemplet.Key);
	}

	private void OnClickSortToggle(bool bSet)
	{
		if (bSet)
		{
			m_eCurSortType = eSrotType.ST_ORDER_IDX;
			m_lstBGMData = lstBGMInfoTemplet;
			RefreshSlots();
		}
	}

	private void OnClickSortToggleName(bool bSet)
	{
		if (bSet)
		{
			m_eCurSortType = eSrotType.ST_BGM_NAME;
			m_lstBGMData = lstBGMInfoTemplet;
			RefreshSlots();
		}
	}

	private void UpdateMusicTime(float fValue)
	{
		NKCSoundManager.SetChangeMusicTime(fValue);
		m_fCurPlayTime = m_fCurTotalTime * fValue;
		UpdatePlayTimeUI();
	}

	private void OnClickTotalListToggle(bool bSet)
	{
		if (bSet)
		{
			m_curListType = LIST_TYPE.TOTAL;
			UpdateMusicSlotList();
		}
	}

	private void OnClickFavoriteListToggle(bool bSet)
	{
		if (bSet)
		{
			m_curListType = LIST_TYPE.FAVORITE;
			UpdateMusicSlotList();
		}
	}

	private void UpdateMusicSlotList()
	{
		if (m_curListType == LIST_TYPE.TOTAL)
		{
			m_lstBGMData = lstBGMInfoTemplet;
			if (m_ctglRandom.m_bSelect)
			{
				List<NKCBGMInfoTemplet> list = new List<NKCBGMInfoTemplet>();
				list.Add(m_CurBGMInfoTemplet);
				m_lstBGMData.Remove(m_CurBGMInfoTemplet);
				list.AddRange(m_lstBGMData.OrderBy((NKCBGMInfoTemplet g) => Guid.NewGuid()).ToList());
				m_lstBGMData = list;
			}
		}
		if (m_curListType == LIST_TYPE.FAVORITE)
		{
			m_lstBGMData = lstBGMInfoTemplet;
			if (m_ctglRandom.m_bSelect)
			{
				m_lstBGMData = m_lstBGMData.OrderBy((NKCBGMInfoTemplet g) => Guid.NewGuid()).ToList();
			}
		}
		RefreshSlots();
	}

	private void OnClickFavorite(bool bSet)
	{
		List<int> list = LoadFavoriteList();
		if (bSet)
		{
			if (!list.Contains(m_CurBGMInfoTemplet.Key))
			{
				list.Add(m_CurBGMInfoTemplet.Key);
			}
		}
		else if (list.Contains(m_CurBGMInfoTemplet.Key))
		{
			list.Remove(m_CurBGMInfoTemplet.Key);
		}
		SaveFavoriteList(list);
		foreach (NKCJukeBoxSlot lstJukeBoxSlot in m_lstJukeBoxSlots)
		{
			if (!(null == lstJukeBoxSlot))
			{
				lstJukeBoxSlot.SetFavorite(list.Contains(lstJukeBoxSlot.Index));
			}
		}
	}

	private void OnClickPlayToggle(bool bSet)
	{
		if (bSet)
		{
			PlayMusic();
			float value = m_sdProgressBar.value;
			UpdateMusicTime(value);
		}
		else
		{
			PauseMusic();
		}
	}

	private void PauseMusic()
	{
		NKCSoundManager.StopMusic();
		m_curMusicStatus = MUSIC_STATUS.PAUSE;
		if (NKCScenManager.GetScenManager().GetNKCPowerSaveMode().IsJukeBoxMode)
		{
			NKCUIManager.GetNKCUIPowerSaveMode().SetFinishJukeBox(bFinish: true);
		}
	}

	private void UpdateAlbumUI()
	{
		if (m_CurBGMInfoTemplet != null)
		{
			Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_bgm_albumcover", m_CurBGMInfoTemplet.m_BgmCoverIMGID);
			NKCUtil.SetImageSprite(m_imgCover, orLoadAssetResource);
			List<int> list = LoadFavoriteList();
			m_ctglFavorite.Select(list.Contains(m_CurBGMInfoTemplet.Key), bForce: true);
			NKCUtil.SetLabelText(m_lbBGMTitle, NKCStringTable.GetString(m_CurBGMInfoTemplet.m_BgmNameStringID));
			AudioClip audioClip = GetAudioClip(m_CurBGMInfoTemplet.m_BgmAssetID);
			if (!(audioClip == null))
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(audioClip.length);
				NKCUtil.SetLabelText(m_lbPlayTime, $"{0:00}:{0:00}");
				NKCUtil.SetLabelText(m_lbTotalTime, $"-{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
				m_sdProgressBar.value = 0f;
				UpdateJukeBoxTitle();
			}
		}
	}

	private void UpdateSlotUI()
	{
		if (m_CurBGMInfoTemplet == null)
		{
			return;
		}
		int num = m_lstBGMData.FindIndex((NKCBGMInfoTemplet e) => e.Key == m_CurBGMInfoTemplet.Key);
		if (num >= 0)
		{
			if (num == 0 || num == m_lstBGMData.Count - 1)
			{
				m_LoopScroll.SetIndexPosition(num);
			}
			else
			{
				m_LoopScroll?.ScrollToCell(num, 0.4f, LoopScrollRect.ScrollTarget.Center);
			}
		}
		foreach (NKCJukeBoxSlot lstJukeBoxSlot in m_lstJukeBoxSlots)
		{
			lstJukeBoxSlot.SetStat(lstJukeBoxSlot.Index == m_CurBGMInfoTemplet.Key);
		}
	}

	private void UpdateRepeatUI()
	{
		NKCUtil.SetGameobjectActive(m_objRepeatSingle, m_curRepeatStatus == REPEAT_STATUS.SINGLE_REPEAT);
		NKCUtil.SetGameobjectActive(m_objRepeatTotal, m_curRepeatStatus == REPEAT_STATUS.TOTAL_REPEAT);
		NKCUtil.SetGameobjectActive(m_objRepeatNormal, m_curRepeatStatus == REPEAT_STATUS.NONE);
	}

	private void UpdatePlayTimeUI()
	{
		if (m_curMusicStatus == MUSIC_STATUS.PLAY)
		{
			m_fCurPlayTime = NKCSoundManager.GetMusicTime();
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(m_fCurPlayTime);
		TimeSpan timeSpan2 = TimeSpan.FromSeconds(m_fCurTotalTime - m_fCurPlayTime);
		NKCUtil.SetLabelText(m_lbPlayTime, $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
		NKCUtil.SetLabelText(m_lbTotalTime, $"-{timeSpan2.Minutes:00}:{timeSpan2.Seconds:00}");
	}

	private void Update()
	{
		if (m_curMusicStatus == MUSIC_STATUS.PLAY)
		{
			m_fCurPlayTime = NKCSoundManager.GetMusicTime();
			m_sdProgressBar.value = m_fCurPlayTime / m_fCurTotalTime;
			UpdatePlayTimeUI();
			if (m_fCurTotalTime - m_fTimeOffSet <= m_fCurPlayTime)
			{
				switch (m_curRepeatStatus)
				{
				case REPEAT_STATUS.SINGLE_REPEAT:
					PlayMusic();
					break;
				case REPEAT_STATUS.NONE:
				case REPEAT_STATUS.TOTAL_REPEAT:
					OnClickNextMusic();
					break;
				}
			}
		}
		if (m_fSaveDelayTime > 0f)
		{
			m_fSaveDelayTime -= Time.deltaTime;
		}
	}

	private void PlayMusic()
	{
		if (m_CurBGMInfoTemplet != null)
		{
			NKCSoundManager.StopMusic();
			NKCSoundManager.PlayMusic(m_CurBGMInfoTemplet.m_BgmAssetID, bLoop: false, m_CurBGMInfoTemplet.BGMVolume, bForce: true);
			AudioClip audioClip = GetAudioClip(m_CurBGMInfoTemplet.m_BgmAssetID);
			if (null != audioClip)
			{
				m_fCurTotalTime = audioClip.length;
			}
			m_fCurPlayTime = 0f;
			m_curMusicStatus = MUSIC_STATUS.PLAY;
			m_ctglPlayOrPause.Select(bSelect: true, bForce: true);
		}
	}

	public static AudioClip GetAudioClip(string audioClipName)
	{
		string bundleName = NKCAssetResourceManager.GetBundleName(audioClipName);
		if (string.IsNullOrEmpty(bundleName))
		{
			return null;
		}
		if (!NKCAssetResourceManager.IsBundleExists(bundleName, audioClipName))
		{
			return null;
		}
		return NKCAssetResourceManager.OpenResource<AudioClip>(bundleName, audioClipName)?.GetAsset<AudioClip>();
	}
}
