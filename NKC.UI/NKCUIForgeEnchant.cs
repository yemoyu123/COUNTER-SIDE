using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Item;
using NKC.PacketHandler;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForgeEnchant : MonoBehaviour
{
	public enum FORGE_ENCHANT_TYPE
	{
		EQUIP,
		MISC
	}

	public class CandidateMaterial
	{
		public NKMEquipTemplet m_NKMEquipTemplet;

		public NKMEquipItemData m_NKMEquipItemData;
	}

	public class CompForAutoSelect : IComparer<CandidateMaterial>
	{
		public int Compare(CandidateMaterial x, CandidateMaterial y)
		{
			if (x.m_NKMEquipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ENCHANT && y.m_NKMEquipTemplet.m_ItemEquipPosition != ITEM_EQUIP_POSITION.IEP_ENCHANT)
			{
				return -1;
			}
			if (x.m_NKMEquipTemplet.m_ItemEquipPosition != ITEM_EQUIP_POSITION.IEP_ENCHANT && y.m_NKMEquipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ENCHANT)
			{
				return 1;
			}
			if (x.m_NKMEquipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ENCHANT && y.m_NKMEquipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ENCHANT)
			{
				if (x.m_NKMEquipTemplet.m_NKM_ITEM_TIER < y.m_NKMEquipTemplet.m_NKM_ITEM_TIER)
				{
					return 1;
				}
				if (x.m_NKMEquipTemplet.m_NKM_ITEM_TIER > y.m_NKMEquipTemplet.m_NKM_ITEM_TIER)
				{
					return -1;
				}
				if (x.m_NKMEquipTemplet.m_NKM_ITEM_GRADE < y.m_NKMEquipTemplet.m_NKM_ITEM_GRADE)
				{
					return 1;
				}
				if (x.m_NKMEquipTemplet.m_NKM_ITEM_GRADE > y.m_NKMEquipTemplet.m_NKM_ITEM_GRADE)
				{
					return -1;
				}
				if (x.m_NKMEquipItemData.m_ItemUid < y.m_NKMEquipItemData.m_ItemUid)
				{
					return -1;
				}
				if (x.m_NKMEquipItemData.m_ItemUid > y.m_NKMEquipItemData.m_ItemUid)
				{
					return 1;
				}
				return 0;
			}
			if (x.m_NKMEquipTemplet.m_NKM_ITEM_TIER < y.m_NKMEquipTemplet.m_NKM_ITEM_TIER)
			{
				return -1;
			}
			if (x.m_NKMEquipTemplet.m_NKM_ITEM_TIER > y.m_NKMEquipTemplet.m_NKM_ITEM_TIER)
			{
				return 1;
			}
			if (x.m_NKMEquipTemplet.m_NKM_ITEM_GRADE < y.m_NKMEquipTemplet.m_NKM_ITEM_GRADE)
			{
				return -1;
			}
			if (x.m_NKMEquipTemplet.m_NKM_ITEM_GRADE > y.m_NKMEquipTemplet.m_NKM_ITEM_GRADE)
			{
				return 1;
			}
			if (x.m_NKMEquipItemData.m_ItemUid < y.m_NKMEquipItemData.m_ItemUid)
			{
				return -1;
			}
			if (x.m_NKMEquipItemData.m_ItemUid > y.m_NKMEquipItemData.m_ItemUid)
			{
				return 1;
			}
			return 0;
		}
	}

	private long m_LeftEquipUID;

	private int m_needCredit;

	public NKCUIRectMove m_rmEnchantRoot;

	[Header("재료 타입별 오브젝트")]
	public GameObject m_objEquip;

	public GameObject m_objMisc;

	[Header("공용 상단")]
	public NKCUIComToggle m_tglEquip;

	public NKCUIComToggle m_tglMisc;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01_Before;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_02_Before;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01_After;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_02_After;

	public Text m_ENCHANT_BEFORE_TEXT;

	public GameObject m_ENCHANT_ARROW;

	public Text m_ENCHANT_AFTER_TEXT;

	public Text m_NKM_UI_FACTORY_ENCHANT_EXP_BG_TEXT;

	public GameObject m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER;

	public GameObject m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_NEW;

	public Slider m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_Slider;

	public Slider m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_NEW_Slider;

	[Header("장비")]
	public List<NKCUISlot> m_listNKCUIEnhanceSlot;

	public List<GameObject> m_listEffect;

	public NKCUIItemCostSlot m_creditSlot;

	[Header("Misc")]
	public List<Text> m_lstMaterialUseCount = new List<Text>(4);

	public List<NKCUIItemCostSlot> m_lstUIItemReq = new List<NKCUIItemCostSlot>(4);

	public List<Text> m_lstMaterialHaveCount = new List<Text>(4);

	public List<GameObject> m_lstMiscEffect = new List<GameObject>();

	public NKCUIComStateButton m_csbtnMaterial_1_Add;

	public NKCUIComStateButton m_csbtnMaterial_1_Minus;

	public NKCUIComStateButton m_csbtnMaterial_2_Add;

	public NKCUIComStateButton m_csbtnMaterial_2_Minus;

	public NKCUIComStateButton m_csbtnMaterial_3_Add;

	public NKCUIComStateButton m_csbtnMaterial_3_Minus;

	public NKCUIComStateButton m_csbtnMaterial_4_Add;

	public NKCUIComStateButton m_csbtnMaterial_4_Minus;

	public NKCUIComStateButton m_btnEnchantTo2;

	public NKCUIComStateButton m_btnEnchantTo5;

	public NKCUIComStateButton m_btnEnchantTo7;

	public NKCUIComStateButton m_btnEnchantToMax;

	[Header("공용 하단")]
	public Text m_lbCreditReq;

	public Text m_txt_NKM_UI_FACTORY_ENCHANT_RESET_TEXT;

	public Image m_img_NKM_UI_FACTORY_ENCHANT_ENCHANT_ICON;

	public Text m_txt_NKM_UI_FACTORY_ENCHANT_ENCHANT_TEXT;

	public Image m_Img_NKM_UI_FACTORY_ENCHANT_BUTTON_RESET;

	public Image m_Img_NKM_UI_FACTORY_ENCHANT_BUTTON_ENCHANT;

	public GameObject m_NKM_UI_FACTORY_ENCHANT_BUTTON_ENCHANT_LIGHT;

	public NKCUIComStateButton m_btn_NKM_UI_FACTORY_ENCHANT_AUTOSELECT;

	public NKCUIComButton m_btn_NKM_UI_FACTORY_ENCHANT_BUTTON_RESET;

	public NKCUIComButton m_btnNKM_UI_FACTORY_ENCHANT_BUTTON_ENCHANT;

	private List<long> m_listCurrentMaterialEquipsToEnhance = new List<long>();

	private List<MiscItemData> m_lstMaterials = new List<MiscItemData>(4);

	private NKCUIInventory m_UIInventory;

	private bool m_bMaxLevel;

	private NKMEquipItemData m_cNKMEquipItemDataTarget;

	private NKMEquipTemplet m_cNKMEquipTempletTarget;

	private int m_expectedLevel;

	private int m_expectedExp;

	private float m_currentExpPercent;

	private float m_expectedExpPercent;

	private FORGE_ENCHANT_TYPE m_curEnchantType = FORGE_ENCHANT_TYPE.MISC;

	public const float PRESS_GAP_MAX = 0.3f;

	public const float PRESS_GAP_MIN = 0.01f;

	public const float DAMPING = 0.8f;

	private float m_fDelay = 0.5f;

	private float m_fHoldTime;

	private int m_iChangeValue;

	private bool m_bPress;

	private bool m_bWasHold;

	private int targetId;

	private int m_targetLevel;

	public FORGE_ENCHANT_TYPE GetCurEnchantType()
	{
		return m_curEnchantType;
	}

	public void InitUI()
	{
		m_curEnchantType = FORGE_ENCHANT_TYPE.MISC;
		if (m_listNKCUIEnhanceSlot != null)
		{
			for (int i = 0; i < m_listNKCUIEnhanceSlot.Count; i++)
			{
				if (m_listNKCUIEnhanceSlot[i] != null)
				{
					m_listNKCUIEnhanceSlot[i].Init();
				}
			}
		}
		for (int j = 0; j < 4; j++)
		{
			if (j < NKMCommonConst.EquipEnchantMiscConst.Materials.Count)
			{
				MiscItemData miscItemData = new MiscItemData();
				miscItemData.itemId = NKMCommonConst.EquipEnchantMiscConst.Materials[j].ItemId;
				miscItemData.count = 0;
				m_lstMaterials.Add(miscItemData);
			}
		}
		m_btn_NKM_UI_FACTORY_ENCHANT_AUTOSELECT.PointerClick.RemoveAllListeners();
		m_btn_NKM_UI_FACTORY_ENCHANT_AUTOSELECT.PointerClick.AddListener(AutoSelect);
		m_btn_NKM_UI_FACTORY_ENCHANT_BUTTON_RESET.PointerClick.RemoveAllListeners();
		m_btn_NKM_UI_FACTORY_ENCHANT_BUTTON_RESET.PointerClick.AddListener(ResetMaterialEquipSlotsToEnhance);
		m_btnNKM_UI_FACTORY_ENCHANT_BUTTON_ENCHANT.PointerClick.RemoveAllListeners();
		m_btnNKM_UI_FACTORY_ENCHANT_BUTTON_ENCHANT.PointerClick.AddListener(TrySendEquipEnchant);
		m_tglEquip.OnValueChanged.RemoveAllListeners();
		m_tglEquip.OnValueChanged.AddListener(OnTglEquip);
		m_tglMisc.OnValueChanged.RemoveAllListeners();
		m_tglMisc.OnValueChanged.AddListener(OnTglMisc);
		m_csbtnMaterial_1_Add.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_1_Add.PointerDown.AddListener(OnPlusDown_1);
		m_csbtnMaterial_1_Add.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_1_Add.PointerUp.AddListener(OnButtonUp);
		m_csbtnMaterial_2_Add.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_2_Add.PointerDown.AddListener(OnPlusDown_2);
		m_csbtnMaterial_2_Add.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_2_Add.PointerUp.AddListener(OnButtonUp);
		m_csbtnMaterial_3_Add.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_3_Add.PointerDown.AddListener(OnPlusDown_3);
		m_csbtnMaterial_3_Add.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_3_Add.PointerUp.AddListener(OnButtonUp);
		m_csbtnMaterial_4_Add.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_4_Add.PointerDown.AddListener(OnPlusDown_4);
		m_csbtnMaterial_4_Add.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_4_Add.PointerUp.AddListener(OnButtonUp);
		m_csbtnMaterial_1_Minus.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_1_Minus.PointerDown.AddListener(OnMinusDown_1);
		m_csbtnMaterial_1_Minus.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_1_Minus.PointerUp.AddListener(OnButtonUp);
		m_csbtnMaterial_2_Minus.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_2_Minus.PointerDown.AddListener(OnMinusDown_2);
		m_csbtnMaterial_2_Minus.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_2_Minus.PointerUp.AddListener(OnButtonUp);
		m_csbtnMaterial_3_Minus.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_3_Minus.PointerDown.AddListener(OnMinusDown_3);
		m_csbtnMaterial_3_Minus.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_3_Minus.PointerUp.AddListener(OnButtonUp);
		m_csbtnMaterial_4_Minus.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_4_Minus.PointerDown.AddListener(OnMinusDown_4);
		m_csbtnMaterial_4_Minus.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_4_Minus.PointerUp.AddListener(OnButtonUp);
		if (m_btnEnchantTo2 != null)
		{
			m_btnEnchantTo2.PointerClick.RemoveAllListeners();
			m_btnEnchantTo2.PointerClick.AddListener(OnClickEnchantTo2);
		}
		if (m_btnEnchantTo5 != null)
		{
			m_btnEnchantTo5.PointerClick.RemoveAllListeners();
			m_btnEnchantTo5.PointerClick.AddListener(OnClickEnchantTo5);
		}
		if (m_btnEnchantTo7 != null)
		{
			m_btnEnchantTo7.PointerClick.RemoveAllListeners();
			m_btnEnchantTo7.PointerClick.AddListener(OnClickEnchantTo7);
		}
		if (m_btnEnchantToMax != null)
		{
			m_btnEnchantToMax.PointerClick.RemoveAllListeners();
			m_btnEnchantToMax.PointerClick.AddListener(OnClickEnchantToMax);
		}
		NKCUtil.SetBindFunction(m_csbtnMaterial_1_Add, delegate
		{
			OnChangeCount(NKMCommonConst.EquipEnchantMiscConst.Materials[0].ItemId);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_2_Add, delegate
		{
			OnChangeCount(NKMCommonConst.EquipEnchantMiscConst.Materials[1].ItemId);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_3_Add, delegate
		{
			OnChangeCount(NKMCommonConst.EquipEnchantMiscConst.Materials[2].ItemId);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_4_Add, delegate
		{
			OnChangeCount(NKMCommonConst.EquipEnchantMiscConst.Materials[3].ItemId);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_1_Minus, delegate
		{
			OnChangeCount(NKMCommonConst.EquipEnchantMiscConst.Materials[0].ItemId, bPlus: false);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_2_Minus, delegate
		{
			OnChangeCount(NKMCommonConst.EquipEnchantMiscConst.Materials[1].ItemId, bPlus: false);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_3_Minus, delegate
		{
			OnChangeCount(NKMCommonConst.EquipEnchantMiscConst.Materials[2].ItemId, bPlus: false);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_4_Minus, delegate
		{
			OnChangeCount(NKMCommonConst.EquipEnchantMiscConst.Materials[3].ItemId, bPlus: false);
		});
	}

	public void SetLeftEquipUID(long uid)
	{
		m_LeftEquipUID = uid;
		EnableUI(m_LeftEquipUID != 0);
	}

	public void SetOut()
	{
		m_rmEnchantRoot.Set("Out");
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void AnimateOutToIn()
	{
		ClearEnhanceEffect();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_rmEnchantRoot.Set("Out");
		m_rmEnchantRoot.Transit("In");
		m_tglEquip.Select(m_curEnchantType == FORGE_ENCHANT_TYPE.EQUIP, bForce: true);
		m_tglMisc.Select(m_curEnchantType == FORGE_ENCHANT_TYPE.MISC, bForce: true);
		NKCUtil.SetGameobjectActive(m_objEquip, m_curEnchantType == FORGE_ENCHANT_TYPE.EQUIP);
		NKCUtil.SetGameobjectActive(m_objMisc, m_curEnchantType == FORGE_ENCHANT_TYPE.MISC);
		ResetMaterialEquipSlotsToEnhanceUI();
	}

	public void PlayEnhanceEffect()
	{
		for (int i = 0; i < m_listNKCUIEnhanceSlot.Count; i++)
		{
			if (!m_listNKCUIEnhanceSlot[i].IsEmpty())
			{
				NKCUtil.SetGameobjectActive(m_listEffect[i], bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_listEffect[i], bValue: false);
			}
		}
		for (int j = 0; j < m_lstMiscEffect.Count; j++)
		{
			if (m_lstMaterials[j].count > 0)
			{
				NKCUtil.SetGameobjectActive(m_lstMiscEffect[j], bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstMiscEffect[j], bValue: false);
			}
		}
	}

	public void ClearEnhanceEffect()
	{
		for (int i = 0; i < m_listNKCUIEnhanceSlot.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_listEffect[i], bValue: false);
		}
		for (int j = 0; j < m_lstMiscEffect.Count; j++)
		{
			NKCUtil.SetGameobjectActive(m_lstMiscEffect[j], bValue: false);
		}
	}

	public void ClearCurrentMaterialEquipsToEnhance()
	{
		m_listCurrentMaterialEquipsToEnhance.Clear();
		for (int i = 0; i < m_lstMaterials.Count; i++)
		{
			m_lstMaterials[i].count = 0;
		}
		SetUIItemReq();
		CalcExp();
		UpdateExpUI();
	}

	public void RemoveCurrentMaterialEquipToEnhance(long equipUID)
	{
		m_listCurrentMaterialEquipsToEnhance.Remove(equipUID);
	}

	public void OnFinishMultiSelectionToEnhance(List<long> listEquipSlot)
	{
		if (m_UIInventory != null && m_UIInventory.IsOpen)
		{
			m_UIInventory.Close();
		}
		SetMaterials(listEquipSlot);
	}

	private void SetMaterials(List<long> listEquipSlot)
	{
		m_listCurrentMaterialEquipsToEnhance.Clear();
		m_listCurrentMaterialEquipsToEnhance.AddRange(listEquipSlot);
		ResetMaterialEquipSlotsToEnhanceUI();
		CalcExp();
		UpdateExpUI();
		SetUIItemReq();
	}

	public void ResetMaterialEquipSlotsToEnhance()
	{
		ClearCurrentMaterialEquipsToEnhance();
		ResetMaterialEquipSlotsToEnhanceUI();
		CalcExp();
		UpdateExpUI();
		SetUIItemReq();
	}

	public void AutoSelect()
	{
		if (m_LeftEquipUID == 0L)
		{
			return;
		}
		NKMInventoryData inventoryData = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData;
		NKMEquipItemData itemEquip = inventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip == null || NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID) == null)
		{
			return;
		}
		List<NKMEquipItemData> list = new List<NKMEquipItemData>(inventoryData.EquipItems.Values);
		int num = 0;
		for (num = 0; num < list.Count; num++)
		{
			bool flag = false;
			NKMEquipItemData nKMEquipItemData = list[num];
			if (nKMEquipItemData != null)
			{
				if (nKMEquipItemData.m_OwnerUnitUID > 0 || nKMEquipItemData.m_EnchantLevel > 0 || nKMEquipItemData.m_bLock || nKMEquipItemData.m_ItemUid == m_LeftEquipUID)
				{
					flag = true;
				}
				else
				{
					NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(nKMEquipItemData.m_ItemEquipID);
					if (equipTemplet == null)
					{
						flag = true;
					}
					else if (equipTemplet.m_NKM_ITEM_GRADE > NKM_ITEM_GRADE.NIG_R && equipTemplet.m_EquipUnitStyleType != NKM_UNIT_STYLE_TYPE.NUST_ENCHANT)
					{
						flag = true;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				list.RemoveAt(num);
				num--;
			}
		}
		List<CandidateMaterial> list2 = new List<CandidateMaterial>();
		for (num = 0; num < list.Count; num++)
		{
			NKMEquipItemData nKMEquipItemData2 = list[num];
			NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(nKMEquipItemData2.m_ItemEquipID);
			if (equipTemplet2 != null)
			{
				CandidateMaterial candidateMaterial = new CandidateMaterial();
				candidateMaterial.m_NKMEquipItemData = nKMEquipItemData2;
				candidateMaterial.m_NKMEquipTemplet = equipTemplet2;
				list2.Add(candidateMaterial);
			}
		}
		list2.Sort(new CompForAutoSelect());
		int needExpToMaxLevel = NKMItemManager.GetNeedExpToMaxLevel(itemEquip);
		int num2 = 0;
		List<long> list3 = new List<long>();
		for (num = 0; num < list2.Count && num < m_listNKCUIEnhanceSlot.Count; num++)
		{
			if (needExpToMaxLevel <= 0)
			{
				break;
			}
			list3.Add(list2[num].m_NKMEquipItemData.m_ItemUid);
			num2 += NKMItemManager.GetEquipEnchantFeedExp(list2[num].m_NKMEquipItemData);
			if (num2 > needExpToMaxLevel)
			{
				break;
			}
		}
		SetMaterials(list3);
	}

	private void SetBeforeStatTextAndSprite(Text beforeText, EQUIP_ITEM_STAT cEQUIP_ITEM_STAT, NKMEquipItemData cNKMEquipItemDataTarget)
	{
		string statShortString = NKCUtilString.GetStatShortString(cEQUIP_ITEM_STAT.type, cEQUIP_ITEM_STAT.stat_value + cEQUIP_ITEM_STAT.stat_level_value * (float)cNKMEquipItemDataTarget.m_EnchantLevel, NKMUnitStatManager.IsPercentStat(cEQUIP_ITEM_STAT.type));
		NKCUtil.SetLabelText(beforeText, statShortString);
	}

	private void SetAfterStatTextAndSprite(Text afterText, int preCalcEnchantLV, EQUIP_ITEM_STAT cEQUIP_ITEM_STAT, NKMEquipItemData cNKMEquipItemDataTarget)
	{
		string statShortString = NKCUtilString.GetStatShortString(cEQUIP_ITEM_STAT.type, cEQUIP_ITEM_STAT.stat_value + cEQUIP_ITEM_STAT.stat_level_value * (float)preCalcEnchantLV, NKMUnitStatManager.IsPercentStat(cEQUIP_ITEM_STAT.type));
		NKCUtil.SetLabelText(afterText, statShortString);
		if (preCalcEnchantLV > cNKMEquipItemDataTarget.m_EnchantLevel)
		{
			string text = afterText.text;
			if (NKMUnitStatManager.IsPercentStat(cEQUIP_ITEM_STAT.type))
			{
				NKCUtil.SetLabelText(afterText, $"{text} <color=#FFDB00>({cEQUIP_ITEM_STAT.stat_level_value * (float)preCalcEnchantLV - cEQUIP_ITEM_STAT.stat_level_value * (float)cNKMEquipItemDataTarget.m_EnchantLevel:+#.0%;-#.0%;0%})</color>");
			}
			else
			{
				NKCUtil.SetLabelText(afterText, $"{text} <color=#FFDB00>({cEQUIP_ITEM_STAT.stat_level_value * (float)preCalcEnchantLV - cEQUIP_ITEM_STAT.stat_level_value * (float)cNKMEquipItemDataTarget.m_EnchantLevel:+#;-#;''})</color>");
			}
		}
	}

	public void TrySendEquipEnchant()
	{
		if (m_LeftEquipUID <= 0 || m_cNKMEquipItemDataTarget == null)
		{
			return;
		}
		if (m_cNKMEquipTempletTarget != null && m_cNKMEquipItemDataTarget.m_EnchantLevel >= NKMItemManager.GetMaxEquipEnchantLevel(m_cNKMEquipTempletTarget.m_NKM_ITEM_TIER))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_ENCHANT_ALREADY_MAX);
		}
		else if (m_tglEquip.IsSelected)
		{
			if (m_listCurrentMaterialEquipsToEnhance.Count > 0)
			{
				NKM_ERROR_CODE nKM_ERROR_CODE = NKMItemManager.CanEnchantItem(NKCScenManager.GetScenManager().GetMyUserData(), m_cNKMEquipItemDataTarget.m_ItemUid, m_listCurrentMaterialEquipsToEnhance);
				switch (nKM_ERROR_CODE)
				{
				case NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT:
					NKCShopManager.OpenItemLackPopup(1, m_needCredit);
					break;
				default:
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE));
					break;
				case NKM_ERROR_CODE.NEC_OK:
				{
					NKMPacket_EQUIP_ITEM_ENCHANT_REQ nKMPacket_EQUIP_ITEM_ENCHANT_REQ = new NKMPacket_EQUIP_ITEM_ENCHANT_REQ();
					nKMPacket_EQUIP_ITEM_ENCHANT_REQ.equipItemUID = m_LeftEquipUID;
					nKMPacket_EQUIP_ITEM_ENCHANT_REQ.consumeEquipItemUIDList = new List<long>(m_listCurrentMaterialEquipsToEnhance);
					NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_ITEM_ENCHANT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
					break;
				}
				}
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_ENCHANT_NEED_CONSUME);
			}
		}
		else
		{
			if (!m_tglMisc.IsSelected)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < m_lstMaterials.Count; i++)
			{
				if (m_lstMaterials[i].count > 0)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				NKM_ERROR_CODE nKM_ERROR_CODE2 = NKMItemManager.CanEnchantItem(NKCScenManager.GetScenManager().GetMyUserData(), m_LeftEquipUID, m_lstMaterials);
				switch (nKM_ERROR_CODE2)
				{
				case NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT:
					NKCShopManager.OpenItemLackPopup(1, m_needCredit);
					break;
				default:
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE2));
					break;
				case NKM_ERROR_CODE.NEC_OK:
					NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_ENCHANT_USING_MISC_ITEM_REQ(m_LeftEquipUID, m_lstMaterials);
					break;
				}
			}
		}
	}

	public void CalcExp()
	{
		if (m_cNKMEquipItemDataTarget == null)
		{
			return;
		}
		m_expectedLevel = m_cNKMEquipItemDataTarget.m_EnchantLevel;
		m_expectedExp = m_cNKMEquipItemDataTarget.m_EnchantExp;
		int num = 0;
		if (m_tglEquip.IsSelected)
		{
			for (int i = 0; i < m_listCurrentMaterialEquipsToEnhance.Count; i++)
			{
				NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_listCurrentMaterialEquipsToEnhance[i]);
				if (itemEquip != null && NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID) != null)
				{
					int equipEnchantFeedExp = NKMItemManager.GetEquipEnchantFeedExp(itemEquip);
					if (equipEnchantFeedExp != -1)
					{
						num += equipEnchantFeedExp;
					}
				}
			}
		}
		else if (m_tglMisc.IsSelected)
		{
			for (int j = 0; j < m_lstMaterials.Count; j++)
			{
				if (m_lstMaterials[j].count > 0)
				{
					num += NKMCommonConst.EquipEnchantMiscConst.Materials[j].Exp * m_lstMaterials[j].count;
				}
			}
		}
		m_expectedExp += num;
		m_needCredit = num * 8;
		NKCCompanyBuff.SetDiscountOfCreditInEnchantTuning(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref m_needCredit);
		m_creditSlot.SetData(1, m_needCredit, NKCScenManager.CurrentUserData().GetCredit(), bShowTooltip: true, bShowBG: true, NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT));
		int enchantRequireExp = NKMItemManager.GetEnchantRequireExp(m_cNKMEquipItemDataTarget);
		int num2 = m_expectedExp;
		m_bMaxLevel = false;
		while (enchantRequireExp <= num2)
		{
			if (m_expectedLevel >= NKMItemManager.GetMaxEquipEnchantLevel(m_cNKMEquipTempletTarget.m_NKM_ITEM_TIER))
			{
				m_bMaxLevel = true;
				break;
			}
			m_expectedLevel++;
			num2 -= enchantRequireExp;
			enchantRequireExp = NKMItemManager.GetEnchantRequireExp(m_cNKMEquipTempletTarget.m_NKM_ITEM_TIER, m_expectedLevel, m_cNKMEquipTempletTarget.m_NKM_ITEM_GRADE);
		}
		m_expectedExp = num2;
		if (m_expectedLevel > m_cNKMEquipItemDataTarget.m_EnchantLevel)
		{
			m_currentExpPercent = 0f;
			m_expectedExpPercent = (float)m_expectedExp / (float)enchantRequireExp;
		}
		else
		{
			m_currentExpPercent = (float)m_cNKMEquipItemDataTarget.m_EnchantExp / (float)enchantRequireExp;
			m_expectedExpPercent = (float)(m_expectedExp - m_cNKMEquipItemDataTarget.m_EnchantExp) / (float)(enchantRequireExp - m_cNKMEquipItemDataTarget.m_EnchantExp);
		}
	}

	public void UpdateExpUI()
	{
		m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_NEW_Slider.value = 0f;
		m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_Slider.value = 0f;
		m_cNKMEquipItemDataTarget = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (m_cNKMEquipItemDataTarget == null)
		{
			return;
		}
		m_cNKMEquipTempletTarget = NKMItemManager.GetEquipTemplet(m_cNKMEquipItemDataTarget.m_ItemEquipID);
		if (m_cNKMEquipTempletTarget == null)
		{
			return;
		}
		m_needCredit = 0;
		NKCUtil.SetLabelText(m_ENCHANT_BEFORE_TEXT, string.Format(NKCUtilString.GET_STRING_FORGE_ENCHANT_LEVEL_ONE_PARAM, m_cNKMEquipItemDataTarget.m_EnchantLevel));
		NKCUtil.SetGameobjectActive(m_ENCHANT_ARROW, m_expectedLevel > m_cNKMEquipItemDataTarget.m_EnchantLevel);
		NKCUtil.SetGameobjectActive(m_ENCHANT_AFTER_TEXT, m_expectedLevel > m_cNKMEquipItemDataTarget.m_EnchantLevel);
		NKCUtil.SetLabelText(m_ENCHANT_AFTER_TEXT, "");
		if (m_expectedLevel >= NKMItemManager.GetMaxEquipEnchantLevel(m_cNKMEquipTempletTarget.m_NKM_ITEM_TIER))
		{
			m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_NEW_Slider.value = 0f;
			m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_Slider.value = 0f;
			if (m_expectedExp > m_cNKMEquipItemDataTarget.m_EnchantExp || m_bMaxLevel)
			{
				NKCUtil.SetLabelText(m_ENCHANT_AFTER_TEXT, $"+{m_expectedLevel}");
			}
		}
		else if (m_expectedLevel > m_cNKMEquipItemDataTarget.m_EnchantLevel)
		{
			NKCUtil.SetLabelText(m_ENCHANT_AFTER_TEXT, $"+{m_expectedLevel}");
			m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_Slider.value = 0f;
			m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_NEW_Slider.value = m_expectedExpPercent;
		}
		else
		{
			m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_Slider.value = m_currentExpPercent;
			m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_NEW_Slider.value = m_expectedExpPercent;
		}
		if (m_bMaxLevel)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_ENCHANT_EXP_BG_TEXT, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_ENCHANT_EXP_BG_TEXT, bValue: true);
			if (m_expectedExp > m_cNKMEquipItemDataTarget.m_EnchantExp)
			{
				NKCUtil.SetLabelText(m_NKM_UI_FACTORY_ENCHANT_EXP_BG_TEXT, $"<color=#FFDB00>{m_expectedExp}</color>/{NKMItemManager.GetEnchantRequireExp(m_cNKMEquipItemDataTarget)}");
			}
			else
			{
				NKCUtil.SetLabelText(m_NKM_UI_FACTORY_ENCHANT_EXP_BG_TEXT, $"{m_expectedExp}/{NKMItemManager.GetEnchantRequireExp(m_cNKMEquipItemDataTarget)}");
			}
		}
		float x = m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER.GetComponent<RectTransform>().anchoredPosition.x;
		float width = m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER.GetComponent<RectTransform>().GetWidth();
		float value = m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_Slider.value;
		float x2 = x + width * value;
		float newSize = m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER.GetComponent<RectTransform>().GetWidth() * (1f - m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_Slider.value);
		Vector2 anchoredPosition = m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_NEW.GetComponent<RectTransform>().anchoredPosition;
		anchoredPosition.x = x2;
		m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_NEW.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
		m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_NEW.GetComponent<RectTransform>().SetWidth(newSize);
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01_Before, "");
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_02_Before, "");
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01_After, "");
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_02_After, "");
		for (int i = 0; i < m_cNKMEquipItemDataTarget.m_Stat.Count; i++)
		{
			EQUIP_ITEM_STAT eQUIP_ITEM_STAT = m_cNKMEquipItemDataTarget.m_Stat[i];
			if (eQUIP_ITEM_STAT != null && i == 0)
			{
				SetBeforeStatTextAndSprite(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01_Before, eQUIP_ITEM_STAT, m_cNKMEquipItemDataTarget);
				SetAfterStatTextAndSprite(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01_After, m_expectedLevel, eQUIP_ITEM_STAT, m_cNKMEquipItemDataTarget);
			}
		}
		EnableUI(bActive: true);
	}

	private void SetUIItemReq()
	{
		if (m_lstUIItemReq == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_csbtnMaterial_1_Minus, m_lstMaterials[0].count > 0);
		NKCUtil.SetGameobjectActive(m_csbtnMaterial_2_Minus, m_lstMaterials[1].count > 0);
		NKCUtil.SetGameobjectActive(m_csbtnMaterial_3_Minus, m_lstMaterials[2].count > 0);
		NKCUtil.SetGameobjectActive(m_csbtnMaterial_4_Minus, m_lstMaterials[3].count > 0);
		for (int i = 0; i < m_lstUIItemReq.Count; i++)
		{
			if (m_lstMaterials[i].count > 0 && m_LeftEquipUID > 0)
			{
				NKCUtil.SetLabelText(m_lstMaterialUseCount[i], m_lstMaterials[i].count.ToString("#,##0"));
			}
			else
			{
				NKCUtil.SetLabelText(m_lstMaterialUseCount[i], "");
			}
			m_lstUIItemReq[i].SetData(m_lstMaterials[i].itemId, 0, 0L, bShowTooltip: false, bShowBG: false);
			NKCUtil.SetLabelText(m_lstMaterialHaveCount[i], NKCUtilString.GET_STRING_HAVE_COUNT_ONE_PARAM, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_lstMaterials[i].itemId).ToString("#,##0"));
		}
	}

	public void ResetMaterialEquipSlotsToEnhanceUI()
	{
		if (m_listNKCUIEnhanceSlot != null)
		{
			for (int i = 0; i < m_listNKCUIEnhanceSlot.Count; i++)
			{
				if (m_listNKCUIEnhanceSlot[i] != null)
				{
					if (i < m_listCurrentMaterialEquipsToEnhance.Count)
					{
						NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeEquipData(NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_listCurrentMaterialEquipsToEnhance[i]));
						m_listNKCUIEnhanceSlot[i].SetData(data, bShowName: false, bShowNumber: true, bEnableLayoutElement: true, OnClickMaterialEquipToEnhance);
					}
					else
					{
						m_listNKCUIEnhanceSlot[i].SetEmptyMaterial(OnClickEmptyMaterialEquipToEnhance);
					}
				}
			}
		}
		SetUIItemReq();
	}

	public void OnClickMaterialEquipToEnhance(NKCUISlot.SlotData slotData, bool bLocked)
	{
		RemoveCurrentMaterialEquipToEnhance(slotData.UID);
		ResetMaterialEquipSlotsToEnhanceUI();
		CalcExp();
		UpdateExpUI();
		SetUIItemReq();
	}

	private void OnClickEmptyMaterialEquipToEnhance(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (m_LeftEquipUID <= 0)
		{
			return;
		}
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip == null)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
		if (equipTemplet != null && itemEquip.m_EnchantLevel >= NKMItemManager.GetMaxEquipEnchantLevel(equipTemplet.m_NKM_ITEM_TIER))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_ENCHANT_ALREADY_MAX);
			return;
		}
		NKCUIInventory.EquipSelectListOptions options = new NKCUIInventory.EquipSelectListOptions(NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL, _bMultipleSelect: false);
		options.m_NKC_INVENTORY_OPEN_TYPE = NKC_INVENTORY_OPEN_TYPE.NIOT_EQUIP_SELECT;
		options.m_dOnSelectedEquipSlot = null;
		options.lstSortOption = NKCEquipSortSystem.FORGE_MATERIAL_SORT_LIST;
		HashSet<long> equipsBeingUsed = NKCUtil.GetEquipsBeingUsed(NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.m_dicMyUnit, NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData);
		if (!equipsBeingUsed.Contains(m_LeftEquipUID))
		{
			equipsBeingUsed.Add(m_LeftEquipUID);
		}
		options.m_hsSelectedEquipUIDToShow = GetCurrentMaterialEquipsHashSetToEnhance();
		options.setExcludeEquipUID = equipsBeingUsed;
		options.bHideLockItem = true;
		options.bHideMaxLvItem = false;
		options.bLockMaxItem = false;
		options.bMultipleSelect = true;
		options.iMaxMultipleSelect = 10;
		options.m_dOnFinishMultiSelection = OnFinishMultiSelectionToEnhance;
		options.bSkipItemEquipBox = true;
		if (m_UIInventory == null)
		{
			m_UIInventory = NKCUIInventory.OpenNewInstance();
		}
		options.strEmptyMessage = NKCUtilString.GET_STRING_FORGE_ENCHANT_NO_EXIST_CONSUME;
		options.m_ButtonMenuType = NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_OK;
		m_UIInventory?.Open(options, null, 0L);
	}

	private HashSet<long> GetCurrentMaterialEquipsHashSetToEnhance()
	{
		return new HashSet<long>(m_listCurrentMaterialEquipsToEnhance);
	}

	public void Close()
	{
		m_LeftEquipUID = 0L;
		ClearAllUI();
		OnCloseInstance();
	}

	public void OnCloseInstance()
	{
		if (m_UIInventory != null && m_UIInventory.IsOpen)
		{
			m_UIInventory.Close();
		}
		m_UIInventory = null;
	}

	public void ClearAllUI()
	{
		m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_NEW_Slider.value = 0f;
		m_NKM_UI_FACTORY_ENCHANT_EXP_SLIDER_Slider.value = 0f;
		NKCUtil.SetLabelText(m_ENCHANT_BEFORE_TEXT, string.Format(NKCUtilString.GET_STRING_FORGE_ENCHANT_LEVEL_ONE_PARAM, 0));
		NKCUtil.SetLabelText(m_NKM_UI_FACTORY_ENCHANT_EXP_BG_TEXT, "");
		m_ENCHANT_ARROW.SetActive(value: false);
		NKCUtil.SetLabelText(m_ENCHANT_AFTER_TEXT, "");
		m_creditSlot.SetData(0, 0, 0L);
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01_Before, "0");
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01_After, "0");
		foreach (NKCUISlot item in m_listNKCUIEnhanceSlot)
		{
			item.SetEmptyMaterial();
		}
		EnableUI(bActive: false);
	}

	private void EnableUI(bool bActive)
	{
		if (!bActive)
		{
			m_Img_NKM_UI_FACTORY_ENCHANT_BUTTON_RESET.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
			m_Img_NKM_UI_FACTORY_ENCHANT_BUTTON_ENCHANT.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
		}
		else
		{
			m_Img_NKM_UI_FACTORY_ENCHANT_BUTTON_RESET.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_BLUE);
			m_Img_NKM_UI_FACTORY_ENCHANT_BUTTON_ENCHANT.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW);
		}
		Color col = (bActive ? NKCUtil.GetColor("#FFFFFF") : NKCUtil.GetColor("#222222"));
		NKCUtil.SetLabelTextColor(m_txt_NKM_UI_FACTORY_ENCHANT_RESET_TEXT, col);
		m_img_NKM_UI_FACTORY_ENCHANT_ENCHANT_ICON.color = NKCUtil.GetUITextColor(bActive);
		m_txt_NKM_UI_FACTORY_ENCHANT_ENCHANT_TEXT.color = NKCUtil.GetUITextColor(bActive);
		m_NKM_UI_FACTORY_ENCHANT_BUTTON_ENCHANT_LIGHT.SetActive(bActive);
		NKCUtil.SetGameobjectActive(m_btn_NKM_UI_FACTORY_ENCHANT_AUTOSELECT.gameObject, bActive);
	}

	private void OnTglEquip(bool bValue)
	{
		if (bValue)
		{
			m_curEnchantType = FORGE_ENCHANT_TYPE.EQUIP;
			ResetMaterialEquipSlotsToEnhance();
			NKCUtil.SetGameobjectActive(m_objEquip, bValue: true);
			NKCUtil.SetGameobjectActive(m_objMisc, bValue: false);
		}
	}

	private void OnTglMisc(bool bValue)
	{
		if (bValue)
		{
			m_curEnchantType = FORGE_ENCHANT_TYPE.MISC;
			ResetMaterialEquipSlotsToEnhance();
			NKCUtil.SetGameobjectActive(m_objEquip, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMisc, bValue: true);
		}
	}

	private void Update()
	{
		if (m_tglMisc.IsSelected)
		{
			OnUpdateButtonHold();
		}
	}

	private void OnMinusDown_1(PointerEventData eventData)
	{
		targetId = NKMCommonConst.EquipEnchantMiscConst.Materials[0].ItemId;
		m_iChangeValue = -1;
		m_bPress = true;
		m_fDelay = 0.3f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
		m_bMaxLevel = false;
	}

	private void OnPlusDown_1(PointerEventData eventData)
	{
		targetId = NKMCommonConst.EquipEnchantMiscConst.Materials[0].ItemId;
		m_iChangeValue = 1;
		m_bPress = true;
		m_fDelay = 0.3f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
	}

	private void OnMinusDown_2(PointerEventData eventData)
	{
		targetId = NKMCommonConst.EquipEnchantMiscConst.Materials[1].ItemId;
		m_iChangeValue = -1;
		m_bPress = true;
		m_fDelay = 0.3f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
		m_bMaxLevel = false;
	}

	private void OnPlusDown_2(PointerEventData eventData)
	{
		targetId = NKMCommonConst.EquipEnchantMiscConst.Materials[1].ItemId;
		m_iChangeValue = 1;
		m_bPress = true;
		m_fDelay = 0.3f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
	}

	private void OnMinusDown_3(PointerEventData eventData)
	{
		targetId = NKMCommonConst.EquipEnchantMiscConst.Materials[2].ItemId;
		m_iChangeValue = -1;
		m_bPress = true;
		m_fDelay = 0.3f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
		m_bMaxLevel = false;
	}

	private void OnPlusDown_3(PointerEventData eventData)
	{
		targetId = NKMCommonConst.EquipEnchantMiscConst.Materials[2].ItemId;
		m_iChangeValue = 1;
		m_bPress = true;
		m_fDelay = 0.3f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
	}

	private void OnMinusDown_4(PointerEventData eventData)
	{
		targetId = NKMCommonConst.EquipEnchantMiscConst.Materials[3].ItemId;
		m_iChangeValue = -1;
		m_bPress = true;
		m_fDelay = 0.3f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
		m_bMaxLevel = false;
	}

	private void OnPlusDown_4(PointerEventData eventData)
	{
		targetId = NKMCommonConst.EquipEnchantMiscConst.Materials[3].ItemId;
		m_iChangeValue = 1;
		m_bPress = true;
		m_fDelay = 0.3f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
	}

	private void OnButtonUp()
	{
		m_iChangeValue = 0;
		m_fDelay = 0.3f;
		m_bPress = false;
	}

	public void OnUpdateButtonHold()
	{
		if (!m_bPress || m_LeftEquipUID <= 0 || (m_bMaxLevel && m_iChangeValue > 0))
		{
			return;
		}
		m_fHoldTime += Time.deltaTime;
		if (m_fHoldTime >= m_fDelay)
		{
			m_fHoldTime = 0f;
			m_fDelay *= 0.8f;
			int num = ((!(m_fDelay < 0.01f)) ? 1 : 5);
			m_fDelay = Mathf.Clamp(m_fDelay, 0.01f, 0.3f);
			MiscItemData miscItemData = m_lstMaterials.Find((MiscItemData e) => e.itemId == targetId);
			if (miscItemData == null)
			{
				return;
			}
			m_bWasHold = true;
			for (int num2 = 0; num2 < num; num2++)
			{
				if (!m_bPress)
				{
					break;
				}
				if (m_bMaxLevel)
				{
					break;
				}
				miscItemData.count += m_iChangeValue;
				if (m_iChangeValue < 0 && miscItemData.count < 0)
				{
					miscItemData.count = 0;
					m_bPress = false;
				}
				if (m_iChangeValue > 0)
				{
					int notSelectedTotalCount = 0;
					m_lstMaterials.ForEach(delegate(MiscItemData x)
					{
						if (x.itemId != targetId)
						{
							notSelectedTotalCount += x.count;
						}
					});
					int val = Math.Max(0, NKMCommonConst.EquipEnchantMiscConst.MaxMaterialUsageLimit - notSelectedTotalCount);
					val = Math.Min(val, (int)NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(miscItemData.itemId));
					if (miscItemData.count >= val)
					{
						miscItemData.count = val;
						m_bPress = false;
					}
				}
				CalcExp();
			}
		}
		UpdateExpUI();
		SetUIItemReq();
	}

	public void OnChangeCount(int targetItemId, bool bPlus = true)
	{
		targetId = targetItemId;
		if (m_bWasHold)
		{
			m_bWasHold = false;
		}
		else
		{
			if (m_LeftEquipUID <= 0 || (NKCUIUnitInfo.IsInstanceOpen && NKCUIUnitInfo.Instance.IsBlockedUnit()))
			{
				return;
			}
			if (m_bMaxLevel)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_FORGE_ENCHANT_ALREADY_MAX);
				return;
			}
			MiscItemData miscItemData = m_lstMaterials.Find((MiscItemData e) => e.itemId == targetId);
			if (miscItemData == null)
			{
				return;
			}
			if (bPlus)
			{
				int notSelectedTotalCount = 0;
				m_lstMaterials.ForEach(delegate(MiscItemData x)
				{
					if (x.itemId != targetId)
					{
						notSelectedTotalCount += x.count;
					}
				});
				int val = Math.Max(0, NKMCommonConst.EquipEnchantMiscConst.MaxMaterialUsageLimit - notSelectedTotalCount);
				val = Math.Min(val, (int)NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(miscItemData.itemId));
				if (miscItemData.count >= val)
				{
					if (miscItemData.count + notSelectedTotalCount >= NKMCommonConst.EquipEnchantMiscConst.MaxMaterialUsageLimit)
					{
						NKCPopupMessageManager.AddPopupMessage(NKM_ERROR_CODE.NEC_FAIL_NEGOTIATION_INVALID_MATERIAL_COUNT);
					}
					else
					{
						NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_NOT_ENOUGH_NEGOTIATE_MATERIALS);
					}
					return;
				}
			}
			else
			{
				int num = 0;
				if (miscItemData.count <= num)
				{
					return;
				}
			}
			miscItemData.count += (bPlus ? 1 : (-1));
			CalcExp();
			UpdateExpUI();
			SetUIItemReq();
		}
	}

	private void OnClickEnchantTo2()
	{
		m_targetLevel = 2;
		AutoSelectMiscItem(m_targetLevel);
	}

	private void OnClickEnchantTo5()
	{
		m_targetLevel = 5;
		AutoSelectMiscItem(m_targetLevel);
	}

	private void OnClickEnchantTo7()
	{
		m_targetLevel = 7;
		AutoSelectMiscItem(m_targetLevel);
	}

	private void OnClickEnchantToMax()
	{
		m_targetLevel = 10;
		AutoSelectMiscItem(m_targetLevel);
	}

	private void AutoSelectMiscItem(int targetLevel)
	{
		if (m_LeftEquipUID <= 0)
		{
			m_targetLevel = 0;
			return;
		}
		m_listCurrentMaterialEquipsToEnhance.Clear();
		for (int i = 0; i < m_lstMaterials.Count; i++)
		{
			m_lstMaterials[i].count = 0;
		}
		if (m_cNKMEquipItemDataTarget.m_EnchantLevel >= targetLevel)
		{
			CalcExp();
			UpdateExpUI();
			SetUIItemReq();
			return;
		}
		int maxEquipEnchantLevel = NKMItemManager.GetMaxEquipEnchantLevel(m_cNKMEquipTempletTarget.m_NKM_ITEM_TIER);
		if (targetLevel >= maxEquipEnchantLevel)
		{
			targetLevel = maxEquipEnchantLevel;
		}
		int j = m_cNKMEquipItemDataTarget.m_EnchantLevel;
		int num = -m_cNKMEquipItemDataTarget.m_EnchantExp;
		for (; j < targetLevel; j++)
		{
			num += NKMItemManager.GetEnchantRequireExp(m_cNKMEquipTempletTarget.m_NKM_ITEM_TIER, j, m_cNKMEquipTempletTarget.m_NKM_ITEM_GRADE);
		}
		for (int num2 = NKMCommonConst.EquipEnchantMiscConst.Materials.Count - 1; num2 >= 0; num2--)
		{
			int num3 = num / NKMCommonConst.EquipEnchantMiscConst.Materials[num2].Exp;
			long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(NKMCommonConst.EquipEnchantMiscConst.Materials[num2].ItemId);
			if (countMiscItem < num3)
			{
				num3 = (int)countMiscItem;
			}
			m_lstMaterials[num2].count = num3;
			num -= num3 * NKMCommonConst.EquipEnchantMiscConst.Materials[num2].Exp;
		}
		if (num > 0)
		{
			for (int k = 0; k < NKMCommonConst.EquipEnchantMiscConst.Materials.Count; k++)
			{
				long countMiscItem2 = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(NKMCommonConst.EquipEnchantMiscConst.Materials[k].ItemId);
				long num4 = num / NKMCommonConst.EquipEnchantMiscConst.Materials[k].Exp + 1;
				if (countMiscItem2 >= m_lstMaterials[k].count + num4)
				{
					m_lstMaterials[k].count += num / NKMCommonConst.EquipEnchantMiscConst.Materials[k].Exp + 1;
					break;
				}
			}
		}
		CalcExp();
		UpdateExpUI();
		SetUIItemReq();
		if (num > 0 && m_targetLevel > m_expectedLevel)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_EQUIP_ENCHANT_NOT_ENOUGH_MODULE, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
	}
}
