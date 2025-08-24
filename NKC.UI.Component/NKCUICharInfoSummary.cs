using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUICharInfoSummary : MonoBehaviour
{
	[Header("유닛 상세정보 루트")]
	public GameObject m_objInfoRoot;

	[Header("기본")]
	public Text m_lbCodename;

	public Text m_lbName;

	public NKCUIComTextUnitLevel m_lbLevel;

	public Text m_lbMaxLevel;

	public Text m_lbCost;

	public Image m_imgExpBar;

	public Text m_lbExp;

	public NKCUIComStarRank m_comStarRank;

	[Header("작전능력")]
	public Text m_lbPowerSummary;

	[Header("타입")]
	public GameObject m_objUnitClassRoot;

	public GameObject m_objClassIconCounter;

	public GameObject m_objClassIconMechanic;

	public GameObject m_objClassIconSolider;

	public GameObject m_objClassIconETC;

	[Header("클래스 & 레어리티")]
	public Text m_lbClass;

	public GameObject m_objTag;

	public Text m_lbTag;

	public GameObject m_objRarityN;

	public GameObject m_objRarityR;

	public GameObject m_objRaritySR;

	public GameObject m_objRaritySSR;

	public Image m_imgRarity;

	public GameObject m_objAwakenSR;

	public GameObject m_objAwakenSSR;

	[Header("약점 태그")]
	public GameObject m_objWeakTag;

	public GameObject m_objWeakTagMain;

	public Image m_imgWeakTagMain;

	public Text m_lbWeakTagMain;

	public GameObject m_objWeakTagSub;

	public Image m_imgWeakTagSub;

	public Text m_lbWeakTagSub;

	[Header("클래스 정보")]
	public Image m_imgRole;

	public Text m_lbRole;

	public Image m_imgMoveType;

	public Text m_lbMoveType;

	public Image m_imgAttackType;

	public Text m_lbAttackType;

	public Image m_imgDefType;

	public Text m_lbDefType;

	public GameObject m_objClassStar;

	[Header("정보창 연결")]
	public NKCUIComStateButton m_NKM_UI_UNIT_CLASS;

	public NKCUIComStateButton m_NKM_UI_UNIT_BATTLE_TYPE;

	public NKCUIComStateButton m_NKM_UI_UNIT_BATTLE_TYPE_ATK;

	public NKCUIComStateButton m_NKM_UI_UNIT_DEFENCE_TYPE;

	public NKCUIComStateButton m_NKM_UI_UNIT_TAG;

	public NKCUIComStateButton m_NKM_UI_UNIT_WEAK_TAG;

	[Header("레벨&경험치")]
	public GameObject m_NKM_UI_UNIT_INFO_DESC_SUMMARY_LEVEL;

	[Header("전술업데이트")]
	public NKCUITacticUpdateLevelSlot m_tacticUpdateLvSlot;

	private int m_SelectUnitID;

	private NKMUnitData m_UnitInfo;

	public void SetUnitClassRootActive(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objUnitClassRoot, value);
	}

	public void Init(bool bShowLevel = true)
	{
		if (null != m_NKM_UI_UNIT_CLASS)
		{
			m_NKM_UI_UNIT_CLASS.PointerClick.RemoveAllListeners();
			m_NKM_UI_UNIT_CLASS.PointerClick.AddListener(delegate
			{
				OpenInfo(NKCPopupUnitRoleInfo.Page.BASIC_INFO);
			});
		}
		if (null != m_NKM_UI_UNIT_BATTLE_TYPE)
		{
			m_NKM_UI_UNIT_BATTLE_TYPE.PointerClick.RemoveAllListeners();
			m_NKM_UI_UNIT_BATTLE_TYPE.PointerClick.AddListener(delegate
			{
				OpenInfo(NKCPopupUnitRoleInfo.Page.ATTACK_TYPE);
			});
		}
		if (null != m_NKM_UI_UNIT_BATTLE_TYPE_ATK)
		{
			m_NKM_UI_UNIT_BATTLE_TYPE_ATK.PointerClick.RemoveAllListeners();
			m_NKM_UI_UNIT_BATTLE_TYPE_ATK.PointerClick.AddListener(delegate
			{
				OpenInfo(NKCPopupUnitRoleInfo.Page.ATTACK_TYPE);
			});
		}
		if (null != m_NKM_UI_UNIT_DEFENCE_TYPE)
		{
			m_NKM_UI_UNIT_DEFENCE_TYPE.PointerClick.RemoveAllListeners();
			m_NKM_UI_UNIT_DEFENCE_TYPE.PointerClick.AddListener(delegate
			{
				OpenInfo(NKCPopupUnitRoleInfo.Page.BASIC_INFO);
			});
		}
		if (null != m_NKM_UI_UNIT_TAG)
		{
			m_NKM_UI_UNIT_TAG.PointerClick.RemoveAllListeners();
			m_NKM_UI_UNIT_TAG.PointerClick.AddListener(delegate
			{
				OpenInfo(NKCPopupUnitRoleInfo.Page.TAG);
			});
		}
		if (m_NKM_UI_UNIT_WEAK_TAG != null)
		{
			m_NKM_UI_UNIT_WEAK_TAG.PointerClick.RemoveAllListeners();
			m_NKM_UI_UNIT_WEAK_TAG.PointerClick.AddListener(delegate
			{
				OpenInfo(NKCPopupUnitRoleInfo.Page.ATTACK_TYPE);
			});
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_INFO_DESC_SUMMARY_LEVEL, bShowLevel);
	}

	public void SetData(NKMUnitData unitData)
	{
		if (unitData != null)
		{
			NKCUtil.SetGameobjectActive(m_objUnitClassRoot, bValue: true);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
			NKCUtil.SetGameobjectActive(m_objClassIconCounter, unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_COUNTER);
			NKCUtil.SetGameobjectActive(m_objClassIconMechanic, unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_MECHANIC);
			NKCUtil.SetGameobjectActive(m_objClassIconSolider, unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_SOLDIER);
			NKCUtil.SetGameobjectActive(m_objClassIconETC, unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER);
			NKCUtil.SetLabelText(m_lbClass, NKCUtilString.GetUnitStyleMarkString(unitTempletBase));
			if (m_lbClass != null)
			{
				m_lbClass.color = NKCUtil.GetColorForUnitGrade(unitTempletBase.m_NKM_UNIT_GRADE);
			}
			string unitAbilityName = NKCUtilString.GetUnitAbilityName(unitData.m_UnitID);
			NKCUtil.SetGameobjectActive(m_objTag, !string.IsNullOrEmpty(unitAbilityName));
			NKCUtil.SetLabelText(m_lbTag, unitAbilityName);
			NKCUtil.SetGameobjectActive(m_objWeakTag, NKMOpenTagManager.IsOpened("UNIT_SOURCE_TYPE") && unitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
			if (unitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				NKCUtil.SetGameobjectActive(m_objWeakTagMain, unitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
				NKCUtil.SetImageSprite(m_imgWeakTagMain, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(unitTempletBase.m_NKM_UNIT_SOURCE_TYPE, bSmall: true));
				NKCUtil.SetLabelText(m_lbWeakTagMain, NKCUtilString.GetSourceTypeName(unitTempletBase.m_NKM_UNIT_SOURCE_TYPE));
				NKCUtil.SetGameobjectActive(m_objWeakTagSub, unitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
				if (unitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
				{
					NKCUtil.SetImageSprite(m_imgWeakTagSub, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(unitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB, bSmall: true));
					NKCUtil.SetLabelText(m_lbWeakTagSub, NKCUtilString.GetSourceTypeName(unitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB));
				}
			}
			SetUnitRank(unitTempletBase.m_NKM_UNIT_GRADE, unitTempletBase.m_bAwaken);
			NKCUtil.SetLabelText(m_lbCodename, unitTempletBase.GetUnitTitle());
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
			NKCUtil.SetLabelText(m_lbCost, (unitStatTemplet != null) ? unitStatTemplet.GetRespawnCost(bLeader: false, null, null).ToString() : "-");
			NKCUtil.SetLabelText(m_lbName, unitTempletBase.GetUnitName() + NKCUtilString.GetRespawnCountText(unitData.m_UnitID));
			m_lbLevel?.SetLevel(unitData, 0, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM);
			NKMLimitBreakTemplet lBInfo = NKMUnitLimitBreakManager.GetLBInfo(unitData.m_LimitBreakLevel);
			if (lBInfo != null)
			{
				NKCUtil.SetLabelText(m_lbMaxLevel, "/" + lBInfo.m_iMaxLevel);
			}
			NKCUtil.SetLabelText(m_lbPowerSummary, unitData.CalculateOperationPower(NKCScenManager.CurrentUserData().m_InventoryData).ToString("N0"));
			NKCUtil.SetGameobjectActive(m_imgExpBar, bValue: true);
			if (m_imgExpBar != null)
			{
				if (NKCExpManager.GetUnitMaxLevel(unitData) == unitData.m_UnitLevel)
				{
					m_imgExpBar.fillAmount = 1f;
				}
				else
				{
					m_imgExpBar.fillAmount = NKCExpManager.GetUnitNextLevelExpProgress(unitData);
				}
			}
			NKCUtil.SetLabelText(m_lbExp, $"{NKCExpManager.GetCurrentExp(unitData)}/{NKCExpManager.GetRequiredExp(unitData)}");
			m_comStarRank?.SetStarRank(unitData);
			if (m_tacticUpdateLvSlot != null)
			{
				m_tacticUpdateLvSlot.SetLevel(unitData.tacticLevel);
				NKCUtil.SetGameobjectActive(m_tacticUpdateLvSlot.gameObject, bValue: true);
			}
			m_UnitInfo = unitData;
			SetUnitInfo(unitData.m_UnitID);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objInfoRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnitClassRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objClassIconCounter, bValue: false);
			NKCUtil.SetGameobjectActive(m_objClassIconMechanic, bValue: false);
			NKCUtil.SetGameobjectActive(m_objClassIconSolider, bValue: false);
			NKCUtil.SetLabelText(m_lbClass, "");
			SetUnitRank(NKM_UNIT_GRADE.NUG_COUNT, isAwaken: false);
			NKCUtil.SetLabelText(m_lbCodename, "");
			NKCUtil.SetLabelText(m_lbName, "");
			NKCUtil.SetLabelText(m_lbLevel, "-");
			NKCUtil.SetLabelText(m_lbPowerSummary, "");
			NKCUtil.SetGameobjectActive(m_imgExpBar, bValue: false);
			NKCUtil.SetLabelText(m_lbCost, "-");
			m_comStarRank?.SetStarRank(0, 0);
			NKCUtil.SetGameobjectActive(m_tacticUpdateLvSlot.gameObject, bValue: false);
		}
		SetEnableClassStar(bEnable: true);
	}

	private void SetUnitRank(NKM_UNIT_GRADE grade, bool isAwaken)
	{
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_ICON", grade switch
		{
			NKM_UNIT_GRADE.NUG_R => "NKM_UI_COMMON_RANK_R", 
			NKM_UNIT_GRADE.NUG_SR => "NKM_UI_COMMON_RANK_SR", 
			NKM_UNIT_GRADE.NUG_SSR => "NKM_UI_COMMON_RANK_SSR", 
			_ => "NKM_UI_COMMON_RANK_N", 
		});
		if (orLoadAssetResource == null)
		{
			Debug.LogError("Rarity sprite not found!");
		}
		NKCUtil.SetImageSprite(m_imgRarity, orLoadAssetResource, bDisableIfSpriteNull: true);
		NKCUtil.SetGameobjectActive(m_objRarityN, grade == NKM_UNIT_GRADE.NUG_N);
		NKCUtil.SetGameobjectActive(m_objRarityR, grade == NKM_UNIT_GRADE.NUG_R);
		NKCUtil.SetGameobjectActive(m_objRaritySR, grade == NKM_UNIT_GRADE.NUG_SR);
		NKCUtil.SetGameobjectActive(m_objRaritySSR, grade == NKM_UNIT_GRADE.NUG_SSR);
		NKCUtil.SetGameobjectActive(m_objAwakenSR, isAwaken && grade == NKM_UNIT_GRADE.NUG_SR);
		NKCUtil.SetGameobjectActive(m_objAwakenSSR, isAwaken && grade == NKM_UNIT_GRADE.NUG_SSR);
	}

	private void SetUnitInfo(int unitID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (unitTempletBase != null)
		{
			m_SelectUnitID = unitID;
			NKCUtil.SetGameobjectActive(m_objInfoRoot, !unitTempletBase.IsTrophy);
			NKCUtil.SetImageSprite(m_imgRole, NKCResourceUtility.GetOrLoadUnitRoleIcon(unitTempletBase), bDisableIfSpriteNull: true);
			NKCUtil.SetLabelText(m_lbRole, NKCUtilString.GetRoleText(unitTempletBase));
			if (m_imgMoveType != null)
			{
				m_imgMoveType.sprite = NKCUtil.GetMoveTypeImg(unitTempletBase.m_bAirUnit);
			}
			if (m_lbMoveType != null)
			{
				m_lbMoveType.text = NKCUtilString.GetMoveTypeText(unitTempletBase.m_bAirUnit);
			}
			NKCUtil.SetImageSprite(m_imgAttackType, NKCResourceUtility.GetOrLoadUnitAttackTypeIcon(unitTempletBase), bDisableIfSpriteNull: true);
			NKCUtil.SetLabelText(m_lbAttackType, NKCUtilString.GetAtkTypeText(unitTempletBase));
		}
	}

	public void OpenInfo(NKCPopupUnitRoleInfo.Page page)
	{
		if (m_UnitInfo != null)
		{
			NKCPopupUnitRoleInfo.Instance.OpenPopup(m_UnitInfo, page);
		}
	}

	public void SetEnableClassStar(bool bEnable)
	{
		NKCUtil.SetGameobjectActive(m_objClassStar, bEnable);
	}

	public void SetEnableLevelInfo(bool bEnable)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_INFO_DESC_SUMMARY_LEVEL, bEnable);
	}
}
