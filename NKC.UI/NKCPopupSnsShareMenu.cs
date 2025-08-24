using NKC.Publisher;
using UnityEngine.EventSystems;

namespace NKC.UI;

public class NKCPopupSnsShareMenu : NKCUIBase
{
	public delegate void OnClickSnsIcon(NKCPublisherModule.SNS_SHARE_TYPE eSNS_SHARE_TYPE);

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_SHARE";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_BOX_SHARE";

	private static NKCPopupSnsShareMenu m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUIComStateButton m_BtnWechat;

	public NKCUIComStateButton m_BtnWechatMoments;

	public NKCUIComStateButton m_BtnQQ;

	public NKCUIComStateButton m_BtnWeibo;

	public EventTrigger m_etBG;

	public NKCUIComStateButton m_btnClose;

	private OnClickSnsIcon m_dOnClickSnsIcon;

	public static NKCPopupSnsShareMenu Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupSnsShareMenu>("AB_UI_NKM_UI_POPUP_SHARE", "NKM_UI_POPUP_BOX_SHARE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupSnsShareMenu>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupSnsShareMenu";

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

	private void OnClickShare(NKCPublisherModule.SNS_SHARE_TYPE eSNS_SHARE_TYPE)
	{
		Close();
		if (m_dOnClickSnsIcon != null)
		{
			m_dOnClickSnsIcon(eSNS_SHARE_TYPE);
		}
	}

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		m_BtnWechat.PointerClick.RemoveAllListeners();
		m_BtnWechat.PointerClick.AddListener(delegate
		{
			OnClickShare(NKCPublisherModule.SNS_SHARE_TYPE.SST_WECHAT);
		});
		m_BtnWechatMoments.PointerClick.RemoveAllListeners();
		m_BtnWechatMoments.PointerClick.AddListener(delegate
		{
			OnClickShare(NKCPublisherModule.SNS_SHARE_TYPE.SST_WECHAT_MOMENTS);
		});
		m_BtnQQ.PointerClick.RemoveAllListeners();
		m_BtnQQ.PointerClick.AddListener(delegate
		{
			OnClickShare(NKCPublisherModule.SNS_SHARE_TYPE.SST_QQ);
		});
		m_BtnWeibo.PointerClick.RemoveAllListeners();
		m_BtnWeibo.PointerClick.AddListener(delegate
		{
			OnClickShare(NKCPublisherModule.SNS_SHARE_TYPE.SST_WEIBO);
		});
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(delegate
		{
			Close();
		});
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_etBG.triggers.Add(entry);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(OnClickSnsIcon dOnClickSnsIcon)
	{
		m_dOnClickSnsIcon = dOnClickSnsIcon;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
