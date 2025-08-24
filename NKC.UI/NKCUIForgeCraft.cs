using System.Collections.Generic;
using ClientPacket.Item;
using NKC.UI.NPC;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForgeCraft : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_factory";

	private const string UI_ASSET_NAME = "NKM_UI_FACTORY_CRAFT";

	private static NKCUIForgeCraft m_Instance;

	private readonly List<int> RESOURCE_LIST = new List<int> { 1, 2, 101 };

	public GameObject m_objUnlockEffect;

	public GameObject m_objBackGround;

	public ScrollRect m_srForgeCraftSlot;

	public List<NKCUIForgeCraftSlot> m_lstNKCUIForgeCraftSlot;

	public Text m_NKM_UI_FACTORY_CRAFT_INSTANT_TEXT;

	private float m_fRefreshTime;

	private int m_ReservedInstanceGetIndex = -1;

	public GameObject NKM_UI_FACTORY_CRAFT_NPC;

	public NKCUIFactoryShortCutMenu m_NKM_UI_FACTORY_SHORTCUT_MENU;

	[Header("npc")]
	private NKCUINPCFactoryAnastasia m_UINPC_Factory;

	public GameObject m_objNPCFactory_TouchArea;

	public static NKCUIForgeCraft Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIForgeCraft>("ab_ui_nkm_ui_factory", "NKM_UI_FACTORY_CRAFT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIForgeCraft>();
				m_Instance.InitUI();
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

	public override string MenuName => NKCUtilString.GET_STRING_FORGE_CRAFT;

	public override List<int> UpsideMenuShowResourceList => RESOURCE_LIST;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string GuideTempletID => "ARTICLE_EQUIP_MAKE";

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

	public void InitUI()
	{
		base.gameObject.SetActive(value: false);
		for (int i = 0; i < m_lstNKCUIForgeCraftSlot.Count; i++)
		{
			m_lstNKCUIForgeCraftSlot[i].Init(i + 1, OnClickSlotBySelectBtn, OnClickSlotByGetBtn, OnClickSlotByInstanceGetBtn);
		}
		NKCUtil.SetScrollHotKey(m_srForgeCraftSlot);
	}

	private void Update()
	{
		if (base.IsOpen && m_fRefreshTime < Time.time)
		{
			m_fRefreshTime = Time.time + 0.5f;
			for (int i = 0; i < m_lstNKCUIForgeCraftSlot.Count; i++)
			{
				m_lstNKCUIForgeCraftSlot[i].ResetUI(bUpdateIconSlot: false);
			}
		}
	}

	public void OnClickSlotBySelectBtn(int index)
	{
		NKCUIForgeCraftMold.Instance.Open();
	}

	public void OnClickSlotByGetBtn(int index)
	{
		NKMPacket_CRAFT_COMPLETE_REQ nKMPacket_CRAFT_COMPLETE_REQ = new NKMPacket_CRAFT_COMPLETE_REQ();
		nKMPacket_CRAFT_COMPLETE_REQ.index = (byte)index;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_CRAFT_COMPLETE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void SendInstantCompletePacket()
	{
		if (NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetCountMiscItem(1012) < 1)
		{
			NKCShopManager.OpenItemLackPopup(1012, 1);
			return;
		}
		NKMPacket_CRAFT_INSTANT_COMPLETE_REQ nKMPacket_CRAFT_INSTANT_COMPLETE_REQ = new NKMPacket_CRAFT_INSTANT_COMPLETE_REQ();
		nKMPacket_CRAFT_INSTANT_COMPLETE_REQ.index = (byte)m_ReservedInstanceGetIndex;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_CRAFT_INSTANT_COMPLETE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void OnClickSlotByInstanceGetBtn(int index)
	{
		m_ReservedInstanceGetIndex = index;
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(1012);
		if (itemMiscTempletByID != null)
		{
			NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCUtilString.GET_STRING_FORGE_CRAFT_USE_MISC_ONE_PARAM, itemMiscTempletByID.GetItemName()), itemMiscTempletByID.m_ItemMiscID, 1, SendInstantCompletePacket);
		}
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		AddNPC();
		ResetUI();
		UIOpened();
		TutorialCheck();
	}

	public void ResetUI()
	{
		for (int i = 0; i < m_lstNKCUIForgeCraftSlot.Count; i++)
		{
			m_lstNKCUIForgeCraftSlot[i].ResetUI();
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(1012);
		if (itemMiscTempletByID != null)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			m_NKM_UI_FACTORY_CRAFT_INSTANT_TEXT.text = itemMiscTempletByID.GetItemName();
			NKMItemMiscData itemMisc = myUserData.m_InventoryData.GetItemMisc(1012);
			if (itemMisc != null)
			{
				Text nKM_UI_FACTORY_CRAFT_INSTANT_TEXT = m_NKM_UI_FACTORY_CRAFT_INSTANT_TEXT;
				nKM_UI_FACTORY_CRAFT_INSTANT_TEXT.text = nKM_UI_FACTORY_CRAFT_INSTANT_TEXT.text + " :  " + itemMisc.TotalCount;
			}
			else
			{
				m_NKM_UI_FACTORY_CRAFT_INSTANT_TEXT.text += " : 0";
			}
		}
		else
		{
			m_NKM_UI_FACTORY_CRAFT_INSTANT_TEXT.text = NKCUtilString.GET_STRING_FORGE_CRAFT_ITEM_NO_FOUND;
		}
		m_NKM_UI_FACTORY_SHORTCUT_MENU.SetData(ContentsType.FACTORY_CRAFT);
	}

	public void OnRecvSlotOpen(int unlockedSlotNum)
	{
		if (unlockedSlotNum - 1 < m_lstNKCUIForgeCraftSlot.Count)
		{
			NKCUIForgeCraftSlot nKCUIForgeCraftSlot = m_lstNKCUIForgeCraftSlot[unlockedSlotNum - 1];
			if (m_objUnlockEffect != null && nKCUIForgeCraftSlot != null)
			{
				Transform parent = nKCUIForgeCraftSlot.transform;
				m_objUnlockEffect.transform.SetParent(parent);
				m_objUnlockEffect.transform.localPosition = Vector3.zero;
				m_objUnlockEffect.transform.localScale = Vector3.one;
				NKCUtil.SetGameobjectActive(m_objUnlockEffect, bValue: false);
				NKCUtil.SetGameobjectActive(m_objUnlockEffect, bValue: true);
				NKCSoundManager.PlaySound("FX_UI_CONTRACT_SLOT_OPEN", 1f, 0f, 0f);
			}
		}
	}

	public override void CloseInternal()
	{
		m_objUnlockEffect.transform.SetParent(base.transform);
		NKCUtil.SetGameobjectActive(m_objUnlockEffect, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		RemoveNPC();
	}

	public override void Hide()
	{
		m_objUnlockEffect.transform.SetParent(base.transform);
		NKCUtil.SetGameobjectActive(m_objUnlockEffect, bValue: false);
		base.Hide();
	}

	public override void UnHide()
	{
		base.UnHide();
	}

	private void AddNPC()
	{
		if (m_UINPC_Factory == null)
		{
			m_UINPC_Factory = m_objNPCFactory_TouchArea.GetComponent<NKCUINPCFactoryAnastasia>();
			m_UINPC_Factory.Init();
		}
		else
		{
			m_UINPC_Factory.PlayAni(NPC_ACTION_TYPE.START);
		}
	}

	private void RemoveNPC()
	{
		if (m_UINPC_Factory != null)
		{
			m_UINPC_Factory = null;
		}
	}

	private void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.FactoryCraft);
	}

	public NKCUIForgeCraftSlot GetSlot(int index)
	{
		if (index < 1 || index > NKMCraftData.MAX_CRAFT_SLOT_DATA)
		{
			return null;
		}
		return m_lstNKCUIForgeCraftSlot.Find((NKCUIForgeCraftSlot v) => v.GetIndex() == index);
	}
}
