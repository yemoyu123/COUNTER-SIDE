using System;
using System.Collections.Generic;
using ClientPacket.Common;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCDeckViewUnitSlot : MonoBehaviour
{
	[NonSerialized]
	public int m_Index;

	[NonSerialized]
	public bool m_bLeader;

	[NonSerialized]
	public NKMUnitData m_NKMUnitData;

	[NonSerialized]
	public NKMUnitTempletBase m_NKMUnitTempletBase;

	private RectTransform m_RectTransform;

	public GameObject m_objMain;

	public RectTransform m_rectMain;

	private Animator m_animatorMain;

	public NKCUIComButton m_NKCUIComButton;

	public NKCUIComDrag m_NKCUIComDrag;

	public Image m_imgBGPanel;

	public Image m_imgBgAddPanel;

	public GameObject m_objUnitMain;

	public Image m_imgUnitPanel;

	public Image m_imgUnitAddPanel;

	public Image m_imgUnitGrayPanel;

	public Image m_imgSlotCardBlur;

	public GameObject m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_COST_BG_Panel;

	public Text m_textCardCost;

	public GameObject m_objArrow;

	public GameObject m_objLeader;

	public NKCUIComStarRank m_comStarRank;

	public GameObject m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_LEVEL_GAUGE_SLIDER;

	public Slider m_sliderExp;

	public GameObject m_objExpMax;

	public NKCUIComTextUnitLevel m_textLevel;

	public Text m_lbTextLevelDesc;

	public Image m_imgAttackType;

	public GameObject m_objRearm;

	public GameObject m_objSourceType;

	public Image m_imgSourceType;

	public Image m_imgSourceTypeSub;

	private float m_PosXOrg;

	private float m_PosYOrg;

	public NKMTrackingFloat m_PosX = new NKMTrackingFloat();

	public NKMTrackingFloat m_PosY = new NKMTrackingFloat();

	public NKMTrackingFloat m_ScaleX = new NKMTrackingFloat();

	public NKMTrackingFloat m_ScaleY = new NKMTrackingFloat();

	[Header("Enemy")]
	public GameObject m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_BOSS;

	public Image m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_BOSS_img;

	[Header("지부 소속 정보")]
	public GameObject m_objCityLeaderRoot;

	public Image m_imgCityLeaderBG;

	public Text m_lbCityLeader;

	public Sprite m_spCityLeaderBG;

	public Sprite m_spOtherCityBG;

	private NKMWorldMapManager.WorldMapLeaderState m_eWorldmapState;

	[Header("밴 정보")]
	public GameObject m_objBan;

	public Text m_lbBanLevel;

	[Header("압류")]
	public GameObject m_objSeized;

	[Header("리그밴")]
	public GameObject m_objLeagueBan;

	public GameObject m_objLeaguePickEnable;

	public DOTweenAnimation m_doTweenAnimation;

	[Header("각성 애니메이션")]
	public Animator m_animAwakenFX;

	[Header("전술업데이트")]
	public NKCUITacticUpdateLevelSlot m_tacticUpdateLvSlot;

	public GameObject m_objTacticMAXLv;

	[Header("토너먼트 변경표시")]
	public GameObject m_objChanged;

	private NKCAssetInstanceData m_instace;

	private bool m_bInDrag;

	private bool m_bEnableDrag = true;

	private bool m_bEnableShowBan;

	private bool m_bEnableShowUpUnit;

	private float fTimeTest = 3f;

	private const string DECK_SPRITE_BUNDLE_NAME = "ab_ui_unit_slot_deck_sprite";

	public bool GetInDrag()
	{
		return m_bInDrag;
	}

	public void SetEnableShowBan(bool bSet)
	{
		m_bEnableShowBan = bSet;
	}

	public void SetEnableShowUpUnit(bool bSet)
	{
		m_bEnableShowUpUnit = bSet;
	}

	public static NKCDeckViewUnitSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_UNIT_SLOT_DECK", "NKM_UI_DECK_VIEW_UNIT_SLOT");
		NKCDeckViewUnitSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCDeckViewUnitSlot>();
		if (component == null)
		{
			Debug.LogError("NKCDeckViewUnitSlot Prefab null!");
			return null;
		}
		component.m_instace = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void Init(int index, bool bEnableDrag = true)
	{
		m_Index = index;
		m_RectTransform = GetComponent<RectTransform>();
		m_rectMain = m_objMain.GetComponent<RectTransform>();
		m_animatorMain = m_objMain.GetComponent<Animator>();
		m_PosXOrg = m_rectMain.position.x;
		m_PosYOrg = m_rectMain.position.y;
		m_bEnableDrag = bEnableDrag;
		if (!m_bEnableDrag)
		{
			m_NKCUIComDrag.enabled = false;
			m_NKCUIComButton.m_bSelect = false;
			m_NKCUIComButton.m_bSelectByClick = false;
		}
		SetExp(null);
		InitTransform();
	}

	public void SetEnemyData(NKMUnitTempletBase cNKMUnitTempletBase, NKCEnemyData cNKMEnemyData)
	{
		if (cNKMEnemyData == null)
		{
			return;
		}
		SetCityLeaderTag(NKMWorldMapManager.WorldMapLeaderState.None);
		NKCUtil.SetGameobjectActive(m_objLeader, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_COST_BG_Panel, bValue: false);
		NKCUtil.SetGameobjectActive(m_objArrow, bValue: false);
		NKCUtil.SetGameobjectActive(m_comStarRank, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_LEVEL_GAUGE_SLIDER, bValue: false);
		m_textCardCost.text = "";
		Sprite backPanelImage = GetBackPanelImage(NKM_UNIT_GRADE.NUG_N);
		m_imgBGPanel.sprite = backPanelImage;
		m_imgBgAddPanel.sprite = backPanelImage;
		if (backPanelImage == null)
		{
			Debug.LogError("SetEnemyData m_spPanelN: null");
		}
		if (m_imgBGPanel.sprite == null)
		{
			Debug.LogError("SetEnemyData m_imgBGPanel.sprite: null");
		}
		if (cNKMUnitTempletBase != null)
		{
			Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, cNKMUnitTempletBase);
			m_imgUnitPanel.sprite = sprite;
			m_textLevel.SetText(cNKMEnemyData.m_Level.ToString(), 0);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_BOSS, cNKMEnemyData.m_NKM_BOSS_TYPE >= NKM_BOSS_TYPE.NBT_DUNGEON_BOSS);
			if (cNKMEnemyData.m_NKM_BOSS_TYPE == NKM_BOSS_TYPE.NBT_DUNGEON_BOSS)
			{
				m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_BOSS_img.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_POPUP_ENEMY_ICON");
			}
			else if (cNKMEnemyData.m_NKM_BOSS_TYPE == NKM_BOSS_TYPE.NBT_WARFARE_BOSS)
			{
				m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_BOSS_img.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_POPUP_ENEMY_BOSS_ICON");
			}
			Sprite orLoadUnitRoleAttackTypeIcon = NKCResourceUtility.GetOrLoadUnitRoleAttackTypeIcon(cNKMUnitTempletBase, bSmall: true);
			NKCUtil.SetImageSprite(m_imgAttackType, orLoadUnitRoleAttackTypeIcon, bDisableIfSpriteNull: true);
			NKCUtil.SetGameobjectActive(m_objSourceType, NKMOpenTagManager.IsOpened("UNIT_SOURCE_TYPE") && cNKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
			if (cNKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				Sprite orLoadUnitSourceTypeIcon = NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(cNKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE, bSmall: true);
				NKCUtil.SetImageSprite(m_imgSourceType, orLoadUnitSourceTypeIcon, bDisableIfSpriteNull: true);
				Sprite orLoadUnitSourceTypeIcon2 = NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(cNKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB, bSmall: true);
				NKCUtil.SetImageSprite(m_imgSourceTypeSub, orLoadUnitSourceTypeIcon2, bDisableIfSpriteNull: true);
			}
			NKCUtil.SetGameobjectActive(m_objUnitMain, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgUnitPanel, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgUnitGrayPanel, bValue: false);
			NKCUtil.SetGameobjectActive(m_textLevel, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgBgAddPanel, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgUnitAddPanel, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgSlotCardBlur, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRearm, cNKMUnitTempletBase.IsRearmUnit);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objUnitMain, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_BOSS, bValue: false);
		}
	}

	public void SetData(NKMUnitData cNKMUnitData, NKCUIDeckViewer.DeckViewerOption deckViewerOption, bool bEnableButton = true)
	{
		if (deckViewerOption.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.WorldMapMissionDeckSelect)
		{
			if (cNKMUnitData != null)
			{
				NKMWorldMapManager.WorldMapLeaderState unitWorldMapLeaderState = NKMWorldMapManager.GetUnitWorldMapLeaderState(NKCScenManager.CurrentUserData(), cNKMUnitData.m_UnitUID, deckViewerOption.WorldMapMissionCityID);
				SetCityLeaderTag(unitWorldMapLeaderState);
			}
			else
			{
				SetCityLeaderTag(NKMWorldMapManager.WorldMapLeaderState.None);
			}
			_SetData(cNKMUnitData, bEnableButton);
			return;
		}
		if (deckViewerOption.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.PrepareLocalDeck)
		{
			SetCityLeaderTag(NKMWorldMapManager.WorldMapLeaderState.None);
			_SetData(cNKMUnitData, bEnableButton);
			SetLeader(m_bLeader, bEffect: false);
			return;
		}
		SetCityLeaderTag(NKMWorldMapManager.WorldMapLeaderState.None);
		_SetData(cNKMUnitData, bEnableButton);
		bool bLeader = false;
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		if (armyData != null && cNKMUnitData != null)
		{
			NKMDeckData deckData = armyData.GetDeckData(deckViewerOption.DeckIndex);
			bLeader = deckData != null && cNKMUnitData.m_UnitUID == deckData.GetLeaderUnitUID();
		}
		SetLeader(bLeader, bEffect: false);
	}

	public void SetData(NKMUnitData cNKMUnitData, bool bEnableButton = true)
	{
		SetCityLeaderTag(NKMWorldMapManager.WorldMapLeaderState.None);
		_SetData(cNKMUnitData, bEnableButton);
	}

	private void _SetData(NKMUnitData cNKMUnitData, bool bEnableButton = true)
	{
		if (m_NKCUIComButton != null)
		{
			m_NKCUIComButton.enabled = bEnableButton;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_COST_BG_Panel, bValue: true);
		NKCUtil.SetGameobjectActive(m_comStarRank, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_LEVEL_GAUGE_SLIDER, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_BOSS, bValue: false);
		NKCUtil.SetGameobjectActive(m_objChanged, bValue: false);
		m_NKMUnitData = cNKMUnitData;
		if (cNKMUnitData != null && m_NKMUnitData.m_UnitID != 0)
		{
			m_NKMUnitTempletBase = NKMUnitManager.GetUnitTempletBase(m_NKMUnitData.m_UnitID);
			Sprite backPanelImage = GetBackPanelImage(m_NKMUnitTempletBase.m_NKM_UNIT_GRADE);
			NKCUtil.SetImageSprite(m_imgBGPanel, backPanelImage);
			NKCUtil.SetImageSprite(m_imgBgAddPanel, backPanelImage);
			NKCUtil.SetGameobjectActive(m_objTacticMAXLv, NKCTacticUpdateUtil.IsMaxTacticLevel(cNKMUnitData.tacticLevel));
			NKCUtil.SetAwakenFX(m_animAwakenFX, m_NKMUnitTempletBase);
			m_comStarRank.SetStarRank(cNKMUnitData);
			Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, cNKMUnitData);
			m_imgUnitPanel.sprite = sprite;
			m_imgUnitAddPanel.sprite = sprite;
			m_imgUnitGrayPanel.sprite = sprite;
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(m_NKMUnitTempletBase.m_UnitID);
			if (unitStatTemplet != null)
			{
				int num = 0;
				bool num2 = NKCBanManager.IsBanUnit(m_NKMUnitTempletBase.m_UnitID);
				bool flag = NKCBanManager.IsUpUnit(m_NKMUnitTempletBase.m_UnitID);
				if (num2 && m_bEnableShowBan)
				{
					num = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, NKCBanManager.GetBanData(), null);
					m_textCardCost.text = string.Format(NKCUtilString.GET_STRING_UNIT_BAN_COST, num.ToString());
					NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
					int unitBanLevel = NKCBanManager.GetUnitBanLevel(m_NKMUnitTempletBase.m_UnitID);
					NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_LEVEL_ONE_PARAM, unitBanLevel));
					m_lbBanLevel.color = Color.red;
				}
				else if (flag && m_bEnableShowUpUnit)
				{
					num = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, null, NKCBanManager.m_dicNKMUpData);
					m_textCardCost.text = string.Format(NKCUtilString.GET_STRING_UNIT_UP_COST, num.ToString());
					NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
					int unitUpLevel = NKCBanManager.GetUnitUpLevel(m_NKMUnitTempletBase.m_UnitID);
					NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_UP_LEVEL_ONE_PARAM, unitUpLevel));
					m_lbBanLevel.color = NKCBanManager.UP_COLOR;
				}
				else
				{
					num = unitStatTemplet.GetRespawnCost(bPVP: false, bLeader: false, null, null);
					m_textCardCost.text = num.ToString();
					NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
				}
			}
			m_textLevel.SetLevel(cNKMUnitData, 0);
			Sprite orLoadUnitRoleAttackTypeIcon = NKCResourceUtility.GetOrLoadUnitRoleAttackTypeIcon(cNKMUnitData, bSmall: true);
			NKCUtil.SetImageSprite(m_imgAttackType, orLoadUnitRoleAttackTypeIcon, bDisableIfSpriteNull: true);
			NKCUtil.SetGameobjectActive(m_objSourceType, m_NKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
			if (m_NKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				Sprite orLoadUnitSourceTypeIcon = NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(m_NKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE, bSmall: true);
				NKCUtil.SetImageSprite(m_imgSourceType, orLoadUnitSourceTypeIcon, bDisableIfSpriteNull: true);
				Sprite orLoadUnitSourceTypeIcon2 = NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(m_NKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB, bSmall: true);
				NKCUtil.SetImageSprite(m_imgSourceTypeSub, orLoadUnitSourceTypeIcon2, bDisableIfSpriteNull: true);
			}
			m_objUnitMain.SetActive(value: true);
			m_imgUnitPanel.gameObject.SetActive(value: true);
			m_imgUnitGrayPanel.gameObject.SetActive(value: false);
			m_objArrow.SetActive(value: false);
			m_objLeader.SetActive(value: false);
			m_textLevel.gameObject.SetActive(value: true);
			m_imgBgAddPanel.gameObject.SetActive(value: false);
			m_imgUnitAddPanel.gameObject.SetActive(value: false);
			m_imgSlotCardBlur.gameObject.SetActive(value: false);
			NKCUtil.SetGameobjectActive(m_objSeized, cNKMUnitData.IsSeized);
			bool bValue = true;
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMUnitData.m_UnitID);
			if (unitTempletBase != null)
			{
				bValue = !unitTempletBase.m_bMonster;
			}
			NKCUtil.SetGameobjectActive(m_objRearm, unitTempletBase?.IsRearmUnit ?? false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_COST_BG_Panel, bValue);
			NKCUtil.SetGameobjectActive(m_textCardCost.gameObject, bValue);
			NKCUtil.SetGameobjectActive(m_comStarRank, bValue);
			NKCUtil.SetGameobjectActive(m_tacticUpdateLvSlot.gameObject, bValue: true);
			m_tacticUpdateLvSlot.SetLevel(m_NKMUnitData.tacticLevel);
			NKCUtil.SetGameobjectActive(m_objChanged, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
			Sprite emptyPanelImage = GetEmptyPanelImage();
			m_imgBGPanel.sprite = emptyPanelImage;
			m_imgBgAddPanel.sprite = emptyPanelImage;
			NKCUtil.SetGameobjectActive(m_comStarRank, bValue: false);
			m_objUnitMain.SetActive(value: false);
			m_imgBgAddPanel.gameObject.SetActive(value: false);
			m_imgUnitPanel.gameObject.SetActive(value: false);
			m_imgUnitAddPanel.gameObject.SetActive(value: false);
			m_imgUnitGrayPanel.gameObject.SetActive(value: false);
			m_objArrow.SetActive(value: false);
			m_objLeader.SetActive(value: false);
			m_textCardCost.gameObject.SetActive(value: false);
			m_textLevel.gameObject.SetActive(value: false);
			NKCUtil.SetGameobjectActive(m_imgAttackType, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSourceType, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSeized, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRearm, bValue: false);
			NKCUtil.SetAwakenFX(m_animAwakenFX, null);
			NKCUtil.SetGameobjectActive(m_tacticUpdateLvSlot.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_objTacticMAXLv, bValue: false);
			NKCUtil.SetGameobjectActive(m_objChanged, bValue: false);
		}
		ResetEffect();
		ResetPos();
		SetExp(cNKMUnitData);
		InitTransform();
		ButtonDeSelect();
	}

	public void SetUpBanData(NKMUnitData unitData, Dictionary<int, NKMBanData> dicBbanData, Dictionary<int, NKMUnitUpData> dicUpData, bool bLeader)
	{
		if (unitData == null || (dicBbanData == null && dicUpData == null))
		{
			return;
		}
		int num = 0;
		if (dicBbanData != null && dicBbanData.Count > 0 && dicBbanData.ContainsKey(unitData.m_UnitID))
		{
			num = dicBbanData[unitData.m_UnitID].m_BanLevel;
		}
		int num2 = 0;
		if (dicUpData != null && dicUpData.Count > 0 && dicUpData.ContainsKey(unitData.m_UnitID))
		{
			num2 = dicUpData[unitData.m_UnitID].upLevel;
		}
		if (num == 0 && num2 == 0)
		{
			return;
		}
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
		if (unitStatTemplet != null)
		{
			if (num > 0)
			{
				int respawnCost = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader, dicBbanData, null);
				NKCUtil.SetLabelText(m_textCardCost, string.Format(NKCUtilString.GET_STRING_UNIT_BAN_COST, respawnCost.ToString()));
				NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
				NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_LEVEL_ONE_PARAM, num));
				NKCUtil.SetLabelTextColor(m_lbBanLevel, Color.red);
			}
			else if (num2 > 0)
			{
				int respawnCost2 = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader, null, dicUpData);
				NKCUtil.SetLabelText(m_textCardCost, string.Format(NKCUtilString.GET_STRING_UNIT_UP_COST, respawnCost2.ToString()));
				NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
				NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_UP_LEVEL_ONE_PARAM, num2));
				NKCUtil.SetLabelTextColor(m_lbBanLevel, NKCBanManager.UP_COLOR);
			}
			else
			{
				int respawnCost3 = unitStatTemplet.GetRespawnCost(bPVP: false, bLeader: false, null, null);
				NKCUtil.SetLabelText(m_textCardCost, respawnCost3.ToString());
				NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
			}
		}
	}

	public void SetPrivate()
	{
		SetData(null, bEnableButton: false);
		SetLeader(bLeader: false, bEffect: false);
		m_imgBGPanel.sprite = GetPrivatePanelImage();
	}

	public void InitTransform()
	{
		m_PosX.SetNowValue(m_PosXOrg);
		m_PosY.SetNowValue(m_PosYOrg);
	}

	private void SetExp(NKMUnitData unitData)
	{
		NKCUtil.SetGameobjectActive(m_objExpMax, bValue: false);
		if (unitData != null)
		{
			float endValue = ((unitData.m_UnitID == 0) ? 0f : ((NKCExpManager.GetUnitMaxLevel(unitData) != unitData.m_UnitLevel) ? NKCExpManager.GetUnitNextLevelExpProgress(unitData) : 1f));
			m_sliderExp.value = 0f;
			m_sliderExp.DOValue(endValue, 2f).SetEase(Ease.OutCubic);
		}
		else
		{
			m_sliderExp.value = 0f;
		}
	}

	public void Update()
	{
		if (m_bEnableDrag)
		{
			m_PosX.Update(Time.deltaTime);
			m_PosY.Update(Time.deltaTime);
		}
		if (m_bEnableDrag)
		{
			UpdatePos();
		}
		fTimeTest -= Time.deltaTime;
		if (fTimeTest < 0f)
		{
			if (m_imgBGPanel.sprite == null)
			{
				Debug.LogError("SetEnemyData m_imgBGPanel.sprite: null");
			}
			fTimeTest = 3f;
		}
	}

	private Sprite GetBackPanelImage(NKM_UNIT_GRADE unitGrade)
	{
		return unitGrade switch
		{
			NKM_UNIT_GRADE.NUG_N => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_unit_slot_deck_sprite", "FACE_DECK_BG_N"), 
			NKM_UNIT_GRADE.NUG_R => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_unit_slot_deck_sprite", "FACE_DECK_BG_R"), 
			NKM_UNIT_GRADE.NUG_SR => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_unit_slot_deck_sprite", "FACE_DECK_BG_SR"), 
			NKM_UNIT_GRADE.NUG_SSR => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_unit_slot_deck_sprite", "FACE_DECK_BG_SSR"), 
			_ => null, 
		};
	}

	private Sprite GetEmptyPanelImage()
	{
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_unit_slot_deck_sprite", "FACE_DECK_EMPTY");
	}

	private Sprite GetPrivatePanelImage()
	{
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_unit_slot_deck_sprite", "FACE_DECK_BG_ASYNC_PRIVATE");
	}

	public void UpdatePos()
	{
		Vector3 position = m_rectMain.position;
		position.Set(m_PosX.GetNowValue(), m_PosY.GetNowValue(), position.z);
		m_rectMain.position = position;
	}

	public void SetSelectable(bool bSelectable)
	{
		if (bSelectable)
		{
			ButtonSelect();
		}
		else
		{
			ButtonDeSelect(bForce: true, bImmediate: true);
		}
	}

	public void SetLeader(bool bLeader, bool bEffect)
	{
		if (bLeader)
		{
			m_objLeader.SetActive(value: true);
			if (bEffect)
			{
				PlayEffect();
			}
		}
		else
		{
			m_objLeader.SetActive(value: false);
		}
		m_bLeader = bLeader;
		if (m_NKMUnitTempletBase == null)
		{
			return;
		}
		int num = 0;
		bool flag = NKCBanManager.IsBanUnit(m_NKMUnitTempletBase.m_UnitID);
		bool flag2 = NKCBanManager.IsUpUnit(m_NKMUnitTempletBase.m_UnitID);
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(m_NKMUnitTempletBase.m_UnitID);
		if (unitStatTemplet != null)
		{
			if (flag && m_bEnableShowBan)
			{
				num = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader, NKCBanManager.GetBanData(), null);
				m_textCardCost.text = string.Format(NKCUtilString.GET_STRING_UNIT_BAN_COST, num.ToString());
			}
			else if (flag2 && m_bEnableShowUpUnit)
			{
				num = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader, null, NKCBanManager.m_dicNKMUpData);
				m_textCardCost.text = string.Format(NKCUtilString.GET_STRING_UNIT_UP_COST, num.ToString());
			}
			else if (bLeader)
			{
				num = unitStatTemplet.GetRespawnCost(bPVP: false, bLeader, null, null);
				m_textCardCost.text = $"<color=#FFCD07>{num.ToString()}</color>";
			}
			else
			{
				num = unitStatTemplet.GetRespawnCost(bPVP: false, bLeader, null, null);
				m_textCardCost.text = num.ToString();
			}
		}
	}

	public void SetLeagueBan(bool bBanUnit)
	{
		NKCUtil.SetGameobjectActive(m_objLeagueBan, bBanUnit);
	}

	public void SetEnableShowLevelText(bool bShow)
	{
		NKCUtil.SetGameobjectActive(m_lbTextLevelDesc, bShow);
		NKCUtil.SetGameobjectActive(m_textLevel, bShow);
	}

	public void SetLeaguePickEnable(bool bEnable)
	{
		NKCUtil.SetGameobjectActive(m_objLeaguePickEnable, bEnable);
		if (m_doTweenAnimation != null && bEnable)
		{
			DOTween.Sequence().Restart();
			m_doTweenAnimation.DOGoto(0f, andPlay: true);
		}
	}

	public void PlayEffect()
	{
		if (base.gameObject.activeInHierarchy)
		{
			m_animatorMain.Play("NKM_UI_GAME_DECK_CARD_READY", -1, 0f);
		}
	}

	private void ResetEffect()
	{
		m_imgBgAddPanel.gameObject.SetActive(value: false);
		m_imgUnitAddPanel.gameObject.SetActive(value: false);
		m_imgSlotCardBlur.gameObject.SetActive(value: false);
	}

	public void ButtonSelect()
	{
		m_NKCUIComButton.Select(bSelect: true);
	}

	public void ButtonDeSelect(bool bForce = false, bool bImmediate = false)
	{
		m_NKCUIComButton.Select(bSelect: false, bForce, bImmediate);
	}

	public void BeginDrag()
	{
		if (m_bEnableDrag)
		{
			NKCUtil.SetGameobjectActive(m_objCityLeaderRoot, bValue: false);
			m_bInDrag = true;
			m_objMain.transform.SetParent(NKCUIManager.Get_NUF_DRAG().transform);
		}
	}

	public void Drag(PointerEventData eventData)
	{
		if (m_bEnableDrag && m_bInDrag)
		{
			Vector3 vector = NKCCamera.GetSubUICamera().ScreenToWorldPoint(eventData.position);
			m_PosX.SetNowValue(vector.x);
			m_PosY.SetNowValue(vector.y);
		}
	}

	public void EndDrag()
	{
		if (m_bEnableDrag)
		{
			SetCityLeaderTag(m_eWorldmapState);
			m_bInDrag = false;
			ReturnToParent();
			ReturnToOrg();
		}
	}

	public void EnableDrag(bool bEnalbe)
	{
		m_bEnableDrag = bEnalbe;
	}

	public void Swap(NKCDeckViewUnitSlot cNKCDeckViewUnitSlot)
	{
		m_PosX.SetTracking(cNKCDeckViewUnitSlot.m_PosXOrg, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
		m_PosY.SetTracking(cNKCDeckViewUnitSlot.m_PosYOrg, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
	}

	public void ReturnToParent()
	{
		if (m_bEnableDrag)
		{
			m_objMain.transform.SetParent(base.transform);
		}
	}

	public void ReturnToOrg()
	{
		if (m_bEnableDrag)
		{
			m_PosX.SetTracking(m_PosXOrg, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
			m_PosY.SetTracking(m_PosYOrg, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
	}

	public bool IsEnter(Vector3 incomingSlotPosition)
	{
		if (incomingSlotPosition.x > m_RectTransform.position.x + m_RectTransform.rect.width * 0.5f)
		{
			return false;
		}
		if (incomingSlotPosition.x < m_RectTransform.position.x - m_RectTransform.rect.width * 0.5f)
		{
			return false;
		}
		if (incomingSlotPosition.y > m_RectTransform.position.y + m_RectTransform.rect.height * 0.5f)
		{
			return false;
		}
		if (incomingSlotPosition.y < m_RectTransform.position.y - m_RectTransform.rect.height * 0.5f)
		{
			return false;
		}
		return true;
	}

	public void ResetPos(bool bImmediate = false)
	{
		if (m_bEnableDrag)
		{
			m_PosXOrg = m_RectTransform.position.x;
			m_PosYOrg = m_RectTransform.position.y;
			m_PosX.SetTracking(m_PosXOrg, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
			m_PosY.SetTracking(m_PosYOrg, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
			if (bImmediate)
			{
				m_PosX.SetNowValue(m_PosXOrg);
				m_PosY.SetNowValue(m_PosYOrg);
			}
		}
	}

	public void OnDisable()
	{
		ResetEffect();
	}

	public void SetCityLeaderTag(NKMWorldMapManager.WorldMapLeaderState eWorldmapState)
	{
		m_eWorldmapState = eWorldmapState;
		bool flag = false;
		bool flag2;
		switch (eWorldmapState)
		{
		default:
			flag2 = false;
			break;
		case NKMWorldMapManager.WorldMapLeaderState.CityLeaderOther:
			flag2 = true;
			flag = true;
			break;
		case NKMWorldMapManager.WorldMapLeaderState.CityLeader:
			flag2 = true;
			flag = false;
			break;
		}
		NKCUtil.SetGameobjectActive(m_objCityLeaderRoot, flag2);
		if (flag2)
		{
			if (!flag)
			{
				NKCUtil.SetImageSprite(m_imgCityLeaderBG, m_spCityLeaderBG);
				NKCUtil.SetLabelText(m_lbCityLeader, NKCUtilString.GET_STRING_WORLDMAP_CITY_LEADER);
			}
			else
			{
				NKCUtil.SetImageSprite(m_imgCityLeaderBG, m_spOtherCityBG);
				NKCUtil.SetLabelText(m_lbCityLeader, NKCUtilString.GET_STRING_WORLDMAP_ANOTHER_CITY);
			}
		}
	}

	public void SetDataChanged(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objChanged, bValue);
	}

	private void OnDestroy()
	{
		CloseInstance();
	}

	public void CloseInstance()
	{
		if (m_instace != null)
		{
			NKCAssetResourceManager.CloseInstance(m_instace);
			m_instace = null;
		}
	}

	public bool IsEmpty()
	{
		if (m_NKMUnitData == null)
		{
			return true;
		}
		if (m_NKMUnitData.m_UnitUID == 0L)
		{
			return true;
		}
		return false;
	}

	public void SetIconEtcDefault()
	{
		NKCUtil.SetGameobjectActive(m_objUnitMain, bValue: true);
		NKCUtil.SetGameobjectActive(m_imgUnitPanel.gameObject, bValue: true);
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_unit_face_card", "AB_UNIT_FACE_CARD_ETC_DEFAULT");
		NKCUtil.SetImageSprite(m_imgUnitPanel, orLoadAssetResource);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DECK_VIEW_UNIT_SLOT_CARD_COST_BG_Panel.gameObject, bValue: false);
		SetEnableShowLevelText(bShow: false);
	}
}
