using System.Collections.Generic;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupUnitRoleInfo : NKCUIBase
{
	public enum Page
	{
		BASIC_INFO,
		ATTACK_TYPE,
		TAG
	}

	private const string UI_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_UNIT_INFOPOPUP";

	private static NKCPopupUnitRoleInfo m_Instance;

	[Header("탑 메뉴")]
	public List<NKCUIComToggle> m_lstTopToggle = new List<NKCUIComToggle>();

	public NKCUIComStateButton m_btnClose;

	public NKCUIComStateButton m_btnBG;

	[Header("페이지 오브젝트")]
	public List<GameObject> m_lstPage = new List<GameObject>();

	[Header("유닛타입")]
	public GameObject m_objStyleCounter;

	public GameObject m_objStyleSoldier;

	public GameObject m_objStyleMechanic;

	public GameObject m_objStyleEtc;

	[Header("롤 타입")]
	public GameObject m_objRoleStriker;

	public GameObject m_objRoleRanger;

	public GameObject m_objRoleSniper;

	public GameObject m_objRoleDefender;

	public GameObject m_objRoleSiege;

	public GameObject m_objRoleSupporter;

	public GameObject m_objRoleTower;

	[Header("공격 타입")]
	public GameObject m_objAttackTypeGround;

	public GameObject m_objAttackTypeAir;

	public GameObject m_objAttackTypeAll;

	[Header("태그")]
	public GameObject m_objPatrol;

	public GameObject m_objSwingby;

	public GameObject m_objRespawnFreePos;

	public GameObject m_objRevenge;

	public GameObject m_objFury;

	public GameObject m_objRespawnLimit;

	public GameObject m_objMultiUnit;

	[Header("근원성")]
	public GameObject m_objConflict;

	public GameObject m_objStable;

	public GameObject m_objLiberal;

	[Header("태그")]
	public NKCUIComUnitTagList m_UIComUnitTagList;

	private bool m_bInitComplete;

	private NKCUIOpenAnimator m_openAni;

	public static NKCPopupUnitRoleInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupUnitRoleInfo>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_UNIT_INFOPOPUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupUnitRoleInfo>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_UNIT_ROLE_INFO;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		if (m_bInitComplete)
		{
			return;
		}
		m_openAni = new NKCUIOpenAnimator(base.gameObject);
		m_lstTopToggle[0].OnValueChanged.RemoveAllListeners();
		m_lstTopToggle[0].OnValueChanged.AddListener(delegate
		{
			OnClickTab(0);
		});
		m_lstTopToggle[1].OnValueChanged.RemoveAllListeners();
		m_lstTopToggle[1].OnValueChanged.AddListener(delegate
		{
			OnClickTab(1);
		});
		if (NKMOpenTagManager.IsOpened("UNIT_SOURCE_TYPE"))
		{
			NKCUtil.SetGameobjectActive(m_lstTopToggle[2], bValue: true);
			m_lstTopToggle[2].OnValueChanged.RemoveAllListeners();
			m_lstTopToggle[2].OnValueChanged.AddListener(delegate
			{
				OnClickTab(2);
			});
		}
		else if (m_lstTopToggle.Count > 2)
		{
			NKCUtil.SetGameobjectActive(m_lstTopToggle[2], bValue: false);
		}
		m_btnClose.PointerDown.RemoveAllListeners();
		m_btnClose.PointerDown.AddListener(delegate
		{
			OnClickClose();
		});
		m_btnBG.PointerClick.RemoveAllListeners();
		m_btnBG.PointerClick.AddListener(OnClickClose);
		m_bInitComplete = true;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void OpenDefaultPopup()
	{
		if (m_openAni != null)
		{
			m_openAni.PlayOpenAni();
		}
		NKCUtil.SetGameobjectActive(m_objStyleCounter, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStyleSoldier, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStyleMechanic, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStyleEtc, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRoleStriker, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRoleRanger, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRoleSniper, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRoleDefender, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRoleSiege, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRoleSupporter, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRoleTower, bValue: false);
		NKCUtil.SetGameobjectActive(m_objAttackTypeGround, bValue: false);
		NKCUtil.SetGameobjectActive(m_objAttackTypeAir, bValue: false);
		NKCUtil.SetGameobjectActive(m_objAttackTypeAll, bValue: false);
		NKCUtil.SetGameobjectActive(m_objPatrol, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSwingby, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRespawnFreePos, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRevenge, bValue: false);
		NKCUtil.SetGameobjectActive(m_objFury, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRespawnLimit, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMultiUnit, bValue: false);
		NKCUtil.SetGameobjectActive(m_objConflict, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStable, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLiberal, bValue: false);
		NKCUtil.SetGameobjectActive(m_UIComUnitTagList, bValue: false);
		UIOpened();
	}

	public void OpenPopup(NKMUnitData unitData, Page page = Page.BASIC_INFO)
	{
		OpenPopup(NKMUnitManager.GetUnitTempletBase(unitData), page);
	}

	public void OpenPopup(NKMUnitTempletBase unitTempletBase, Page page)
	{
		if (!m_bInitComplete)
		{
			InitUI();
		}
		if (unitTempletBase == null)
		{
			Debug.LogError("unitTempletBase is null");
			return;
		}
		if (m_openAni != null)
		{
			m_openAni.PlayOpenAni();
		}
		SetUnitStyleType(unitTempletBase.m_NKM_UNIT_STYLE_TYPE);
		SetUnitRoleType(unitTempletBase.m_NKM_UNIT_ROLE_TYPE);
		if (unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc == NKM_FIND_TARGET_TYPE.NFTT_INVALID)
		{
			SetAttackType(unitTempletBase.m_NKM_FIND_TARGET_TYPE);
		}
		else
		{
			SetAttackType(unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc);
		}
		NKCUtil.SetGameobjectActive(m_objPatrol, unitTempletBase.HasUnitTagType(NKM_UNIT_TAG.NUT_PATROL));
		NKCUtil.SetGameobjectActive(m_objSwingby, unitTempletBase.HasUnitTagType(NKM_UNIT_TAG.NUT_SWINGBY));
		NKCUtil.SetGameobjectActive(m_objRespawnFreePos, unitTempletBase.HasUnitTagType(NKM_UNIT_TAG.NUT_RESPAWN_FREE_POS));
		NKCUtil.SetGameobjectActive(m_objRevenge, unitTempletBase.HasUnitTagType(NKM_UNIT_TAG.NUT_REVENGE));
		NKCUtil.SetGameobjectActive(m_objFury, unitTempletBase.HasUnitTagType(NKM_UNIT_TAG.NUT_FURY));
		NKCUtil.SetGameobjectActive(m_objRespawnLimit, unitTempletBase.GetRespawnLimitCount() > 0);
		NKCUtil.SetGameobjectActive(m_objMultiUnit, unitTempletBase.HasUnitTagType(NKM_UNIT_TAG.NUT_MULTI_UNIT));
		NKCUtil.SetGameobjectActive(m_objConflict, unitTempletBase.HasSourceType(NKM_UNIT_SOURCE_TYPE.NUST_CONFLICT));
		NKCUtil.SetGameobjectActive(m_objStable, unitTempletBase.HasSourceType(NKM_UNIT_SOURCE_TYPE.NUST_STABLE));
		NKCUtil.SetGameobjectActive(m_objLiberal, unitTempletBase.HasSourceType(NKM_UNIT_SOURCE_TYPE.NUST_LIBERAL));
		m_UIComUnitTagList.SetData(unitTempletBase);
		if (m_lstTopToggle.Count > (int)page)
		{
			m_lstTopToggle[(int)page].Select(bSelect: true);
		}
		OnClickTab((int)page);
		UIOpened();
	}

	private void Update()
	{
		if (base.IsOpen && m_openAni != null)
		{
			m_openAni.Update();
		}
	}

	private void SetUnitStyleType(NKM_UNIT_STYLE_TYPE styleType)
	{
		NKCUtil.SetGameobjectActive(m_objStyleCounter, styleType == NKM_UNIT_STYLE_TYPE.NUST_COUNTER);
		NKCUtil.SetGameobjectActive(m_objStyleSoldier, styleType == NKM_UNIT_STYLE_TYPE.NUST_SOLDIER);
		NKCUtil.SetGameobjectActive(m_objStyleMechanic, styleType == NKM_UNIT_STYLE_TYPE.NUST_MECHANIC);
		if (styleType != NKM_UNIT_STYLE_TYPE.NUST_COUNTER && styleType != NKM_UNIT_STYLE_TYPE.NUST_SOLDIER && styleType != NKM_UNIT_STYLE_TYPE.NUST_MECHANIC)
		{
			NKCUtil.SetGameobjectActive(m_objStyleEtc, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objStyleEtc, bValue: false);
		}
	}

	private void SetUnitRoleType(NKM_UNIT_ROLE_TYPE roleType)
	{
		NKCUtil.SetGameobjectActive(m_objRoleStriker, roleType == NKM_UNIT_ROLE_TYPE.NURT_STRIKER);
		NKCUtil.SetGameobjectActive(m_objRoleRanger, roleType == NKM_UNIT_ROLE_TYPE.NURT_RANGER);
		NKCUtil.SetGameobjectActive(m_objRoleSniper, roleType == NKM_UNIT_ROLE_TYPE.NURT_SNIPER);
		NKCUtil.SetGameobjectActive(m_objRoleDefender, roleType == NKM_UNIT_ROLE_TYPE.NURT_DEFENDER);
		NKCUtil.SetGameobjectActive(m_objRoleSiege, roleType == NKM_UNIT_ROLE_TYPE.NURT_SIEGE);
		NKCUtil.SetGameobjectActive(m_objRoleSupporter, roleType == NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER);
		NKCUtil.SetGameobjectActive(m_objRoleTower, roleType == NKM_UNIT_ROLE_TYPE.NURT_TOWER);
	}

	private void SetAttackType(NKM_FIND_TARGET_TYPE attackType)
	{
		NKCUtil.SetGameobjectActive(m_objAttackTypeGround, bValue: false);
		NKCUtil.SetGameobjectActive(m_objAttackTypeAir, bValue: false);
		NKCUtil.SetGameobjectActive(m_objAttackTypeAll, bValue: false);
		switch (attackType)
		{
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND_RANGER_SUPPORTER_SNIPER_FIRST:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_LAND:
			NKCUtil.SetGameobjectActive(m_objAttackTypeGround, bValue: true);
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_AIR:
			NKCUtil.SetGameobjectActive(m_objAttackTypeAir, bValue: true);
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR_FIRST:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_ONLY:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM:
			NKCUtil.SetGameobjectActive(m_objAttackTypeAll, bValue: true);
			break;
		}
	}

	public void OnClickTab(int tabIndex)
	{
		for (int i = 0; i < m_lstPage.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstPage[i], tabIndex == i);
		}
	}

	public void OnClickClose()
	{
		Close();
	}
}
