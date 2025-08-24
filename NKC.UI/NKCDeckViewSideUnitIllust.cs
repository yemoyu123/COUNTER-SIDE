using System.Collections.Generic;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCDeckViewSideUnitIllust : MonoBehaviour
{
	public delegate void OnUnitInfoClick(NKMUnitData currentUnitData);

	public delegate void OnUnitChangeClick(NKM_UNIT_TYPE _NKM_UNIT_TYPE, long selectedUnitUID);

	public delegate void OnLeaderChange(NKMUnitData currentUnitData);

	public enum eUnitChangePossible
	{
		OK,
		WORLDMAP_MISSION,
		WARFARE,
		DIVE
	}

	private bool m_bOpen;

	private NKMUnitData m_UnitData;

	private NKMOperator m_OperatorData;

	private NKMUnitTempletBase m_UnitTempletBase;

	private bool m_bLeader;

	public RectTransform m_rectSpineObjectRoot;

	public RectTransform m_rectShipSpineObjectRoot;

	public RectTransform m_rectSubMenu;

	private Vector2 m_vSubMenuAnchoredPosOrg;

	private const float SUBMENU_OFFSET_Y_FOR_PVP = 95f;

	private Vector2 m_vSpineObjectRootAnchoredPosOrg;

	private Vector2 m_vShipObjectRootAnchoredPosOrg;

	[Header("버튼")]
	public NKCUIComButton m_cbtnInfo;

	public NKCUIComButton m_cbtnChange;

	public NKCUIComButton m_cbtnLeader;

	public GameObject m_objChangeBlocked;

	public Text m_lbChangeBlocked;

	[Header("데이터")]
	public Text m_textName;

	public NKCUIComTextUnitLevel m_textLevel;

	public GameObject m_objExpMax;

	public GameObject m_objSkillMax;

	public NKCUIComStarRank m_comStarRank;

	public Text m_lbPowerSummary;

	public NKCUICharacterView m_UICharacterView;

	public NKCUICharacterView m_UIShipView;

	public Animator m_BGAnimator;

	private Animator m_animLoading;

	private const float SHIP_SMALL_SCALE = 0.7f;

	private OnUnitInfoClick dOnUnitInfoClick;

	private OnUnitChangeClick dOnUnitChangeClick;

	private OnLeaderChange dOnLeaderChange;

	private bool m_bEnableControlLeaderBtn = true;

	private bool m_bShipOnlyMode;

	private eUnitChangePossible m_eCurrentUnitChangeBtnState;

	private NKCUIDeckViewer.DeckViewerMode m_eMode;

	private bool m_bShowLoadingAnimWhenEmpty = true;

	public NKMUnitData GetUnitData()
	{
		return m_UnitData;
	}

	public bool hasUnitData()
	{
		return m_UnitTempletBase != null;
	}

	public void SetEnableControlLeaderBtn(bool bSet)
	{
		m_bEnableControlLeaderBtn = bSet;
	}

	public void SetShipOnlyMode(bool value)
	{
		m_bShipOnlyMode = value;
		SetObject();
	}

	public void SetShowLoadingAnimWhenEmpty(bool bSet)
	{
		m_bShowLoadingAnimWhenEmpty = bSet;
	}

	public void Init(OnUnitInfoClick onUnitInfo, OnUnitChangeClick onUnitChange, OnLeaderChange onLeaderChange, Animator LoadingAnimator)
	{
		ResetObj();
		m_animLoading = LoadingAnimator;
		dOnUnitInfoClick = onUnitInfo;
		dOnUnitChangeClick = onUnitChange;
		dOnLeaderChange = onLeaderChange;
		if (m_cbtnInfo != null)
		{
			m_cbtnInfo.PointerClick.RemoveAllListeners();
			m_cbtnInfo.PointerClick.AddListener(OnUnitInfoClicked);
		}
		if (m_cbtnChange != null)
		{
			m_cbtnChange.PointerClick.RemoveAllListeners();
			m_cbtnChange.PointerClick.AddListener(OnUnitChangeClicked);
		}
		if (m_cbtnLeader != null)
		{
			m_cbtnLeader.PointerClick.RemoveAllListeners();
			m_cbtnLeader.PointerClick.AddListener(OnLeaderChangeClicked);
		}
		m_vSubMenuAnchoredPosOrg = m_rectSubMenu.anchoredPosition;
		m_vSpineObjectRootAnchoredPosOrg = m_rectSpineObjectRoot.anchoredPosition;
		if (m_rectShipSpineObjectRoot.GetComponent<NKCUIComSafeArea>() != null)
		{
			m_rectShipSpineObjectRoot.GetComponent<NKCUIComSafeArea>().SetSafeAreaUI();
		}
		m_vShipObjectRootAnchoredPosOrg = m_rectShipSpineObjectRoot.anchoredPosition;
	}

	public void ResetObj()
	{
		m_UnitData = null;
		m_OperatorData = null;
		m_UnitTempletBase = null;
		m_UICharacterView.CleanUp();
		m_UIShipView.CleanUp();
		SetSubMenuPosition(bIn: false, bAnimate: false);
	}

	public void Open(NKCUIDeckViewer.DeckViewerMode mode, bool bInit)
	{
		if (!m_bOpen)
		{
			m_bOpen = true;
			m_eMode = mode;
			if (mode == NKCUIDeckViewer.DeckViewerMode.PrepareRaid || mode == NKCUIDeckViewer.DeckViewerMode.GuildCoopBoss)
			{
				m_rectShipSpineObjectRoot.anchoredPosition = new Vector2(m_vShipObjectRootAnchoredPosOrg.x, m_vShipObjectRootAnchoredPosOrg.y + 70f);
				m_bShowLoadingAnimWhenEmpty = false;
			}
			else
			{
				m_rectShipSpineObjectRoot.anchoredPosition = m_vShipObjectRootAnchoredPosOrg;
				m_bShowLoadingAnimWhenEmpty = true;
			}
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			if (bInit)
			{
				ResetObj();
			}
			else if (m_UnitData != null)
			{
				SetObject();
				SetSubMenuPosition(bIn: true);
			}
			else
			{
				SetSubMenuPosition(bIn: false);
			}
		}
	}

	public void Close()
	{
		if (m_bOpen)
		{
			m_bOpen = false;
			m_UICharacterView.CleanUp();
			m_UIShipView.CleanUp();
			SetSubMenuPosition(bIn: false);
			m_rectSubMenu.anchoredPosition = m_vSubMenuAnchoredPosOrg;
			m_rectSpineObjectRoot.anchoredPosition = m_vSpineObjectRootAnchoredPosOrg;
			m_rectShipSpineObjectRoot.anchoredPosition = m_vShipObjectRootAnchoredPosOrg;
		}
	}

	private void SetMenuActive(NKMUnitData unitData)
	{
		if (!m_bEnableControlLeaderBtn)
		{
			return;
		}
		if (unitData != null)
		{
			switch (NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID).m_NKM_UNIT_TYPE)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
				NKCUtil.SetGameobjectActive(m_cbtnLeader, bValue: true);
				break;
			default:
				NKCUtil.SetGameobjectActive(m_cbtnLeader, bValue: false);
				break;
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_cbtnLeader, bValue: false);
		}
	}

	public void SetUnitData(NKMUnitData unitData, bool bLeader, eUnitChangePossible unitchangePossible, bool bForce = false)
	{
		if (unitData != null && m_UnitData != null && m_UnitData.m_UnitUID == unitData.m_UnitUID && !bForce)
		{
			return;
		}
		SetMenuActive(unitData);
		SetLeader(bLeader);
		NKCUtil.SetGameobjectActive(m_cbtnChange, unitchangePossible == eUnitChangePossible.OK);
		m_eCurrentUnitChangeBtnState = unitchangePossible;
		NKCUtil.SetGameobjectActive(m_objChangeBlocked, unitchangePossible != eUnitChangePossible.OK);
		switch (unitchangePossible)
		{
		case eUnitChangePossible.WARFARE:
		case eUnitChangePossible.DIVE:
			NKCUtil.SetLabelText(m_lbChangeBlocked, NKCUtilString.GET_STRING_DECK_UNIT_STATE_DOING);
			break;
		case eUnitChangePossible.WORLDMAP_MISSION:
			NKCUtil.SetLabelText(m_lbChangeBlocked, NKCUtilString.GET_STRING_DECK_UNIT_STATE_DOING_MISSION);
			break;
		}
		m_OperatorData = null;
		if (unitData == null)
		{
			m_UnitData = null;
			m_UnitTempletBase = null;
			m_textLevel.SetText("0", 0);
			NKCUtil.SetGameobjectActive(m_objExpMax, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSkillMax, bValue: false);
			m_textName.text = "";
			m_comStarRank.SetStarRank(0, 1);
			m_lbPowerSummary.text = "";
		}
		else
		{
			m_UnitData = unitData;
			m_UnitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitData.m_UnitID);
			NKCUtil.SetGameobjectActive(m_objExpMax, bValue: false);
			if (m_UnitTempletBase.IsShip())
			{
				NKCUtil.SetGameobjectActive(m_objSkillMax, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objSkillMax, !NKMUnitSkillManager.CheckHaveUpgradableSkill(unitData));
			}
			m_textLevel.SetLevel(unitData, 0);
			NKCUtil.SetLabelText(m_textName, m_UnitTempletBase.GetUnitName() + NKCUtilString.GetRespawnCountText(m_UnitData.m_UnitID));
			if (m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL || m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
			{
				m_comStarRank.SetStarRank(unitData);
				bool flag = NKCUtil.CheckPossibleShowBan(m_eMode);
				int num = unitData.CalculateOperationPower(NKCScenManager.CurrentUserData().m_InventoryData, 0, flag ? NKCBanManager.GetBanDataShip() : null);
				m_lbPowerSummary.text = num.ToString("N0");
			}
			else if (m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR)
			{
				m_comStarRank.SetStarRank(0, 0);
				NKCUtil.SetLabelText(m_lbPowerSummary, m_UnitData.CalculateOperationPower(null, 0, null, NKCOperatorUtil.GetOperatorData(m_UnitData.m_UnitUID)).ToString("N0"));
			}
		}
		SetObject();
	}

	public void SetOperatorData(NKMOperator operatorData, eUnitChangePossible unitchangePossible, bool bForce = false)
	{
		if (m_OperatorData != operatorData || bForce)
		{
			SetMenuActive(null);
			SetLeader(bLeader: false);
			NKCUtil.SetGameobjectActive(m_cbtnChange, unitchangePossible == eUnitChangePossible.OK);
			m_eCurrentUnitChangeBtnState = unitchangePossible;
			NKCUtil.SetGameobjectActive(m_objChangeBlocked, unitchangePossible != eUnitChangePossible.OK);
			switch (unitchangePossible)
			{
			case eUnitChangePossible.WARFARE:
			case eUnitChangePossible.DIVE:
				NKCUtil.SetLabelText(m_lbChangeBlocked, NKCUtilString.GET_STRING_DECK_UNIT_STATE_DOING);
				break;
			case eUnitChangePossible.WORLDMAP_MISSION:
				NKCUtil.SetLabelText(m_lbChangeBlocked, NKCUtilString.GET_STRING_DECK_UNIT_STATE_DOING_MISSION);
				break;
			}
			m_UnitData = null;
			if (operatorData == null)
			{
				m_OperatorData = null;
				m_UnitTempletBase = null;
				m_textLevel.SetText("0", 0);
				NKCUtil.SetGameobjectActive(m_objExpMax, bValue: false);
				NKCUtil.SetGameobjectActive(m_objSkillMax, bValue: false);
				m_textName.text = "";
				m_comStarRank.SetStarRank(0, 1);
				m_lbPowerSummary.text = "";
			}
			else
			{
				m_OperatorData = operatorData;
				m_UnitTempletBase = NKMUnitManager.GetUnitTempletBase(m_OperatorData.id);
				m_textLevel.SetLevel(m_OperatorData);
				NKCUtil.SetGameobjectActive(m_objExpMax, bValue: false);
				NKCUtil.SetGameobjectActive(m_objSkillMax, bValue: false);
				NKCUtil.SetLabelText(m_textName, m_UnitTempletBase.GetUnitName() ?? "");
				m_comStarRank.SetStarRank(0, 0);
				NKCUtil.SetLabelText(m_lbPowerSummary, m_OperatorData.CalculateOperatorOperationPower().ToString("N0"));
			}
			SetObject();
		}
	}

	public bool IsMatchedSideIllustToUnitType(NKM_UNIT_TYPE targetType)
	{
		if (m_UnitTempletBase != null && m_UnitTempletBase.m_NKM_UNIT_TYPE == targetType)
		{
			return true;
		}
		return false;
	}

	public void UpdateUnit(NKMUnitData unitData)
	{
		if (unitData != null && m_UnitData != null && m_UnitData.m_UnitUID == unitData.m_UnitUID)
		{
			SetUnitData(unitData, m_bLeader, m_eCurrentUnitChangeBtnState, bForce: true);
		}
	}

	public void UpdateOperator(NKMOperator operatorData)
	{
		if (operatorData != null && m_OperatorData != null && m_OperatorData.uid == operatorData.uid)
		{
			SetOperatorData(operatorData, m_eCurrentUnitChangeBtnState, bForce: true);
		}
	}

	public void SetObject()
	{
		NKM_UNIT_TYPE nKM_UNIT_TYPE = NKM_UNIT_TYPE.NUT_INVALID;
		NKM_UNIT_TYPE nKM_UNIT_TYPE2 = NKM_UNIT_TYPE.NUT_INVALID;
		if (!m_bOpen)
		{
			SetIllust();
			SetShipIllust();
			SetLeader();
		}
		else
		{
			if (m_UICharacterView.HasCharacterIllust())
			{
				nKM_UNIT_TYPE = NKM_UNIT_TYPE.NUT_NORMAL;
				Vector2 vSpineObjectRootAnchoredPosOrg = m_vSpineObjectRootAnchoredPosOrg;
				vSpineObjectRootAnchoredPosOrg.x = 900f;
				float num = 0.3f * (900f - m_rectSpineObjectRoot.anchoredPosition.x) / 900f;
				m_rectSpineObjectRoot.DOKill();
				m_rectSpineObjectRoot.DOAnchorPos(vSpineObjectRootAnchoredPosOrg, num).SetEase(Ease.OutCubic).OnComplete(SetIllust);
				SetCharacterFade(m_UICharacterView, 0f, num);
			}
			else
			{
				m_rectSpineObjectRoot.DOKill();
				Vector2 vSpineObjectRootAnchoredPosOrg2 = m_vSpineObjectRootAnchoredPosOrg;
				vSpineObjectRootAnchoredPosOrg2.x = 900f;
				m_rectSpineObjectRoot.anchoredPosition = vSpineObjectRootAnchoredPosOrg2;
				SetIllust();
			}
			if (m_UIShipView.HasCharacterIllust())
			{
				nKM_UNIT_TYPE = NKM_UNIT_TYPE.NUT_SHIP;
				if (m_UnitTempletBase == null || m_UIShipView.IsDiffrentCharacter(m_UnitTempletBase.m_UnitID, 0))
				{
					m_rectShipSpineObjectRoot.DOKill();
					m_rectShipSpineObjectRoot.DOScale(new Vector3(-0.7f, 0.7f, 0f), 0.3f).SetEase(Ease.OutCubic).OnComplete(SetShipIllust);
					SetCharacterFade(m_UIShipView, 0f, 0.3f);
				}
			}
			else
			{
				m_rectShipSpineObjectRoot.DOKill();
				m_rectShipSpineObjectRoot.localScale = new Vector3(-0.7f, 0.7f, 0f);
				SetShipIllust();
			}
			nKM_UNIT_TYPE2 = (((m_UnitData != null || m_OperatorData != null) && m_UnitTempletBase != null) ? m_UnitTempletBase.m_NKM_UNIT_TYPE : NKM_UNIT_TYPE.NUT_INVALID);
		}
		if (m_bOpen)
		{
			switch (nKM_UNIT_TYPE2)
			{
			case NKM_UNIT_TYPE.NUT_INVALID:
				ShowUnitBG(value: false);
				if (m_bShowLoadingAnimWhenEmpty)
				{
					PlayLoadingAnim("BASE");
				}
				else
				{
					PlayLoadingAnim("CLOSE");
				}
				break;
			case NKM_UNIT_TYPE.NUT_NORMAL:
			case NKM_UNIT_TYPE.NUT_OPERATOR:
				ShowUnitBG(value: true);
				if (nKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
				{
					PlayLoadingAnim("CHANGE");
				}
				else
				{
					PlayLoadingAnim("CLOSE");
				}
				break;
			case NKM_UNIT_TYPE.NUT_SHIP:
				ShowUnitBG(value: false);
				if (nKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
				{
					PlayLoadingAnim("CHANGE");
				}
				else
				{
					PlayLoadingAnim("CLOSE");
				}
				break;
			}
		}
		SetSubMenuPosition(m_UnitData != null || m_OperatorData != null);
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	private void SetCharacterFade(NKCUICharacterView uiChar, float targetAlpha, float time)
	{
		DOTween.To(() => uiChar.GetColor().a, delegate(float a)
		{
			uiChar.SetColor(-1f, -1f, -1f, a);
		}, targetAlpha, time);
	}

	private void SetIllust()
	{
		if (m_bShipOnlyMode)
		{
			return;
		}
		if (m_UnitTempletBase != null && (m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL || m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR))
		{
			if (m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
			{
				m_UICharacterView.SetCharacterIllust(m_UnitData);
			}
			else if (m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR)
			{
				m_UICharacterView.SetCharacterIllust(m_OperatorData);
			}
			m_rectSpineObjectRoot.DOKill();
			m_rectSpineObjectRoot.DOAnchorPos(m_vSpineObjectRootAnchoredPosOrg, 0.3f).SetEase(Ease.OutCubic);
			m_UICharacterView.SetColor(-1f, -1f, -1f, 0f);
			SetCharacterFade(m_UICharacterView, 1f, 0.3f);
		}
		else
		{
			m_UICharacterView.CleanUp();
		}
	}

	private void SetShipIllust()
	{
		if (m_UnitTempletBase != null && m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			m_UIShipView.SetCharacterIllust(m_UnitData);
			m_rectShipSpineObjectRoot.DOKill();
			m_rectShipSpineObjectRoot.DOScale(new Vector3(-1f, 1f, 0f), 0.3f).SetEase(Ease.OutCubic);
			m_UIShipView.SetColor(-1f, -1f, -1f, 0f);
			SetCharacterFade(m_UIShipView, 1f, 0.3f);
		}
		else
		{
			m_UIShipView.CleanUp();
		}
	}

	private void SetSubMenuPosition(bool bIn, bool bAnimate = true)
	{
		m_rectSubMenu.DOKill();
		Vector2 vSubMenuAnchoredPosOrg = m_vSubMenuAnchoredPosOrg;
		if (m_eMode != NKCUIDeckViewer.DeckViewerMode.DeckSetupOnly)
		{
			vSubMenuAnchoredPosOrg.y += 95f;
		}
		vSubMenuAnchoredPosOrg.x = ((!bIn || m_bShipOnlyMode) ? 900 : 0);
		if (bAnimate)
		{
			m_rectSubMenu.anchoredPosition = new Vector2(m_rectSubMenu.anchoredPosition.x, vSubMenuAnchoredPosOrg.y);
			m_rectSubMenu.DOAnchorPos(vSubMenuAnchoredPosOrg, 0.6f).SetEase(Ease.OutCubic);
		}
		else
		{
			m_rectSubMenu.anchoredPosition = vSubMenuAnchoredPosOrg;
		}
	}

	public void SetLeader(bool bLeader)
	{
		m_bLeader = bLeader;
		SetLeader();
	}

	public void SetLeader()
	{
		if (m_bLeader)
		{
			m_cbtnLeader.UnLock();
			m_cbtnLeader.Select(bSelect: true);
		}
		else
		{
			m_cbtnLeader.UnLock();
			m_cbtnLeader.Select(bSelect: false);
		}
	}

	private void OnUnitInfoClicked()
	{
		if (m_UnitTempletBase != null && m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR && m_OperatorData != null)
		{
			NKCUIOperatorInfo.Instance.Open(m_OperatorData, new NKCUIOperatorInfo.OpenOption(new List<long> { m_OperatorData.uid }));
		}
		else if (dOnUnitInfoClick != null)
		{
			dOnUnitInfoClick(m_UnitData);
		}
	}

	private void OnUnitChangeClicked()
	{
		if (dOnUnitChangeClick == null)
		{
			return;
		}
		if (m_OperatorData != null && m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_OperatorData.id);
			if (unitTempletBase != null)
			{
				dOnUnitChangeClick(unitTempletBase.m_NKM_UNIT_TYPE, m_OperatorData.uid);
			}
		}
		else if (m_UnitData != null)
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(m_UnitData.m_UnitID);
			if (unitTempletBase2 != null)
			{
				dOnUnitChangeClick(unitTempletBase2.m_NKM_UNIT_TYPE, m_UnitData.m_UnitUID);
			}
		}
	}

	public void OnLeaderChangeClicked()
	{
		if (dOnLeaderChange != null)
		{
			dOnLeaderChange(m_UnitData);
		}
	}

	public void PlayLoadingAnim(string name)
	{
		if (!(m_animLoading == null) && !m_bShipOnlyMode && m_animLoading.gameObject.activeInHierarchy)
		{
			m_animLoading.Play(name, -1, 0f);
		}
	}

	private void ShowUnitBG(bool value)
	{
		if (m_BGAnimator != null)
		{
			m_BGAnimator.SetBool("UnitSelected", value);
		}
	}

	private Sprite GetSpriteMoveType(NKM_UNIT_STYLE_TYPE type)
	{
		string stringMoveType = GetStringMoveType(type);
		if (string.IsNullOrEmpty(stringMoveType))
		{
			return null;
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_DECK_VIEW_SPRITE", stringMoveType);
	}

	private string GetStringMoveType(NKM_UNIT_STYLE_TYPE type)
	{
		string result = string.Empty;
		switch (type)
		{
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT:
			result = "NKM_DECK_VIEW_SHIP_MOVETYPE_1";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY:
			result = "NKM_DECK_VIEW_SHIP_MOVETYPE_4";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER:
			result = "NKM_DECK_VIEW_SHIP_MOVETYPE_2";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL:
			result = "NKM_DECK_VIEW_SHIP_MOVETYPE_3";
			break;
		}
		return result;
	}
}
