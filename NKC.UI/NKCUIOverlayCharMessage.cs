using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOverlayCharMessage : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_character_message";

	public const string UI_ASSET_NAME = "NKM_UI_CHARACTER_MESSAGE_BOX";

	private static NKCUIOverlayCharMessage m_Instance;

	public GameObject m_goRootCharacter;

	public Image m_imgCharacter;

	public Text m_lbMessage;

	public Image m_imgBackGround;

	private bool m_bModal;

	private bool m_bWaitForClose;

	private UnityAction dOnComplete;

	private const float MODAL_DELAY_TIME = 0.4f;

	private float m_fTargetOpenTime;

	private float m_fOpenedTime;

	public static NKCUIOverlayCharMessage Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIOverlayCharMessage>("ab_ui_nkm_ui_character_message", "NKM_UI_CHARACTER_MESSAGE_BOX", NKCUIManager.eUIBaseRect.UIOverlay, CleanupInstance).GetInstance<NKCUIOverlayCharMessage>();
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

	public override string MenuName => "캐릭터 메시지";

	public override bool IgnoreBackButtonWhenOpen => m_bModal;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open(NKMUnitTempletBase unitTempletBase, string message, float fOpenTime, UnityAction onComplete, bool bForceAnimation = false)
	{
		Sprite spChar = null;
		if (unitTempletBase != null)
		{
			spChar = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase);
		}
		Open(spChar, message, fOpenTime, onComplete, bForceAnimation);
	}

	public void Open(string invenIconAssetName, string message, float fOpenTime, UnityAction onComplete, bool bForceAnimation = false)
	{
		Sprite spChar = null;
		if (!string.IsNullOrEmpty(invenIconAssetName))
		{
			spChar = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_UNIT", invenIconAssetName);
		}
		Open(spChar, message, fOpenTime, onComplete, bForceAnimation);
	}

	public void Open(Sprite spChar, string message, float fOpenTime, UnityAction onComplete, bool bForceAnimation = false)
	{
		SetData(spChar, message);
		dOnComplete = onComplete;
		m_bModal = fOpenTime == 0f;
		m_fOpenedTime = 0f;
		m_fTargetOpenTime = (m_bModal ? 0.4f : fOpenTime);
		NKCUtil.SetGameobjectActive(m_imgBackGround, m_bModal);
		if (!base.IsOpen)
		{
			UIOpened();
		}
	}

	public void SetData(NKMUnitTempletBase unitTempletBase, string message)
	{
		Sprite spChar = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase);
		SetData(spChar, message);
	}

	public void SetData(Sprite spChar, string message)
	{
		if (spChar == null)
		{
			NKCUtil.SetGameobjectActive(m_goRootCharacter, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_goRootCharacter, bValue: true);
			NKCUtil.SetImageSprite(m_imgCharacter, spChar);
		}
		NKCUtil.SetLabelText(m_lbMessage, message);
	}

	private void Update()
	{
		if (m_fOpenedTime >= 0f)
		{
			m_fOpenedTime += Time.deltaTime;
		}
		if (!(m_fOpenedTime >= m_fTargetOpenTime))
		{
			return;
		}
		if (m_bModal)
		{
			if (Input.anyKeyDown)
			{
				m_fOpenedTime = -1f;
				dOnComplete?.Invoke();
			}
		}
		else
		{
			m_fOpenedTime = -1f;
			dOnComplete?.Invoke();
		}
	}

	public void SetBGScreenAlpha(float alpha)
	{
		m_imgBackGround.color = new Color(m_imgBackGround.color.r, m_imgBackGround.color.g, m_imgBackGround.color.b, alpha);
	}
}
