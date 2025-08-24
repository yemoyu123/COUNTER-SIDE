using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIInvenEquipSlot : MonoBehaviour
{
	public enum EQUIP_SLOT_STATE
	{
		ESS_NONE,
		ESS_SELECTED,
		ESS_DELETE
	}

	private enum eOptionType
	{
		MainStat,
		ModifiableStat,
		ExclusiveStat,
		RelicStat
	}

	public Image m_NKM_UI_ITEM_EQUIP_SLOT_BG;

	public Image m_NKM_UI_ITEM_EQUIP_SLOT_ICON;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_NAME;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_TIER_TEXT;

	[Space]
	public Text m_lbEquipType;

	public Text m_lbEquipReqUnit;

	public Image m_imgGrade;

	public GameObject m_objTier_7;

	[Space]
	public GameObject m_NKM_UI_ITEM_EQUIP_SLOT_REINFORCE;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_REINFORCE_TEXT;

	[Space]
	public GameObject m_ObjStat_01;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01;

	private static Color m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01_ORG_Color = new Color(0.7058824f, 0.7058824f, 0.7058824f, 1f);

	public GameObject NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_01_SELECT;

	public GameObject m_ObjStat_RELIC;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_RELIC;

	private static Color m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_RELIC_ORG_Color = new Color(0.7058824f, 0.7058824f, 0.7058824f, 1f);

	public GameObject NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_RELIC_SELECT;

	public GameObject m_objStat_RELIC_2;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_RELIC_2;

	public GameObject NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_RELIC_SELECT_2;

	[Space]
	public GameObject m_ObjStat_02;

	public NKCUIComStateButton m_btnStat_02;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_02;

	private static Color m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_02_ORG_Color = new Color(0.7058824f, 0.7058824f, 0.7058824f, 1f);

	public Image m_imgStat_02_Num;

	public Image m_imgStat_02_Lock;

	public GameObject NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_02_SELECT;

	[Space]
	public GameObject m_ObjStat_03;

	public NKCUIComStateButton m_btnStat_03;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_03;

	private static Color m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_03_ORG_Color = new Color(0.7058824f, 0.7058824f, 0.7058824f, 1f);

	public Image m_imgStat_03_Num;

	public GameObject NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_03_SELECT;

	[Header("렐릭")]
	public GameObject m_objRelic;

	public List<Image> m_lstRelicImg = new List<Image>();

	public GameObject[] m_objSocketSymbol;

	[Header("장착중 유닛")]
	public GameObject m_NKM_UI_ITEM_EQUIP_SLOT_USED;

	public Image m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_USED_UNIT;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_USED_UNIT_DESC;

	[Header("세트옵션")]
	public GameObject NKM_UI_ITEM_EQUIP_SLOT_BOTTOM_SET_INFO;

	public Image m_NKM_UI_ITEM_EQUIP_SLOT_BOTTOM_SET_ICON;

	public Text m_NKM_UI_ITEM_EQUIP_SLOT_BOTTOM_SET_NAME;

	public NKCUIComStateButton m_btnSetSlot;

	public Image m_SET_TEXT01_DOT;

	public Text m_SET_TEXT01_TEXT;

	public Image m_SET_TEXT02_DOT;

	public Text m_SET_TEXT02_TEXT;

	[Header("장비 설명")]
	public Text m_lbEquipDesc;

	[Header("스크롤")]
	public ScrollRect m_srDetail;

	[Space]
	public GameObject m_NKM_UI_ITEM_EQUIP_SLOT_NOT_EMPTY;

	public GameObject m_NKM_UI_ITEM_EQUIP_SLOT_LOCK;

	public GameObject m_objDetail;

	[Header("유닛 리액터")]
	public GameObject m_objReactorBG;

	public GameObject m_objReactorDetail;

	public List<NKCUIReactorSkillInfo> m_lstReactor;

	private NKMEquipItemData m_cNKMEquipItemData;

	private NKMEquipTemplet m_cNKMEquipTemplet;

	private EQUIP_SLOT_STATE m_EQUIP_SLOT_STATE;

	public NKMEquipItemData GetNKMEquipItemData()
	{
		return m_cNKMEquipItemData;
	}

	public NKMEquipTemplet GetNKMEquipTemplet()
	{
		return m_cNKMEquipTemplet;
	}

	public EQUIP_SLOT_STATE Get_EQUIP_SLOT_STATE()
	{
		return m_EQUIP_SLOT_STATE;
	}

	public bool IsActive()
	{
		return base.gameObject.activeInHierarchy;
	}

	public void SetActive(bool bSet)
	{
		if (base.gameObject.activeSelf == !bSet)
		{
			base.gameObject.SetActive(bSet);
		}
	}

	private string GetBGIconName(NKM_ITEM_GRADE eNKM_ITEM_GRADE)
	{
		return eNKM_ITEM_GRADE switch
		{
			NKM_ITEM_GRADE.NIG_N => "AB_UI_ITEM_EQUIP_SLOT_CARD_N", 
			NKM_ITEM_GRADE.NIG_R => "AB_UI_ITEM_EQUIP_SLOT_CARD_R", 
			NKM_ITEM_GRADE.NIG_SR => "AB_UI_ITEM_EQUIP_SLOT_CARD_SR", 
			NKM_ITEM_GRADE.NIG_SSR => "AB_UI_ITEM_EQUIP_SLOT_CARD_SSR", 
			_ => "", 
		};
	}

	private string GetGradeIconName(NKM_ITEM_GRADE grade)
	{
		return grade switch
		{
			NKM_ITEM_GRADE.NIG_N => "AB_UI_ITEM_EQUIP_SLOT_N", 
			NKM_ITEM_GRADE.NIG_R => "AB_UI_ITEM_EQUIP_SLOT_R", 
			NKM_ITEM_GRADE.NIG_SR => "AB_UI_ITEM_EQUIP_SLOT_SR", 
			NKM_ITEM_GRADE.NIG_SSR => "AB_UI_ITEM_EQUIP_SLOT_SSR", 
			_ => "", 
		};
	}

	public void SetHighlightOnlyOneStatColor(int index)
	{
		NKCUtil.SetGameobjectActive(NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_01_SELECT, index == 0);
		NKCUtil.SetGameobjectActive(NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_02_SELECT, index == 1);
		NKCUtil.SetGameobjectActive(NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_03_SELECT, index == 2);
		NKCUtil.SetGameobjectActive(NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_RELIC_SELECT, index == 3);
		NKCUtil.SetGameobjectActive(NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_RELIC_SELECT_2, index == 3);
	}

	public void SetEmpty(NKCUISlotEquip.OnSelectedEquipSlot selectedSlot = null, NKMEquipItemData cNKMEquipItemData = null)
	{
		if (cNKMEquipItemData != null)
		{
			m_cNKMEquipItemData = cNKMEquipItemData;
		}
	}

	public void SetData(NKMEquipItemData cNKMEquipItemData, bool bShowFierceInfo = false, bool bPresetContained = false)
	{
		if (cNKMEquipItemData == null)
		{
			return;
		}
		m_cNKMEquipItemData = cNKMEquipItemData;
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(cNKMEquipItemData.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return;
		}
		m_btnStat_02.PointerClick.RemoveAllListeners();
		m_btnStat_02.PointerClick.AddListener(OnClickStat_02);
		m_btnStat_03.PointerClick.RemoveAllListeners();
		m_btnStat_03.PointerClick.AddListener(OnClickStat_03);
		m_btnSetSlot.PointerClick.RemoveAllListeners();
		m_btnSetSlot.PointerClick.AddListener(OnClickSetOption);
		NKCUtil.SetGameobjectActive(m_NKM_UI_ITEM_EQUIP_SLOT_NOT_EMPTY, bValue: true);
		m_cNKMEquipTemplet = equipTemplet;
		m_NKM_UI_ITEM_EQUIP_SLOT_BG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_ITEM_EQUIP_SLOT_CARD_SPRITE", GetBGIconName(equipTemplet.m_NKM_ITEM_GRADE));
		NKCUtil.SetImageSprite(m_imgGrade, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_ITEM_EQUIP_SLOT_CARD_SPRITE", GetGradeIconName(equipTemplet.m_NKM_ITEM_GRADE)));
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_NAME, equipTemplet.GetItemName());
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_TIER_TEXT, NKCUtilString.GetItemEquipTier(equipTemplet.m_NKM_ITEM_TIER));
		NKCUtil.SetGameobjectActive(m_objTier_7, equipTemplet.m_bShowEffect);
		NKCUtil.SetGameobjectActive(m_objDetail, bValue: true);
		NKCUtil.SetGameobjectActive(m_objReactorDetail, bValue: false);
		NKCUtil.SetGameobjectActive(m_objReactorBG, bValue: false);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(equipTemplet.GetPrivateUnitID());
		if (unitTempletBase != null)
		{
			NKCUtil.SetLabelText(m_lbEquipReqUnit, "- " + NKCUtilString.GetEquipPositionStringByUnitStyle(equipTemplet));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEquipReqUnit, "");
		}
		NKCUtil.SetLabelText(m_lbEquipType, "- " + NKCUtilString.GetEquipPositionStringByUnitStyle(equipTemplet, unitTempletBase != null));
		if (cNKMEquipItemData.m_EnchantLevel > 0)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_ITEM_EQUIP_SLOT_REINFORCE, bValue: true);
			m_NKM_UI_ITEM_EQUIP_SLOT_REINFORCE_TEXT.text = "+" + cNKMEquipItemData.m_EnchantLevel;
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_ITEM_EQUIP_SLOT_REINFORCE, bValue: false);
			m_NKM_UI_ITEM_EQUIP_SLOT_REINFORCE_TEXT.text = "";
		}
		NKCUtil.SetImageSprite(m_NKM_UI_ITEM_EQUIP_SLOT_ICON, NKCResourceUtility.GetOrLoadEquipIcon(equipTemplet));
		NKCUtil.SetGameobjectActive(m_objRelic, equipTemplet.IsRelic());
		if (equipTemplet.IsRelic())
		{
			for (int i = 0; i < m_lstRelicImg.Count; i++)
			{
				if (cNKMEquipItemData.potentialOptions.Count <= 0 || cNKMEquipItemData.potentialOptions[0] == null || i >= cNKMEquipItemData.potentialOptions[0].sockets.Length)
				{
					NKCUtil.SetGameobjectActive(m_lstRelicImg[i], bValue: false);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstRelicImg[i], cNKMEquipItemData.potentialOptions[0].sockets[i] != null);
				}
			}
		}
		NKCUtil.SetGameobjectActive(NKM_UI_ITEM_EQUIP_SLOT_BOTTOM_SET_INFO, equipTemplet.m_EquipUnitStyleType != NKM_UNIT_STYLE_TYPE.NUST_ENCHANT);
		if (NKM_UI_ITEM_EQUIP_SLOT_BOTTOM_SET_INFO.gameObject.activeSelf)
		{
			NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(cNKMEquipItemData.m_SetOptionId);
			if (equipSetOptionTemplet != null)
			{
				NKCUtil.SetGameobjectActive(m_SET_TEXT01_DOT, bValue: true);
				NKCUtil.SetGameobjectActive(m_SET_TEXT01_TEXT, bValue: true);
				NKCUtil.SetImageSprite(m_NKM_UI_ITEM_EQUIP_SLOT_BOTTOM_SET_ICON, NKCUtil.GetSpriteEquipSetOptionIcon(equipSetOptionTemplet));
				int num = 0;
				if (cNKMEquipItemData.m_OwnerUnitUID > 0)
				{
					num = NKMItemManager.GetMatchingSetOptionItem(cNKMEquipItemData);
				}
				NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_BOTTOM_SET_NAME, $"{NKCStringTable.GetString(equipSetOptionTemplet.m_EquipSetName)} ({num}/{equipSetOptionTemplet.m_EquipSetPart})");
				string setOptionDescription = NKMItemManager.GetSetOptionDescription(equipSetOptionTemplet.m_StatType_1, equipSetOptionTemplet.m_StatValue_1);
				NKCUtil.SetLabelText(m_SET_TEXT01_TEXT, setOptionDescription);
				if (equipSetOptionTemplet.m_StatType_2 != NKM_STAT_TYPE.NST_RANDOM)
				{
					string setOptionDescription2 = NKMItemManager.GetSetOptionDescription(equipSetOptionTemplet.m_StatType_2, equipSetOptionTemplet.m_StatValue_2);
					NKCUtil.SetLabelText(m_SET_TEXT02_TEXT, setOptionDescription2);
				}
				Color color = ((!NKMItemManager.IsActiveSetOptionItem(cNKMEquipItemData)) ? NKCUtil.GetColor("#656565") : NKCUtil.GetColor("#FFFFFF"));
				NKCUtil.SetImageColor(m_SET_TEXT01_DOT, color);
				NKCUtil.SetImageColor(m_SET_TEXT02_DOT, color);
				NKCUtil.SetGameobjectActive(m_SET_TEXT02_DOT, equipSetOptionTemplet.m_StatType_2 != NKM_STAT_TYPE.NST_RANDOM);
				NKCUtil.SetGameobjectActive(m_SET_TEXT02_TEXT, equipSetOptionTemplet.m_StatType_2 != NKM_STAT_TYPE.NST_RANDOM);
			}
			else
			{
				NKCUtil.SetImageSprite(m_NKM_UI_ITEM_EQUIP_SLOT_BOTTOM_SET_ICON, NKCUtil.GetSpriteEquipSetOptionIcon(null));
				NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_BOTTOM_SET_NAME, NKCUtilString.GET_STRING_STAT_SHORT_NAME_FOR_INVEN_EQUIP_RANDOM_SET);
				NKCUtil.SetGameobjectActive(m_SET_TEXT01_DOT, bValue: false);
				NKCUtil.SetGameobjectActive(m_SET_TEXT02_DOT, bValue: false);
				NKCUtil.SetGameobjectActive(m_SET_TEXT01_TEXT, bValue: false);
				NKCUtil.SetGameobjectActive(m_SET_TEXT02_TEXT, bValue: false);
			}
		}
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01, "");
		NKCUtil.SetLabelTextColor(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01, m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01_ORG_Color);
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_RELIC, "");
		NKCUtil.SetLabelTextColor(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_RELIC, m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_RELIC_ORG_Color);
		NKCUtil.SetGameobjectActive(m_ObjStat_RELIC, m_cNKMEquipTemplet.IsRelic());
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_RELIC_2, "");
		NKCUtil.SetLabelTextColor(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_RELIC_2, m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_RELIC_ORG_Color);
		NKCUtil.SetGameobjectActive(m_objStat_RELIC_2, m_cNKMEquipTemplet.IsRelic() && m_cNKMEquipTemplet.potentialOptionGroupId2 > 0);
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_02, "");
		NKCUtil.SetLabelTextColor(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_02, m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_02_ORG_Color);
		NKCUtil.SetGameobjectActive(m_ObjStat_02, m_cNKMEquipTemplet.m_StatGroupID != 0);
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_03, "");
		NKCUtil.SetLabelTextColor(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_03, m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_02_ORG_Color);
		NKCUtil.SetGameobjectActive(m_ObjStat_03, m_cNKMEquipTemplet.m_StatGroupID_2 != 0);
		if (equipTemplet.m_EquipUnitStyleType == NKM_UNIT_STYLE_TYPE.NUST_ENCHANT)
		{
			NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01, NKCStringTable.GetString("SI_DP_STAT_SHORT_NAME_FOR_INVEN_EQUIP_ENCHANT"));
			SetColor(0, eOptionType.MainStat);
		}
		else
		{
			for (int j = 0; j < cNKMEquipItemData.m_Stat.Count; j++)
			{
				EQUIP_ITEM_STAT statData = cNKMEquipItemData.m_Stat[j];
				switch (j)
				{
				case 0:
					SetItemStatText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01, cNKMEquipItemData, statData, j);
					break;
				case 1:
					SetItemStatText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_02, cNKMEquipItemData, statData, j);
					break;
				case 2:
					SetItemStatText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_03, cNKMEquipItemData, statData, j);
					break;
				}
				int statGroupID;
				switch (j)
				{
				case 0:
					SetColor(j, eOptionType.MainStat);
					continue;
				default:
					statGroupID = m_cNKMEquipTemplet.m_StatGroupID_2;
					break;
				case 1:
					statGroupID = m_cNKMEquipTemplet.m_StatGroupID;
					break;
				}
				if (!NKMEquipTuningManager.IsChangeableStatGroup(statGroupID))
				{
					SetColor(j, eOptionType.ExclusiveStat);
				}
				else
				{
					SetColor(j, eOptionType.ModifiableStat);
				}
			}
			if (m_cNKMEquipTemplet.IsRelic())
			{
				NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_RELIC, NKCUtil.GetPotentialStatText(cNKMEquipItemData));
				NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_RELIC_2, NKCUtil.GetPotentialStatText(cNKMEquipItemData, 1));
				if (m_objSocketSymbol != null)
				{
					int num2 = m_objSocketSymbol.Length;
					for (int k = 0; k < num2; k++)
					{
						if (cNKMEquipItemData.potentialOptions.Count <= 0 || cNKMEquipItemData.potentialOptions[0] == null || cNKMEquipItemData.potentialOptions[0].sockets.Length <= k)
						{
							NKCUtil.SetGameobjectActive(m_objSocketSymbol[k], bValue: false);
						}
						else
						{
							NKCUtil.SetGameobjectActive(m_objSocketSymbol[k], cNKMEquipItemData.potentialOptions[0].sockets[k] != null);
						}
					}
				}
			}
		}
		bool flag = false;
		if (cNKMEquipItemData.m_OwnerUnitUID > 0)
		{
			NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(cNKMEquipItemData.m_OwnerUnitUID);
			if (unitFromUID != null)
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(unitFromUID.m_UnitID);
				if (unitTempletBase2 != null)
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_ITEM_EQUIP_SLOT_USED, bValue: true);
					flag = true;
					NKCUtil.SetImageSprite(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_USED_UNIT, NKCResourceUtility.GetOrLoadMinimapFaceIcon(unitTempletBase2));
					NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_USED_UNIT_DESC, unitTempletBase2.GetUnitName() + " " + NKCStringTable.GetString("SI_PF_FILTER_EQUIP_2"));
				}
			}
		}
		if (!flag)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_ITEM_EQUIP_SLOT_USED, bValue: false);
		}
		NKCUtil.SetLabelText(m_lbEquipDesc, equipTemplet.GetItemDesc());
		NKCUtil.SetGameobjectActive(m_NKM_UI_ITEM_EQUIP_SLOT_LOCK, m_cNKMEquipItemData.m_bLock);
		m_srDetail.normalizedPosition = Vector2.one;
	}

	public void SetData(NKMUnitReactorTemplet reactorTemplet, int reactorLv, bool bShowFierceInfo = false, bool bPresetContained = false)
	{
		if (reactorTemplet == null)
		{
			return;
		}
		NKMReactorSkillTemplet[] skillTemplets = reactorTemplet.skillTemplets;
		if (skillTemplets == null)
		{
			return;
		}
		m_cNKMEquipItemData = null;
		NKCUtil.SetGameobjectActive(m_NKM_UI_ITEM_EQUIP_SLOT_NOT_EMPTY, bValue: true);
		m_cNKMEquipTemplet = null;
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_NAME, reactorTemplet.GetName());
		NKCUtil.SetLabelText(m_lbEquipType, NKCUtilString.GET_STRING_UNIT_REACTOR_EQUIP_TYPE);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(reactorTemplet.BaseUnitID);
		if (unitTempletBase != null)
		{
			NKCUtil.SetLabelText(m_lbEquipReqUnit, string.Format(NKCUtilString.GET_STRING_UNIT_REACTOR_EQUIP_UNIT_STYLE_PRIVATE_01, unitTempletBase.GetUnitName()));
		}
		NKCUtil.SetLabelText(m_NKM_UI_ITEM_EQUIP_SLOT_TIER_TEXT, "");
		NKCUtil.SetGameobjectActive(m_objTier_7, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgGrade, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDetail, bValue: false);
		NKCUtil.SetGameobjectActive(m_objReactorDetail, bValue: true);
		NKCUtil.SetGameobjectActive(m_objReactorBG, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_ITEM_EQUIP_SLOT_REINFORCE, bValue: false);
		NKCUtil.SetImageSprite(m_NKM_UI_ITEM_EQUIP_SLOT_ICON, NKCResourceUtility.GetReactorIcon(reactorTemplet.ReactorIcon));
		for (int i = 0; i < m_lstReactor.Count; i++)
		{
			if (skillTemplets[i] == null || i >= skillTemplets.Length || (skillTemplets[i] != null && !skillTemplets[i].EnableByTag))
			{
				NKCUtil.SetGameobjectActive(m_lstReactor[i].gameObject, bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(m_lstReactor[i], bValue: true);
			m_lstReactor[i].SetData(skillTemplets[i], reactorLv <= i);
		}
		NKCUtil.SetGameobjectActive(m_objRelic, bValue: false);
		NKCUtil.SetGameobjectActive(NKM_UI_ITEM_EQUIP_SLOT_BOTTOM_SET_INFO, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_ITEM_EQUIP_SLOT_USED, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_ITEM_EQUIP_SLOT_LOCK, bValue: false);
		m_srDetail.normalizedPosition = Vector2.one;
	}

	private void SetItemStatText(Text label, NKMEquipItemData cNKMEquipItemData, EQUIP_ITEM_STAT statData, int idx = -1)
	{
		if (label == null || cNKMEquipItemData == null || statData == null)
		{
			return;
		}
		bool bPercentStat = false;
		if (idx > 0)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(cNKMEquipItemData.m_ItemEquipID);
			if (equipTemplet != null)
			{
				foreach (NKMEquipRandomStatTemplet equipRandomStatGroup in NKMEquipTuningManager.GetEquipRandomStatGroupList((idx == 1) ? equipTemplet.m_StatGroupID : equipTemplet.m_StatGroupID_2))
				{
					if (equipRandomStatGroup.m_StatType == statData.type)
					{
						bPercentStat = NKCUIForgeTuning.IsPercentStat(equipRandomStatGroup);
					}
				}
			}
		}
		else
		{
			bPercentStat = NKMUnitStatManager.IsPercentStat(statData.type);
		}
		if (statData.type == NKM_STAT_TYPE.NST_RANDOM)
		{
			label.text = NKCUtilString.GET_STRING_INVEN_RANDOM_OPTION;
		}
		else if (statData.stat_value == 0f)
		{
			label.text = NKCUtilString.GetStatShortName(statData.type);
		}
		else
		{
			NKCUtil.SetLabelText(label, NKCUIForgeTuning.GetTuningOptionStatString(statData, cNKMEquipItemData, bPercentStat));
		}
	}

	private void SetColor(int idx, eOptionType type)
	{
		Color color = Color.white;
		switch (type)
		{
		case eOptionType.RelicStat:
			color = NKCUtil.GetColor("#C2C2C2");
			break;
		case eOptionType.ModifiableStat:
			color = NKCUtil.GetColor("#CD9833");
			break;
		case eOptionType.ExclusiveStat:
			color = NKCUtil.GetColor("#9A3FA8");
			break;
		}
		switch (idx)
		{
		case 0:
			NKCUtil.SetLabelTextColor(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_01, color);
			break;
		case 1:
			NKCUtil.SetLabelTextColor(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_02, color);
			NKCUtil.SetImageColor(m_imgStat_02_Num, color);
			NKCUtil.SetGameobjectActive(m_imgStat_02_Lock, type == eOptionType.ExclusiveStat);
			break;
		case 2:
			NKCUtil.SetLabelTextColor(m_NKM_UI_ITEM_EQUIP_SLOT_ITEM_STAT_TEXT_03, color);
			NKCUtil.SetImageColor(m_imgStat_03_Num, color);
			break;
		}
	}

	public long GetEquipItemUID()
	{
		if (m_cNKMEquipItemData != null)
		{
			return m_cNKMEquipItemData.m_ItemUid;
		}
		return 0L;
	}

	private void OnClickStat_02()
	{
		if (NKCPopupFilterSubUIEquipStat.IsInstanceOpen())
		{
			NKCPopupFilterSubUIEquipStat.Instance.Close();
		}
		IReadOnlyList<NKMEquipRandomStatTemplet> equipRandomStatGroupList = NKMEquipTuningManager.GetEquipRandomStatGroupList(m_cNKMEquipTemplet.m_StatGroupID);
		List<NKM_STAT_TYPE> list = new List<NKM_STAT_TYPE>();
		for (int i = 0; i < equipRandomStatGroupList.Count; i++)
		{
			if (!list.Contains(equipRandomStatGroupList[i].m_StatType))
			{
				list.Add(equipRandomStatGroupList[i].m_StatType);
			}
		}
		if (list.Count > 1)
		{
			NKCPopupFilterSubUIEquipStat.Instance.Open(list, 0);
		}
	}

	private void OnClickStat_03()
	{
		if (NKCPopupFilterSubUIEquipStat.IsInstanceOpen())
		{
			NKCPopupFilterSubUIEquipStat.Instance.Close();
		}
		IReadOnlyList<NKMEquipRandomStatTemplet> equipRandomStatGroupList = NKMEquipTuningManager.GetEquipRandomStatGroupList(m_cNKMEquipTemplet.m_StatGroupID_2);
		List<NKM_STAT_TYPE> list = new List<NKM_STAT_TYPE>();
		for (int i = 0; i < equipRandomStatGroupList.Count; i++)
		{
			if (!list.Contains(equipRandomStatGroupList[i].m_StatType))
			{
				list.Add(equipRandomStatGroupList[i].m_StatType);
			}
		}
		if (list.Count > 1)
		{
			NKCPopupFilterSubUIEquipStat.Instance.Open(list, 1);
		}
	}

	private void OnClickSetOption()
	{
		if (NKCPopupFilterSubUIEquipStat.IsInstanceOpen())
		{
			NKCPopupFilterSubUIEquipStat.Instance.Close();
		}
		if (m_cNKMEquipTemplet.m_lstSetGroup.Count > 1)
		{
			NKCPopupFilterSubUIEquipStat.Instance.Open(m_cNKMEquipTemplet.m_lstSetGroup);
		}
	}
}
