using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine.EventSystems;

namespace NKC.UI.Fierce;

public class NKCUIPopupFierceBattleNotice : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_fierce_battle";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FIERCE_BATTLE_NOTICE";

	private static NKCUIPopupFierceBattleNotice m_Instance;

	public EventTrigger m_POPUP_FIERCE_BATTLE_NOTICE_Bg;

	public NKCUIComStateButton m_NKM_UI_POPUP_CLOSE_BUTTON;

	public NKCUIComStateButton m_POPUP_FIERCE_BATTLE_NOTICE_BUTTON;

	public List<NKCUIFierceBattleNoticeSlot> m_lstFierceBattleNoticeSlot;

	public static NKCUIPopupFierceBattleNotice Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupFierceBattleNotice>("ab_ui_nkm_ui_fierce_battle", "NKM_UI_POPUP_FIERCE_BATTLE_NOTICE", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupFierceBattleNotice>();
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

	public override string MenuName => "FIERCE_BATTLE_NOTICE";

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

	public void Init()
	{
		if (m_POPUP_FIERCE_BATTLE_NOTICE_Bg != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				CheckInstanceAndClose();
			});
			m_POPUP_FIERCE_BATTLE_NOTICE_Bg.triggers.Add(entry);
		}
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_CLOSE_BUTTON, CheckInstanceAndClose);
		NKCUtil.SetBindFunction(m_POPUP_FIERCE_BATTLE_NOTICE_BUTTON, MoveToFierce);
	}

	public void Open()
	{
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (nKCFierceBattleSupportDataMgr == null || nKCFierceBattleSupportDataMgr.FierceTemplet == null)
		{
			return;
		}
		List<int> fierceBossGroupIdList = nKCFierceBattleSupportDataMgr.FierceTemplet.FierceBossGroupIdList;
		for (int i = 0; i < m_lstFierceBattleNoticeSlot.Count; i++)
		{
			int num = 0;
			if (fierceBossGroupIdList.Count > i)
			{
				num = fierceBossGroupIdList[i];
				string bossFaceCardName = "";
				if (NKMFierceBossGroupTemplet.Groups.ContainsKey(num))
				{
					foreach (NKMFierceBossGroupTemplet item in NKMFierceBossGroupTemplet.Groups[num])
					{
						if (item.Level == 1)
						{
							bossFaceCardName = item.UI_BossFaceCard;
							break;
						}
					}
				}
				string targetBossName = nKCFierceBattleSupportDataMgr.GetTargetBossName(num);
				m_lstFierceBattleNoticeSlot[i].SetData(bossFaceCardName, targetBossName);
				NKCUtil.SetGameobjectActive(m_lstFierceBattleNoticeSlot[i], bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstFierceBattleNoticeSlot[i], bValue: false);
			}
		}
		UIOpened();
	}

	private void MoveToFierce()
	{
		if (NKCContentManager.IsContentsUnlocked(ContentsType.WORLDMAP) && NKCContentManager.IsContentsUnlocked(ContentsType.FIERCE))
		{
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_FIERCE, "");
		}
	}
}
