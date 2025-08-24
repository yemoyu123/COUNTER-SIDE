using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupChangeLobbyPreview : NKCUIBase
{
	public delegate void OnConfirm(int index);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_user_info";

	private const string UI_ASSET_NAME = "NKM_UI_USER_INFO_LOBBY_CHANGE_PREVIEW";

	private static NKCUIPopupChangeLobbyPreview m_Instance;

	public Text m_lbTitle;

	public Text m_lbDesc;

	public RectTransform m_rtPreviewLimit;

	public RectTransform m_rtPreviewBG;

	public Image m_imgPreview;

	public GameObject m_objPreviewNotExist;

	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnCancel;

	public float m_fPadding = 27.5f;

	private OnConfirm dOnConfirm;

	private int m_Index;

	public static NKCUIPopupChangeLobbyPreview Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupChangeLobbyPreview>("ab_ui_nkm_ui_user_info", "NKM_UI_USER_INFO_LOBBY_CHANGE_PREVIEW", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupChangeLobbyPreview>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

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

	public override string MenuName => "Preset preview";

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

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnOK, OnOK);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_csbtnCancel, base.Close);
	}

	private void OnOK()
	{
		Close();
		dOnConfirm?.Invoke(m_Index);
	}

	public void Open(int presetIndex, string title, string previewPath, string desc, OnConfirm onConfirm)
	{
		m_Index = presetIndex;
		NKCUtil.SetLabelText(m_lbTitle, title);
		NKCUtil.SetLabelText(m_lbDesc, desc);
		dOnConfirm = onConfirm;
		Sprite sprite = NKCResourceUtility.LoadNewSprite(previewPath);
		NKCUtil.SetGameobjectActive(m_rtPreviewBG, sprite != null);
		NKCUtil.SetGameobjectActive(m_imgPreview, sprite != null);
		NKCUtil.SetGameobjectActive(m_objPreviewNotExist, sprite == null);
		if (sprite != null)
		{
			NKCUtil.SetImageSprite(m_imgPreview, sprite);
			float num = m_rtPreviewLimit.GetWidth() - m_fPadding * 2f;
			float num2 = m_rtPreviewLimit.GetHeight() - m_fPadding * 2f;
			float num3 = num / num2;
			float num4 = (float)sprite.texture.width / (float)sprite.texture.height;
			float num5;
			float num6;
			if (num3 > num4)
			{
				num5 = num2;
				num6 = num4 * num5;
			}
			else
			{
				num6 = num;
				num5 = num6 / num4;
			}
			float x = num6 + m_fPadding * 2f;
			float y = num5 + m_fPadding * 2f;
			m_rtPreviewBG.SetSize(new Vector2(x, y));
		}
		UIOpened();
	}
}
