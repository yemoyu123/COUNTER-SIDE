using System.Collections;
using System.IO;
using NKC.Publisher;
using NKC.UI.Component;
using NKC.Util;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupSnsShare : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_result";

	private const string UI_ASSET_NAME = "NKM_UI_RESULT_GET_SHARE";

	private static NKCPopupSnsShare m_Instance;

	public NKCUICharacterView m_charview;

	public NKCUICharInfoSummary m_charSummary;

	public Text m_lbLevel;

	public Text m_lbUserName;

	public Text m_lbUserUID;

	public GameObject m_objBGN;

	public GameObject m_objBGR;

	public GameObject m_objBGSR;

	public GameObject m_objBGSSR;

	public GameObject m_qrImage;

	private NKCPublisherModule.SNS_SHARE_TYPE m_SNS_SHARE_TYPE = NKCPublisherModule.SNS_SHARE_TYPE.SST_NONE;

	private const string CAPTURE_FILE_NAME = "ScreenCapture.png";

	private const string THUMBNAIL_FILE_NAME = "Thumbnail.png";

	public static NKCPopupSnsShare Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupSnsShare>("ab_ui_nkm_ui_result", "NKM_UI_RESULT_GET_SHARE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupSnsShare>();
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

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override string MenuName => "Share";

	private string CapturePath => Path.Combine(Application.persistentDataPath, "ScreenCapture.png");

	private string ThumbnailPath => Path.Combine(Application.persistentDataPath, "Thumbnail.png");

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
		m_charview?.CleanUp();
		base.gameObject.SetActive(value: false);
	}

	public override void OnBackButton()
	{
	}

	public void Open(NKMUserData userData, NKMUnitData targetUnit, NKCPublisherModule.SNS_SHARE_TYPE eSST)
	{
		m_SNS_SHARE_TYPE = eSST;
		m_charSummary?.SetData(targetUnit);
		if (m_charview != null)
		{
			m_charview.SetCharacterIllust(targetUnit);
			m_charview.SetAnimation(NKCASUIUnitIllust.eAnimation.UNIT_IDLE, loop: true);
		}
		NKCUtil.SetLabelText(m_lbLevel, NKCStringTable.GetString("SI_DP_LEVEL_ONE_PARAM"), userData.m_UserLevel);
		NKCUtil.SetLabelText(m_lbUserName, userData.m_UserNickName);
		NKCUtil.SetLabelText(m_lbUserUID, NKCUtilString.GetFriendCode(userData.m_FriendCode));
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(targetUnit);
		NKCUtil.SetGameobjectActive(m_objBGN, unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_N);
		NKCUtil.SetGameobjectActive(m_objBGR, unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_R);
		NKCUtil.SetGameobjectActive(m_objBGSR, unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SR);
		NKCUtil.SetGameobjectActive(m_objBGSSR, unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR);
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.SNS_SHARE_QR) && NKCPublisherModule.Marketing.SnsQRImageEnabled())
		{
			NKCUtil.SetGameobjectActive(m_qrImage, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_qrImage, bValue: false);
		}
		UIOpened();
		StartCoroutine(Process());
	}

	private IEnumerator Process()
	{
		yield return null;
		NKCScenManager.GetScenManager().GetEffectManager().DeleteAllEffect();
		NKCUIManager.CloseAllOverlay();
		yield return new WaitForEndOfFrame();
		if (!NKCScreenCaptureUtility.CaptureScreenWithThumbnail(CapturePath, ThumbnailPath))
		{
			OnShareFinished(NKC_PUBLISHER_RESULT_CODE.NPRC_MARKETING_SNS_SHARE_FAIL, null);
			yield break;
		}
		yield return null;
		NKCPublisherModule.Marketing.TrySnsShare(m_SNS_SHARE_TYPE, CapturePath, ThumbnailPath, OnShareFinished);
	}

	private void OnShareFinished(NKC_PUBLISHER_RESULT_CODE result, string additionalError)
	{
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.CHN))
		{
			Close();
		}
		else if (NKCPublisherModule.CheckError(result, additionalError, bCloseWaitBox: true, base.Close))
		{
			Close();
		}
	}
}
