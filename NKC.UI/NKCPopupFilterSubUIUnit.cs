using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupFilterSubUIUnit : MonoBehaviour
{
	public delegate void OnFilterOptionChange(NKCUnitSortSystem.eFilterOption filterOption);

	[Header("해당 프리팹에서 사용하는것만 연결")]
	[Header("치트전용 몬스터 타입")]
	public GameObject m_objMonsterType;

	public NKCUIComToggle m_tglReplacer;

	public NKCUIComToggle m_tglCorrupted;

	[Header("Scout")]
	public GameObject m_objScoutType;

	public NKCUIComToggle m_tglCanScout;

	public NKCUIComToggle m_tglNoScout;

	[Header("Unit Type")]
	public GameObject m_objUnitType;

	public NKCUIComToggle m_tglCounter;

	public NKCUIComToggle m_tglSoldier;

	public NKCUIComToggle m_tglMechanic;

	public NKCUIComToggle m_tglTrophy;

	[Header("Unit Class")]
	public GameObject m_objUnitClass;

	public NKCUIComToggle m_tglStriker;

	public NKCUIComToggle m_tglDefender;

	public NKCUIComToggle m_tglRanger;

	public NKCUIComToggle m_tglSniper;

	public NKCUIComToggle m_tglSupporter;

	public NKCUIComToggle m_tglSiege;

	public NKCUIComToggle m_tglTower;

	[Header("이동 타입")]
	public GameObject m_objUnitMoveType;

	public NKCUIComToggle m_tglMoveGround;

	public NKCUIComToggle m_tglMoveAir;

	[Header("공격 타입")]
	public GameObject m_objUnitBattleType;

	public NKCUIComToggle m_tglGround;

	public NKCUIComToggle m_tglAir;

	public NKCUIComToggle m_tglAll;

	[Header("Unit Cost")]
	public GameObject m_objUnitCost;

	public NKCUIComToggle m_tglCost_10;

	public NKCUIComToggle m_tglCost_9;

	public NKCUIComToggle m_tglCost_8;

	public NKCUIComToggle m_tglCost_7;

	public NKCUIComToggle m_tglCost_6;

	public NKCUIComToggle m_tglCost_5;

	public NKCUIComToggle m_tglCost_4;

	public NKCUIComToggle m_tglCost_3;

	public NKCUIComToggle m_tglCost_2;

	public NKCUIComToggle m_tglCost_1;

	[Header("Tactic Level")]
	public GameObject m_objUnitTacticLv;

	public NKCUIComToggle m_tglTacticLv_6;

	public NKCUIComToggle m_tglTacticLv_5;

	public NKCUIComToggle m_tglTacticLv_4;

	public NKCUIComToggle m_tglTacticLv_3;

	public NKCUIComToggle m_tglTacticLv_2;

	public NKCUIComToggle m_tglTacticLv_1;

	public NKCUIComToggle m_tglTacticLv_0;

	[Header("MoveIn")]
	public GameObject m_objRoomIn;

	public NKCUIComToggle m_tglRoomIn;

	public NKCUIComToggle m_tglRoomOut;

	[Header("Loyalty")]
	public GameObject m_objLoyalty;

	public NKCUIComToggle m_tglLoyaltyZero;

	public NKCUIComToggle m_tglLoyaltyMid;

	public NKCUIComToggle m_tglLoyaltyMax;

	[Header("Life Contract")]
	public GameObject m_objLiftContract;

	public NKCUIComToggle m_tglLiftContractFalse;

	public NKCUIComToggle m_tglLiftContractTrue;

	[Header("Awaken / Rearm / Normal")]
	public GameObject m_objSpecialType;

	public NKCUIComToggle m_tglAwaken;

	public NKCUIComToggle m_tglRearm;

	public NKCUIComToggle m_tglNormal;

	public NKCUIComToggle m_tglTacticUpdatePossible;

	public NKCUIComToggle m_tglUnitReactorPossible;

	[Header("SourceType")]
	public GameObject m_objSourceType;

	public NKCUIComToggle m_tglConflict;

	public NKCUIComToggle m_tglStable;

	public NKCUIComToggle m_tglLiberal;

	[Header("특수기 타입")]
	public GameObject m_objSkillType;

	public NKCUIComToggle m_tglSkillNormal;

	public NKCUIComToggle m_tglSkillFury;

	[Header("사정거리")]
	public GameObject m_objAttackRange;

	public NKCUIComToggle m_tglAttackRangeMelee;

	public NKCUIComToggle m_tglAttackRangeDistant;

	[Header("스킨")]
	public GameObject m_objSKin;

	public NKCUIComToggle m_tglSkinHave;

	[Header("Ship Type")]
	public GameObject m_objShipType;

	public NKCUIComToggle m_tglAssault;

	public NKCUIComToggle m_tglCruiser;

	public NKCUIComToggle m_tglHeavy;

	public NKCUIComToggle m_tglSpecial;

	[Header("In Collection")]
	public GameObject m_objCollected;

	public NKCUIComToggle m_tglCollected;

	public NKCUIComToggle m_tglNotCollected;

	[Header("Have")]
	public GameObject m_objHave;

	public NKCUIComToggle m_tglHave;

	public NKCUIComToggle m_tglNotHave;

	[Header("Rarity")]
	public GameObject m_objRare;

	public NKCUIComToggle m_tglRare_SSR;

	public NKCUIComToggle m_tglRare_SR;

	public NKCUIComToggle m_tglRare_R;

	public NKCUIComToggle m_tglRare_N;

	[Header("Level")]
	public GameObject m_objLevel;

	public NKCUIComToggle m_tglLevel_1;

	public NKCUIComToggle m_tglLevel_Other;

	public NKCUIComToggle m_tglLevel_Max;

	[Header("Deck")]
	public GameObject m_objDeck;

	public NKCUIComToggle m_tglDecked;

	public NKCUIComToggle m_tglWait;

	[Header("Lock")]
	public GameObject m_objLock;

	public NKCUIComToggle m_tglLocked;

	public NKCUIComToggle m_tglUnlocked;

	private RectTransform m_RectTransform;

	private Dictionary<NKCUnitSortSystem.eFilterOption, NKCUIComToggle> m_dicFilterBtn = new Dictionary<NKCUnitSortSystem.eFilterOption, NKCUIComToggle>();

	private OnFilterOptionChange dOnFilterOptionChange;

	private bool m_bInitComplete;

	private bool m_bReset;

	public RectTransform RectTransform
	{
		get
		{
			if (m_RectTransform == null)
			{
				m_RectTransform = GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	private void Init()
	{
		m_dicFilterBtn.Clear();
		SetToggleListner(m_tglHave, NKCUnitSortSystem.eFilterOption.Have);
		SetToggleListner(m_tglNotHave, NKCUnitSortSystem.eFilterOption.NotHave);
		SetToggleListner(m_tglCounter, NKCUnitSortSystem.eFilterOption.Unit_Counter);
		SetToggleListner(m_tglSoldier, NKCUnitSortSystem.eFilterOption.Unit_Soldier);
		SetToggleListner(m_tglMechanic, NKCUnitSortSystem.eFilterOption.Unit_Mechanic);
		SetToggleListner(m_tglTrophy, NKCUnitSortSystem.eFilterOption.Unit_Trainer);
		SetToggleListner(m_tglStriker, NKCUnitSortSystem.eFilterOption.Role_Striker);
		SetToggleListner(m_tglDefender, NKCUnitSortSystem.eFilterOption.Role_Defender);
		SetToggleListner(m_tglRanger, NKCUnitSortSystem.eFilterOption.Role_Ranger);
		SetToggleListner(m_tglSniper, NKCUnitSortSystem.eFilterOption.Role_Sniper);
		SetToggleListner(m_tglSupporter, NKCUnitSortSystem.eFilterOption.Role_Supporter);
		SetToggleListner(m_tglSiege, NKCUnitSortSystem.eFilterOption.Role_Siege);
		SetToggleListner(m_tglTower, NKCUnitSortSystem.eFilterOption.Role_Tower);
		SetToggleListner(m_tglMoveGround, NKCUnitSortSystem.eFilterOption.Unit_Move_Ground);
		SetToggleListner(m_tglMoveAir, NKCUnitSortSystem.eFilterOption.Unit_Move_Air);
		SetToggleListner(m_tglGround, NKCUnitSortSystem.eFilterOption.Unit_Target_Ground);
		SetToggleListner(m_tglAir, NKCUnitSortSystem.eFilterOption.Unit_Target_Air);
		SetToggleListner(m_tglAll, NKCUnitSortSystem.eFilterOption.Unit_Target_All);
		SetToggleListner(m_tglCost_10, NKCUnitSortSystem.eFilterOption.Unit_Cost_10);
		SetToggleListner(m_tglCost_9, NKCUnitSortSystem.eFilterOption.Unit_Cost_9);
		SetToggleListner(m_tglCost_8, NKCUnitSortSystem.eFilterOption.Unit_Cost_8);
		SetToggleListner(m_tglCost_7, NKCUnitSortSystem.eFilterOption.Unit_Cost_7);
		SetToggleListner(m_tglCost_6, NKCUnitSortSystem.eFilterOption.Unit_Cost_6);
		SetToggleListner(m_tglCost_5, NKCUnitSortSystem.eFilterOption.Unit_Cost_5);
		SetToggleListner(m_tglCost_4, NKCUnitSortSystem.eFilterOption.Unit_Cost_4);
		SetToggleListner(m_tglCost_3, NKCUnitSortSystem.eFilterOption.Unit_Cost_3);
		SetToggleListner(m_tglCost_2, NKCUnitSortSystem.eFilterOption.Unit_Cost_2);
		SetToggleListner(m_tglCost_1, NKCUnitSortSystem.eFilterOption.Unit_Cost_1);
		SetToggleListner(m_tglTacticLv_6, NKCUnitSortSystem.eFilterOption.Unit_TacticLv_6);
		SetToggleListner(m_tglTacticLv_5, NKCUnitSortSystem.eFilterOption.Unit_TacticLv_5);
		SetToggleListner(m_tglTacticLv_4, NKCUnitSortSystem.eFilterOption.Unit_TacticLv_4);
		SetToggleListner(m_tglTacticLv_3, NKCUnitSortSystem.eFilterOption.Unit_TacticLv_3);
		SetToggleListner(m_tglTacticLv_2, NKCUnitSortSystem.eFilterOption.Unit_TacticLv_2);
		SetToggleListner(m_tglTacticLv_1, NKCUnitSortSystem.eFilterOption.Unit_TacticLv_1);
		SetToggleListner(m_tglTacticLv_0, NKCUnitSortSystem.eFilterOption.Unit_TacticLv_0);
		SetToggleListner(m_tglAssault, NKCUnitSortSystem.eFilterOption.Ship_Assault);
		SetToggleListner(m_tglCruiser, NKCUnitSortSystem.eFilterOption.Ship_Cruiser);
		SetToggleListner(m_tglHeavy, NKCUnitSortSystem.eFilterOption.Ship_Heavy);
		SetToggleListner(m_tglSpecial, NKCUnitSortSystem.eFilterOption.Ship_Special);
		SetToggleListner(m_tglRare_SSR, NKCUnitSortSystem.eFilterOption.Rarily_SSR);
		SetToggleListner(m_tglRare_SR, NKCUnitSortSystem.eFilterOption.Rarily_SR);
		SetToggleListner(m_tglRare_R, NKCUnitSortSystem.eFilterOption.Rarily_R);
		SetToggleListner(m_tglRare_N, NKCUnitSortSystem.eFilterOption.Rarily_N);
		SetToggleListner(m_tglLevel_1, NKCUnitSortSystem.eFilterOption.Level_1);
		SetToggleListner(m_tglLevel_Other, NKCUnitSortSystem.eFilterOption.Level_other);
		SetToggleListner(m_tglLevel_Max, NKCUnitSortSystem.eFilterOption.Level_Max);
		SetToggleListner(m_tglDecked, NKCUnitSortSystem.eFilterOption.Decked);
		SetToggleListner(m_tglWait, NKCUnitSortSystem.eFilterOption.NotDecked);
		SetToggleListner(m_tglLocked, NKCUnitSortSystem.eFilterOption.Locked);
		SetToggleListner(m_tglUnlocked, NKCUnitSortSystem.eFilterOption.Unlocked);
		SetToggleListner(m_tglCollected, NKCUnitSortSystem.eFilterOption.Collected);
		SetToggleListner(m_tglNotCollected, NKCUnitSortSystem.eFilterOption.NotCollected);
		SetToggleListner(m_tglCanScout, NKCUnitSortSystem.eFilterOption.CanScout);
		SetToggleListner(m_tglNoScout, NKCUnitSortSystem.eFilterOption.NoScout);
		SetToggleListner(m_tglReplacer, NKCUnitSortSystem.eFilterOption.Unit_Replacer);
		SetToggleListner(m_tglCorrupted, NKCUnitSortSystem.eFilterOption.Unit_Corrupted);
		SetToggleListner(m_tglRoomIn, NKCUnitSortSystem.eFilterOption.InRoom);
		SetToggleListner(m_tglRoomOut, NKCUnitSortSystem.eFilterOption.OutRoom);
		SetToggleListner(m_tglLoyaltyZero, NKCUnitSortSystem.eFilterOption.Loyalty_Zero);
		SetToggleListner(m_tglLoyaltyMid, NKCUnitSortSystem.eFilterOption.Loyalty_Intermediate);
		SetToggleListner(m_tglLoyaltyMax, NKCUnitSortSystem.eFilterOption.Loyalty_Max);
		SetToggleListner(m_tglLiftContractFalse, NKCUnitSortSystem.eFilterOption.LifeContract_Unsigned);
		SetToggleListner(m_tglLiftContractTrue, NKCUnitSortSystem.eFilterOption.LifeContract_Signed);
		SetToggleListner(m_tglAwaken, NKCUnitSortSystem.eFilterOption.SpecialType_Awaken);
		SetToggleListner(m_tglRearm, NKCUnitSortSystem.eFilterOption.SpecialType_Rearm);
		SetToggleListner(m_tglNormal, NKCUnitSortSystem.eFilterOption.SpecialType_Normal);
		NKCUtil.SetGameobjectActive(m_tglNormal, NKMOpenTagManager.IsOpened("COLLECTION_V2"));
		SetToggleListner(m_tglTacticUpdatePossible, NKCUnitSortSystem.eFilterOption.TacticUpdate_Possible);
		SetToggleListner(m_tglUnitReactorPossible, NKCUnitSortSystem.eFilterOption.UnitReactor_Possible);
		SetToggleListner(m_tglConflict, NKCUnitSortSystem.eFilterOption.SourceType_Conflict);
		SetToggleListner(m_tglStable, NKCUnitSortSystem.eFilterOption.SourceType_Stable);
		SetToggleListner(m_tglLiberal, NKCUnitSortSystem.eFilterOption.SourceType_Liberal);
		SetToggleListner(m_tglSkillNormal, NKCUnitSortSystem.eFilterOption.Skill_Normal);
		SetToggleListner(m_tglSkillFury, NKCUnitSortSystem.eFilterOption.Skill_Fury);
		SetToggleListner(m_tglAttackRangeMelee, NKCUnitSortSystem.eFilterOption.Range_Melee);
		SetToggleListner(m_tglAttackRangeDistant, NKCUnitSortSystem.eFilterOption.Range_Distant);
		SetToggleListner(m_tglSkinHave, NKCUnitSortSystem.eFilterOption.Skin_Have);
		m_bInitComplete = true;
	}

	private void SetToggleListner(NKCUIComToggle toggle, NKCUnitSortSystem.eFilterOption filterOption)
	{
		if (toggle != null)
		{
			m_dicFilterBtn.Add(filterOption, toggle);
			toggle.OnValueChanged.RemoveAllListeners();
			toggle.OnValueChanged.AddListener(delegate(bool value)
			{
				OnFilterButton(value, filterOption);
			});
		}
	}

	public void OpenFilterPopup(HashSet<NKCUnitSortSystem.eFilterOption> setFilterOption, OnFilterOptionChange onFilterOptionChange, NKM_UNIT_TYPE unitType, NKCPopupFilterUnit.FILTER_OPEN_TYPE filterOpenType)
	{
		OpenFilterPopup(setFilterOption, NKCPopupFilterUnit.MakeDefaultFilterOption(unitType, filterOpenType), onFilterOptionChange);
	}

	public void OpenFilterPopup(HashSet<NKCUnitSortSystem.eFilterOption> setFilterOption, HashSet<NKCUnitSortSystem.eFilterCategory> setFilterCategory, OnFilterOptionChange onFilterOptionChange, bool bShowTrophyFilter = false)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		dOnFilterOptionChange = onFilterOptionChange;
		SetFilter(setFilterOption);
		NKCUtil.SetGameobjectActive(m_objHave, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.Have));
		NKCUtil.SetGameobjectActive(m_objUnitType, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.UnitType));
		NKCUtil.SetGameobjectActive(m_objUnitClass, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.UnitRole));
		NKCUtil.SetGameobjectActive(m_objUnitBattleType, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.UnitTargetType));
		NKCUtil.SetGameobjectActive(m_objUnitCost, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.Cost));
		NKCUtil.SetGameobjectActive(m_objUnitTacticLv, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.TacticLv));
		NKCUtil.SetGameobjectActive(m_objShipType, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.ShipType));
		NKCUtil.SetGameobjectActive(m_objRare, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.Rarity));
		NKCUtil.SetGameobjectActive(m_objLevel, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.Level));
		NKCUtil.SetGameobjectActive(m_objDeck, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.Decked));
		NKCUtil.SetGameobjectActive(m_objLock, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.Locked));
		NKCUtil.SetGameobjectActive(m_objRoomIn, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.InRoom));
		NKCUtil.SetGameobjectActive(m_objLoyalty, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.Loyalty));
		NKCUtil.SetGameobjectActive(m_objLiftContract, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.LifeContract));
		NKCUtil.SetGameobjectActive(m_objUnitMoveType, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.UnitMoveType));
		NKCUtil.SetGameobjectActive(m_objSpecialType, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.SpecialType));
		NKCUtil.SetGameobjectActive(m_objScoutType, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.Scout));
		NKCUtil.SetGameobjectActive(m_objCollected, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.Collected));
		NKCUtil.SetGameobjectActive(m_objMonsterType, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.MonsterType));
		if (NKMOpenTagManager.IsOpened("UNIT_SOURCE_TYPE"))
		{
			NKCUtil.SetGameobjectActive(m_objSourceType, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.SourceType));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objSourceType, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objSkillType, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.SkillType));
		NKCUtil.SetGameobjectActive(m_objAttackRange, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.AttackRange));
		NKCUtil.SetGameobjectActive(m_objSKin, setFilterCategory.Contains(NKCUnitSortSystem.eFilterCategory.Skin));
		NKCUtil.SetGameobjectActive(m_tglTrophy, bShowTrophyFilter);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	private void SetFilter(HashSet<NKCUnitSortSystem.eFilterOption> setFilterOption)
	{
		ResetFilter();
		m_bReset = true;
		foreach (NKCUnitSortSystem.eFilterOption item in setFilterOption)
		{
			if (m_dicFilterBtn.ContainsKey(item) && m_dicFilterBtn[item] != null)
			{
				m_dicFilterBtn[item].Select(bSelect: true);
			}
		}
		m_bReset = false;
	}

	private void OnFilterButton(bool bSelect, NKCUnitSortSystem.eFilterOption filterOption)
	{
		if (!m_dicFilterBtn.ContainsKey(filterOption))
		{
			return;
		}
		NKCUIComToggle nKCUIComToggle = m_dicFilterBtn[filterOption];
		if (nKCUIComToggle != null)
		{
			nKCUIComToggle.Select(bSelect, bForce: true, bImmediate: true);
			if (!m_bReset)
			{
				dOnFilterOptionChange?.Invoke(filterOption);
			}
		}
	}

	public void ResetFilter()
	{
		m_bReset = true;
		NKCUIComToggle[] componentsInChildren = base.transform.GetComponentsInChildren<NKCUIComToggle>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Select(bSelect: false);
		}
		m_bReset = false;
	}
}
