using NKC.Patcher;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPatcher : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "ab_ui_patch";

	public const string UI_ASSET_NAME = "NKM_UI_PATCH";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	[Header("왼쪽 위 텍스트")]
	public Text m_lbAppVersion;

	[Header("아래쪽")]
	public GameObject m_objBackgroundDownloadNotice;

	public Text m_lbDownloadProgress;

	public Slider m_slProgress;

	public static bool IsInstanceOpen
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsUIOpen;
			}
			return false;
		}
	}

	public static bool IsInstanceLoaded
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsLoadComplete;
			}
			return false;
		}
	}

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "패치";

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUIPatcher>("ab_ui_patch", "NKM_UI_PATCH", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUIPatcher GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUIPatcher>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open()
	{
		base.gameObject.SetActive(value: true);
		m_lbAppVersion.text = $"CounterSide {NKCUtilString.GetAppVersionText()}({960}/{NKMDataVersion.DataVersion})";
		NKCUtil.SetGameobjectActive(m_objBackgroundDownloadNotice, NKCPatchDownloader.Instance.BackgroundDownloadAvailble);
		UIOpened();
	}

	public void Update()
	{
		SetDownloadPercent(NKCPatchDownloader.Instance.DownloadPercent);
	}

	private void SetDownloadPercent(float percent)
	{
		m_lbDownloadProgress.text = $"{(int)(percent * 100f)}%";
		m_slProgress.value = percent;
	}
}
