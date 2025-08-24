using System.Collections.Generic;
using System.IO;
using ClientPacket.Pvp;
using Cs.GameServer.Replay;
using Cs.Logging;
using NKC.Util;
using NKM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupReplayViewer : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_REPLAY_VIEWER";

	private static NKCPopupReplayViewer m_Instance;

	public LoopScrollRect scrollRect;

	public NKCUIComStateButton closeButton;

	public NKCUIComStateButton BtnDefalutPath;

	public TMP_InputField pathInputField;

	public NKCUIComToggle tglSingle;

	private const string PATH_SAVE_KEY = "SaveKey_ReplayFolder";

	private string defaultPath;

	private static string currentPath;

	private List<DirectoryInfo> directoryList = new List<DirectoryInfo>();

	private List<FileInfo> fileList = new List<FileInfo>();

	public static NKCPopupReplayViewer Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupReplayViewer>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_REPLAY_VIEWER", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupReplayViewer>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

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

	public override string MenuName => "Replay Viewer";

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

	private void Init()
	{
		NKCUtil.SetButtonClickDelegate(closeButton, base.Close);
		NKCUtil.SetButtonClickDelegate(BtnDefalutPath, MoveToDefaultPath);
		NKCUtil.SetUnityEvent(pathInputField.onEndEdit, OnPathEndEdit);
		scrollRect.dOnGetObject += GetSlot;
		scrollRect.dOnReturnObject += ReturnSlot;
		scrollRect.dOnProvideData += ProvideData;
		scrollRect.ContentConstraintCount = 1;
		scrollRect.PrepareCells();
		NKCUtil.SetScrollHotKey(scrollRect);
		defaultPath = ReplayRecorder.ReplaySavePath;
		if (PlayerPrefs.HasKey("SaveKey_ReplayFolder"))
		{
			currentPath = PlayerPrefs.GetString("SaveKey_ReplayFolder");
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open()
	{
		NKCScenManager.GetScenManager().GetNKCReplayMgr().StopPlaying();
		if (string.IsNullOrEmpty(currentPath))
		{
			RefreshList(defaultPath);
		}
		else
		{
			RefreshList(currentPath);
		}
		UIOpened();
	}

	private void RefreshList(string path)
	{
		directoryList.Clear();
		fileList.Clear();
		if (string.IsNullOrEmpty(path))
		{
			currentPath = path;
			pathInputField.SetTextWithoutNotify("DRIVE");
			DriveInfo[] drives = DriveInfo.GetDrives();
			foreach (DriveInfo driveInfo in drives)
			{
				if (driveInfo.DriveType == DriveType.Fixed)
				{
					directoryList.Add(driveInfo.RootDirectory);
				}
			}
		}
		else
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			while (!directoryInfo.Exists)
			{
				directoryInfo = Directory.GetParent(directoryInfo.FullName);
			}
			currentPath = directoryInfo.FullName;
			pathInputField.SetTextWithoutNotify(currentPath);
			PlayerPrefs.SetString("SaveKey_ReplayFolder", currentPath);
			PlayerPrefs.Save();
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			FileInfo[] files = directoryInfo.GetFiles();
			FileAttributes fileAttributes = FileAttributes.Hidden | FileAttributes.System;
			directoryList.Add(null);
			DirectoryInfo[] array = directories;
			foreach (DirectoryInfo directoryInfo2 in array)
			{
				if ((directoryInfo2.Attributes & fileAttributes) != fileAttributes)
				{
					directoryList.Add(directoryInfo2);
				}
			}
			FileInfo[] array2 = files;
			foreach (FileInfo fileInfo in array2)
			{
				if ((fileInfo.Attributes & fileAttributes) != fileAttributes && !(fileInfo.Extension != ".replay"))
				{
					fileList.Add(fileInfo);
				}
			}
		}
		scrollRect.TotalCount = directoryList.Count + fileList.Count;
		scrollRect.SetIndexPosition(0);
	}

	private RectTransform GetSlot(int index)
	{
		return NKCPopupReplayViewerSlot.GetNewInstance(null)?.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform tr)
	{
		NKCPopupReplayViewerSlot component = tr.GetComponent<NKCPopupReplayViewerSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	private void ProvideData(Transform tr, int index)
	{
		NKCPopupReplayViewerSlot component = tr.GetComponent<NKCPopupReplayViewerSlot>();
		if (!(component == null))
		{
			if (index < directoryList.Count)
			{
				DirectoryInfo dirInfo = directoryList[index];
				component.SetData(dirInfo, null, OnClickSlot);
			}
			else if (index - directoryList.Count < fileList.Count)
			{
				component.SetData(null, fileList[index - directoryList.Count], OnClickSlot);
			}
			else
			{
				component.SetData(null, null, null);
			}
		}
	}

	private void LoadReplay(string fullPath)
	{
		ReplayData replayData = ReplayLoader.Load(fullPath);
		if (replayData != null)
		{
			if (replayData.gameData != null)
			{
				replayData.replayName = NKCReplayMgr.MakeReplayDataFileName(replayData.gameData.m_GameUID);
				replayData.gameData.SetGameType(NKM_GAME_TYPE.NGT_INVALID);
				replayData.gameData.m_bLocal = true;
			}
			NKMOpenTagManager.TryAddTag("PVP_REPLAY");
			NKMUserData myUserData = new NKMUserData();
			NKCScenManager.GetScenManager().SetMyUserData(myUserData);
			NKCScenManager.GetScenManager().GetNKCReplayMgr().StartPlaying(replayData);
		}
		else
		{
			Log.Error("ReplayData not loaded: " + fullPath, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupReplayViewer.cs", 234);
		}
	}

	private void PlayReenactGame(string fullPath)
	{
		if (NKCDefineManager.DEFINE_USE_CHEAT())
		{
			ReplayData replayData = ReplayLoader.Load(fullPath);
			NKMContentsVersionManager.LoadDefaultVersion();
			if (NKCDefineManager.DEFINE_UNITY_EDITOR())
			{
				NKCContentsVersionManager.TryRecoverTag();
			}
			NKCTempletUtility.PostJoin();
			NKCPacketSender.Send_NKMPacket_DEV_GAME_LOAD_REQ(replayData.gameData, replayData.gameRuntimeData);
		}
	}

	private void MoveToDefaultPath()
	{
		RefreshList(defaultPath);
	}

	private void OnPathEndEdit(string path)
	{
		if (File.Exists(path))
		{
			FileInfo fileInfo = new FileInfo(path);
			OnClickSlot(null, fileInfo);
		}
		else if (Directory.Exists(path))
		{
			DirectoryInfo dirInfo = new DirectoryInfo(path);
			OnClickSlot(dirInfo, null);
		}
		else
		{
			RefreshList(currentPath);
		}
	}

	private void OnClickSlot(DirectoryInfo dirInfo, FileInfo fileInfo)
	{
		if (dirInfo != null)
		{
			RefreshList(dirInfo.FullName);
			return;
		}
		if (fileInfo != null)
		{
			if (tglSingle != null && tglSingle.IsSelected)
			{
				PlayReenactGame(fileInfo.FullName);
			}
			else
			{
				LoadReplay(fileInfo.FullName);
			}
			return;
		}
		DirectoryInfo parent = Directory.GetParent(currentPath);
		if (parent != null)
		{
			RefreshList(parent.FullName);
		}
		else
		{
			RefreshList(null);
		}
	}
}
