using System.Collections.Generic;
using System.Linq;
using NKM;
using NKM.Event;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventBarCreateMenu : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	public enum Step
	{
		Step1,
		Step2
	}

	public delegate void OnSelectMenu(int cocktailID);

	public delegate void OnChangeUnitAnimation(NKCASUIUnitIllust.eAnimation animation);

	public delegate void OnCreate();

	public delegate void OnCreateRefuse();

	public GameObject m_objStepTechnique;

	public GameObject m_objStepAmount;

	[Header("Step1 Resource")]
	public NKCUIEventBarIngradientSlot[] m_ingradientSlotArray;

	public Text m_lbIngradientCount;

	public NKCUIComStateButton m_csbtnShake;

	public NKCUIComStateButton m_csbtnStir;

	public NKCUIComStateButton m_csbtnNextStep;

	[Header("Step2 Resource")]
	public Text m_lbUnitName;

	public Text m_lbScript;

	public Text m_lbCreateCount;

	public NKCUIItemCostSlot m_ingradientSlot1;

	public NKCUIItemCostSlot m_ingradientSlot2;

	public NKCUIComStateButton m_csbtnUp;

	public NKCUIComStateButton m_csbtnDown;

	public NKCUIComStateButton m_csbtnMax;

	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnCancel;

	[Header("애니메이션")]
	public NKCASUIUnitIllust.eAnimation order;

	public NKCASUIUnitIllust.eAnimation creating;

	public NKCASUIUnitIllust.eAnimation insufficient;

	[Header("대사")]
	public string m_BartenderShake;

	public string m_BartenderStir;

	public string m_SelectedIngrCount;

	private List<int> m_ingradientList = new List<int>();

	private List<int> m_selectedIngradient = new List<int>();

	private ManufacturingTechnique m_selectedTechnique;

	private Step m_step;

	private int m_iEventID;

	private int m_creatingCocktail;

	private int m_creatingCount;

	private const int MinCreatingCount = 1;

	private const int MaxCreatingCount = 999;

	private const int MaxIngradient = 2;

	private OnSelectMenu m_dOnSelectMenu;

	private OnChangeUnitAnimation m_dOnChangeUnitAnimation;

	private OnCreate m_dOnCreate;

	private OnCreateRefuse m_dOnCreateRefuse;

	public Step CurrentStep => m_step;

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnNextStep, OnClickNextStep);
		NKCUtil.SetButtonClickDelegate(m_csbtnShake, OnClickShake);
		NKCUtil.SetButtonClickDelegate(m_csbtnStir, OnClickStir);
		NKCUtil.SetButtonClickDelegate(m_csbtnUp, OnClickUp);
		m_csbtnUp.dOnPointerHoldPress = OnClickUp;
		NKCUtil.SetButtonClickDelegate(m_csbtnDown, OnClickDown);
		m_csbtnDown.dOnPointerHoldPress = OnClickDown;
		NKCUtil.SetButtonClickDelegate(m_csbtnMax, OnClickMax);
		NKCUtil.SetButtonClickDelegate(m_csbtnOK, OnClickOK);
		NKCUtil.SetButtonClickDelegate(m_csbtnCancel, OnClickCancel);
		if (m_ingradientSlotArray != null)
		{
			int num = m_ingradientSlotArray.Length;
			for (int i = 0; i < num; i++)
			{
				m_ingradientSlotArray[i].Init();
			}
		}
		NKCUtil.SetHotkey(m_csbtnUp, HotkeyEventType.Plus);
		NKCUtil.SetHotkey(m_csbtnDown, HotkeyEventType.Minus);
		NKCUtil.SetHotkey(m_csbtnMax, HotkeyEventType.Down);
		NKCUtil.SetHotkey(m_csbtnNextStep, HotkeyEventType.Right);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		NKCUtil.SetHotkey(m_csbtnCancel, HotkeyEventType.Left);
	}

	public void Open(int eventID, int bartenderID, OnSelectMenu onSelectMenu, OnChangeUnitAnimation onChangeUnitAnimation, OnCreate onCreate = null, OnCreateRefuse onCreateRefuse = null)
	{
		m_dOnSelectMenu = onSelectMenu;
		m_dOnChangeUnitAnimation = onChangeUnitAnimation;
		m_dOnCreate = onCreate;
		m_dOnCreateRefuse = onCreateRefuse;
		m_iEventID = eventID;
		m_creatingCocktail = 0;
		m_creatingCount = 0;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(bartenderID);
		if (unitTempletBase != null)
		{
			NKCUtil.SetLabelText(m_lbUnitName, unitTempletBase.GetUnitName());
		}
		SetStep1State(eventID);
	}

	public void Refresh()
	{
		switch (m_step)
		{
		case Step.Step1:
			m_creatingCocktail = 0;
			m_creatingCount = 0;
			SetStep1State(m_iEventID);
			break;
		case Step.Step2:
			SetStep2State();
			break;
		}
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (m_step == Step.Step2)
		{
			if (eventData.scrollDelta.y < 0f)
			{
				OnClickDown();
			}
			else if (eventData.scrollDelta.y > 0f)
			{
				OnClickUp();
			}
		}
	}

	public void Close()
	{
		m_ingradientList?.Clear();
		m_selectedIngradient?.Clear();
		m_dOnSelectMenu = null;
		m_dOnChangeUnitAnimation = null;
		m_dOnCreate = null;
	}

	private void SetStep1State(int eventID)
	{
		if (m_ingradientList.Count <= 0)
		{
			foreach (NKMEventBarTemplet value in NKMEventBarTemplet.Values)
			{
				if (value.EventID == eventID)
				{
					if (!m_ingradientList.Contains(value.MaterialItemId01))
					{
						m_ingradientList.Add(value.MaterialItemId01);
					}
					if (!m_ingradientList.Contains(value.MaterialItemId02))
					{
						m_ingradientList.Add(value.MaterialItemId02);
					}
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_objStepTechnique, bValue: true);
		NKCUtil.SetGameobjectActive(m_objStepAmount, bValue: false);
		int num = 0;
		if (m_ingradientSlotArray != null)
		{
			num = m_ingradientSlotArray.Length;
			for (int i = 0; i < num; i++)
			{
				if (m_ingradientList.Count <= i)
				{
					NKCUtil.SetGameobjectActive(m_ingradientSlotArray[i], bValue: false);
					continue;
				}
				NKCUtil.SetGameobjectActive(m_ingradientSlotArray[i], bValue: true);
				m_ingradientSlotArray[i].SetData(m_ingradientList[i], OnSelectIngredient);
			}
		}
		m_selectedIngradient.Clear();
		m_csbtnNextStep?.SetLock(value: true);
		m_csbtnShake?.Select(bSelect: false);
		m_csbtnStir?.Select(bSelect: false);
		m_selectedTechnique = ManufacturingTechnique.none;
		m_step = Step.Step1;
		NKCUtil.SetLabelText(m_lbIngradientCount, string.Format(NKCStringTable.GetString(m_SelectedIngrCount), m_selectedIngradient.Count, 2));
	}

	private void SetStep2State()
	{
		NKCUtil.SetGameobjectActive(m_objStepTechnique, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStepAmount, bValue: true);
		SetCocktailMakingScript();
		m_creatingCount = 1;
		NKMEventBarTemplet nKMEventBarTemplet = NKMEventBarTemplet.Find(m_creatingCocktail);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMEventBarTemplet != null && nKMUserData != null)
		{
			long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(nKMEventBarTemplet.MaterialItemId01);
			m_ingradientSlot1?.SetData(nKMEventBarTemplet.MaterialItemId01, nKMEventBarTemplet.MaterialItemValue01 * m_creatingCount, countMiscItem);
			countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(nKMEventBarTemplet.MaterialItemId02);
			m_ingradientSlot2?.SetData(nKMEventBarTemplet.MaterialItemId02, nKMEventBarTemplet.MaterialItemValue02 * m_creatingCount, countMiscItem);
		}
		ChangeStep2State(m_creatingCount);
		m_step = Step.Step2;
	}

	private void SetCocktailMakingScript()
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(m_creatingCocktail);
		if (itemMiscTempletByID != null)
		{
			switch (m_selectedTechnique)
			{
			case ManufacturingTechnique.shake:
				NKCUtil.SetLabelText(m_lbScript, string.Format(NKCStringTable.GetString(m_BartenderShake), itemMiscTempletByID.GetItemName()));
				break;
			case ManufacturingTechnique.stir:
				NKCUtil.SetLabelText(m_lbScript, string.Format(NKCStringTable.GetString(m_BartenderStir), itemMiscTempletByID.GetItemName()));
				break;
			default:
				NKCUtil.SetLabelText(m_lbScript, " - ");
				break;
			}
		}
	}

	private int GetCreatedCocktailID()
	{
		if (m_selectedIngradient.Count != 2 || m_selectedTechnique == ManufacturingTechnique.none)
		{
			return 0;
		}
		int result = 0;
		NKMEventBarTemplet nKMEventBarTemplet = NKMEventBarTemplet.Values.FirstOrDefault((NKMEventBarTemplet e) => e.EventID == m_iEventID && m_selectedIngradient.Contains(e.MaterialItemId01) && m_selectedIngradient.Contains(e.MaterialItemId02) && m_selectedTechnique == e.Technique);
		if (nKMEventBarTemplet != null)
		{
			result = nKMEventBarTemplet.RewardItemId;
		}
		return result;
	}

	private void ChangeStep1State()
	{
		m_creatingCocktail = GetCreatedCocktailID();
		m_csbtnNextStep?.SetLock(m_creatingCocktail == 0);
		if (m_dOnSelectMenu != null)
		{
			m_dOnSelectMenu(m_creatingCocktail);
		}
	}

	private void ChangeIngradientSlotCount()
	{
		NKMEventBarTemplet nKMEventBarTemplet = NKMEventBarTemplet.Find(m_creatingCocktail);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMEventBarTemplet != null && nKMUserData != null)
		{
			long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(nKMEventBarTemplet.MaterialItemId01);
			m_ingradientSlot1?.SetCount(nKMEventBarTemplet.MaterialItemValue01 * m_creatingCount, countMiscItem);
			countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(nKMEventBarTemplet.MaterialItemId02);
			m_ingradientSlot2?.SetCount(nKMEventBarTemplet.MaterialItemValue02 * m_creatingCount, countMiscItem);
		}
	}

	private void ChangeStep2State(int destCount)
	{
		if (CanCreateDestCount(destCount))
		{
			NKCUtil.SetLabelTextColor(m_lbCreateCount, Color.white);
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbCreateCount, Color.red);
		}
		m_creatingCount = destCount;
		NKCUtil.SetLabelText(m_lbCreateCount, m_creatingCount.ToString("D3"));
	}

	private bool CanCreateDestCount(int destCount)
	{
		NKMEventBarTemplet nKMEventBarTemplet = NKMEventBarTemplet.Find(m_creatingCocktail);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMEventBarTemplet == null || nKMUserData == null)
		{
			return false;
		}
		long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(m_ingradientSlot1.ItemID);
		long countMiscItem2 = nKMUserData.m_InventoryData.GetCountMiscItem(m_ingradientSlot2.ItemID);
		if (countMiscItem < nKMEventBarTemplet.MaterialItemValue01 * destCount || countMiscItem2 < nKMEventBarTemplet.MaterialItemValue02 * destCount)
		{
			return false;
		}
		return true;
	}

	private bool OnSelectIngredient(int ingradientID, bool select)
	{
		if (select && m_selectedIngradient.Count >= 2)
		{
			return false;
		}
		if (select)
		{
			m_selectedIngradient.Add(ingradientID);
		}
		else
		{
			m_selectedIngradient.Remove(ingradientID);
		}
		NKCUtil.SetLabelText(m_lbIngradientCount, string.Format(NKCStringTable.GetString(m_SelectedIngrCount), m_selectedIngradient.Count, 2));
		ChangeStep1State();
		return select;
	}

	private void OnClickNextStep()
	{
		SetStep2State();
		if (m_dOnChangeUnitAnimation != null)
		{
			m_dOnChangeUnitAnimation(order);
		}
	}

	private void OnClickShake()
	{
		if (!m_csbtnShake.m_bSelect)
		{
			m_csbtnShake?.Select(bSelect: true);
			m_csbtnStir?.Select(bSelect: false);
			m_selectedTechnique = ManufacturingTechnique.shake;
			ChangeStep1State();
		}
	}

	private void OnClickStir()
	{
		if (!m_csbtnStir.m_bSelect)
		{
			m_csbtnShake?.Select(bSelect: false);
			m_csbtnStir?.Select(bSelect: true);
			m_selectedTechnique = ManufacturingTechnique.stir;
			ChangeStep1State();
		}
	}

	private void OnClickUp()
	{
		SetCocktailMakingScript();
		int num = Mathf.Min(m_creatingCount + 1, 999);
		if (CanCreateDestCount(num))
		{
			m_creatingCount = num;
			NKCUtil.SetLabelTextColor(m_lbCreateCount, Color.white);
			NKCUtil.SetLabelText(m_lbCreateCount, m_creatingCount.ToString("D3"));
			ChangeIngradientSlotCount();
		}
	}

	private void OnClickDown()
	{
		SetCocktailMakingScript();
		int destCount = Mathf.Max(m_creatingCount - 1, 1);
		ChangeStep2State(destCount);
		ChangeIngradientSlotCount();
	}

	private void OnClickMax()
	{
		SetCocktailMakingScript();
		NKMEventBarTemplet nKMEventBarTemplet = NKMEventBarTemplet.Find(m_creatingCocktail);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMEventBarTemplet != null && nKMUserData != null)
		{
			long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(m_ingradientSlot1.ItemID);
			long countMiscItem2 = nKMUserData.m_InventoryData.GetCountMiscItem(m_ingradientSlot2.ItemID);
			long num = countMiscItem / nKMEventBarTemplet.MaterialItemValue01;
			long num2 = countMiscItem2 / nKMEventBarTemplet.MaterialItemValue02;
			m_creatingCount = (int)Mathf.Min(num, num2);
			m_creatingCount = Mathf.Min(m_creatingCount, 999);
			if (m_creatingCount >= 1)
			{
				NKCUtil.SetLabelTextColor(m_lbCreateCount, Color.white);
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_lbCreateCount, Color.red);
				m_creatingCount = 1;
			}
			NKCUtil.SetLabelText(m_lbCreateCount, m_creatingCount.ToString("D3"));
			ChangeIngradientSlotCount();
		}
	}

	private void OnClickOK()
	{
		if (NKCUIEventBarResult.IsInstanceOpen)
		{
			return;
		}
		if (!CanCreateDestCount(m_creatingCount))
		{
			if (m_dOnChangeUnitAnimation != null)
			{
				m_dOnChangeUnitAnimation(insufficient);
			}
			if (m_dOnCreateRefuse != null)
			{
				m_dOnCreateRefuse();
			}
			return;
		}
		if (m_dOnChangeUnitAnimation != null)
		{
			m_dOnChangeUnitAnimation(creating);
		}
		if (m_dOnCreate != null)
		{
			m_dOnCreate();
		}
		NKCPacketSender.Send_NKMPacket_EVENT_BAR_CREATE_COCKTAIL_REQ(m_creatingCocktail, m_creatingCount);
	}

	private void OnClickCancel()
	{
		NKCUtil.SetGameobjectActive(m_objStepTechnique, bValue: true);
		NKCUtil.SetGameobjectActive(m_objStepAmount, bValue: false);
		m_step = Step.Step1;
	}

	private void OnDestroy()
	{
		if (m_ingradientList != null)
		{
			m_ingradientList.Clear();
			m_ingradientList = null;
		}
		if (m_selectedIngradient != null)
		{
			m_selectedIngradient.Clear();
			m_selectedIngradient = null;
		}
		m_dOnSelectMenu = null;
		m_dOnChangeUnitAnimation = null;
		m_dOnCreate = null;
	}
}
