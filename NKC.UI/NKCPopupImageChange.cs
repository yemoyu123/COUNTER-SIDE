using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupImageChange : NKCUIBase
{
	public delegate void OnClickOK(NKCUISlot slot);

	private struct slotData
	{
		public int unitID;

		public int skinID;

		public bool bOnlySkin;

		public slotData(int uID, int sID, bool hasNoUnit)
		{
			unitID = uID;
			skinID = sID;
			bOnlySkin = hasNoUnit;
		}
	}

	public class UnitWithSkin
	{
		public NKMUnitTempletBase m_NKMUnitTempletBase;

		public NKMSkinTemplet m_NKMSkinTemplet;
	}

	public class CompUnitWithSkin : IComparer<UnitWithSkin>
	{
		public int Compare(UnitWithSkin x, UnitWithSkin y)
		{
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			if (x.m_NKMSkinTemplet != null && y.m_NKMSkinTemplet == null)
			{
				return -1;
			}
			if (x.m_NKMSkinTemplet == null && y.m_NKMSkinTemplet != null)
			{
				return 1;
			}
			if (x.m_NKMSkinTemplet != null && y.m_NKMSkinTemplet != null)
			{
				if (x.m_NKMSkinTemplet.m_SkinEquipUnitID < y.m_NKMSkinTemplet.m_SkinEquipUnitID)
				{
					return -1;
				}
				if (x.m_NKMSkinTemplet.m_SkinEquipUnitID > y.m_NKMSkinTemplet.m_SkinEquipUnitID)
				{
					return 1;
				}
				if (x.m_NKMSkinTemplet.m_SkinGrade > y.m_NKMSkinTemplet.m_SkinGrade)
				{
					return -1;
				}
				if (x.m_NKMSkinTemplet.m_SkinGrade < y.m_NKMSkinTemplet.m_SkinGrade)
				{
					return 1;
				}
				x.m_NKMSkinTemplet.m_SkinID.CompareTo(y.m_NKMSkinTemplet.m_SkinID);
			}
			return x.m_NKMUnitTempletBase.m_UnitID.CompareTo(y.m_NKMUnitTempletBase.m_UnitID);
		}
	}

	private class CompGradeAndID : IComparer<ItemMiscDataAndTemplet>
	{
		public int Compare(ItemMiscDataAndTemplet x, ItemMiscDataAndTemplet y)
		{
			if (x == null || x.m_NKMItemMiscTemplet == null || x.m_NKMItemMiscData == null)
			{
				return 1;
			}
			if (y == null || y.m_NKMItemMiscTemplet == null || y.m_NKMItemMiscData == null)
			{
				return -1;
			}
			if (x.m_NKMItemMiscTemplet.m_NKM_ITEM_GRADE > y.m_NKMItemMiscTemplet.m_NKM_ITEM_GRADE)
			{
				return -1;
			}
			if (x.m_NKMItemMiscTemplet.m_NKM_ITEM_GRADE < y.m_NKMItemMiscTemplet.m_NKM_ITEM_GRADE)
			{
				return 1;
			}
			return x.m_NKMItemMiscTemplet.m_ItemMiscID.CompareTo(y.m_NKMItemMiscTemplet.m_ItemMiscID);
		}
	}

	private class ItemMiscDataAndTemplet
	{
		public NKMItemMiscData m_NKMItemMiscData;

		public NKMItemMiscTemplet m_NKMItemMiscTemplet;
	}

	private enum OPEN_TYPE
	{
		NONE,
		UNIT,
		ITEN,
		EMOTICON
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_IMAGE_CHANGE";

	private static NKCPopupImageChange m_Instance;

	public Transform m_ParentOfSlots;

	public Text m_NKM_UI_POPUP_IMAGE_CHANGE_TOP_TEXT;

	public GameObject m_NKM_UI_POPUP_IMAGE_CHANGE_HAVE_NO_TEXT_layoutgruop;

	public Text m_NKM_UI_POPUP_IMAGE_CHANGE_NO_TEXT;

	public LoopVerticalScrollRect m_PROFILE_IMAGE_CHANGE_ScrollView;

	public GridLayoutGroup m_GridLayoutGroup;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private OnClickOK m_OnClickOK;

	public NKCUIComButton m_NKM_UI_POPUP_OK_BOX_OK;

	private List<NKCUISlot> m_slotList = new List<NKCUISlot>();

	private Stack<RectTransform> m_slotPool = new Stack<RectTransform>();

	private List<slotData> m_lstSlotData = new List<slotData>();

	private OPEN_TYPE m_OpenType;

	private List<NKMItemMiscData> m_lstItemMiscData = new List<NKMItemMiscData>();

	private List<int> m_lstEmoticon = new List<int>();

	public static NKCPopupImageChange Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupImageChange>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_IMAGE_CHANGE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupImageChange>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "ImageChange";

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

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_PROFILE_IMAGE_CHANGE_ScrollView.dOnGetObject += OnGetObject;
		m_PROFILE_IMAGE_CHANGE_ScrollView.dOnProvideData += OnProvideData;
		m_PROFILE_IMAGE_CHANGE_ScrollView.dOnReturnObject += OnReturnObject;
		m_PROFILE_IMAGE_CHANGE_ScrollView.PrepareCells();
		NKCUtil.SetScrollHotKey(m_PROFILE_IMAGE_CHANGE_ScrollView);
		if (m_NKM_UI_POPUP_OK_BOX_OK != null)
		{
			m_NKM_UI_POPUP_OK_BOX_OK.PointerClick.RemoveAllListeners();
			m_NKM_UI_POPUP_OK_BOX_OK.PointerClick.AddListener(OK);
			NKCUtil.SetHotkey(m_NKM_UI_POPUP_OK_BOX_OK, HotkeyEventType.Confirm);
		}
	}

	private RectTransform OnGetObject(int index)
	{
		if (m_slotPool.Count > 0)
		{
			RectTransform rectTransform = m_slotPool.Pop();
			NKCUtil.SetGameobjectActive(rectTransform, bValue: true);
			return rectTransform;
		}
		NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_ParentOfSlots);
		if (newInstance == null)
		{
			return null;
		}
		newInstance.transform.localScale = Vector3.one;
		m_slotList.Add(newInstance);
		return newInstance.GetComponent<RectTransform>();
	}

	private void OnProvideData(Transform tr, int index)
	{
		NKCUISlot component = tr.GetComponent<NKCUISlot>();
		if (component == null)
		{
			return;
		}
		component.SetSelected(bSelected: false);
		switch (m_OpenType)
		{
		case OPEN_TYPE.UNIT:
			component.SetUnitData(m_lstSlotData[index].unitID, 0, m_lstSlotData[index].skinID, bShowName: false, bShowLevel: false, bEnableLayoutElement: true, OnClickSlot);
			if (m_lstSlotData[index].bOnlySkin)
			{
				component.SetShowArrowBGText(bSet: true);
				component.SetArrowBGText(NKCStringTable.GetString("SI_DP_PROFILE_REPRESENT_SKIN_ONLY_NO_UNIT"), NKCUtil.GetColor("#A30000"));
				component.SetDisable(disable: true);
			}
			else
			{
				component.SetShowArrowBGText(bSet: false);
				component.SetDisable(disable: false);
			}
			break;
		case OPEN_TYPE.EMOTICON:
			component.SetEmoticonData(NKCUISlot.SlotData.MakeEmoticonData(m_lstEmoticon[index]), bShowName: true, bShowCount: false, bEnableLayoutElement: true, OnClickSlot);
			break;
		case OPEN_TYPE.ITEN:
			if (m_lstItemMiscData[index] == null)
			{
				component.SetEmpty(OnClickSlot);
			}
			else
			{
				component.SetMiscItemData(m_lstItemMiscData[index], bShowName: false, bShowCount: true, bEnableLayoutElement: true, OnClickSlot);
			}
			break;
		}
	}

	private void OnReturnObject(Transform tr)
	{
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
		m_slotPool.Push(tr.GetComponent<RectTransform>());
	}

	public void OpenForUnit(OnClickOK _OnClickOK)
	{
		m_OpenType = OPEN_TYPE.UNIT;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		HashSet<int> hashSet = new HashSet<int>(myUserData.m_ArmyData.m_illustrateUnit);
		IEnumerable<int> skinIds = myUserData.m_InventoryData.SkinIds;
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_IMAGE_CHANGE_TOP_TEXT, NKCUtilString.GET_STRING_FRIEND_CHANGE_IMAGE);
		if (hashSet == null)
		{
			return;
		}
		m_GridLayoutGroup.spacing = new Vector2(m_GridLayoutGroup.spacing.x, 0f);
		List<UnitWithSkin> list = new List<UnitWithSkin>();
		if (skinIds != null)
		{
			foreach (int item in skinIds)
			{
				NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(item);
				if (skinTemplet != null)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(skinTemplet.m_SkinEquipUnitID);
					if (unitTempletBase != null)
					{
						UnitWithSkin unitWithSkin = new UnitWithSkin();
						unitWithSkin.m_NKMSkinTemplet = skinTemplet;
						unitWithSkin.m_NKMUnitTempletBase = unitTempletBase;
						list.Add(unitWithSkin);
					}
				}
			}
		}
		HashSet<int> hashSet2 = new HashSet<int>();
		foreach (int item2 in hashSet)
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(item2);
			if (unitTempletBase2 != null && unitTempletBase2.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
			{
				UnitWithSkin unitWithSkin2 = new UnitWithSkin();
				unitWithSkin2.m_NKMSkinTemplet = null;
				unitWithSkin2.m_NKMUnitTempletBase = unitTempletBase2;
				list.Add(unitWithSkin2);
				if (!hashSet.Contains(unitTempletBase2.m_BaseUnitID))
				{
					hashSet2.Add(unitTempletBase2.m_BaseUnitID);
				}
			}
		}
		hashSet.UnionWith(hashSet2);
		list.Sort(new CompUnitWithSkin());
		if (list.Count <= 0)
		{
			return;
		}
		m_lstSlotData.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			int num = 0;
			bool hasNoUnit = false;
			int unitID = list[i].m_NKMUnitTempletBase.m_UnitID;
			if (list[i].m_NKMSkinTemplet != null)
			{
				num = list[i].m_NKMSkinTemplet.m_SkinID;
			}
			if (num != 0 && !hashSet.Contains(unitID))
			{
				hasNoUnit = true;
			}
			if (num != 0 || list[i].m_NKMUnitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_NORMAL || list[i].m_NKMUnitTempletBase.IsTrophy || NKCCollectionManager.GetUnitTemplet(unitID) != null)
			{
				m_lstSlotData.Add(new slotData(unitID, num, hasNoUnit));
			}
		}
		m_OnClickOK = _OnClickOK;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_IMAGE_CHANGE_HAVE_NO_TEXT_layoutgruop, bValue: false);
		m_NKCUIOpenAnimator.PlayOpenAni();
		m_PROFILE_IMAGE_CHANGE_ScrollView.TotalCount = m_lstSlotData.Count;
		m_PROFILE_IMAGE_CHANGE_ScrollView.SetIndexPosition(0);
		m_PROFILE_IMAGE_CHANGE_ScrollView.RefreshCells(bForce: true);
		UIOpened();
	}

	public void OpenForItem(string title, List<NKMItemMiscData> lstNKMItemMiscData, OnClickOK _OnClickOK, bool bUseEmpty = true, string emptyNoticeText = "")
	{
		m_GridLayoutGroup.spacing = new Vector2(m_GridLayoutGroup.spacing.x, 0f);
		List<ItemMiscDataAndTemplet> list = new List<ItemMiscDataAndTemplet>();
		for (int i = 0; i < lstNKMItemMiscData.Count; i++)
		{
			ItemMiscDataAndTemplet itemMiscDataAndTemplet = new ItemMiscDataAndTemplet();
			itemMiscDataAndTemplet.m_NKMItemMiscData = lstNKMItemMiscData[i];
			itemMiscDataAndTemplet.m_NKMItemMiscTemplet = NKMItemManager.GetItemMiscTempletByID(lstNKMItemMiscData[i].ItemID);
			list.Add(itemMiscDataAndTemplet);
		}
		list.Sort(new CompGradeAndID());
		if (bUseEmpty)
		{
			ItemMiscDataAndTemplet itemMiscDataAndTemplet2 = new ItemMiscDataAndTemplet();
			itemMiscDataAndTemplet2.m_NKMItemMiscData = null;
			itemMiscDataAndTemplet2.m_NKMItemMiscTemplet = null;
			list.Insert(0, itemMiscDataAndTemplet2);
		}
		m_OnClickOK = _OnClickOK;
		m_NKM_UI_POPUP_IMAGE_CHANGE_TOP_TEXT.text = title;
		m_lstItemMiscData.Clear();
		foreach (ItemMiscDataAndTemplet item in list)
		{
			if (item.m_NKMItemMiscData == null)
			{
				m_lstItemMiscData.Add(null);
			}
			else
			{
				m_lstItemMiscData.Add(item.m_NKMItemMiscData);
			}
		}
		m_OpenType = OPEN_TYPE.ITEN;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_PROFILE_IMAGE_CHANGE_ScrollView.TotalCount = m_lstItemMiscData.Count;
		m_PROFILE_IMAGE_CHANGE_ScrollView.SetIndexPosition(0);
		m_PROFILE_IMAGE_CHANGE_ScrollView.RefreshCells(bForce: true);
		if (emptyNoticeText.Length > 0 && m_lstItemMiscData.Count <= 0)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_IMAGE_CHANGE_HAVE_NO_TEXT_layoutgruop, bValue: true);
			NKCUtil.SetLabelText(m_NKM_UI_POPUP_IMAGE_CHANGE_NO_TEXT, emptyNoticeText);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_IMAGE_CHANGE_HAVE_NO_TEXT_layoutgruop, bValue: false);
		}
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	public void OpenForEmoticon(List<int> lstEmoticonID, OnClickOK _OnClickOK)
	{
		m_OpenType = OPEN_TYPE.EMOTICON;
		m_GridLayoutGroup.spacing = new Vector2(m_GridLayoutGroup.spacing.x, 40f);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_IMAGE_CHANGE_TOP_TEXT, NKCUtilString.GET_DEV_CONSOLE_CHEAT_EMOTICON_CHEAT);
		m_OnClickOK = _OnClickOK;
		m_lstEmoticon.Clear();
		m_lstEmoticon = lstEmoticonID;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_IMAGE_CHANGE_HAVE_NO_TEXT_layoutgruop, bValue: false);
		m_PROFILE_IMAGE_CHANGE_ScrollView.TotalCount = m_lstEmoticon.Count;
		m_PROFILE_IMAGE_CHANGE_ScrollView.SetIndexPosition(0);
		m_PROFILE_IMAGE_CHANGE_ScrollView.RefreshCells(bForce: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	private void OnClickSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		foreach (NKCUISlot slot in m_slotList)
		{
			if (slot.GetSlotData() == slotData)
			{
				slot.SetSelected(bSelected: true);
			}
			else
			{
				slot.SetSelected(bSelected: false);
			}
		}
	}

	public void OK()
	{
		if (m_OnClickOK == null)
		{
			return;
		}
		foreach (NKCUISlot slot in m_slotList)
		{
			if (slot.GetSelected())
			{
				m_OnClickOK(slot);
			}
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}
}
