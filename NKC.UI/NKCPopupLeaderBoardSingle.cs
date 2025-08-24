using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupLeaderBoardSingle : NKCUILeaderBoard
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_leader_board";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_LEADER_BOARD";

	private static NKCPopupLeaderBoardSingle m_Instance;

	[Header("팝업 전용 좌측메뉴")]
	public Image m_imgSingleBanner;

	public Text m_lbSingleTitle;

	public Text m_lbSingleSubTitle;

	public Text m_lbSingleDesc;

	public NKCUIComStateButton m_btnClose;

	public new static NKCPopupLeaderBoardSingle Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupLeaderBoardSingle>("ab_ui_nkm_ui_leader_board", "NKM_UI_POPUP_LEADER_BOARD", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupLeaderBoardSingle>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public new static bool IsInstanceOpen
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

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Invalid;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		Init();
	}

	public void OpenSingle(NKMLeaderBoardTemplet reservedTemplet)
	{
		if (reservedTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		bool flag = !string.IsNullOrEmpty(reservedTemplet.m_BoardPopupImg);
		NKCUtil.SetGameobjectActive(m_imgSingleBanner, flag);
		if (flag)
		{
			NKCUtil.SetImageSprite(m_imgSingleBanner, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_LEADER_BOARD_TEXTURE", reservedTemplet.m_BoardPopupImg));
		}
		bool flag2 = !string.IsNullOrEmpty(reservedTemplet.GetPopupTitle());
		NKCUtil.SetGameobjectActive(m_lbSingleTitle, flag2);
		if (flag2)
		{
			NKCUtil.SetLabelText(m_lbSingleTitle, reservedTemplet.GetPopupTitle());
		}
		bool flag3 = !string.IsNullOrEmpty(reservedTemplet.GetPopupName());
		NKCUtil.SetGameobjectActive(m_lbSingleSubTitle, flag3);
		if (flag3)
		{
			NKCUtil.SetLabelText(m_lbSingleSubTitle, reservedTemplet.GetPopupName());
		}
		bool flag4 = !string.IsNullOrEmpty(reservedTemplet.GetPopupDesc());
		NKCUtil.SetGameobjectActive(m_lbSingleDesc, flag4);
		if (flag4)
		{
			NKCUtil.SetLabelText(m_lbSingleDesc, reservedTemplet.GetPopupDesc());
		}
		Open(reservedTemplet);
	}
}
