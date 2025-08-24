using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupResourceDivision : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAME_OPTION";

	private const string UI_ASSET_NAME = "NKM_UI_GAME_OPTION_RESOURCE_DIVISION";

	private static NKCPopupResourceDivision m_Instance;

	public EventTrigger m_eventTriggerBg;

	public NKCUIComStateButton m_csbtnConfirm;

	public Image m_imgPakagaMedalPaid;

	public Text m_lbPakageMedalPaidCount;

	public Image m_imgPakagaMedalFree;

	public Text m_lbPakageMedalFreeCount;

	public Image m_imgQuartzPaid;

	public Text m_lbQuartzPaidCount;

	public Image m_imgQuartzFree;

	public Text m_lbQuartzFreeCount;

	public Image m_imgContractClassifiedPaid;

	public Text m_lbContractClassifiedPaidCount;

	public Image m_imgContractClassifiedFree;

	public Text m_lbContractClassifiedFreeCount;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public static NKCPopupResourceDivision Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupResourceDivision>("AB_UI_NKM_UI_GAME_OPTION", "NKM_UI_GAME_OPTION_RESOURCE_DIVISION", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupResourceDivision>();
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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "Resource Division";

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

	private void Init()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		if (m_eventTriggerBg != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				Close();
			});
			m_eventTriggerBg.triggers.Add(entry);
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnConfirm, base.Close);
		Sprite orLoadMiscItemIcon = NKCResourceUtility.GetOrLoadMiscItemIcon(102);
		NKCUtil.SetImageSprite(m_imgPakagaMedalFree, orLoadMiscItemIcon);
		NKCUtil.SetImageSprite(m_imgPakagaMedalPaid, orLoadMiscItemIcon);
		orLoadMiscItemIcon = NKCResourceUtility.GetOrLoadMiscItemIcon(101);
		NKCUtil.SetImageSprite(m_imgQuartzFree, orLoadMiscItemIcon);
		NKCUtil.SetImageSprite(m_imgQuartzPaid, orLoadMiscItemIcon);
		orLoadMiscItemIcon = NKCResourceUtility.GetOrLoadMiscItemIcon(1034);
		NKCUtil.SetImageSprite(m_imgContractClassifiedFree, orLoadMiscItemIcon);
		NKCUtil.SetImageSprite(m_imgContractClassifiedPaid, orLoadMiscItemIcon);
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open()
	{
		long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(102, isPaid: true);
		NKCUtil.SetLabelText(m_lbPakageMedalPaidCount, countMiscItem.ToString());
		long countMiscItem2 = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(102, isPaid: false);
		NKCUtil.SetLabelText(m_lbPakageMedalFreeCount, countMiscItem2.ToString());
		countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(101, isPaid: true);
		NKCUtil.SetLabelText(m_lbQuartzPaidCount, countMiscItem.ToString());
		countMiscItem2 = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(101, isPaid: false);
		NKCUtil.SetLabelText(m_lbQuartzFreeCount, countMiscItem2.ToString());
		countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(1034, isPaid: true);
		NKCUtil.SetLabelText(m_lbContractClassifiedPaidCount, countMiscItem.ToString());
		countMiscItem2 = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(1034, isPaid: false);
		NKCUtil.SetLabelText(m_lbContractClassifiedFreeCount, countMiscItem2.ToString());
		base.gameObject.SetActive(value: true);
		m_NKCUIOpenAnimator?.PlayOpenAni();
		UIOpened();
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator?.Update();
		}
	}
}
