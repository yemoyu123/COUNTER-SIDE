using NKC.Patcher;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOverlayPatchProgress : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_hud";

	public const string UI_ASSET_NAME = "AB_UI_GAME_HUD_PATCH_DOWNLOAD";

	private static NKCUIOverlayPatchProgress m_Instance;

	public Text m_lbCurrentPercent;

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public static NKCUIOverlayPatchProgress Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIOverlayPatchProgress>("ab_ui_nkm_ui_hud", "AB_UI_GAME_HUD_PATCH_DOWNLOAD", NKCUIManager.eUIBaseRect.UIOverlay, CleanupInstance).GetInstance<NKCUIOverlayPatchProgress>();
				m_Instance?.Init();
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

	public override eMenutype eUIType => eMenutype.Overlay;

	public override string MenuName => "Patch Progress Overlay";

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

	public override void CloseInternal()
	{
		NKCUIBase.SetGameObjectActive(base.gameObject, bValue: false);
	}

	public void Init()
	{
		NKCUIBase.SetGameObjectActive(base.gameObject, bValue: false);
	}

	public static void OpenWhenDownloading()
	{
		if (!(NKCPatchDownloader.Instance == null))
		{
			if (!NKCPatchDownloader.Instance.IsBackGroundDownload())
			{
				m_Instance?.Close();
			}
			else if ((int)(NKCPatchDownloader.Instance.DownloadPercent * 100f) >= 100)
			{
				m_Instance?.Close();
			}
			else if (!(Instance == null) && !m_Instance.IsOpen)
			{
				m_Instance.Open();
			}
		}
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UIOpened();
	}

	private void Update()
	{
		if (!(NKCPatchDownloader.Instance == null) && !(m_Instance == null) && m_Instance.IsOpen)
		{
			float downloadPercent = NKCPatchDownloader.Instance.DownloadPercent;
			m_lbCurrentPercent.text = $"{(int)(downloadPercent * 100f)}%";
			if ((int)(downloadPercent * 100f) >= 100)
			{
				Close();
			}
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}
}
