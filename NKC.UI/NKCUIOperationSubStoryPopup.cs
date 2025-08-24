using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationSubStoryPopup : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_OPERATION";

	private const string UI_ASSET_NAME = "AB_UI_OPERATION_UI_SUB_02";

	private static NKCUIOperationSubStoryPopup m_Instance;

	public Image m_imgBG;

	public TMP_Text m_lbTitle;

	public TMP_Text m_lbSubTitle;

	public TMP_Text m_lbDesc;

	public GameObject m_objBGM;

	public TMP_Text m_lbBGM;

	public NKCUIComStateButton m_btnStart;

	[Header("우상단 드롭다운")]
	public NKCUIOperationRelateEpList m_DropDown;

	private NKMEpisodeTempletV2 m_EpisodeTemplet;

	public static NKCUIOperationSubStoryPopup Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIOperationSubStoryPopup>("AB_UI_OPERATION", "AB_UI_OPERATION_UI_SUB_02", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIOperationSubStoryPopup>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "";

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.LeftsideWithHamburger;

	public override string GuideTempletID => "ARTICLE_OPERATION_SIDE_STORY";

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

	public static bool isOpen()
	{
		if (m_Instance != null)
		{
			return m_Instance.IsOpen;
		}
		return false;
	}

	private void InitUI()
	{
		m_btnStart.PointerClick.RemoveAllListeners();
		m_btnStart.PointerClick.AddListener(OnClickStart);
		if (m_DropDown != null)
		{
			m_DropDown.InitUI();
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(NKMEpisodeTempletV2 epTemplet)
	{
		if (epTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_EpisodeTemplet = epTemplet;
		NKCUtil.SetLabelText(m_lbTitle, m_EpisodeTemplet.GetEpisodeTitle());
		NKCUtil.SetLabelText(m_lbSubTitle, m_EpisodeTemplet.GetEpisodeName());
		NKCUtil.SetLabelText(m_lbDesc, m_EpisodeTemplet.GetEpisodeDesc());
		NKCUtil.SetImageSprite(m_imgBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Bg", m_EpisodeTemplet.m_EPThumbnail));
		NKCBGMInfoTemplet nKCBGMInfoTemplet = NKCBGMInfoTemplet.Find(m_EpisodeTemplet.m_BG_Music);
		if (nKCBGMInfoTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_objBGM, bValue: true);
			NKCUtil.SetLabelText(m_lbBGM, NKCStringTable.GetString(nKCBGMInfoTemplet.m_BgmNameStringID));
			NKCSoundManager.PlayMusic(nKCBGMInfoTemplet.m_BgmAssetID, bLoop: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objBGM, bValue: false);
		}
		if (m_DropDown != null)
		{
			m_DropDown.SetData(epTemplet);
		}
		UIOpened();
		TutorialCheck();
	}

	private void OnClickStart()
	{
		Close();
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetLastPlayedSubStream(m_EpisodeTemplet.m_EpisodeID);
		NKCUIOperationNodeViewer.Instance.Open(m_EpisodeTemplet);
	}

	private void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Operation_SubStream_Popup);
	}
}
