using NKC.UI.Shop;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Fierce;

public class NKCUIPopupFierceBattleEnd : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_world_map_renewal";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FIERCE_BATTLE_END";

	private static NKCUIPopupFierceBattleEnd m_Instance;

	public Text m_lbEndInfo;

	public NKCUIComStateButton m_SEASON_REWARD_Button;

	public EventTrigger m_POPUP_CONSORTIUM_COOP_END_Bg;

	public static NKCUIPopupFierceBattleEnd Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupFierceBattleEnd>("ab_ui_nkm_ui_world_map_renewal", "NKM_UI_POPUP_FIERCE_BATTLE_END", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupFierceBattleEnd>();
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

	public override string MenuName => "FIERCE_BATTLE_END_POPUP";

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
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		if (m_SEASON_REWARD_Button != null)
		{
			m_SEASON_REWARD_Button.PointerClick.RemoveAllListeners();
			m_SEASON_REWARD_Button.PointerClick.AddListener(delegate
			{
				NKCUIShop.ShopShortcut("TAB_SEASON_FIERCE_POINT");
			});
		}
		if (m_POPUP_CONSORTIUM_COOP_END_Bg != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				CheckInstanceAndClose();
			});
			m_POPUP_CONSORTIUM_COOP_END_Bg.triggers.Add(entry);
		}
	}

	public void Open()
	{
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (nKCFierceBattleSupportDataMgr != null && !nKCFierceBattleSupportDataMgr.IsCanAccessFierce())
		{
			NKCUtil.SetLabelText(m_lbEndInfo, nKCFierceBattleSupportDataMgr.GetAccessDeniedMessage());
			UIOpened();
		}
	}
}
